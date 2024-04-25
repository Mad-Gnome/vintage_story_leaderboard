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
    public class StatToolsNapped : Leaderstat
    {
        private static  bool debug = false;
        private static StatToolsNapped me;
        public StatToolsNapped(ICoreServerAPI api) : base(api)
        {
            Title = "Tools Napped";
            Init(GetKeyPrefix());
            Id = "TOOLS_KNAPPED";
            OverrideMethod = "None";      
            me = this;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BlockEntityKnappingSurface), "CheckIfFinished")]
        public static void CheckIfFinished(BlockEntityKnappingSurface __instance, IPlayer byPlayer)
        {
            if (__instance.Api.Side.IsClient()) return;

            if(__instance.SelectedRecipe == null)
            {
                string key = me.GetKeyPrefix();
                string name = byPlayer?.Entity?.GetName();
                if (name == null) return;
                int oldValue = me.GetOldValue(key + name);
                me.Process(key, oldValue + 1, name);
                if(debug) __instance.Api.Logger.Debug("All done! {0}", byPlayer.Entity.GetName());
            }
        }
    }
}
 