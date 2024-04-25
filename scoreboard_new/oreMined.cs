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
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace scoreboard
{
    public class StatOreMined: Leaderstat
    {
        private bool debug = false;
        public StatOreMined(ICoreServerAPI api) : base(api)
        {
            Title = "Ore (All Types) Mined";
            Init(GetKeyPrefix());
            Id = "ORE_ALLTYPES_MINED";
            OverrideMethod = "DidBreakBlock";
        }

        public override void OverrideCB(IServerPlayer byPlayer, int oldblockId, BlockSelection blockSel)
        {
            Block block = sapi.World.BlockAccessor.GetBlock(oldblockId);
            if (block == null) return;
            if (block.Code == null) return;
            string blockName = block.Code.ToString();
            //sapi.Logger.Debug(blockName);
            if(blockName.Contains("ore"))
            {
                string name = byPlayer?.Entity?.GetName();
                if (name == null) return;
                string key = GetKeyPrefix();
                int oldValue = GetOldValue(key + name);
                Process(key, oldValue + 1, name);
            }
        }
    }
}
