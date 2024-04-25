using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
using ProtoBuf;
using Vintagestory.API.Util;
using Vintagestory.API.MathTools;
using HarmonyLib;
using Vintagestory.GameContent;
namespace scoreboard
{

    [HarmonyPatch] // Place on any class with harmony patches
    public class ScoreboardSystem : ModSystem
    {
        ConfigSettings config;
        ICoreClientAPI capi;
        private ICoreServerAPI sapi;
       
        GuiDialog dialog;
        public Dictionary<string, List<Leaderstat>> leaderStats;
        public Dictionary<string,int> tabCounts = new();
        public Harmony harmony;
        public override void Start(ICoreAPI api)
        {
            config = api.LoadModConfig<ConfigSettings>("scoreboard.json");
            if (config == null)
            {
                api.StoreModConfig(new ConfigSettings(), "scoreboard.json");
                config = new ConfigSettings();
            }

            api.Network.RegisterChannel("stat_request")
                .RegisterMessageType(typeof(StatRequestResponse))
                .RegisterMessageType(typeof(StatRequest))
            ;
            if (!Harmony.HasAnyPatches(Mod.Info.ModID))
            {
                harmony = new Harmony(Mod.Info.ModID);
                harmony.PatchAll(); // Applies all harmony patches
            }
        }

        public override void StartClientSide(ICoreClientAPI api)
        {
            base.StartClientSide(api);
            
            capi = api;
            string abc = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            int i = abc.IndexOf(config.open_dialog_key.ToUpper());
            capi.Input.RegisterHotKey("scoreboardgui", "A scoreboard display.", (GlKeys)(83 + i), HotkeyType.GUIOrOtherControls);
            capi.Input.SetHotKeyHandler("scoreboardgui", ToggleGui);

            api.Network.GetChannel("stat_request")
                .SetMessageHandler<StatRequestResponse>(OnReceiveStats)
            ;
        }

        

        public override void StartServerSide(ICoreServerAPI api)
        {
            api.Logger.Debug("SERVER START LEADERSTATS");
            this.sapi = api;
            
            api.Network.GetChannel("stat_request")
                .SetMessageHandler<StatRequest>(OnClientRequestStats);
            AddAllStats();
        }

        public void AddAllStats()
        {
            leaderStats = new ();
            leaderStats["Deaths"] = new List<Leaderstat> {};
            if(config.StatTimesDied) leaderStats["Deaths"].Add(new StatTimesDied(sapi));
            if(config.StatChickensKilled) leaderStats["Deaths"].Add(new StatChickensKilled(sapi));
            if(config.StatKilledByChicken) leaderStats["Deaths"].Add(new StatKilledByChicken(sapi));
            if(config.StatWolvesKilled) leaderStats["Deaths"].Add(new StatWolvesKilled(sapi));
            if(config.StatKilledByWolf) leaderStats["Deaths"].Add(new StatKilledByWolf(sapi));
            if(config.StatPlayersKilled) leaderStats["Deaths"].Add(new StatPlayersKilled(sapi));
            if(config.StatKilledByPlayers) leaderStats["Deaths"].Add(new StatKilledByPlayers(sapi));
            if(config.StatKilledByDrowning) leaderStats["Deaths"].Add(new StatKilledByDrowning(sapi));
            if(config.StatKilledByFallDamage) leaderStats["Deaths"].Add(new StatKilledByFallDamage(sapi));
            if(config.StatKilledByStarvation) leaderStats["Deaths"].Add(new StatKilledByStarvation(sapi));

            leaderStats["Blocks"] = new List<Leaderstat> {};
            if (config.StatBlocksBroken) leaderStats["Blocks"].Add(new StatBlocksBroken(sapi));
            if (config.StatBlocksPlaced) leaderStats["Blocks"].Add(new StatBlocksPlaced(sapi));
            if (config.StatOreMined) leaderStats["Blocks"].Add(new StatOreMined(sapi));
            if (config.StatTreesChopped) leaderStats["Blocks"].Add(new StatTreesChopped(sapi));

            leaderStats["Crafting/Smithing"] = new List<Leaderstat> {};
            if (config.StatToolsNapped) leaderStats["Crafting/Smithing"].Add(new StatToolsNapped(sapi));
            if (config.StatClayCrafted) leaderStats["Crafting/Smithing"].Add(new StatClayCrafted(sapi));
            if (config.StatIngotsPoured) leaderStats["Crafting/Smithing"].Add(new StatIngotsPoured(sapi));
            if (config.StatSmithingItemsCrafted) leaderStats["Crafting/Smithing"].Add(new StatSmithingItemsCrafted(sapi));
            if (config.StatChiselStrikes) leaderStats["Crafting/Smithing"].Add(new StatChiselStrikes(sapi));

            leaderStats["Server"] = new List<Leaderstat>{};
            if (config.StatTimeOnServer) leaderStats["Server"].Add(new StatTimeOnServer(sapi));
            if (config.StatChatWordsSent) leaderStats["Server"].Add(new StatChatWordsSent(sapi));

            leaderStats["Misc"] = new List<Leaderstat>{};
            if (config.StatDistanceWalked) leaderStats["Misc"].Add(new StatDistanceWalked(sapi));


            foreach (string tab in leaderStats.Keys)
            {
                foreach (Leaderstat stat in leaderStats[tab])
                {
                    tabCounts[tab] = leaderStats[tab].Count;
                    RegisterStat(stat);
                }
            }
            
        }

        

