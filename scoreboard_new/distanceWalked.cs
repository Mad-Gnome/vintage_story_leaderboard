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
    public class StatDistanceWalked: Leaderstat
    {
        private bool debug = false;
        private Dictionary<String, FastVec3d> positions = new();
        public StatDistanceWalked(ICoreServerAPI api) : base(api)
        {
            if (api?.Side == null) return;
            if (api.Side.IsClient()) return;
            Title = "Blocks Walked";
            sapi.Logger.Debug("distance walked key {0}", GetKeyPrefix());
            Init(GetKeyPrefix());
            Id = "DISTANCE_WALKED";
            OverrideMethod = "PlayerNowPlaying";
            sapi.Event.RegisterGameTickListener(onTick, 10 * 1000);
        }

        public override void OverrideCB(IServerPlayer byPlayer)
        {
            if (byPlayer?.PlayerUID == null) return;
            if (byPlayer?.Entity?.ServerPos == null) return;
            EntityPos pos = byPlayer.Entity.ServerPos;
            positions[byPlayer.PlayerUID] = new FastVec3d(pos.X, pos.Y, pos.Z);
            
        }

        private void onTick(float dt)
        {
            if (sapi?.Side == null) return;
            if (sapi.Side.IsClient()) return;
            foreach (IPlayer player in sapi.World.AllOnlinePlayers)
            {
                if (player?.Entity == null) continue;
                string name = player?.Entity?.GetName();
                if (name == null) continue;
                if (player?.Entity?.ServerPos == null) continue;
                if (!positions.ContainsKey(player.PlayerUID)) continue;
                EntityPos curEntPos = player?.Entity?.ServerPos;
                if(curEntPos == null) continue;
                FastVec3d newVec = new(curEntPos.X, curEntPos.Y, curEntPos.Z);
                FastVec3d oldPos = positions[player.PlayerUID];
                if(debug) sapi.Logger.Debug("old: {0} {1} {2} ", oldPos.X, oldPos.Y, oldPos.Z);
                if(debug) sapi.Logger.Debug("new: {0} {1} {2} ", curEntPos.X, curEntPos.Y, curEntPos.Z);
                double newDist = newVec.Distance(oldPos);
                string key = GetKeyPrefix();
                int value = GetOldValue(key + name);
                if (newDist < 100) //ignore impossible distances
                {
                    value += (int)newDist;
                    Process(GetKeyPrefix(), (int)value, player.Entity.GetName());
                    if (debug) sapi.Logger.Debug("{0} Just walked {1}", name, newDist.ToString());
                }
                
                
                Process(key, value, name);
                positions[player.PlayerUID] = newVec;
            }
            

            
            
            
            
           
        }
    }
}
