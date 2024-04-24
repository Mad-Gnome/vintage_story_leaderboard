using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
    public class StatTimeOnServer : Leaderstat
    {
        private bool debug = false;
        private IServerPlayer player;
        int timeOnServer;
        public StatTimeOnServer(ICoreServerAPI api) : base(api)
        {
            Title = "Time on Server (mins)";
            Init(GetKeyPrefix());
            Id = "TIME_ON_SERVER";
            OverrideMethod = "PlayerNowPlaying";
        }

        public override void OverrideCB(IServerPlayer byPlayer)
        {
            player = byPlayer;
            int oldValue = GetOldValue(GetKeyPrefix() + player.Entity.GetName());
            timeOnServer = oldValue;
            if (debug) sapi.Logger.Debug("Already been on {0} minutes.", oldValue.ToString());
            sapi.Event.RegisterGameTickListener(onTick, 60 * 1000);
        }
        private void onTick(float dt)
        {
            timeOnServer++;
            Process(GetKeyPrefix(), timeOnServer, player.Entity.GetName());
            if (debug) sapi.Logger.Debug("Just played another minute.");
        }
    }
}
