# Cloud task integration, 2026-11-07

This review branch combines the independently published PT32, resource teardown,
diagnostics, factory, shared-host, portable room tooling and DSP settings changes.
Base: `d110aed242faf3b4f6ef0e7ab3bd46f07fec93c4` in
`KalmarCoDna/Remains-Unity`. Main is unchanged. Individual task branches remain
available; this integration branch is not a replacement for reviewing them.

## Exact common-source verification

At `dcc91aa16a7ce9261bab98b9e57366cab77fb930`, Unity 6000.0.39f1 on Linux,
OpenGLCore/llvmpipe completed **22 native checks, zero failures**, exit 0.
The adjacent `cloud-goal-20261107-native.json` preserves each case and duration.

The checks cover actual grip mist rendering and transform attachment; fallback
ComputeBuffer and shadow teardown; actual Mirror diagnostic reconnect; firearm
content; ladder entry, authored contact declarations, jump and standing support;
factory loot, encounters and danger; real grate projectiles; movement; host graph
identity, mutation and authority; portable ladder rules and empty room layers;
DSP preset persistence and reset failures; audio loop recovery; and one-frame
held-prop release/landing sound observation.

Run from a licensed Editor with a graphical display:

```sh
Unity -batchmode -quit -force-glcore -job-worker-count 2 \
  -projectPath RemainsUnity \
  -executeMethod Remains.Editor.CloudGoalAcceptanceChecks.RunBatch \
  -logFile native-checks.log
```

Set `REMAINS_TEST_REVISION` to the checked Git revision for the JSON report.
The report is written to the checkout's `Verification/cloud-goal/results.json`.
The foot declaration case does not replace the separate tests requiring original
private PNG sources. Audio fake-device checks do not measure hardware latency.

## Windows cross-builds

The official Windows Mono module for the same Editor was installed from Unity's
release distribution. The exact common revision above built a non-Development
Windows x86-64 Player successfully: BuildReport **2,169,280,911 bytes**, exit 0.
The adjacent `cloud-goal-20261107-windows-release.json` records executable,
managed assembly and UnityPlayer hashes. The earlier `a7a3620b7` common revision
also built a Development Windows Player successfully.

```sh
Unity -batchmode -quit -force-glcore -job-worker-count 2 \
  -buildTarget Win64 -projectPath RemainsUnity \
  -executeMethod Remains.Editor.CloudGoalAcceptanceChecks.BuildWindowsReleaseBatch \
  -logFile windows-release.log
```

Building a Windows Player on Linux does not establish Windows/D3D11 execution or
two-computer LAN acceptance. Native graphical paired checks use Linux Players.

The same Unity source (documentation tip `df01532a02`) also built Linux x86-64
Mono Release successfully, exit 0, BuildReport **2,166,308,507 bytes**. Later tool
packaging and documentation commits do not change the Player source.

## Ready static site

The standalone room editor is packaged as `remains-room-editor-20261107.zip`
(60,039,547 bytes), source `a9b9365bc13ee64ca20bca8ebd742695b711aa8e`.
SHA256: `99c1e2813d89a2d6688338cd989fa4f42d27ce86ee90a709218c541c8639c2b7`.
Its README explains serving the ready `site/` directory without Unity or an SDK.
No site has been deployed.

An actual Unity shader probe rendered four 32x32 float targets using the game's
WorldTerrain/WorldSprite shaders: maximum analytic RGB error 0.000543, alpha error
0, tolerance 0.003. Both browser renderers matched those sampled colors within two
sRGB byte values. This covers terrain/sprite rendering, not the missing original
light reference. Browser edit, map, IndexedDB and maximum-size room checks passed.

## Branch boundaries

- `feat/td33-host-single-world` retains the remote validation path and explicit
  `--host-self-ticks on|off`; Release defaults to the shared authority graph and
  Development defaults to self ticks. Its `Tests/HostWorld` documentation records
  paired scenario revisions separately from these Editor checks.
- `feat/browser-room-editor` and `feat/room-editor-tile-rendering` provide a static
  WASM site with shared C# format/geometry/movement and real tile textures.
  All 57 shipped room files round-trip byte-for-byte. There is no deployment in
  this change. Missing private `LightReference.cs` remains a light-preview blocker;
  navigation timing is not a light-preview benchmark. Custom maps are tool data,
  not an implemented in-game custom-map loading workflow.
- The six previously published lighting fixes and their native acceptance are
  on **`test/light-queue-acceptance`**, separate from this common integration.
- Active grenade telekinesis compatibility with TD33 is on
  **`test/td16-native-validation`** and
  **`fix/shared-host-grenade-telekinesis`**, with protocol **100** in that separate
  integration. The common branch keeps actual main's protocol **98**. Players from
  those two integrations must not be mixed.
- Audit-only branches for texture memory, content/save contracts, factory content
  and `compp` preserve their own evidence and blockers; they do not add invented
  progression, item pools or private fixtures to gameplay.

## Remaining external evidence and design work

Original FoE Remains 1.04 comparisons and the previously unreproduced screenshot
cases remain open. Missing original pools/rules prevent a justified `paint` loot
substitution or arbitrary game-stage change for `compp`. Missing private research
fixtures remain explicit failures/blockers in the broader source-dependent suites.
Hardware audio, subjective visuals and physical LAN tests remain separate.
Skills, trade, host migration quorum and deferred game rules need their own design
and acceptance scope. Existing positive contract coverage does not implement every
future feature listed in the historical backlog.
