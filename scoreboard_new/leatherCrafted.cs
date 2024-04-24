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
    public class StatLeatherCrafted: Leaderstat
    {
        private bool debug = false;
        public StatLeatherCrafted(ICoreServerAPI api) : base(api)
        {
            Title = "Leather Crafted";
            Init(GetKeyPrefix());
            Id = "LEATHER_CRAFTED";
            OverrideMethod = "None";
        }

        /*public override void OverrideCB()
        {
            
        }*/
    }
}
