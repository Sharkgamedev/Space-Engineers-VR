using System.Runtime.InteropServices;
using Valve.VR;
using VRage.OpenVRWrapper;

namespace Sevr
{
    public class Input
    {
        public static void Update()
        {
            if (MyOpenVR.Static == null || MyOpenVR.VRInput == null) return;

            MyOpenVR.VRInput.UpdateActionState(MyOpenVR.InputActions.ActionSet_Main, (uint)(Marshal.SizeOf(typeof(VRActiveActionSet_t))));
        }
    }
}
