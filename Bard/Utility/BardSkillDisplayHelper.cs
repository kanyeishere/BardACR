using Wotou.Bard.Data;

namespace Wotou.Bard.Utility;

public static class BardSkillDisplayHelper
{
    public static string GetPreferredSkillName(uint skillId)
    {
        var chinese = BardDefinesData.SkillDictionary
            .Where(kv => kv.Value == skillId && ContainsChinese(kv.Key))
            .Select(kv => kv.Key)
            .FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(chinese)) return chinese;

        var any = BardDefinesData.SkillDictionary.FirstOrDefault(kv => kv.Value == skillId).Key;
        return string.IsNullOrWhiteSpace(any) ? $"未知技能({skillId})" : any;
    }

    public static List<(uint Id, string DisplayName)> BuildSkillOptions(List<KeyValuePair<string, uint>> allSkills)
    {
        return allSkills
            .GroupBy(kv => kv.Value)
            .Select(g => (Id: g.Key, DisplayName: g.Select(x => x.Key).FirstOrDefault(ContainsChinese) ?? g.First().Key))
            .OrderBy(x => x.DisplayName)
            .ToList();
    }

    private static bool ContainsChinese(string text)
    {
        return text.Any(ch => ch >= 0x4E00 && ch <= 0x9FFF);
    }
}
