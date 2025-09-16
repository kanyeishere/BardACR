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
using Dalamud.Bindings.ImGui;
using Wotou.Dancer.Data;
using Wotou.Dancer.Setting;

namespace Wotou.Dancer.Utility;

public class ClosedPositionHotkeyResolver : IHotkeyResolver
{
    private uint SpellId;
    private int Index;

    public ClosedPositionHotkeyResolver(int index)
    {
        SpellId = DancerDefinesData.Spells.ClosedPosition;
        Index = index;
    }
    
    public void Draw(Vector2 size)
    {
        var id = SpellId;
        if (Core.Me.HasLocalPlayerAura(DancerDefinesData.Buffs.ClosedPosition))
            id = DancerDefinesData.Spells.Ending;
        
        var size1 = size * 0.8f;
        ImGui.SetCursorPos(size * 0.1f);
        if (!Core.Resolve<MemApiIcon>().GetActionTexture(id, out var textureWrap))
            return;
        ImGui.Image(textureWrap.Handle, size1);
        
        if (SpellId.GetSpell().Cooldown.TotalMilliseconds > 0 || Core.Resolve<JobApi_Dancer>().IsDancing)
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
        /*SpellTargetType targetType = Index >= 1 && Index <= 8 ? (SpellTargetType)(Index + 3) : SpellTargetType.Target;
        SpellHelper.DrawSpellInfo(Core.Resolve<MemApiSpell>().CheckActionChange(this.SpellId).GetSpell(targetType), size, isActive);*/
    }

    public int Check()
    {
        if (DancerDefinesData.Spells.ClosedPosition.GetSpell().Cooldown.TotalMilliseconds != 0 || 
            Core.Resolve<JobApi_Dancer>().IsDancing)
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
        if (DancerDefinesData.Spells.ClosedPosition.GetSpell().Cooldown.TotalMilliseconds != 0 || 
            Core.Resolve<JobApi_Dancer>().IsDancing)
            return;
        
        if (!DancerBattleData.Instance.HotkeyUseHighPrioritySlot)
        {
            AI.Instance.BattleData.NextSlot ??= new Slot();
            if (Core.Me.HasLocalPlayerAura(DancerDefinesData.Buffs.ClosedPosition))
            {
                AI.Instance.BattleData.NextSlot.Add(DancerDefinesData.Spells.Ending.GetSpell());
                AI.Instance.BattleData.NextSlot.Add(new Spell(DancerDefinesData.Spells.ClosedPosition,
                    partyMembers[index]));
            }
            else
            {
                AI.Instance.BattleData.NextSlot.Add(new Spell(DancerDefinesData.Spells.ClosedPosition,
                    partyMembers[index]));
            }
        }
        else
        {
            var slot = new Slot();
            if (Core.Me.HasLocalPlayerAura(DancerDefinesData.Buffs.ClosedPosition))
            {
                slot.Add(DancerDefinesData.Spells.Ending.GetSpell());
                slot.Add(new Spell(DancerDefinesData.Spells.ClosedPosition, partyMembers[index]));
            }
            else
            {
                slot.Add(new Spell(DancerDefinesData.Spells.ClosedPosition, partyMembers[index]));
            }
            AI.Instance.BattleData.HighPrioritySlots_OffGCD.Enqueue(slot);
        }
        
        if (DancerSettings.Instance.UseDancePartnerMacro)
        {
            // 将多行输入分割为字符串数组，每个元素是一行
            var macroLines = DancerSettings.Instance.DancePartnerMacroText.Split('\n');

            foreach (var line in macroLines)
            {
                // 确保不发送空行
                if (!string.IsNullOrWhiteSpace(line))
                {
                    var number = index + 1;
                    // 替换 <t> 为目标的名称并发送每行消息
                    var formattedLine = line.Replace("<t>", "<" + number + ">");
                    ChatHelper.SendMessage(formattedLine);
                }
            }
        }
    }
}