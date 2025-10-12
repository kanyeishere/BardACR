using System.Numerics;
using AEAssist;
using AEAssist.CombatRoutine.View.JobView;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using Dalamud.Bindings.ImGui;
using Dalamud.Plugin.Ipc.Exceptions;
using ECommons.DalamudServices;
using Wotou.Bard;
using Wotou.Bard.Data;

namespace Wotou.Common;

public class StopMoveHotkeyResolver : IHotkeyResolver
{
    // 删掉 imagePath 字段

    public void Draw(Vector2 size)
    {
        // 仍然沿用 10% padding，让图标占可视区 80%
        Vector2 padding = size * 0.1f;
        ImGui.SetCursorPos(padding);

        // 当前可绘制区域左上角（屏幕坐标）
        Vector2 topLeft = ImGui.GetCursorScreenPos();
        float diameter = MathF.Min(size.X, size.Y) * 0.8f;
        Vector2 center = new Vector2(topLeft.X + diameter / 2f, topLeft.Y + diameter / 2f);

        // 画矢量 STOP 图标
        CuteStopSign.DrawDefault(center, diameter);

        // 占位，防止后续控件覆盖并维持布局
        ImGui.Dummy(new Vector2(diameter, diameter));
    }

    public void DrawExternal(Vector2 size, bool isActive)
    {
        // 可选：根据 isActive 画个高亮环
        // 留空同原实现
    }

    public int Check() => 0;

    public void Run()
    {
        if (!VNavAvailable()) return;
        // Core.Resolve<MemApiMove>().CancelMove();
        var navStop = Svc.PluginInterface.GetIpcSubscriber<object>("vnavmesh.Path.Stop");
        navStop?.InvokeAction();
        BardBattleData.Instance.TargetPosition = null;
        BardBattleData.Instance.FollowingTarget = null;
        BardBattleData.Instance.IsFollowing = false;
    }
    
    private bool VNavAvailable()
    {
        var plist = Svc.PluginInterface.InstalledPlugins;
        if (plist != null)
        {
            var meta = plist.FirstOrDefault(p =>
                string.Equals(p.InternalName, "vnavmesh", StringComparison.OrdinalIgnoreCase));
            if (meta is null || !meta.IsLoaded)
                return false;
        }

        try
        {
            var isReady = Svc.PluginInterface
                .GetIpcSubscriber<bool>("vnavmesh.Nav.IsReady")
                .InvokeFunc();

            if (!isReady)
                return false;
        }
        catch (IpcNotReadyError)        { return false; } // 提供方未就绪（加载/切图中）
        catch                           { return false; } // 其他异常一律视为不可用
        return true;
    }
}