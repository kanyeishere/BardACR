using System.Numerics;
using AEAssist;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Dalamud.Interface.Textures.TextureWraps;
using ImGuiNET;

namespace Wotou.Common;

public class StopMoveHotkeyResolver : IHotkeyResolver
{
    private string imagePath = "../../ACR/Wotou/Resources/stop.png";
    public void Draw(Vector2 size)
    {
        Vector2 size1 = size * 0.8f;
        ImGui.SetCursorPos(size * 0.1f);
        if (Core.Resolve<MemApiIcon>().TryGetTexture(imagePath, out var textureWrap))
        {
            ImGui.Image(textureWrap.ImGuiHandle, size1);
        }
    }

    public void DrawExternal(Vector2 size, bool isActive)
    {
    }

    public int Check() => 0;

    public void Run()
    {
        Core.Resolve<MemApiMove>().CancelMove();
    }
}
