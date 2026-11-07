using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Remains.Editor
{
    public static class AnimationBufferLifetimeChecks
    {
        public static void Run()
        {
            var type=typeof(SpriteSkin).Assembly.GetType("UnityEngine.U2D.Animation.GpuDeformationSystem",true);
            const BindingFlags flags=BindingFlags.Static|BindingFlags.NonPublic;
            var create=type.GetMethod("CreateFallbackBuffer",flags);
            var clear=type.GetMethod("ClearFallbackBuffer",flags);
            var field=type.GetField("s_FallbackBuffer",flags);
            var quitting=typeof(Application).GetField("quitting",flags);
            if(create==null || clear==null || field==null || quitting==null)
                throw new InvalidOperationException("Изменился контракт диагностики закреплённого пакета 2D Animation.");
            try
            {
                for(int iteration=0;iteration<2;iteration++)
                {
                    create.Invoke(null,null);create.Invoke(null,null);
                    var buffer=(ComputeBuffer)field.GetValue(null);
                    var callbacks=((Delegate)quitting.GetValue(null))?.GetInvocationList()
                        .Where(callback=>callback.Method==clear).ToArray()??Array.Empty<Delegate>();
                    if(buffer==null || !buffer.IsValid() || callbacks.Length!=1)
                        throw new InvalidOperationException("2D Animation выделяет fallback ComputeBuffer без единственного обработчика завершения приложения.");
                    callbacks[0].DynamicInvoke();
                    if(buffer.IsValid() || field.GetValue(null)!=null)
                        throw new InvalidOperationException("Обработчик завершения 2D Animation не освободил fallback ComputeBuffer.");
                    clear.Invoke(null,null);
                }
                Debug.Log("Fallback-буфер 2D Animation: PASS — повторное создание, один обработчик выхода, фактическое освобождение и повторный cleanup.");
            }
            finally {clear.Invoke(null,null);}
        }
    }
}
