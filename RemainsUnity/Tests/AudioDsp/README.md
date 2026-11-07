# DSP buffer settings checks

The menu offers 256, 512 and 1024 samples. Choosing a button only stages the choice; saving applies one `AudioSettings.Reset` and persists the local audio preferences. The existing sample rate, speaker mode and voice counts are retained. Rejection, partial reset, an unexpected resulting configuration or persistence failure triggers at most one recovery reset. An unchanged choice requires no reset. Volume preview and cancel never reset audio. Old local JSON without a DSP field retains the current project/device default until the user explicitly saves settings.

Run the linked production transaction code with a fake device and writer:

```sh
DOTNET_CLI_HOME=/tmp/remains-audio-dotnet dotnet run --project RemainsUnity/Tests/AudioDsp/AudioDsp.csproj -p:UnityEditorRoot=/path/to/Unity/Editor
```

This project references Unity's existing Newtonsoft.Json assembly and needs no NuGet packages. `Shims.cs` defines only the audio configuration boundary; accidental real device access or an uninjected persistence write fails. Temporary JSON fixtures use a unique test directory and remove it afterwards. The test does not validate native prefab deserialization or audio hardware.

With the Unity project already imported, run `Remains.Editor.AudioDspSettingsChecks.Run` via `-batchmode -nographics -job-worker-count 1 -executeMethod`. It loads the real serialized prefab, invokes all three buffer buttons, volume sliders, cancel and save, and checks failed reset/write recovery with a fake device. The existing `Remains.Runtime.AudioSettingsProbe` continues to test in-game volume/menu behavior and checks that saving includes a supported DSP choice.

A machine with a real audio device is still required to check native reset support, audible interruptions and latency. Null-device cloud runs cannot prove listening quality or hardware latency. These are local preferences; the world save format and network protocol are unchanged.
