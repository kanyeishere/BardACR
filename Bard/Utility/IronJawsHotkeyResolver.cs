using System.Numerics;
using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Bindings.ImGui;
using Wotou.Bard.Data;

namespace Wotou.Bard.Utility;

public class IronJawsHotkeyResolver: IHotkeyResolver
{
    private uint SpellId;
    private SpellTargetType TargetType;
    
    public IronJawsHotkeyResolver(uint spellId, SpellTargetType targetType) 
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
        ImGui.Image(textureWrap.Handle, size1);
    }

    public void DrawExternal(Vector2 size, bool isActive)
    {
        SpellHelper.DrawSpellInfo(Core.Resolve<MemApiSpell>().CheckActionChange(this.SpellId).GetSpell(), size, isActive);
    }

    public int Check()
    {
        return 0;
    }


    public new void Run()
    {
        var spell = Core.Resolve<MemApiSpell>().CheckActionChange(this.SpellId).GetSpell(this.TargetType);
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