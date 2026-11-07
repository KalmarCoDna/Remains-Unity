# Firearm dependency contract on current main

The unchanged main FirearmChecks failed for weapon:friend:grade:0: four directly referenced textures exceeded its old limit of two. The subsequently authored muzzle emission masks are valid direct dependencies. This branch ports only the FirearmChecks part of published 2aac7dfa, counting non-mask dependencies while retaining the existing rest/casing bound and deferred shot/reload checks. No runtime weapon or resource changed.

Fresh native Unity 6000.0.39f1/OpenGLCore/llvmpipe RunBatch passed, exit0: 98 firearm definitions / 60 models, 29 melee weapons, actual frames/muzzle/projectile/audio, original numeric examples, formal-to-working inventory and seed104 factory loot (97 working guns, one formal weapon item). Baseline evidence: combat-motion-baseline-2.log; fixed: firearm-dependency-fixed.log under /workspace/scratch. This does not imply private artwork/source-dependent ContractChecks can all run in this checkout.
