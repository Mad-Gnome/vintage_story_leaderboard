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
    public class StatTimesDied: Leaderstat
    {
        private bool debug = false;
        public StatTimesDied(ICoreServerAPI api) : base(api)
        {
            Title = "Most Times Died";
            Init(GetKeyPrefix());
            Id = "MOST_TIMES_DIED";
            OverrideMethod = "PlayerDeath";
        }

        public override void OverrideCB(IServerPlayer byPlayer, DamageSource damageSource)
        {
            string name = byPlayer.Entity.GetName();
            string key = GetKeyPrefix()  ;
            int oldValue = GetOldValue(key +    name);
            Process(key, oldValue + 1, name);
        }
    }
}
