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
    public void Draw(Vector2 size)
    {
        Vector2 size1 = size * 0.8f;
        ImGui.SetCursorPos(size * 0.1f);
        IDalamudTextureWrap textureWrap;
        if (!Core.Resolve<MemApiIcon>().GetActionTexture(7557U, out textureWrap))
            return;
        ImGui.Image(textureWrap.ImGuiHandle, size1);
    }

    public void DrawExternal(Vector2 size, bool isActive)
    {
        SpellHelper.DrawSpellInfo(7557U.GetSpell(), size, isActive);
    }

    public int Check() => 0;

    public void Run()
    {
        Core.Resolve<MemApiMove>().CancelMove();
    }
}
