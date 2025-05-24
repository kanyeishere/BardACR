using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.Module.Target;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.Trigger.Node;
using AEAssist.Extension;
using Dalamud.Game.ClientState.Objects.Types;

namespace ScriptTest
{
    public class SelectCrystalByRole : ITriggerScript
    {
        private IBattleChara TopCrystal = null;
        private IBattleChara BottomCrystal = null;
        private IBattleChara LeftCrystal = null;
        private IBattleChara RightCrystal = null;
        private ulong? TargetGameObjectId = null;

        public bool Check(ScriptEnv scriptEnv, ITriggerCondParams condParams)
        {
            // 获取所有光水晶 (DataId 17827)
            var crystals = TargetMgr.Instance.Enemys.Values
                .Where(e => e.DataId == 17827)
                .OrderBy(e => e.Position.Z) // 先按 Z 排序
                .ThenBy(e => e.Position.X) // 再按 X 排序
                .ToList();

            if (crystals.Count == 4)
            {
                // 识别四个水晶位置
                TopCrystal = crystals.OrderBy(e => e.Position.Z).First();  // Z 最小 → 上
                BottomCrystal = crystals.OrderBy(e => e.Position.Z).Last(); // Z 最大 → 下
                LeftCrystal = crystals.OrderBy(e => e.Position.X).First();  // X 最小 → 左
                RightCrystal = crystals.OrderBy(e => e.Position.X).Last();  // X 最大 → 右

                // 角色职责 → 目标分配
                IBattleChara targetCrystal = null;
                switch (AI.Instance.PartyRole)
                {
                    case "MT":
                    case "D3":
                        targetCrystal = TopCrystal;
                        break;
                    case "ST":
                    case "D4":
                        targetCrystal = RightCrystal;
                        break;
                    case "D1":
                    case "H1":
                        targetCrystal = LeftCrystal;
                        break;
                    case "D2":
                    case "H2":
                        targetCrystal = BottomCrystal;
                        break;
                    default:
                        targetCrystal = TopCrystal; // 默认d3
                        break;
                }

                if (targetCrystal != null)
                {
                    Core.Me.SetTarget(targetCrystal);
                    TargetGameObjectId = targetCrystal.GameObjectId;
                }
                return false;
            }
            
            if (TargetGameObjectId != null &&crystals.Find(c => c.GameObjectId == TargetGameObjectId) == null)
            {
                // 选择大水晶
                // Core.Me.SetTarget(大水晶);
                return true;
            }
            return false;
        }
    }
}