using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
using static System.Formats.Asn1.AsnWriter;

namespace scoreboard
{
    public class StatDeepestAveDepth: Leaderstat
    {
        private bool debug = false;
        private IServerPlayer player;
        EntityPos curPos;
        private FastVec3d pos;
        private double dist;
        public StatDeepestAveDepth(ICoreServerAPI api) : base(api)
        {
            Title = "Deepest Average Depth (below 0)";
            Init(GetKeyPrefix());
            Id = "DEEPEST_AVE_DEPTH";
            OverrideMethod = "PlayerNowPlaying";
        }

        public override void OverrideCB(IServerPlayer byPlayer)
        {
            player = byPlayer;
            curPos = player.Entity.ServerPos;
            sapi.Event.RegisterGameTickListener(onTick, 5*1000);
        }

        private void onTick(float dt)
        {
            string name = player.Entity.GetName();
            int depthValue = GetOldValue(GetKeyPrefix() + name + "Depth");
            int depthQuotient = GetOldValue(GetKeyPrefix() + name + "Quotient");
            depthValue += Convert.ToInt32(curPos.Y) ;
            depthQuotient += 1;
            double newAve = depthValue / depthQuotient;
            if (debug) sapi.Logger.Debug("New average depth: {0}", newAve.ToString());
            newAve *= -1;
            if (newAve < 0) return;
            sapi.WorldManager.SaveGame.StoreData(GetKeyPrefix() + name + "Depth", depthValue);
            sapi.WorldManager.SaveGame.StoreData(GetKeyPrefix() + name + "Quotient", depthQuotient);
            Process(GetKeyPrefix(), (int)newAve, name );

        }
    }
}