        private void RegisterStat(Leaderstat stat)
        {
            //sapi.Logger.Debug(stat.OverrideMethod);
            if (stat.OverrideMethod == "OnEntityDeath")
            {
                sapi.Event.OnEntityDeath += stat.OverrideCB;
            }
            if (stat.OverrideMethod == "DidBreakBlock")
            {
                sapi.Event.DidBreakBlock += stat.OverrideCB;
            }
            if (stat.OverrideMethod == "DidPlaceBlock")
            {
                sapi.Event.DidPlaceBlock += stat.OverrideCB;
            }
            if (stat.OverrideMethod == "DidUseBlock")
            {
                sapi.Event.DidUseBlock += stat.OverrideCB;
            }
            if (stat.OverrideMethod == "PlayerChat")
            {
                sapi.Event.PlayerChat += stat.OverrideCB;
            }
            if (stat.OverrideMethod == "PlayerJoin")
            {
                sapi.Event.PlayerJoin += stat.OverrideCB;

            }
            if (stat.OverrideMethod == "PlayerDeath")
            {
                sapi.Event.PlayerDeath += stat.OverrideCB;
            }
            if (stat.OverrideMethod == "PlayerNowPlaying")
            {
                sapi.Event.PlayerNowPlaying += stat.OverrideCB;

            }
            
            //OnCreatedByCrafting(ItemSlot[], ItemSlot, GridRecipe)


            //sapi.Event.DidBreakBlock
            //sapi.Event.DidPlaceBlock
            //sapi.Event.DidUseBlock
        }
        private bool ToggleGui(KeyCombination comb)
        {
            if (dialog != null && dialog.IsOpened()) dialog.TryClose();
            else 
            {
               // capi.ShowChatMessage("Sending request to open the gui from client...");
                capi.Network.GetChannel("stat_request").SendPacket(new StatRequest()
                {
                    response = "GET_SCORES",
                    pageID = 0,
                    tabID = "Deaths",
                    fresh = true,
                    newGui = true
                });
            }
            return true;
        }


        private void OnClientRequestStats(IPlayer fromPlayer, StatRequest request)
        {
            /*sapi.SendMessageToGroup(
                GlobalConstants.GeneralChatGroup,
                "Received following request from " + fromPlayer.PlayerName + ": " + request.tabID + ": " + request.pageID.ToString(),
                EnumChatType.Notification
            );*/
            int pageN = request.pageID;

            Leaderstat stat = leaderStats[request.tabID][pageN];
            int i = 0;
            string[] lines = new string[22];
            foreach (Leader leader in stat.leaders)
            {
                lines[i * 2 + 0] = leader.name;
                lines[i * 2 + 1] = leader.value.ToString();
                i += 1;
            }
            lines[20] = stat.Title;
            //sapi.Logger.Debug(request.tabID);
            lines[21] = tabCounts[request.tabID].ToString();
            sapi.Network.GetChannel("stat_request").SendPacket(new StatRequestResponse()
            {
                stats = SerializerUtil.Serialize(lines),
                fresh = request.fresh,
                newGui = request.newGui
            }, fromPlayer as IServerPlayer);
        }
        private void OnReceiveStats(StatRequestResponse networkMessage)
        {
           // capi.ShowChatMessage("Received stats from the server. Opening scoreboard...");
            string[] stats = SerializerUtil.Deserialize<string[]>(networkMessage.stats);

            if (dialog!=null) dialog.TryClose();
            dialog = new ScoreboardGui(capi, stats, networkMessage.fresh, networkMessage.newGui);
            dialog.TryOpen();
        }
    }
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class StatRequestResponse
    {
        public byte[] stats;
        public bool fresh;
        public bool newGui;
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    public class StatRequest
    {
        public string response;
        public int pageID;
        public string tabID;
        public bool fresh;
        public bool newGui;
    }
}