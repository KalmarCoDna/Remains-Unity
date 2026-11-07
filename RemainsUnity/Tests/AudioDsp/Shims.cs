using System;
// Data/API boundary only; the production preference/transaction code is linked unchanged.
namespace UnityEngine
{
 public enum AudioSpeakerMode {Stereo,Surround}
 public struct AudioConfiguration {public int dspBufferSize,sampleRate,numRealVoices,numVirtualVoices;public AudioSpeakerMode speakerMode;}
 public static class AudioSettings
 {
  public static AudioConfiguration GetConfiguration()=>throw new InvalidOperationException("Real device must not be used in fake-device checks");
  public static bool Reset(AudioConfiguration value)=>throw new InvalidOperationException("Real device must not be used in fake-device checks");
 }
}
namespace Remains.Persistence
{
 // AtomicSaveFile already has its own native tests. This check injects all writes; accidental fallback must fail.
 public static class AtomicSaveFile {public static void Write(string path,string json)=>throw new InvalidOperationException("Write adapter was not injected");}
}
