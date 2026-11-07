# Combat and movement target suite

Run Remains.Editor.CombatMotionAcceptanceChecks.RunBatch with graphics in Unity 6000.0.39f1. The bounded suite retains every result in Verification/combat-motion/results.json and returns nonzero if any check fails. These are Editor contracts, not a substitute for real gameplay/LAN/original 1.04 comparisons.

On unchanged d110aed2 main runtime: 17 PASS, 11 FAIL, exit1. The attached JSON records each actual result. Six failures share the cross-platform pony-head fingerprint defect. FirearmChecks has an outdated two-texture limit excluding the subsequently authored muzzle masks. Four checks require missing private .reference/Tools/Research material: PlayerStrikeAnimationChecks, GrenadeChecks, FactoryLadderLandingChecks and FootSupportContentChecks. Sources/guards have not been bypassed. First compile attempt of this new runner had namespace typos; fixed before results.

The native passes cover natural and NPC attacks, fire rules, grenade aim, revival, variable jump, body support, recoil, remote actor/free prop motion, pause, projectile path/persistence/controller/local lifecycle, pony strikes and rest. These passes do not close unimplemented skills/trap effects or full original artwork acceptance.
