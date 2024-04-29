using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Server;

namespace scoreboard
{
    public class Promotion
    {
        private IServerPlayer p;
        ICoreServerAPI sapi;
        public Promotion(IServerPlayer player, ICoreServerAPI api)
        {
            sapi = api;
            sapi.Event.RegisterCallback(PromoteBoard, 5*1000);
            p = player;
        }

        public void PromoteBoard(float dt)
        {
            if (p.Entity != null && p.ConnectionState == EnumClientState.Playing)
            {
                String announcement = "Press U to view the server leaderstats boards.";
                p.SendMessage(GlobalConstants.ServerInfoChatGroup, announcement, EnumChatType.Notification);

            }
        }
    }
}
