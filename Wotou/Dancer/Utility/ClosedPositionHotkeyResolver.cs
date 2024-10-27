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
        uint id = SpellId;
        if (Core.Me.HasLocalPlayerAura(DancerDefinesData.Buffs.ClosedPosition))
            id = DancerDefinesData.Spells.Ending;
        
        Vector2 size1 = size * 0.8f;
        ImGui.SetCursorPos(size * 0.1f);
        IDalamudTextureWrap textureWrap;
        if (!Core.Resolve<MemApiIcon>().GetActionTexture(id, out textureWrap))
            return;
        ImGui.Image(textureWrap.ImGuiHandle, size1);
        
        if (SpellId.GetSpell().Cooldown.Seconds > 0)
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
        
        var cooldownRemaining = SpellId.GetSpell().Cooldown.Seconds;
        if (cooldownRemaining > 0)
        {
            // Convert cooldown to seconds and format as string
            string cooldownText = cooldownRemaining.ToString();

            // 计算文本位置，向左下角偏移
            Vector2 textPos = ImGui.GetItemRectMin();
            textPos.X -= 1; // 向左移动一点
            textPos.Y += size1.Y - ImGui.CalcTextSize(cooldownText).Y + 5; // 向下移动一点

            // 绘制冷却时间文本
            ImGui.GetWindowDrawList().AddText(textPos, ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, 1)), cooldownText);
        }
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