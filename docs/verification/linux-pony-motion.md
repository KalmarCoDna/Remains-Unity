# Independent motion checks after Linux head hash correction

Depends on fix/pony-head-cross-platform-hash (8df168d59). Baseline six checks all failed before reaching their body due to stale headSourceHash on LF checkout. After the hash fix: JumpPoseChecks, StandingEdgePoseChecks and StandingSupportPoseChecks PASS; LadderHoldChecks and LadderPoseContactChecks reach the stale authored-foot count assertion; LadderEntryPoseChecks reaches its outdated missing-contact policy fixture. No movement behavior was changed by this audit. Further fixes are separate branches.

Run CombatMotionAcceptanceChecks.RunBatch with REMAINS_ACCEPTANCE_CHECKS set to a comma-separated subset of check class names. Without a filter the original 28-check suite runs. Unknown names fail explicitly. Native Linux Unity6000.0.39f1/OpenGLCore/llvmpipe; exact six results attached. Filter support added after this same six-check run; tested next with the affected checks rather than rerunning unaffected checks.
