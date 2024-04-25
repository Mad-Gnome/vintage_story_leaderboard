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
using Vintagestory.GameContent;

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
            OverrideMethod = "None";
            sapi.Event.RegisterGameTickListener(OnTick, 60 * 1000);
        }

        private void OnTick(float dt)
        {

            foreach (IPlayer player in sapi.World.AllOnlinePlayers) {
                string name = player?.Entity?.GetName();
                if (name == null) return;
                string key = GetKeyPrefix();
                int oldValue = GetOldValue(key + name);
                Process(key, oldValue + 1, name);
            }
        }
    }
}
