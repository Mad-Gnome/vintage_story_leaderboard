using scoreboard;
using System.Collections.Generic;
using Vintagestory.API.Common;

namespace scoreboard
{
    public class ConfigSettings
    {
         public Dictionary<string, bool> stats { get; set; }  = new Dictionary<string, bool>
         {
            { "AllowPromotion", true },
            { "StatGrassHarvested", true },
            { "StatBlocksBurned", true },
            { "StatTimesDied", true },
            { "StatChickensKilled", true },
            { "StatKilledByChicken", true },
            { "StatWolvesKilled", true },
            { "StatKilledByWolf", true },
            { "StatPlayersKilled", true },
            { "StatKilledByPlayers", true },
            { "StatKilledByDrowning", true },
            { "StatKilledByFallDamage", true },
            { "StatKilledByStarvation", true },
            { "StatBlocksBroken", true },
            { "StatBlocksPlaced", true },
            { "StatOreMined", true },
            { "StatTreesChopped", true },
            { "StatToolsNapped", true },
            { "StatClayCrafted", true },
            { "StatIngotsPoured", true },
            { "StatSmithingItemsCrafted", true },
            { "StatChiselStrikes", true },
            { "StatTimeOnServer", true },
            { "StatChatWordsSent", true },
            { "StatDistanceWalked", true }
        };
    }
}