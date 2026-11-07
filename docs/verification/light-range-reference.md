# Full-range attenuation reference check

This branch depends on the six published November7 light-fix branches combined in test/light-queue-acceptance, plus the independent Linux head-hash correction. The only integration conflict was PrototypeRuntime's emission list: retain hornLights.Append(..., Geometry.Get(shownLevel)) AND Explosions.AppendLights(...), preserving horn occlusion and explosion light.

The accepted range-falloff shader changed from a final10% smoothstep to a normalized full-range squared window. StationaryPolarChecks.EmptyEnergy still computed the previous curve; native GPU0.126 vs old CPU0.267 at(0.35,0.35) failed. This branch updates only that CPU test reference to the accepted shader law; production shaders and tolerances are unchanged.

Unity6000.0.39f1/Linux/OpenGLCore/llvmpipe: corrected RuntimePolarLightingChecks.RunBatch PASS exit0. GPU/CPU energy17556empty-scene pixels,2421layered pixels(313mixed),52808prefixes/8layers; production field-error0. The complete runtime test also checks default backend, odd dimensions, pending flash removal, incremental-vs-full rebuild, camera world-cache and actual actor/weapon masks. This does not replace Windows/D3D11 or real-game lighting acceptance.

Logs: /workspace/scratch/light-queue-runtime.log (old CPU-reference failure), light-queue-runtime-fixed.log (PASS); report /workspace/scratch/light-queue-runtime-fixed/integration.json.
