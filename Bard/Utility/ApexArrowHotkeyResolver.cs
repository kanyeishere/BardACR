using System.Numerics;
using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.Helper;
using AEAssist.JobApi;
using AEAssist.MemoryApi;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Bindings.ImGui;
using Wotou.Bard.Data;

namespace Wotou.Bard.Utility;

public class ApexArrowHotkeyResolver: IHotkeyResolver
{
    private uint SpellId;
    private SpellTargetType TargetType;
    
    public ApexArrowHotkeyResolver(uint spellId, SpellTargetType targetType) 
    {
        SpellId = spellId;
        TargetType = targetType;
    }
    
    public void Draw(Vector2 size)
    {
        var id = Core.Resolve<MemApiSpell>().CheckActionChange(SpellId);
        var size1 = size * 0.8f;
        ImGui.SetCursorPos(size * 0.1f);
        if (!Core.Resolve<MemApiIcon>().GetActionTexture(id, out var textureWrap))
            return;
        ImGui.Image(new ImTextureID(textureWrap.ImGuiHandle), size1);
        
        if (Core.Resolve<JobApi_Bard>().SoulVoice < 20)
        {
            // Use ImGui.GetItemRectMin() and ImGui.GetItemRectMax() for exact icon bounds
            var overlayMin = ImGui.GetItemRectMin();
            var overlayMax = ImGui.GetItemRectMax();

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
            AI.Instance.BattleData.NextSlot ??= new Slot();
            AI.Instance.BattleData.NextSlot.Add(spell);
        }
        else
        {
            var slot = new Slot();
            slot.Add(spell);
            if (spell.IsAbility())
                AI.Instance.BattleData.HighPrioritySlots_OffGCD.Enqueue(slot);
            else
                AI.Instance.BattleData.HighPrioritySlots_GCD.Enqueue(slot);
        }
    }
}