using HarmonyLib;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
    public class StatChiselStrikes : Leaderstat
    {
        private static bool debug = false;
        private static StatChiselStrikes me;
        public StatChiselStrikes(ICoreServerAPI api) : base(api)
        {
            Title = "Chisel Strikes";
            Init(GetKeyPrefix());
            Id = "CHISEL_STRIKES";
            OverrideMethod = "None";
            me = this;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BlockEntityChisel), "UpdateVoxel")]
        public static void UpdateVoxel(BlockEntityChisel __instance, IPlayer byPlayer, ItemSlot itemslot, Vec3i voxelPos, BlockFacing facing, bool isBreak)
        {
            if (__instance?.Api?.Side == null) return;
            if (__instance.Api.Side.IsClient()) return;

            var slot = byPlayer?.InventoryManager?.ActiveHotbarSlot;
            var itemChisel = slot?.Itemstack?.Collectible as ItemChisel;
            if (itemChisel == null) return;

            int? mode = itemChisel.GetToolMode(slot, byPlayer, new BlockSelection() { Position = __instance.Pos });
            if (mode.HasValue)
            {
                if (byPlayer == null || byPlayer.Entity == null) return;
                string key = me.GetKeyPrefix();
                string name = byPlayer.Entity.GetName();
                int oldValue = me.GetOldValue(key + name);
                me.Process(key, oldValue + 1, name);
                if (debug) __instance.Api.Logger.Debug("Chiseled {0}", byPlayer.Entity.GetName());
            }
        }
    }
}
