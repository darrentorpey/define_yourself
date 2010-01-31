using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AngelXNA;
using AngelXNA.Actors;
using AngelXNA.Infrastructure.Logging;
using AngelXNA.Messaging;

namespace DefineYourself.Skills
{
    class SkillWeb
    {
        private static SkillWeb s_Instance = new SkillWeb();
        public static SkillWeb Instance
        {
            get { return s_Instance; }
        }

        public List<SkillNode> Nodes { get; set; }// = new List<SkillNode>();

        public SkillWeb()
        {
            Nodes = new List<SkillNode>();
        }

        public void AddNode(SkillNode node)
        {
            Nodes.Add(node);
        }

        public void Report()
        {
            Console.WriteLine("NodeZ:");
            foreach (SkillNode node in Nodes)
            {
                node.Report();
            }
        }

        public void UpdateSkillProgress(MessageObject message) {
            Actor player = message.play;
            Actor building = message.bldg;

            updatePlayerSkillProgress(player, building.Name);
        }

        public SkillNode FindNodeByName(string name)
        {
            return Nodes.Find(x => { return x.Name == name; });
        }

        public SkillNode FindActiveNodeByNameAndPlayer(string name, string playerName)
        {
            return Nodes.Find(x => { return ((x.Name == name) && (x.ActiveForPlayer(playerName))); });
        }

        private bool updatePlayerSkillProgress(Actor player, string skillName)
        {
            bool updated = false;
            bool justFinished = false;
            if (player.Name == "Player 1")
            {
                SkillNode node = FindActiveNodeByNameAndPlayer(skillName, player.Name);
                if (node != null)
                {
                    if (node.Completed(player.Name))
                    {
                        updated = false;
                    }
                    else
                    {
                        node.P1_Points += 1;
                        if (node.Completed(player.Name))
                        {
                            justFinished = true;
                        }
                        updated = true;
                    }
                }
            }
            else if (player.Name == "Player 2")
            {
                SkillNode node = FindActiveNodeByNameAndPlayer(skillName, player.Name);
                if (node != null)
                {
                    if (node.Completed(player.Name))
                    {
                        updated = false;
                    }
                    else
                    {
                        node.P2_Points += 1;
                        if (node.Completed(player.Name))
                        {
                            justFinished = true;
                        }
                        updated = true;
                    }
                }
            }
            MessageObject messageObject = new MessageObject();
            messageObject.play = player;
            messageObject.boolie = updated;
            messageObject.justFinished = justFinished;

            Switchboard.Instance.Broadcast(new Message("SkillUpdate", messageObject));
            return updated;
        }
    }
}
