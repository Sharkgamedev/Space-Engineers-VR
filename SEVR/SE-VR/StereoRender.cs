using HarmonyLib;
using Sandbox.Game.World;
using SharpDX.Direct3D11;
using System.Reflection;
using VRage.OpenVRWrapper;
using VRageRender.Messages;

namespace Sevr
{
    public enum Eye
    {
        Left,
        Right
    }

    public class StereoRender
    {
        [HarmonyPatch]
        public static class StereoRenderingPatch
        {
            static MethodInfo DrawGameScene;
            static MethodInfo SetupCameraMatrices;

            static PropertyInfo BackbuffResource;
            static PropertyInfo BackBufProp;

            [HarmonyTargetMethod]
            static MethodBase TargetMethod()
            {
                // Methods
                DrawGameScene = AccessTools.Method(AccessTools.TypeByName("VRageRender.MyRender11"), "DrawGameScene");
                SetupCameraMatrices = AccessTools.Method(AccessTools.TypeByName("VRageRender.MyRender11"), "SetupCameraMatrices");

                // Properties
                BackBufProp = AccessTools.Property(AccessTools.TypeByName("VRageRender.MyRender11"), "Backbuffer");
                BackbuffResource = AccessTools.Property(AccessTools.TypeByName("VRage.Render11.Resources.MyBackbuffer"), "Resource");

                return AccessTools.Method(AccessTools.TypeByName("VRageRender.MyRender11"), "DrawScene");
            }

            [HarmonyPrefix]
            static bool Prefix()
            {
                if (MySector.MainCamera == null || MyOpenVR.Static == null) return true; // No idea what to do in this case just return to normal rendering
                MyOpenVR.WaitForNextStart();

                RenderEye(Eye.Left);
                RenderEye(Eye.Right);

                // Don't render the game as normal
                return false;
            }

            static void RenderEye(Eye eye)
            {
                SetupViewForEye(eye);

                // Render Scene
                object[] input = new object[] { BackBufProp.GetValue(null), null };
                DrawGameScene.Invoke(null, input);

                // Submit it
                DisplayEye(eye, input[0]);
            }

            static void DisplayEye(Eye eye, object backbufferObject)
            {
                Resource backbuffer = BackbuffResource.GetValue(backbufferObject) as Resource;
                MyOpenVR.Static.DisplayEye(
                    backbuffer.NativePointer,
                   eye == Eye.Left ? Valve.VR.EVREye.Eye_Left : Valve.VR.EVREye.Eye_Right
                   );
            }

            static void SetupViewForEye(Eye eye)
            {
                MyRenderMessageSetCameraViewMatrix message = new MyRenderMessageSetCameraViewMatrix();

                message.ViewMatrix = MySector.MainCamera.ViewMatrix;

                VRageMath.Matrix viewHMDat0 = MyOpenVR.ViewHMD;
                viewHMDat0.M14 = 0;
                viewHMDat0.M24 = 0;
                viewHMDat0.M34 = 0;
                viewHMDat0.M41 = 0;
                viewHMDat0.M42 = 0;
                viewHMDat0.M43 = 0;
                viewHMDat0.M44 = 1;

                message.ViewMatrix = message.ViewMatrix * viewHMDat0;
                message.CameraPosition = MySector.MainCamera.Position;
                message.FarPlane = MySector.MainCamera.FarPlaneDistance;
                message.FarFarPlane = MySector.MainCamera.FarFarPlaneDistance;
                message.FOV = 0;
                message.FOVForSkybox = 0;
                message.LastMomentUpdateIndex = -1;
                message.NearPlane = MySector.MainCamera.NearPlaneDistance;
                message.ProjectionMatrix = MyOpenVR.GetEyeProjection((Valve.VR.EVREye)eye, MySector.MainCamera.NearPlaneDistance, MySector.MainCamera.FarPlaneDistance); //MyOpenVR.GetEyeProjection((Valve.VR.EVREye)eye, MySector.MainCamera.NearPlaneDistance, MySector.MainCamera.FarPlaneDistance);
                message.ProjectionFarMatrix = MyOpenVR.GetEyeProjection((Valve.VR.EVREye)eye, MySector.MainCamera.NearPlaneDistance, MySector.MainCamera.FarFarPlaneDistance); //MyOpenVR.GetEyeProjection((Valve.VR.EVREye)eye, MySector.MainCamera.NearPlaneDistance, MySector.MainCamera.FarPlaneDistance);//MySector.MainCamera.ProjectionMatrix;
                message.Smooth = false;
                message.ProjectionOffsetX = eye == Eye.Left ? -MyOpenVR.Ipd_2 : MyOpenVR.Ipd_2;
                message.ProjectionOffsetY = 0;

                MySector.MainCamera.UpdateScreenSize(MyOpenVR.ViewportForEye((Valve.VR.EVREye)eye));
                SetupCameraMatrices.Invoke(null, new object[] { message });
            }
        }
    }
}
