using System.Numerics;
using AEAssist;
using AEAssist.CombatRoutine;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Dalamud.Interface.Textures.TextureWraps;
using Dalamud.Bindings.ImGui;
using Wotou.Common;
using Wotou.Dancer.Data;

namespace Wotou.Dancer.Utility;

public class EnAvantHotkeyResolver: IHotkeyResolver
{
    private uint SpellId = DancerDefinesData.Spells.EnAvant;
    private float Rotation;
    
    public EnAvantHotkeyResolver(float rotation = 0)
    {
        this.Rotation = rotation;
    }
    
    public void Draw(Vector2 size)
    {
        var id = Core.Resolve<MemApiSpell>().CheckActionChange(SpellId);
        var size1 = size * 0.8f;
        ImGui.SetCursorPos(size * 0.1f);
        if (!Core.Resolve<MemApiIcon>().GetActionTexture(id, out var textureWrap))
            return;
        ImGui.Image(new ImTextureID(textureWrap.Handle), size1);
        // Check if skill is on cooldown and apply grey overlay if true
        
        if (!Core.Resolve<MemApiSpell>().CheckActionChange(SpellId).GetSpell().IsReadyWithCanCast())
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
        SpellHelper.DrawSpellInfo(Core.Resolve<MemApiSpell>().CheckActionChange(this.SpellId).GetSpell(), size, isActive);
    }

    public int Check()
    {
        // return 1;
        return SpellId.GetSpell().IsReadyWithCanCast() ? 0 : -1;
    }

    public void Run()
    {
        var rotation = RotHelper.GetCameraRotation() + Rotation;

        Core.Resolve<MemApiMove>().SetRot(rotation);
        /*float moveDistance = 0.5f; // **移动的距离（可以调整）**
        Vector3 currentPos = Core.Me.Position;
        Vector3 targetPos = new Vector3(
            currentPos.X + moveDistance * MathF.Sin(rotation), // X 轴计算
            currentPos.Y,  // Y 轴不变（地面高度）
            currentPos.Z + moveDistance * MathF.Cos(rotation)  // Z 轴计算
        );
        
        Core.Resolve<MemApiMove>().MoveToTarget(targetPos);*/
        //  return; 
        var spell = Core.Resolve<MemApiSpell>().CheckActionChange(this.SpellId).GetSpell(SpellTargetType.Self);
        if (!DancerBattleData.Instance.HotkeyUseHighPrioritySlot)
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