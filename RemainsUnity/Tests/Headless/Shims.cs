using System;

// Only the logging and unused batch-exit boundary are replaced. No Unity assembly is loaded.
namespace UnityEngine
{
    public static class Debug
    {
        public static void Log(object message)=>Console.WriteLine(message);
        public static void LogWarning(object message)=>Console.WriteLine("WARNING: "+message);
        public static void LogException(Exception error)=>Console.Error.WriteLine(error);
    }
}
namespace UnityEditor
{
    public static class EditorApplication
    {
        public static void Exit(int code)=>throw new InvalidOperationException("The headless runner must call Run, not RunBatch: "+code);
    }
}
