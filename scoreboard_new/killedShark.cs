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
    public class StatKilledShark : Leaderstat
    {
        private bool debug = false;
        public StatKilledShark(ICoreServerAPI api) : base(api)
        {
            Title = "Most Sharks Killed";
            Init(GetKeyPrefix());
            Id = "MOST_SHARK_KILLED";
            OverrideMethod = "OnEntityDeath";
        }

        public override void OverrideCB(Entity entity, DamageSource damageSource)
        {
            // Check if the entity's code contains "wolf"
            if (!entity.Code.ToString().Contains("shark", StringComparison.OrdinalIgnoreCase)) return;
            if (debug) sapi.Logger.Debug("scoreboard logline!");
            if (debug) sapi.Logger.Debug(entity.Code.ToString());
            if (debug) sapi.Logger.Debug(entity.GetName());

            string killedByName = null;
            if (damageSource != null)
            {
                if (damageSource.GetCauseEntity() == null)
                {
                    killedByName = null;
                }
                else
                {
                    if (damageSource.GetCauseEntity() is not EntityPlayer) return; // Make sure it's a player
                    killedByName = damageSource.GetCauseEntity().GetName(); // The killer
                }
            }

            if (killedByName != null)
            {
                string key = GetKeyPrefix();
                int oldValue = GetOldValue(key + killedByName); // Append player name to get old value
                Process(key, oldValue + 1, killedByName);
            }
        }
    }
}
