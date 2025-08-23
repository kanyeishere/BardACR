using System.Numerics;
using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Dalamud.Interface.Textures.TextureWraps;
using ImGuiNET;
using Wotou.Bard.Data;

namespace Wotou.Bard.Utility;

public class ApexArrowHotkeyResolver: IHotkeyResolver
{
    public uint SpellId;
    public SpellTargetType TargetType;
    
    public ApexArrowHotkeyResolver(uint spellId, SpellTargetType targetType) 
    {
        this.SpellId = spellId;
        this.TargetType = targetType;
    }
    
    public void Draw(Vector2 size)
    {
        uint id = Core.Resolve<MemApiSpell>().CheckActionChange(SpellId);
        Vector2 size1 = size * 0.8f;
        ImGui.SetCursorPos(size * 0.1f);
        IDalamudTextureWrap textureWrap;
        if (!Core.Resolve<MemApiIcon>().GetActionTexture(id, out textureWrap))
            return;
        ImGui.Image(textureWrap.ImGuiHandle, size1);
        
        if (Core.Resolve<JobApi_Bard>().SoulVoice < 20)
        {
            // Use ImGui.GetItemRectMin() and ImGui.GetItemRectMax() for exact icon bounds
            Vector2 overlayMin = ImGui.GetItemRectMin();
            Vector2 overlayMax = ImGui.GetItemRectMax();

            // Draw a grey overlay over the icon
            ImGui.GetWindowDrawList().AddRectFilled(
                overlayMin, 
                overlayMax, 
                ImGui.ColorConvertFloat4ToU32(new Vector4(0, 0, 0, 0.5f))); // 50% transparent grey
        }
    }

    public void DrawExternal(Vector2 size, bool isActive)
    {
        SpellHelper.DrawSpellInfo(Core.Resolve<MemApiSpell>().CheckActionChange(this.SpellId).GetSpell(), size, isActive);
    }

    public int Check()
    {
        if (Core.Resolve<JobApi_Bard>().SoulVoice >= 20)
            return 1;
        return -1;
    }


    public new void Run()
    {
        // var spell = Core.Resolve<MemApiSpell>().CheckActionChange(this.SpellId).GetSpell(this.TargetType);
        var spell = BardUtil.GetSmartAoeSpell(this.SpellId, 1);
        if (!BardBattleData.Instance.HotkeyUseHighPrioritySlot)
        {
            if (AI.Instance.BattleData.NextSlot == null)
                AI.Instance.BattleData.NextSlot = new Slot();
            AI.Instance.BattleData.NextSlot.Add(spell);
        }
        else
        {
            Slot slot = new Slot();
            slot.Add(spell);
            if (spell.IsAbility())
                AI.Instance.BattleData.HighPrioritySlots_OffGCD.Enqueue(slot);
            else
                AI.Instance.BattleData.HighPrioritySlots_GCD.Enqueue(slot);
        }
    }
}