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

public class WardensPaeanHotkeyResolver : IHotkeyResolver
{
    private uint SpellId;
    private int Index;

    public WardensPaeanHotkeyResolver(int index)
    {
        SpellId = BardDefinesData.Spells.TheWardensPaean;
        Index = index;
    }
    
    public void Draw(Vector2 size)
    {
        var id = SpellId;
        var size1 = size * 0.8f;
        ImGui.SetCursorPos(size * 0.1f);
        if (!Core.Resolve<MemApiIcon>().GetActionTexture(id, out var textureWrap))
            return;
        ImGui.Image(new ImTextureID(textureWrap.Handle), size1);
        
        if (SpellId.GetSpell().Cooldown.TotalMilliseconds > 0)
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
        
        var cooldownRemaining = SpellId.GetSpell().Cooldown.TotalMilliseconds / 1000;
        if (cooldownRemaining > 0)
        {
            // Convert cooldown to seconds and format as string
            var cooldownText = Math.Ceiling(cooldownRemaining).ToString();

            // 计算文本位置，向左下角偏移
            var textPos = ImGui.GetItemRectMin();
            textPos.X -= 1; // 向左移动一点
            textPos.Y += size1.Y - ImGui.CalcTextSize(cooldownText).Y + 5; // 向下移动一点

            // 绘制冷却时间文本
            ImGui.GetWindowDrawList().AddText(textPos, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, 1)), cooldownText);
        }
    }

    public void DrawExternal(Vector2 size, bool isActive)
    {
        SpellHelper.DrawSpellInfo(Core.Resolve<MemApiSpell>().CheckActionChange(this.SpellId).GetSpell(), size, isActive);
    }

    public int Check()
    {
        if (!BardDefinesData.Spells.TheWardensPaean.IsUnlockWithCDCheck())
            return -1;
        return 0;
    }

    public void Run()
    {
        ClosedPosition(index: this.Index);
        return;
    }
    
    private void ClosedPosition(int index)
    {
        var partyMembers = PartyHelper.Party;
        if (partyMembers.Count < index + 1)
            return;
        if (!BardDefinesData.Spells.TheWardensPaean.IsUnlockWithCDCheck())
            return;
        
        if (!BardBattleData.Instance.HotkeyUseHighPrioritySlot)
        {
            AI.Instance.BattleData.NextSlot ??= new Slot();
            AI.Instance.BattleData.NextSlot.Add(new Spell(BardDefinesData.Spells.TheWardensPaean,
                    partyMembers[index]));
        }
        else
        {
            var slot = new Slot();
            slot.Add(new Spell(BardDefinesData.Spells.TheWardensPaean, partyMembers[index]));
            AI.Instance.BattleData.HighPrioritySlots_OffGCD.Enqueue(slot);
        }
    }
}