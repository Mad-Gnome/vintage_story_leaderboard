﻿using ProtoBuf;
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
    public class StatKilledChicken: Leaderstat
    {
        private bool debug = false;
        public StatKilledChicken(ICoreServerAPI api) : base(api)
        {
            Title = "Most Chickens Killed";
            Init(GetKeyPrefix());
            Id = "MOST_CHICKEN_KILLED";
            OverrideMethod = "OnEntityDeath";
        }

        public override void OverrideCB(Entity entity, DamageSource damageSource)
        {
            List<string> animals = new List<string>
            { "game:chicken-hen", "game:chicken-rooster", "game:chicken-baby" };
            if (debug) sapi.Logger.Debug("scoreboard logline!");
            if (debug) sapi.Logger.Debug(entity.Code.ToString());
            if (debug) sapi.Logger.Debug(entity.GetName());
            //           if (debug) sapi.Logger.Debug(entity.Code.ToString());

            if (!animals.Contains(entity.Code.ToString())) return;
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
