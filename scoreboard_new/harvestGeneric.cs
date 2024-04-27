using HarmonyLib;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using Vintagestory.GameContent;

namespace scoreboard
{
    [HarmonyPatch] // Place on any class with harmony patches
    public class StatGrassHarvested : Leaderstat
    {
        private static bool debug = false;
        private static StatGrassHarvested me;
        public StatGrassHarvested(ICoreServerAPI api) : base(api)
        {
            Title = "Grass Harvested";
            Init(GetKeyPrefix());
            Id = "GRASS_HARVESTED";
            OverrideMethod = "DidBreakBlock";
            me = this;
        }

        public override void OverrideCB(IServerPlayer byPlayer, int oldblockId, BlockSelection blockSel)
        {
            Block block = sapi.World.BlockAccessor.GetBlock(oldblockId);
            if (block == null) return;
            if (block.Code == null) return;
            if (byPlayer?.InventoryManager.ActiveTool == EnumTool.Knife && block.Variant["tallgrass"] != null && block.Variant["tallgrass"] != "eaten")
            {
                string name = byPlayer?.Entity?.GetName();
                if (name == null) return;
                string key = GetKeyPrefix();
                int oldValue = GetOldValue(key + name);
                Process(key, oldValue + 1, name);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Block), "OnBlockBroken")]
        public static void OnBlockBroken(Block __instance, IWorldAccessor world, BlockPos pos, IPlayer byPlayer)
        {
            if (__instance is not BlockTallGrass) return;
            if (me?.sapi?.Side == null) return;
            if (me.sapi.Side.IsClient()) return;
            if (byPlayer?.InventoryManager.ActiveTool == EnumTool.Scythe && __instance.Variant["tallgrass"] != null && __instance.Variant["tallgrass"] != "eaten")
            {
                string name = byPlayer?.Entity?.GetName();
                if (name == null) return;
                string key = me.GetKeyPrefix();
                int oldValue = me.GetOldValue(key + name);
                me.Process(key, oldValue + 1, name);
            }
        }
    }
}
