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

public class ClosedPositionHotkeyResolver : IHotkeyResolver
{
    private uint SpellId;
    private int Index;

    public ClosedPositionHotkeyResolver(int index)
    {
        this.SpellId = DancerDefinesData.Spells.ClosedPosition;
        this.Index = index;
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
        ClosedPosition(index: this.Index);
        return;
    }
    
    private static void ClosedPosition(int index)
    {
        var partyMembers = PartyHelper.Party;
        if (partyMembers.Count < index + 1)
            return;
        if (!DancerDefinesData.Spells.ClosedPosition.CoolDownInGCDs(0) || 
            Core.Resolve<JobApi_Dancer>().IsDancing)
            return;
        if (AI.Instance.BattleData.NextSlot == null)
            AI.Instance.BattleData.NextSlot = new Slot();
        if (Core.Me.HasLocalPlayerAura(DancerDefinesData.Buffs.ClosedPosition))
        {
            AI.Instance.BattleData.NextSlot.Add(DancerDefinesData.Spells.Ending.GetSpell());
            AI.Instance.BattleData.NextSlot.Add(new Spell(DancerDefinesData.Spells.ClosedPosition, partyMembers[index]));
        }
        else
        {
            AI.Instance.BattleData.NextSlot.Add(new Spell(DancerDefinesData.Spells.ClosedPosition, partyMembers[index]));
        }
    }
}