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
    public class StatKilledOutlaw : Leaderstat
    {
        private bool debug = false;
        public StatKilledOutlaw(ICoreServerAPI api) : base(api)
        {
            Title = "Most Outlaws Killed";
            Init(GetKeyPrefix());
            Id = "MOST_OUTLAW_KILLED";
            OverrideMethod = "OnEntityDeath";
        }

        public override void OverrideCB(Entity entity, DamageSource damageSource)
        {
            List<string> outlaws = new List<string>
            {"game:looter", "game:bandit-axe", "game:bandit-knife", "game:bandit-spear", "game:deserter-archer", "game:deserter-bannerman", "game:deserter-footman",
                "game:hound-feral", "game:hound-hunting", "game:poacher-archer", "game:poacher-spear", "game:yeoman-archer"};
            if (debug) sapi.Logger.Debug("scoreboard logline!");
            if (debug) sapi.Logger.Debug(entity.Code.ToString());
            if (debug) sapi.Logger.Debug(damageSource.GetCauseEntity().Code.ToString());


            if (!outlaws.Contains(entity.Code.ToString())) return;
            string killedByName = null;

            if (damageSource != null)
            {
                if (damageSource.GetCauseEntity() == null)
                {
                    killedByName = null;
                }
                else
                {
                    if (damageSource.GetCauseEntity() is not EntityPlayer) return; //make sure it's a player
                    killedByName = damageSource.GetCauseEntity().GetName(); //the killer
                }
            }
            
            if (killedByName != null) {
                string key = GetKeyPrefix() ;
                int oldValue = GetOldValue(key+killedByName); //append player name to getoldvalue
                Process(key, oldValue + 1, killedByName);
            }
        }
    }
}
