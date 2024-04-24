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
    public class StatKilledByChicken: Leaderstat
    {
        private bool debug = false;
        
        public StatKilledByChicken(ICoreServerAPI api) : base(api)
        {
            Title = "Died to Chickens";
            Init(GetKeyPrefix());
            Id = "MOST_KILLED_BY_CHICKENS";
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
                    killer = damageSource.SourceEntity.GetName();
                }
            }
            List<string> animals = new List<string>
            { "Hen", "Rooster", "Chick" };
            
            if (!animals.Contains(killer)) return;
            string key = GetKeyPrefix() ;
            int oldValue = GetOldValue(key + killer);
            Process(key, oldValue + 1, killer);
        }
    }
}
