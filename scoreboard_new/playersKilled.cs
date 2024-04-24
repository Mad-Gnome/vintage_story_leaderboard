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
    public class StatPlayersKilled: Leaderstat
    {
        private bool debug = false;
        public StatPlayersKilled(ICoreServerAPI api) : base(api)
        {
            Title = "Other Players Killed";
            Init(GetKeyPrefix());
            Id = "PLAYERS_KILLED";
            OverrideMethod = "OnEntityDeath";
        }

        public override void OverrideCB(Entity entity, DamageSource damageSource)
        {
            if (entity is not EntityPlayer) return;
            if (damageSource.SourceEntity != null && damageSource.SourceEntity is EntityPlayer)
            {
                string key = GetKeyPrefix();
                string killedByName = damageSource.SourceEntity.GetName();
                int oldValue = GetOldValue(key + killedByName); //append player name to getoldvalue
                Process(key, oldValue + 1, killedByName);
            }
        }
    }
}
