using System.Security.Cryptography;
using System.Text;
using AEAssist;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Verify;

namespace Wotou.Common;

public class LowVipRestrictor
{
    public static bool IsRestrictedZoneForLowVip()
    {
        uint currentZoneId = Core.Resolve<MemApiZoneInfo>().GetCurrTerrId();
        return currentZoneId == 1238 && Share.VIP.Level == VIPLevel.Normal;
    }
    
    public static bool IsInStaticParty(List<string> storedStaticPartyHashes)
    {
        PartyHelper.UpdateAllies();
        var currentPartyHashes = PartyHelper.Party
            .Select(player => ComputeMd5Hash(player.Name.ToString()))
            .ToList();
        
        int matchCount = currentPartyHashes.Count(hash => storedStaticPartyHashes.Contains(hash));

        // 如果匹配的成员数量 >= 2，判定为固定队，否则为野队
        return matchCount >= 2;
    }
    
    public static string ComputeMd5Hash(string input)
    {
        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = MD5.HashData(inputBytes);

        // 将哈希值转换为十六进制字符串
        return Convert.ToHexString(hashBytes).ToLower(); // .NET 5+ 使用 Convert.ToHexString()
    }
}