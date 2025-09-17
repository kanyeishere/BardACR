using System.Security.Cryptography;
using System.Text;
using AEAssist;
using AEAssist.Helper;
using AEAssist.MemoryApi;
using AEAssist.Verify;
using Wotou.Bard.Setting;
using Wotou.Dancer.Setting;

namespace Wotou.Common;

public class LowVipRestrictor
{
    public static bool IsRestrictedZoneForLowVip()
    {
        var currentZoneId = Core.Resolve<MemApiZoneInfo>().GetCurrTerrId();
        return currentZoneId == 1238 && 
               Share.VIP.Level == VIPLevel.Normal &&
               !ComputeMd5Hash(BardSettings.Instance.UnlockPassword).Equals("1b629ab7c628adc4e40705010ac745f3", 
                   StringComparison.OrdinalIgnoreCase) &&
               !ComputeMd5Hash(DancerSettings.Instance.UnlockPassword).Equals("1b629ab7c628adc4e40705010ac745f3", 
                   StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsLowVip()
    {
        return Share.VIP.Level == VIPLevel.Normal;
    }
    
    public static bool IsInStaticParty(List<string> storedStaticPartyHashes)
    {
        PartyHelper.UpdateAllies();
        var currentPartyHashes = PartyHelper.Party
            .Select(player => ComputeMd5Hash(player.Name.ToString()))
            .ToList();
        
        var matchCount = currentPartyHashes.Count(hash => storedStaticPartyHashes.Contains(hash));
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