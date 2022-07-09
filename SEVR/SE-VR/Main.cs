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
            Input.Update();
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
