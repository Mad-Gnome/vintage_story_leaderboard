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
    public class StatIngotsPoured : Leaderstat
    {
        private static bool debug = false;
        private static StatIngotsPoured me;
        public StatIngotsPoured(ICoreServerAPI api) : base(api)
        {
            Title = "Ingots Poured";
            Init(GetKeyPrefix());
            Id = "INGOTS_POURED";
            OverrideMethod = "None";
            me = this;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlockEntityIngotMold), "ReceiveLiquidMetal")]
        public static void ReceiveLiquidMetalBefore(BlockEntityIngotMold __instance,  ItemStack metal, ref int amount, float temperature, out bool __state)
        {
            if(__instance.fillSide) __state =  __instance.fillLevelRight == 100;
            else __state = __instance.fillLevelLeft == 100;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BlockEntityIngotMold), "ReceiveLiquidMetal")]
        public static void ReceiveLiquidMetalAfter(BlockEntityIngotMold __instance,  ItemStack metal, ref int amount, float temperature, bool __state)
        {
            if (__instance?.Api?.Side == null) return;
            if (__instance.Api.Side.IsClient()) return;
            bool filled;
            if (__instance.fillSide) filled = __instance.fillLevelRight == 100;
            else filled = __instance.fillLevelLeft == 100;
            if (__state == false && filled)
            {
                if (__instance.Api.Side.IsClient()) return;
                IPlayer byPlayer = __instance.Api.World.NearestPlayer(__instance.Pos.X, __instance.Pos.Y, __instance.Pos.Z);
                string name = byPlayer?.Entity?.GetName();
                if(name == null) return;
                string key = me.GetKeyPrefix();
                int oldValue = me.GetOldValue(key + name);
                me.Process(key, oldValue + 1, name);
            }
        }
    }
}
