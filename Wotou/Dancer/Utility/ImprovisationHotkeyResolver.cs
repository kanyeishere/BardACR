using System.Numerics;
using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.Extension;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Dalamud.Interface.Textures.TextureWraps;
using ImGuiNET;
using Wotou.Dancer.Data;

namespace Wotou.Dancer;

public class ImprovisationHotkeyResolver : IHotkeyResolver
{
    private uint SpellId;

    public ImprovisationHotkeyResolver()
    {
        this.SpellId = DancerDefinesData.Spells.Improvisation;
    }
    
    public void Draw(Vector2 size)
    {
        uint id = Core.Resolve<MemApiSpell>().CheckActionChange(this.SpellId);
        Vector2 size1 = size * 0.8f;
        ImGui.SetCursorPos(size * 0.1f);
        IDalamudTextureWrap textureWrap;
        if (!Core.Resolve<MemApiIcon>().GetActionTexture(id, out textureWrap))
            return;
        ImGui.Image(textureWrap.ImGuiHandle, size1);
    }

    public void DrawExternal(Vector2 size, bool isActive)
    {
    }

    public int Check() => 0;

    public void Run()
    {
        Improvisation();
        return;
    }
    
    private static void Improvisation()
    {
        if (!DancerDefinesData.Spells.Improvisation.CoolDownInGCDs(0) || 
            Core.Resolve<JobApi_Dancer>().IsDancing)
            return;
        if (AI.Instance.BattleData.NextSlot == null)
            AI.Instance.BattleData.NextSlot = new Slot();
        AI.Instance.BattleData.NextSlot.Add(DancerDefinesData.Spells.Improvisation.GetSpell());
        AI.Instance.BattleData.NextSlot.Add(DancerDefinesData.Spells.ImprovisationFinish.GetSpell());
    }
}