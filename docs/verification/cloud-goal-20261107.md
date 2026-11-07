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

That exact Release assembly completed **five paired smoke cases / ten successful
endpoint reports with exit 0**: appearance OFF/ON, ordinary grenade OFF/ON and
door with no mode argument (observed shared/OFF). Appearance and door used normal
windows requested at 1280x720; grenades passed at 640x360. The original 1280 grenade
runs are preserved failures at phase 0, before throwing: the proportional test
stance driver does not settle at low frame rates. All original grenade handoff,
damage, item and deduplication assertions passed on the same binary at 640x360.
`cloud-goal-20261107-release-pairs.json` records the exact assembly and raw reports.

The additional `test/frame-rate-safe-grenade-probe` branch replaces only that
diagnostic stance command with the already shared dt-aware probe helper. It is
kept separate from these frozen Player artifacts. A real Unity MovementController
regression reproduced the old 3 Hz failure throughout 45 simulated seconds, then
passed all 16 fixed cases (eight frame rates, both sides) with the original
0.05 position / 0.15 velocity / 0.05 support-height tolerances. Actual authored
grenade range is 6, giving target x=22 from thrower x=17. Whole-file Unity API
compilation also passed. Production movement and damage were unchanged.

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

## Separate TD16 paired acceptance

The separate protocol-100 TD16 integration completed four genuine Linux Release
pairs (release/expire in each host mode), eight successful endpoint reports and
exit codes 0. Tested source `7616938e7`, assembly SHA256
`94ed34eeb1487e94ad04b3afd574d71b2e639c701794c28060a65d075d9ffe3a`;
report-only tip `45c6fcdaf` on `fix/td16-probe-held-observation`.
Both peers observed generation 1 and held generation 2; release produced generation
3/blast 3, held expiry blast 2, each with one blast/visual/receipt and inventory 4/5.
The earlier two failed observations remain recorded. The repaired probe waits for
actual live peer observations, retains the original travel/fuse/count assertions,
and adds a generation-1 requirement on both peers. Actual fuse remains 2.5 seconds.
Software graphics used 640x360; the source and raw evidence are in that branch's
`Tests/HostWorld/TD16-INTEGRATION.md` and `TD16-NATIVE-RESULTS.json`.

## Bounded TD33 performance measurement

TD33 also completed a separate paired seed104 combat measurement from a native
Player-generated immutable input, source `823d6ea3d`, 300 warm-up frames and 180
sample frames. Both guests were admitted before capture. Host-local WorldTick
calls were 69 with self ticks and 0 with the shared graph. The report at
`RemainsUnity/Tests/HostWorld/acceptance-2026-11-07.md` preserves the exact input,
assembly, CPU section scopes, raw paths and limits. Software-rendered wall-frame
p95 did not improve in that sample; total memory reduction was not established.
Allocation counters were unavailable. No hardware FPS improvement is claimed.

## Remaining external evidence and design work

Original FoE Remains 1.04 comparisons and the previously unreproduced screenshot
cases remain open. Missing original pools/rules prevent a justified `paint` loot
substitution or arbitrary game-stage change for `compp`. Missing private research
fixtures remain explicit failures/blockers in the broader source-dependent suites.
Hardware audio, subjective visuals and physical LAN tests remain separate.
Skills, trade, host migration quorum and deferred game rules need their own design
and acceptance scope. Existing positive contract coverage does not implement every
future feature listed in the historical backlog.
