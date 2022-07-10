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
using Sandbox.Game.World;
using System.Reflection;
using VRage.OpenVRWrapper;
using VRage.Plugins;

namespace Sevr.SE_VR
{
    public class Main : IPlugin
    {
        public void Init(object gameInstance)
        {
            Harmony harmony = new Harmony("SevrStereo");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            MySession.AfterLoading += Loaded;
        }

        public void Update()
        {
            // Update subsystems
            //Input.Update();
        }

        public void Loaded()
        {
            new MyOpenVR();
        }

        public void Dispose()
        {

        }
    }
}
