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

            { "StatKilledBear", true },
            { "StatKilledCaninae", true },
            { "StatKilledCapreolinae", true },
            { "StatKilledCasuariidae", true },
            { "StatKilledChicken", true },
            { "StatKilledDinornithidae", true },
            { "StatKilledDrifter", true },
            { "StatKilledFish", true },
            { "StatKilledFox", true },
            { "StatKilledGoat", true },
            { "StatKilledLocust", true },
            { "StatKilledMachairodontinae", true },
            { "StatKilledManidae", true },
            { "StatKilledOutlaw", true },
            { "StatKilledPantherinae", true },
            { "StatKilledPig", true },
            { "StatKilledPlayer", true },
            { "StatKilledRabbit", true },
            { "StatKilledRaccoon", true },
            { "StatKilledRhinocerotidae", true },
            { "StatKilledShark", true },
            { "StatKilledSheep", true },
            { "StatKilledWolf", true },

            { "StatKilledByBear", true },
            { "StatKilledByCaninae", true },
            { "StatKilledByCapreolinae", true },
            { "StatKilledByCasuariidae", true },
            { "StatKilledByChicken", true },
            { "StatKilledByDrifter", true },
            { "StatKilledByDinornithidae", true },
            { "StatKilledByDrowning", true },
            { "StatKilledByFallDamage", true },
            { "StatKilledByFox", true },
            { "StatKilledByGoat", true },
            { "StatKilledByLocust", true },
            { "StatKilledByMachairodontinae", true },
            { "StatKilledByManidae", true },
            { "StatKilledByOutlaw", true },
            { "StatKilledByPantherinae", true },
            { "StatKilledByPig", true },
            { "StatKilledByPlayer", true },
            { "StatKilledByRabbit", false },
            { "StatKilledByRaccoon", true },
            { "StatKilledByRhinocerotidae", true },
            { "StatKilledByShark", true },
            { "StatKilledBySheep", true },
            { "StatKilledByStarvation", true },
            { "StatKilledByWolf", true },
            
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