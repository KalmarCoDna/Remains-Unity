# Residual 2D Animation fallback buffer

On fork main d110aed2, the registry 2D Animation 10.1.4 package allocates its static fallback ComputeBuffer after scene load, including scenes without SpriteSkin. No application-quit callback releases it in that case. The native AnimationBufferLifetimeChecks check failed on unchanged package code with “fallback ComputeBuffer without a single application exit handler”, exit 1.

This branch reapplies the existing 365784e07 patch on current main. The original Unity package, license and notices are retained; the functional package diff is only the idempotent Application.quitting subscription and unsubscription. This is the same patch already present in the historical integration branch. No other animation change was added.

Unity 6000.0.39f1/Linux/OpenGLCore/llvmpipe: AnimationBufferLifetimeChecks passed, exit 0. LightTeardownBatchChecks.RunBatch subsequently passed, exit 0: six create/register/release/recreate fallback lifetimes, then nine ActorShadowTeardownChecks cases covering active pose mask, active projection and an unrelated active render target. These are real native GPU resources; no simulated ComputeBuffer objects. Both cleanup paths can be repeated. Editor logs are retained under /workspace/scratch/light-fallback-baseline.log, light-fallback-fixed.log and light-teardown-six-lifetimes.log.

This scoped check does not repeat the older full telekinesis/travel Player runs or claim Windows/D3D11 acceptance. The original full-package embedding is intentionally retained to pin the corrected code without editing Library/PackageCache.
