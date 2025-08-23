using System.Numerics;
using AEAssist;
using AEAssist.CombatRoutine.Module;
using AEAssist.CombatRoutine.Module.Target;
using AEAssist.CombatRoutine.Trigger;
using AEAssist.CombatRoutine.Trigger.Node;
using AEAssist.Extension;
using AEAssist.MemoryApi;
using Dalamud.Game.ClientState.Objects.Types;

namespace ScriptTest
{
    public class TestScript : ITriggerScript
    {
        private readonly List<Vector2> _triangleVertices = new(); // 保存三角形顶点
        private bool _verticesInitialized = false;
        private Vector2 _basePoint1; // 底边的第一个点
        private Vector2 _basePoint2; // 底边的第二个点
        private Vector2 _topPoint; // 顶点

        public bool Check(ScriptEnv scriptEnv, ITriggerCondParams condParams)
        {
            var nearbyGameObjects = new Dictionary<uint, IGameObject>();

            // 获取附近对象
            Core.Resolve<MemApiTarget>().GetNearbyGameObjects(50, nearbyGameObjects);
            
            foreach (var obj in nearbyGameObjects.Values)
            {
                if (obj.DataId != 11337) continue;

                var position = new Vector2(obj.Position.X, obj.Position.Z);

                if (!_triangleVertices.Contains(position))
                {
                    _triangleVertices.Add(position);
                }
            }

            // 如果发现3个顶点，初始化三角形
            if (_triangleVertices.Count == 3 && !_verticesInitialized)
            {
                _verticesInitialized = true;
                IdentifyTriangleVertices(); // 判断顶点和底边
                // LogHelper.Print("三角形顶点已初始化");
                // LogHelper.Print($"底边点1: {_basePoint1}, 底边点2: {_basePoint2}, 顶点: {_topPoint}");
            }
            
            // 11338 是我们要找的对象
            var id11338 = TargetMgr.Instance.Enemys.Values
                .Where(e => e.DataId is 11338).ToList();

            // 如果顶点已初始化，检查点的象限
            if (_verticesInitialized && id11338.Count == 4)
            {
                foreach (var obj in id11338)
                {
                    var position = new Vector2(obj.Position.X, obj.Position.Z);
                    int quadrant = DetermineQuadrant(position);
                    
                    if (AI.Instance.PartyRole switch
                        {
                            "D1" => quadrant == 4,
                            "D2" => quadrant == 3,
                            "D3" => quadrant == 2,
                            "D4" => quadrant == 1,
                            _ => false
                        })
                    {
                        Core.Me.SetTarget(obj);
                    }
                    return true;
                }
            }
            return false;
            // 返回 true，脚本停止运行
            // 返回 false，脚本每帧运行
        }

        private void IdentifyTriangleVertices()
        {
            // 计算三点之间的距离
            float d1 = Vector2.Distance(_triangleVertices[0], _triangleVertices[1]);
            float d2 = Vector2.Distance(_triangleVertices[1], _triangleVertices[2]);
            float d3 = Vector2.Distance(_triangleVertices[0], _triangleVertices[2]);

            // 找到最长的边
            if (d1 > d2 && d1 > d3)
            {
                _basePoint1 = _triangleVertices[0];
                _basePoint2 = _triangleVertices[1];
                _topPoint = _triangleVertices[2];
            }
            else if (d2 > d1 && d2 > d3)
            {
                _basePoint1 = _triangleVertices[1];
                _basePoint2 = _triangleVertices[2];
                _topPoint = _triangleVertices[0];
            }
            else
            {
                _basePoint1 = _triangleVertices[0];
                _basePoint2 = _triangleVertices[2];
                _topPoint = _triangleVertices[1];
            }
        }

        private int DetermineQuadrant(Vector2 point)
        {
            // 确保三角形顶点已初始化
            if (_triangleVertices.Count != 3) return 0;

            // 获取底边中点和方向向量
            var origin = new Vector2((_basePoint1.X + _basePoint2.X) / 2, (_basePoint1.Y + _basePoint2.Y) / 2);
            var xAxisDirection = Vector2.Normalize(_basePoint2 - _basePoint1);
            var vectorToTop = Vector2.Normalize(_topPoint - origin);
            var yAxisDirection = new Vector2(-xAxisDirection.Y, xAxisDirection.X);
            if (Vector2.Dot(yAxisDirection, vectorToTop) < 0) yAxisDirection = -yAxisDirection;

            // 转换为相对于新坐标系的点
            var relativePoint = point - origin;
            var xProjection = Vector2.Dot(relativePoint, xAxisDirection);
            var yProjection = Vector2.Dot(relativePoint, yAxisDirection);

            // 判断象限
            if (xProjection > 0 && yProjection > 0) return 1; // 第一象限
            if (xProjection < 0 && yProjection > 0) return 2; // 第二象限
            if (xProjection < 0 && yProjection < 0) return 3; // 第三象限
            if (xProjection > 0 && yProjection < 0) return 4; // 第四象限
            return 0; // 原点或坐标轴上
        }
    }
}
