using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Util;
using System.Xml.Linq;
using Vintagestory.API.Datastructures;

namespace scoreboard
{
    public class Leaderstat
    {
        // Property to hold the core server API instance
        protected ICoreServerAPI sapi;
        public List<Leader> leaders = new();
        public string Ver = "";
        // Constructor to initialize the core server API instance
        public Leaderstat(ICoreServerAPI api)
        {
            sapi = api;
            
            for (int i = 0; i < 10; i++)
            {
                leaders.Add(new Leader("Empty", 0));          }
        }

        public void Init(string prefix)
        {
            //sapi.Logger.Debug("loading under prefix {0}", prefix);
            for(int i = 0; i < 10; i++)
            {

                byte[] data = sapi.WorldManager.SaveGame.GetData(prefix + "leadername" + i.ToString());
                if (data != null)
                {
                    //sapi.Logger.Debug("loading previous leader");
                    leaders[i].name = SerializerUtil.Deserialize<string>(data);
                }
                data = sapi.WorldManager.SaveGame.GetData(prefix + "leadervalue" + i.ToString());
                if (data != null)
                {
                    leaders[i].value = SerializerUtil.Deserialize<int>(data);
                }
            }
            
        }
        public string OverrideMethod { get; set; }

        //OnEntityDeath
        public virtual void OverrideCB(Entity entity, DamageSource damageSource)
        {
            Console.WriteLine("Leaderstat's OverrideCB");
        }

        //DidBreakBlock
        public virtual void OverrideCB(IServerPlayer byPlayer, int oldblockId, BlockSelection blockSel)
        {
            Console.WriteLine("Leaderstat's OverrideCB");
        }

        //DidPlaceBlock
        public virtual void OverrideCB(IServerPlayer byPlayer, int oldblockId, BlockSelection blockSel, ItemStack withItemStack)
        {
            Console.WriteLine("Leaderstat's OverrideCB");
        }

        //DidUseBlock
        public virtual void OverrideCB(IServerPlayer byPlayer, BlockSelection blockSel)
        {
            Console.WriteLine("Leaderstat's OverrideCB");
        }

        //PlayerChat
        public virtual void OverrideCB(IServerPlayer byPlayer, int channelId, ref string message, ref string data, BoolRef consumed)
        {
            Console.WriteLine("Leaderstat's OverrideCB");
        }

        //PlayerNowPlaying
        public virtual void OverrideCB(IServerPlayer byPlayer)
        {
            Console.WriteLine("Leaderstat's OverrideCB");
        }

        //PlayerDeath
        public virtual void OverrideCB(IServerPlayer byPlayer, DamageSource damageSource)
        {
            Console.WriteLine("Leaderstat's OverrideCB");
        }

        public string GetKeyPrefix()
        {
            return "scoreboard;" + Title + ";" + Ver;
        }


        public string Id { get; set; } = null;


        public string Title { get; set; } = null;

        

        public void SaveLeaders(string prefix)
        {
            //sapi.Logger.Debug("saving under prefix {0}", prefix);
            int i = 0;
            foreach (Leader leader in leaders)
            {
                sapi.WorldManager.SaveGame.StoreData(prefix + "leadername" + i.ToString(), SerializerUtil.Serialize(leader.name));
                sapi.WorldManager.SaveGame.StoreData(prefix + "leadervalue" + i.ToString(), SerializerUtil.Serialize(leader.value));
                i++;
            }
            
        }

        public int GetOldValue(string key)
        {
            byte[] data = sapi.WorldManager.SaveGame.GetData(key);
            int oldValue;
            if (data == null) oldValue = 0;
            else
            {
                oldValue = SerializerUtil.Deserialize<int>(data);
            }
            return oldValue;
        }

        public void Process(string key, int newValue, string name)
        {
            sapi.WorldManager.SaveGame.StoreData(key + name, newValue);
            AddToLeaders(new Leader(name, newValue));
            SaveLeaders(key);
        }
        public bool AddToLeaders(Leader newLeader)
        {
            // Check if the leader is already in the list and update their score if needed
            foreach (Leader oldLeader in leaders)
            {
                if (oldLeader.name == newLeader.name)  // Assuming each leader has a unique name to identify them
                {
                    if (newLeader.value > oldLeader.value)
                    {
                        oldLeader.value = newLeader.value;  // Update the value if the new score is higher
                        leaders.Sort((a, b) => b.value - a.value);  // Re-sort the list based on value
                        return true;
                    }
                    return false;  // Return false if the new score is not higher
                }
            }

            // If the leader is not already in the list, attempt to add them
            if (leaders.Count < 10)
            {
                leaders.Add(newLeader);
                leaders.Sort((a, b) => b.value - a.value);  // Sort after adding
                return true;
            }
            else if (newLeader.value > leaders[9].value)  // Check if new leader's value is higher than the lowest in the top 10
            {
                leaders[9] = newLeader;  // Replace the last leader
                leaders.Sort((a, b) => b.value - a.value);  // Re-sort the list
                return true;
            }

            return false;  // Return false if the new leader doesn't qualify for the top 10
        }



    }

    
    public class Leader
    {

        public string name;


        public int value;


        public Leader(string setName, int setValue)
        {
            name = setName;
            value = setValue;
        }
    }
}
