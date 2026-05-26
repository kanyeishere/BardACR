namespace Wotou.Common;

public static class SettingsDefaults
{
    public static void EnsureQtDefaults(Dictionary<string, bool> target, IEnumerable<QtDef> defs)
    {
        foreach (var def in defs)
        {
            if (!target.ContainsKey(def.Key))
            {
                target[def.Key] = def.Default;
            }
        }
    }
}
