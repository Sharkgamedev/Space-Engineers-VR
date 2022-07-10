/*   Space Engineers VR Mod
*    Copyright (C) 2022 Owen Silva
*
*    This program is free software: you can redistribute it and/or modify
*    it under the terms of the GNU General Public License as published by
*    the Free Software Foundation, either version 3 of the License, or
*    (at your option) any later version.
*
*    This program is distributed in the hope that it will be useful,
*    but WITHOUT ANY WARRANTY; without even the implied warranty of
*    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
*    GNU General Public License for more details.
*
*    You should have received a copy of the GNU General Public License
*    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

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
