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
    public class StatDistanceWalked: Leaderstat
    {
        private bool debug = false;
        private IServerPlayer player;
        EntityPos curPos;
        private FastVec3d pos;
        private double dist;
        public StatDistanceWalked(ICoreServerAPI api) : base(api)
        {
            Title = "Distance Walked";
            Init(GetKeyPrefix());
            Id = "DISTANCE_WALKED";
            OverrideMethod = "PlayerNowPlaying";
        }

        public override void OverrideCB(IServerPlayer byPlayer)
        {
            player = byPlayer;
            curPos = player.Entity.ServerPos;
            pos = new(curPos.X, curPos.Y, curPos.Z);
            int oldValue = GetOldValue(GetKeyPrefix() + player.Entity.GetName());
            if (debug) sapi.Logger.Debug("Already walked {0}", oldValue.ToString());
            dist = oldValue;
            sapi.Event.RegisterGameTickListener(onTick, 10*1000);
        }

        private void onTick(float dt)
        {
            FastVec3d newPos = new(curPos.X, curPos.Y, curPos.Z);

            double newDist = newPos.Distance(pos);
            if(debug) sapi.Logger.Debug("Just walked {0}", newDist.ToString());
            if(newDist < 100) //ignore impossible distances
            {
                dist += newDist;
                Process(GetKeyPrefix(), (int)dist, player.Entity.GetName());
            }
            pos = newPos;
           
        }
    }
}
