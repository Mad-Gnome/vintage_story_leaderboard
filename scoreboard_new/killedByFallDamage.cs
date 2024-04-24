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
    public class StatKilledByFallDamage: Leaderstat
    {
        private bool debug = false;
        public StatKilledByFallDamage(ICoreServerAPI api) : base(api)
        {
            Title = "Killed By Fall Damage";
            Init(GetKeyPrefix());
            Id = "KILLED_BY_FALL_DAMAGE";
            OverrideMethod = "PlayerDeath";
        }

        public override void OverrideCB(IServerPlayer byPlayer, DamageSource damageSource)
        {
            string name = byPlayer.Entity.GetName();
            string key = GetKeyPrefix();
            int oldValue = GetOldValue(key + name);
            if (damageSource == null || damageSource.Type.ToString() != "Gravity") return;
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