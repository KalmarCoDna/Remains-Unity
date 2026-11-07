// Optional actual-Unity reference. Temporarily copy into Assets/Editor, execute
// Remains.Editor.RoomEditorNativeShaderProbe.Run, then remove the temporary copy.
// It does not build a Player or load a gameplay scene.
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;

namespace Remains.Editor
{
    public static class RoomEditorNativeShaderProbe
    {
        [Serializable] private sealed class Sample {public string name;public Color color;}
        [Serializable] private sealed class Report {public string renderer,version;public List<Sample> samples=new List<Sample>();}
        public static void Run()
        {
            const int Side=32;const float Size=4;
            if(SystemInfo.graphicsDeviceType==GraphicsDeviceType.Null)throw new InvalidOperationException("Graphics context required.");
            var resources=new List<UnityEngine.Object>();
            var target=new RenderTexture(Side,Side,0,RenderTextureFormat.ARGBFloat,RenderTextureReadWrite.Linear){filterMode=FilterMode.Point};resources.Add(target);
            if(!target.Create())throw new InvalidOperationException("Reference render target failed.");
            var pixels=new Texture2D(Side,Side,TextureFormat.RGBAFloat,false,true);resources.Add(pixels);
            var quad=new Mesh();resources.Add(quad);
            quad.vertices=new[]{new Vector3(0,0),new Vector3(Size,0),new Vector3(0,Size),new Vector3(Size,Size)};
            quad.uv=new[]{Vector2.zero,Vector2.right,Vector2.up,Vector2.one};quad.colors=new[]{Color.white,Color.white,Color.white,Color.white};quad.triangles=new[]{0,2,1,1,2,3};
            var previous=RenderTexture.active;var report=new Report{renderer=SystemInfo.graphicsDeviceName,version=Application.unityVersion};
            Texture2D Texture(int w,int h,Color32[] colors,bool linear=false)
            {
                var texture=new Texture2D(w,h,TextureFormat.RGBA32,false,linear){filterMode=FilterMode.Point,wrapMode=TextureWrapMode.Repeat};
                resources.Add(texture);texture.SetPixels32(colors);texture.Apply();return texture;
            }
            var white=Texture(1,1,new[]{new Color32(255,255,255,255)});
            var pattern=Texture(3,1,new[]{new Color32(255,0,0,255),new Color32(0,255,0,255),new Color32(0,0,255,255)});
            var baseColor=Texture(1,1,new[]{new Color32(80,120,160,255)});
            var sourceColor=Texture(1,1,new[]{new Color32(200,60,30,255)});
            Material Material(string resource,Texture2D image)
            {
                var shader=Resources.Load<Shader>(resource);if(shader==null)throw new InvalidOperationException("Missing game shader "+resource);
                var material=new Material(shader);resources.Add(material);material.SetTexture("_MainTex",image);material.SetColor("_Color",Color.white);
                material.DisableKeyword("USE_SHAPE_LIGHT_TYPE_0");material.DisableKeyword("USE_SHAPE_LIGHT_TYPE_1");material.SetFloat("_EraseAffected",0);material.SetFloat("_ReceiveProjectedShadow",0);return material;
            }
            void Draw(List<Material> materials)
            {
                using(var cmd=new CommandBuffer())
                {
                    cmd.SetRenderTarget(target);cmd.ClearRenderTarget(false,true,Color.black);
                    cmd.SetGlobalFloat("_RemainsRadianceEnabled",0);cmd.SetGlobalFloat("_RemainsIndirectEnabled",0);cmd.SetGlobalFloat("_RemainsEraseEnabled",0);cmd.SetGlobalFloat("_HDREmulationScale",1);
                    cmd.SetViewProjectionMatrices(Matrix4x4.identity,GL.GetGPUProjectionMatrix(Matrix4x4.Ortho(0,Size,0,Size,-1,1),false));
                    foreach(var material in materials)cmd.DrawMesh(quad,Matrix4x4.identity,material,0,0);
                    Graphics.ExecuteCommandBuffer(cmd);
                }
                RenderTexture.active=target;pixels.ReadPixels(new Rect(0,0,Side,Side),0,0);pixels.Apply();
            }
            void Sample(string name,int x,int y)=>report.samples.Add(new Sample{name=name,color=pixels.GetPixel(x,Side-1-y)});
            try
            {
                var terrain=Material("Presentation/WorldTerrain",white);terrain.SetTexture("_MaterialTex",pattern);terrain.SetFloat("_TexelsPerMeter",8);terrain.SetVector("_MaterialOrigin",new Vector4(0,Size,0,0));
                Draw(new List<Material>{terrain});Sample("phase-x1",1,10);Sample("phase-x8",8,10);Sample("phase-x11",11,10);
                foreach(int blend in new[]{0,1,2})
                {
                    var background=Material("Presentation/WorldSprite",baseColor);background.SetFloat("_SrcBlend",(float)BlendMode.SrcAlpha);background.SetFloat("_DstBlend",(float)BlendMode.OneMinusSrcAlpha);
                    var foreground=Material("Presentation/WorldSprite",sourceColor);foreground.SetColor("_Color",new Color(1,1,1,.5f));foreground.SetFloat("_BlendMode",blend);
                    foreground.SetFloat("_SrcBlend",(float)(blend==0?BlendMode.SrcAlpha:BlendMode.DstColor));foreground.SetFloat("_DstBlend",(float)(blend==0?BlendMode.OneMinusSrcAlpha:blend==1?BlendMode.Zero:BlendMode.SrcColor));
                    Draw(new List<Material>{background,foreground});Sample("blend-"+blend,12,12);
                }
                foreach(var sample in report.samples)if(!float.IsFinite(sample.color.r)||!float.IsFinite(sample.color.g)||!float.IsFinite(sample.color.b))throw new InvalidOperationException("Nonfinite native shader result "+sample.name);
                string path=Environment.GetEnvironmentVariable("ROOM_EDITOR_NATIVE_REPORT");if(string.IsNullOrEmpty(path))throw new InvalidOperationException("Set ROOM_EDITOR_NATIVE_REPORT to an absolute report path.");
                File.WriteAllText(path,JsonUtility.ToJson(report,true));Debug.Log("ROOM_EDITOR_NATIVE_SHADER_PROBE_PASS: actual WorldTerrain/WorldSprite shader phase and blend samples.");
            }
            finally
            {
                RenderTexture.active=previous;target.Release();foreach(var resource in resources)if(resource!=null)UnityEngine.Object.DestroyImmediate(resource);
            }
        }
    }
}
