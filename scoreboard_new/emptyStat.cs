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
    public class StatEmpty: Leaderstat
    {
        private bool debug = false;
        public StatEmpty(ICoreServerAPI api) : base(api)
        {
            Title = "EMPTY STAT";
            Init(GetKeyPrefix());
            Id = "EMPTY_STAT";
            OverrideMethod = "None";
        }

        /*public override void OverrideCB()
        {
            
        }*/
    }
}
