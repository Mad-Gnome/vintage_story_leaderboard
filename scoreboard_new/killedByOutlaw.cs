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
    public class StatKilledByOutlaw: Leaderstat
    {
        private bool debug = false;
        
        public StatKilledByOutlaw(ICoreServerAPI api) : base(api)
        {
            Title = "Died to Outlaws";
            Init(GetKeyPrefix());
            Id = "MOST_KILLED_BY_OUTLAW";
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
            List<string> outlaws = new List<string>
            { "Looter", "game:dummyarrow-copper", "game:bandit-axe", "game:bandit-knife", "game:bandit-spear", "game:deserter-archer", "game:deserter-bannerman", "game:deserter-footman", 
                "game:hound-feral", "game:hound-hunting", "game:poacher-archer", "game:poacher-spear", "game:yeoman-archer"};

            if (!outlaws.Contains(killer)) return;
            string killed = entity.GetName();
            string key = GetKeyPrefix();
            int oldValue = GetOldValue(key + killed);
            Process(key, oldValue + 1, killed);
        }
    }
}