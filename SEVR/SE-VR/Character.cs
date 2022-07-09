using HarmonyLib;
using Sandbox.Game.Entities.Character;
using System.Reflection;
using VRage.ObjectBuilders;
using VRage.OpenVRWrapper;
using VRageMath;

namespace Sevr
{
    public class Character
    {
        [HarmonyPatch]
        /// <summary>
        /// Disables rendering of the players body for VR
        /// </summary>
        public static class BodyDrawPatch
        {
            [HarmonyTargetMethod]
            static MethodBase TargetMethod() => AccessTools.Method(typeof(MyCharacter), nameof(MyCharacter.Init), new System.Type[] { typeof(MyObjectBuilder_EntityBase) });

            [HarmonyPostfix]
            static void Postfix(MyCharacter __instance) => __instance.Render.NeedsDraw = false; // Don't show the player
        }

        [HarmonyPatch]
        /// <summary>
        /// Shifts the world position of the players ingame head by the worldposition of the VR headset
        /// </summary>
        public static class HeadMatrixPatch
        {
            [HarmonyTargetMethod]
            static MethodBase TargetMethod() => AccessTools.Method(typeof(MyCharacter), nameof(MyCharacter.GetHeadMatrix));

            [HarmonyPostfix]
            static void Postfix(MyCharacter __instance, ref MatrixD __result) => __result = MyOpenVR.HeadsetMatrixD * __instance.WorldMatrix;
        }
    }
}
