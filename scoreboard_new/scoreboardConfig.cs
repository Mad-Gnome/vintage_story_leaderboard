using scoreboard;
using System.Collections.Generic;
using Vintagestory.API.Common;

namespace scoreboard
{
    public class ConfigSettings
    {
        public static ConfigSettings Loaded { get; set; } = new ConfigSettings();

        public string open_dialog_key { get; set; } = "u";
        public bool StatTimesDied { get; set; } = true;
        public bool StatChickensKilled { get; set; } = true;
        public bool StatKilledByChicken { get; set; } = true;
        public bool StatWolvesKilled { get; set; } = true;
        public bool StatKilledByWolf { get; set; } = true;
        public bool StatPlayersKilled { get; set; } = true;
        public bool StatKilledByPlayers { get; set; } = true;
        public bool StatKilledByDrowning { get; set; } = true;
        public bool StatKilledByFallDamage { get; set; } = true;
        public bool StatKilledByStarvation { get; set; } = true;
        public bool StatBlocksBroken { get; set; } = true;
        public bool StatBlocksPlaced { get; set; } = true;
        public bool StatOreMined { get; set; } = true;
        public bool StatTreesChopped { get; set; } = true;
        public bool StatToolsNapped { get; set; } = true;
        public bool StatClayCrafted { get; set; } = true;
        public bool StatIngotsPoured { get; set; } = true;
        public bool StatSmithingItemsCrafted { get; set; } = true;
        public bool StatChiselStrikes { get; set; } = true;
        public bool StatTimeOnServer { get; set; } = true;
        public bool StatChatWordsSent { get; set; } = true;
        public bool StatDistanceWalked { get; set; } = true;         
    }
}