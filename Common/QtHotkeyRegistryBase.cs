using System.Reflection;
using AEAssist.CombatRoutine.View.JobView.HotkeyResolver;

namespace Wotou.Common;

public record QtDef(
    string Key,
    bool Default,
    string Description,
    Action<bool>? Callback,
    string EnglishAlias,
    string ChineseAlias
);

public record HotkeyDef(
    string Id,
    string LabelZh,
    Func<IHotkeyResolver> Factory,
    string EnglishAlias,
    string ChineseAlias
);

public static class QtHotkeyRegistryBase
{
    public static QtDef NewQt(Type qtType, string qtFieldName, bool def, string desc, Action<bool>? cb)
    {
        var field = qtType.GetField(qtFieldName, BindingFlags.Public | BindingFlags.Static);
        var cnValue = field?.GetValue(null)?.ToString() ?? qtFieldName;
        return new QtDef(cnValue, def, desc, cb, qtFieldName.ToLowerInvariant(), cnValue);
    }

    public static HotkeyDef NewHotkey(string id, string labelZh, Func<IHotkeyResolver> factory)
        => new(id, labelZh, factory, id.ToLowerInvariant(), labelZh);

    public static Dictionary<string, string> BuildQtAliasMap(IEnumerable<QtDef> qts)
        => qts
            .SelectMany(q => new[]
            {
                (alias: q.EnglishAlias, key: q.Key),
                (alias: q.ChineseAlias, key: q.Key)
            })
            .GroupBy(x => x.alias, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.Last().key, StringComparer.OrdinalIgnoreCase);

    public static Dictionary<string, string> BuildHotkeyAliasMap(IEnumerable<HotkeyDef> hotkeys)
        => hotkeys
            .SelectMany(h => new[]
            {
                (alias: h.EnglishAlias, id: h.Id),
                (alias: h.ChineseAlias, id: h.Id)
            })
            .ToDictionary(x => x.alias, x => x.id, StringComparer.OrdinalIgnoreCase);

    public static IHotkeyResolver? CreateResolver(IEnumerable<HotkeyDef> hotkeys, string id)
        => hotkeys.FirstOrDefault(h => string.Equals(h.Id, id, StringComparison.OrdinalIgnoreCase))?.Factory();

    public static string? ParseQtKey(Dictionary<string, string> qtAliasToKey, string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        return qtAliasToKey.TryGetValue(input.Trim().ToLowerInvariant(), out var key) ? key : null;
    }
}
