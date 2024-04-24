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
namespace scoreboard
{

    [HarmonyPatch] // Place on any class with harmony patches
    public class ScoreboardSystem : ModSystem
    {
        ICoreClientAPI capi;
        private ICoreServerAPI sapi;
       
        GuiDialog dialog;
        public Dictionary<string, List<Leaderstat>> leaderStats;
        public Dictionary<string,int> tabCounts = new();
        public Harmony harmony;
        public override void Start(ICoreAPI api)
        {
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
            capi.Input.RegisterHotKey("scoreboardgui", "A scoreboard display.", GlKeys.U, HotkeyType.GUIOrOtherControls);
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
            leaderStats["Deaths"] = new List<Leaderstat> {
                new StatChickensKilled(sapi),               //done
                new StatWolvesKilled(sapi),                 //done
                new StatTimesDied(sapi),                    //done
                new StatKilledByChicken(sapi),              //done
                new StatKilledByWolf(sapi),                 //done
                new StatKilledByDrowning(sapi),             //done
                new StatKilledByFallDamage(sapi),           //done
                new StatKilledByStarvation(sapi),           //done
                new StatKilledByPlayers(sapi),              //done
                new StatPlayersKilled(sapi),                //done
            };

            leaderStats["Mining"] = new List<Leaderstat> {
                new StatBlocksBroken(sapi),                 //done
                new StatBlocksPlaced(sapi),                 //done
                new StatOreMined(sapi),
            };
            leaderStats["Crafting/Smithing"] = new List<Leaderstat>
            {
                new StatClayCrafted(sapi),              //done
                new StatSmithingItemsCrafted(sapi),     //done
                new StatLeatherCrafted(sapi),           //?
                new StatIngotsCrafted(sapi),            //?
            };
            leaderStats["Server"] = new List<Leaderstat>
            {
                new StatChatWordsSent(sapi),            //done
                new StatTimeOnServer(sapi),             //done
            };
            leaderStats["Misc"] = new List<Leaderstat>
            {
                //new StatDeepestAveDepth(sapi),        //this won't really work because a value can't go back and forth
                new StatDistanceWalked(sapi),           //done
            };
            

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