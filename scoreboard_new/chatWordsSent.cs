using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.MathTools;
using Vintagestory.API.Server;
using Vintagestory.API.Util;

namespace scoreboard
{
    public class StatChatWordsSent: Leaderstat
    {
        private bool debug = false;
        public StatChatWordsSent(ICoreServerAPI api) : base(api)
        {
            Title = "Chat Words Sent";
            Init(GetKeyPrefix());
            Id = "CHAT_WORDS_SENT";
            OverrideMethod = "PlayerChat";
        }

        public override void OverrideCB(IServerPlayer byPlayer, int channelId, ref string message, ref string data, BoolRef consumed)
        {
            int wordCount = GetOldValue(GetKeyPrefix() + byPlayer.Entity.GetName());
            wordCount += message.Split(" ").Length;
            int avoCount = Regex.Matches(message.ToLower(), "avocado").Count; //use this to count a specific word
            Process(GetKeyPrefix(), wordCount, byPlayer.Entity.GetName());
        }
    }
}
