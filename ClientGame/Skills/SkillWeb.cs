using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;

using AngelXNA;
using AngelXNA.Actors;
using AngelXNA.Infrastructure.Logging;
using AngelXNA.Messaging;

namespace DefineYourself.Skills
{
    class SkillWeb
    {
        private ClientGame _game;

        public ClientGame Game { get { return _game; } set { _game = value; } }

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

        public int GetScoreP1()
        {
            int sum = 0;
            Nodes.ForEach(node => { if (node.Completed("Player 1")) { sum += 1; } });
            return sum;
        }

        public int GetScoreP2()
        {
            int sum = 0;
            Nodes.ForEach(node => { if (node.Completed("Player 2")) { sum += 1; } });
            return sum;
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
                        _game.StatusTextP1.Color = Color.Green;
                        _game.StatusTextP1.DisplayString = skillName + " already completed";
                    }
                    else
                    {
                        node.P1_Points += 1;
                        if (node.Completed(player.Name))
                        {
                            justFinished = true;
                        }
                        updated = true;
                        _game.StatusTextP1.Color = Color.Blue;
                        _game.StatusTextP1.DisplayString = skillName + " updating";
                    }
                }
                else
                {
                    _game.StatusTextP1.Color = Color.White;
                    _game.StatusTextP1.DisplayString = skillName + ": prereqs not met";
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
                        _game.StatusTextP2.Color = Color.Green;
                        _game.StatusTextP2.DisplayString = skillName + " already completed";
                    }
                    else
                    {
                        node.P2_Points += 1;
                        if (node.Completed(player.Name))
                        {
                            justFinished = true;
                        }
                        updated = true;
                        _game.StatusTextP2.Color = Color.Blue;
                        _game.StatusTextP2.DisplayString = skillName + " updating";
                    }
                }
                else
                {
                    _game.StatusTextP2.Color = Color.Black;
                    _game.StatusTextP2.DisplayString = skillName + ": prereqs not met";
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
