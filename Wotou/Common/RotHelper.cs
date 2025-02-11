#nullable disable
using AEAssist.Helper;
using FFXIVClientStructs.FFXIV.Client.Game.Control;

namespace Wotou.Common {
  public class RotHelper
  {
    internal static unsafe float GetCameraRotation()
    {
      //int num1 = RotHelper.RaptureAtkModule->AtkModule.AtkArrayDataHolder.NumberArrays[24]->IntArray[3];
      //int num2 = Math.Sign(num1) == -1 ? -1 : 1;
      // return (float) (Math.Abs((double) num1 * (Math.PI / 180.0)) - Math.PI) * (float) num2;
      var cameraRotation = ((CameraEx*)CameraManager.Instance()->Camera)->DirH;
      return cameraRotation;

    }
  }
}
