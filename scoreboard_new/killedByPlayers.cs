﻿using ProtoBuf;
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
    public class StatKilledByPlayers: Leaderstat
    {
        private bool debug = false;
        public StatKilledByPlayers(ICoreServerAPI api) : base(api)
        {
            Title = "Killed by Other Players";
            Init(GetKeyPrefix());
            Id = "KILLED_BY_PLAYERS";
            OverrideMethod = "PlayerDeath";
        }

        public override void OverrideCB(IServerPlayer byPlayer, DamageSource damageSource)
        {
            if (damageSource != null)
            {
                if (damageSource.SourceEntity != null && damageSource.SourceEntity is EntityPlayer)
                {
                    string name = byPlayer.Entity.GetName();
                    string key = GetKeyPrefix();
                    int oldValue = GetOldValue(key + name);
                    Process(key, oldValue + 1, name);
                }
            }
            
        }
    }
}
