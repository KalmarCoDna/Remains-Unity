# Active grenade telekinesis and shared host compatibility

This separate integration combines TD33 `fb01263e855332828434a2e38b36ed1536c550b9` with the published TD16 feature `656df0a97d9218db42a128c749f981af122dac10`. It includes the new `GrenadeHoldRequest` and persisted holder, so both participants use protocol100. The original TD33 feature branch and its protocol are unchanged.

The existing TD16 `NetworkGrenadeTelekinesisProbe` drives the actual player commands. The client throws; the host catches the live airborne grenade. It verifies ownership transfer, generation1→2, movement while the fuse continues, a stale pose from the former owner, release to generation3 followed by ballistic flight or expiration while held at generation2, exactly one blast/visual/receipt per player, and inventory counts4/5. This integration additionally verifies separate canonical/flight objects, shared-world identity matching host mode, and a stale capture replay through the common request coordinator. Server ownership, actor lifecycle, generation, pause, mana and sequence checks remain strict. The ordinary `grenade` probe is a different scenario and cannot supply this evidence.

Run these Editor methods on the imported integration project:

- `Remains.Editor.GrenadeTelekinesisChecks.Run`
- `Remains.Editor.HostWorldChecks.RunCore`

Then use one exact native Player build for all four graphical pairs, with unique fresh save directories managed by the runner:

```sh
python3 RemainsUnity/Tests/HostWorld/verify.py --player /path/to/Remains --output /path/to/new-results --modes on off --scenarios grenade-telekinesis grenade-telekinesis-expire
```

Both host and guest reports and exit codes must succeed. `detachedFlight` must be true in every report; the host's `sharedWorld` must be false for self-ticks on and true for off, and `staleHoldReplaySent` must be true. All original generation/fuse/inventory/explosion checks remain required. The runner records commands, exact managed assembly hash, modes and reports. Native execution is pending; this document is a plan, not a PASS claim.

`test/host-grenade-telekinesis` is the regression baseline. The dependent `fix/shared-host-grenade-telekinesis` routes the new hold registration and request through TD33's existing coordinator and queue. That fix does not change wire messages or relax the remote validation path.
