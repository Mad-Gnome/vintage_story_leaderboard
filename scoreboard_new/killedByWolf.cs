using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace scoreboard
{
    public class StatKilledByWolf: Leaderstat
    {
        private bool debug = false;
        
        public StatKilledByWolf(ICoreServerAPI api) : base(api)
        {
            Title = "Died to Wolves";
            Init(GetKeyPrefix());
            Id = "MOST_KILLED_BY_WOLF";
            OverrideMethod = "OnEntityDeath";

        }

        public override void OverrideCB(Entity entity, DamageSource damageSource)
        {
            if (entity is not EntityPlayer) return;
            string killer = null;
           
            if (damageSource != null)
            {
                if (damageSource.SourceEntity == null)
                {
                    killer = null;
                }
                else
                {
                    killer = damageSource.SourceEntity.Code.ToString();
                    if (debug) sapi.Logger.Debug("scoreboard logline!");
                    if (debug) sapi.Logger.Debug(damageSource.SourceEntity.GetName());
                    if (debug) sapi.Logger.Debug(damageSource.SourceEntity.Code.ToString());
                }
            }

            if (!killer.Contains("wolf")) return;
            string killed = entity.GetName();
            string key = GetKeyPrefix();
            int oldValue = GetOldValue(key + killed);
            Process(key, oldValue + 1, killed);
        }
    }
}