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
    public class StatBlocksBurned : Leaderstat
    {
        private static  bool debug = false;
        private static StatBlocksBurned me;
        public StatBlocksBurned(ICoreServerAPI api) : base(api)
        {
            Title = "Blocks Burned";
            Init(GetKeyPrefix());
            Id = "BLOCKS_BURNED";
            OverrideMethod = "None";      
            me = this;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(BEBehaviorBurning), "OnFirePlaced")]
        [HarmonyPatch(new[] { typeof(BlockPos), typeof(BlockPos), typeof(string) })] // Specify parameter types here
        public static void OnFirePlaced(BEBehaviorBurning __instance, BlockPos firePos, BlockPos fuelPos, string startedByPlayerUid)
        {
            if (__instance?.Api?.Side == null) return;
            if (__instance.Api.Side.IsClient()) return;
            if (__instance.Api?.World == null) return;
            IPlayer byPlayer = __instance.Api.World.PlayerByUid(startedByPlayerUid);
            if (me == null) return;
            string key = me.GetKeyPrefix();
            string name = byPlayer?.Entity?.GetName();
            if (name == null) return;
            int oldValue = me.GetOldValue(key + name);
            me.Process(key, oldValue + 1, name);
            if(debug) __instance.Api.Logger.Debug("All done! {0}", byPlayer.Entity.GetName());
            
        }
    }
}
 