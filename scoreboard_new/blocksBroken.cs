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
    public class StatBlocksBroken: Leaderstat
    {
        private bool debug = false;
        public StatBlocksBroken(ICoreServerAPI api) : base(api)
        {
            Title = "Blocks Broken";
            Init(GetKeyPrefix());
            Id = "BLOCKS_BROKEN";
            OverrideMethod = "DidBreakBlock";
        }

        public override void OverrideCB(IServerPlayer byPlayer, int oldblockId, BlockSelection blockSel)
        {
            string name = byPlayer.Entity.GetName();
            string key = GetKeyPrefix();
            int oldValue = GetOldValue(key + name);
            Process(key, oldValue + 1, name);
        }
    }
}
