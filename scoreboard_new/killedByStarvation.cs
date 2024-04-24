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
    public class StatKilledByStarvation: Leaderstat
    {
        private bool debug = false;
        public StatKilledByStarvation(ICoreServerAPI api) : base(api)
        {
            Title = "Died of Starvation";
            Init(GetKeyPrefix());
            Id = "KILLED_BY_STARVATION";
            OverrideMethod = "PlayerDeath";
        }

        public override void OverrideCB(IServerPlayer byPlayer, DamageSource damageSource)
        {
            string name = byPlayer.Entity.GetName();
            string key = GetKeyPrefix();
            int oldValue = GetOldValue(key + name);
            if (damageSource == null || damageSource.Type.ToString() != "Hunger") return;
            Process(key, oldValue + 1, name);
        }
    }
}


/*
BluntAttack = 2
Crushing = 9
Electricity = 11
Fire = 1
Frost = 10
Gravity = 0
Heal = 6
Heat = 12
Hunger = 8
Injury = 13
PiercingAttack = 4
Poison = 7
SlashingAttack = 3
Suffocation = 5*/