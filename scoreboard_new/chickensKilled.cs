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
    public class StatChickensKilled: Leaderstat
    {
        private bool debug = false;
        public StatChickensKilled(ICoreServerAPI api) : base(api)
        {
            Title = "Most Chickens Killed";
            Init(GetKeyPrefix());
            Id = "MOST_CHICKENS_KILLED";
            OverrideMethod = "OnEntityDeath";
        }

        public override void OverrideCB(Entity entity, DamageSource damageSource)
        {
            List<string> animals = new List<string>
            { "Dead hen", "Dead rooster", "Dead chick" };
            if (!animals.Contains(entity.GetName())) return;
            string killedByName = null;
            if (damageSource != null)
            {
                if (damageSource.SourceEntity == null)
                {
                    killedByName = null;
                }
                else
                {
                    if (damageSource.SourceEntity is not EntityPlayer) return; //make sure it's a player
                    killedByName = damageSource.SourceEntity.GetName(); //the killer
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
