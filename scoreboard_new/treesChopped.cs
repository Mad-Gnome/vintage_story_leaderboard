using HarmonyLib;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class StatTreesChopped : Leaderstat
    {
        private static bool debug = false;
        private static StatTreesChopped me;
        private bool toggle = false;
        public StatTreesChopped(ICoreServerAPI api) : base(api)
        {
            Title = "Trees Chopped Down";
            Init(GetKeyPrefix());
            Id = "TREES_CHOPPED";
            OverrideMethod = "None";
            me = this;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ItemAxe), "OnBlockBrokenWith")]
        public static void OnBlockBrokenWith(ItemAxe __instance, IWorldAccessor world, Entity byEntity, ItemSlot itemslot, BlockSelection blockSel, float dropQuantityMultiplier = 1)
        {
            if (me.sapi.Side.IsClient()) return;
            Stack<BlockPos> foundPositions = __instance.FindTree(world, blockSel.Position, out int _, out int woodTier);
            
            me.sapi.Logger.Debug("TREE: {0}",foundPositions.Count.ToString());
            if(foundPositions.Count > 0)
            {
                if(me.toggle) //this seems to run twice, so this increments it once; hacky
                {
                    string name = byEntity.GetName();
                    string key = me.GetKeyPrefix();
                    int oldValue = me.GetOldValue(key + name);
                    me.Process(key, oldValue + 1, name);
                    me.toggle = false;
                } else me.toggle = true;
                
            }
        }
    }
}
