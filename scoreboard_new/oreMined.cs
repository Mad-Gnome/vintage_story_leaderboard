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
            OverrideMethod = "None";
        }

        /*public override void OverrideCB()
        {
            
        }*/
    }
}
