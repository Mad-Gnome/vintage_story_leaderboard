using HarmonyLib;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;
using Vintagestory.GameContent.Mechanics;

namespace scoreboard
{
    [HarmonyPatch] // Place on any class with harmony patches
    public class StatSmithingItemsCrafted : Leaderstat
    {
        private static bool debug = false;
        private static StatSmithingItemsCrafted me;
        public StatSmithingItemsCrafted(ICoreServerAPI api) : base(api)
        {
            Title = "Smith Items Crafted";
            Init(GetKeyPrefix());
            Id = "Smith_ITEMS_CRAFTED";
            OverrideMethod = "DidPlaceBlock";
            me = this;
        }


        public override void OverrideCB(IServerPlayer byPlayer, int oldblockId, BlockSelection blockSel, ItemStack withItemStack)
        {
            //sapi.Logger.Debug("placed a block");
            if (byPlayer?.Entity?.PlayerUID == null) return;
            if (blockSel?.Position == null) return;
            if (sapi?.World?.BlockAccessor == null) return;
            Block block = sapi.World.BlockAccessor.GetBlock(blockSel.Position);
            if (block is not BlockAnvil) return;
            string pos = blockSel.Position.X.ToString() + "," + blockSel.Position.Y.ToString() + "," + blockSel.Position.Z.ToString();
            string name = byPlayer.Entity.GetName();
            sapi.Logger.Debug("Pos: {0}", pos);
            sapi.Logger.Debug("Owner: {0}", name);
            sapi.WorldManager.SaveGame.StoreData("anvilData:" + pos, name);
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BlockEntityAnvil), "CheckIfFinished")]
        public static void CheckIfFinished(BlockEntityAnvil __instance, IPlayer byPlayer)
        {
            if (__instance?.Api?.Side == null) return;
            if (__instance.Api.Side.IsClient()) return;
            ICoreServerAPI Api = __instance.Api as ICoreServerAPI;
            if (__instance.WorkItemStack == null)
            {
                string name;
                __instance.Api.Logger.Debug("Smithed item");
                if (byPlayer == null || byPlayer.Entity == null) {
                    //this is probably a helve hammer
                    string pos = __instance.Pos.X.ToString() + "," + __instance.Pos.Y.ToString() + "," + __instance.Pos.Z.ToString();
                    byte[] data = Api.WorldManager.SaveGame.GetData("anvilData:" + pos);
                    if (data == null) return;
                    name  = SerializerUtil.Deserialize<string>(data);
                    //__instance.Api.Logger.Debug("helve hammer");
                }   
                else
                {
                    //someone's just made it with a hammer
                    
                    name = byPlayer?.Entity?.GetName();
                    //__instance.Api.Logger.Debug("regular hammer");
                }
                if (name == null) return;
                string key = me.GetKeyPrefix();
                //__instance.Api.Logger.Debug("Smithed item {0}", name);
                int oldValue = me.GetOldValue(key + name);
                me.Process(key, oldValue + 1, name);
            }
        }
    }
}
