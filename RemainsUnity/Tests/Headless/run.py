#!/usr/bin/env python3
"""Run selected existing C# checks using Unity's bundled .NET tools, without starting Unity."""
import argparse
import hashlib
import json
from pathlib import Path
import re
import subprocess
import sys
import tempfile

HERE = Path(__file__).resolve().parent
SOURCES = HERE.parents[1] / "Assets" / "Remains"
SUITES = {
    "cadence": ("ReplicationCadenceChecks", ["Networking/ReplicationCadence.cs", "Editor/ReplicationCadenceChecks.cs"]),
    "manifest": ("SnapshotManifestChecks", ["Persistence/SnapshotManifest.cs", "Persistence/TextSha256.cs", "Editor/SnapshotManifestChecks.cs"]),
}


def quoted(path):
    value = str(path)
    if '"' in value or "\n" in value or "\r" in value:
        raise ValueError("Unsupported character in compiler path")
    return '"' + value + '"'


def tools(editor):
    editor = editor.resolve()
    if editor.is_file():
        editor = editor.parent
    if not (editor / "Data").is_dir():
        editor = editor / "Editor"
    data = editor / "Data"
    dotnet = data / "NetCoreRuntime" / ("dotnet.exe" if sys.platform == "win32" else "dotnet")
    compiler = data / "DotNetSdkRoslyn" / "csc.dll"
    frameworks = data / "NetCoreRuntime" / "shared" / "Microsoft.NETCore.App"
    versions = [p for p in frameworks.glob("*") if p.is_dir() and re.fullmatch(r"\d+\.\d+\.\d+", p.name)]
    if not dotnet.is_file() or not compiler.is_file() or not versions:
        raise FileNotFoundError("Expected Unity 6000.0.39f1 Editor/Data with NetCoreRuntime and DotNetSdkRoslyn")
    framework = max(versions, key=lambda p: tuple(map(int, p.name.split("."))))
    return dotnet, compiler, framework


def run_suite(name, toolchain, work):
    dotnet, compiler, framework = toolchain
    check, relative = SUITES[name]
    sources = [SOURCES / path for path in relative] + [HERE / "Shims.cs"]
    if name == "manifest":
        sources.append(HERE / "ManifestInput.cs")
    for source in sources:
        if not source.is_file():
            raise FileNotFoundError(source)
    main = work / "Main.cs"
    main.write_text(
        "using System; static class MainProgram {static int Main(){try{Remains.Editor."
        + check + ".Run();return 0;}catch(Exception e){Console.Error.WriteLine(e);return 1;}}}", encoding="utf-8")
    program = work / "Checks.dll"
    response = work / "compile.rsp"
    response.write_text("\n".join(
        ["-nologo", "-target:exe", "-nostdlib+", "-out:" + quoted(program)]
        + ["-r:" + quoted(p) for p in sorted(framework.glob("*.dll"))]
        + [quoted(p) for p in sources + [main]]), encoding="utf-8")
    major, minor, _ = framework.name.split(".")
    (work / "Checks.runtimeconfig.json").write_text(json.dumps({"runtimeOptions": {
        "tfm": "net" + major + "." + minor,
        "framework": {"name": "Microsoft.NETCore.App", "version": framework.name}}}), encoding="utf-8")
    compiled = subprocess.run([str(dotnet), str(compiler), "@" + str(response)], capture_output=True, text=True, encoding="utf-8")
    result = {"suite": name, "compiler_exit": compiled.returncode, "compiler_output": compiled.stdout + compiled.stderr,
              "sources": {str(p.relative_to(SOURCES)) if p.is_relative_to(SOURCES) else p.name:
                          hashlib.sha256(p.read_bytes()).hexdigest() for p in sources}}
    if compiled.returncode:
        result.update(status="compile-failed", exit_code=compiled.returncode)
        return result
    executed = subprocess.run([str(dotnet), str(program)], capture_output=True, text=True, encoding="utf-8")
    result.update(status="passed" if executed.returncode == 0 else "failed", exit_code=executed.returncode,
                  output=executed.stdout + executed.stderr)
    return result


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--editor", required=True, type=Path, help="Installed Unity directory, Editor directory, or Unity executable")
    parser.add_argument("--suite", action="append", choices=SUITES, help="Repeat to select suites; defaults to all")
    parser.add_argument("--output", type=Path, help="Optional JSON report; compilation files are temporary")
    args = parser.parse_args()
    report = {"unity_started": False, "full_project_compilation": False, "results": []}
    try:
        toolchain = tools(args.editor)
        report["dotnet_framework"] = toolchain[2].name
        with tempfile.TemporaryDirectory(prefix="pfe-headless-") as directory:
            for name in dict.fromkeys(args.suite or SUITES):
                work = Path(directory) / name
                work.mkdir()
                result = run_suite(name, toolchain, work)
                report["results"].append(result)
                print(name + ": " + result["status"])
                print(result.get("compiler_output", "") + result.get("output", ""), end="")
    except (OSError, ValueError) as error:
        report["error"] = str(error)
        print(str(error), file=sys.stderr)
    if args.output:
        args.output.parent.mkdir(parents=True, exist_ok=True)
        args.output.write_text(json.dumps(report, ensure_ascii=False, indent=2) + "\n", encoding="utf-8")
    return 0 if "error" not in report and report["results"] and all(r["status"] == "passed" for r in report["results"]) else 1


if __name__ == "__main__":
    sys.exit(main())
