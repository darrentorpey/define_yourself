using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngelXNA.Infrastructure.Console;
using AngelXNA.Infrastructure;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using AngelXNA.Physics;
using AngelXNA.Infrastructure.Logging;
using System.IO;
using System.Reflection;

namespace AngelXNA.Actors
{
    public class ActorFactory
    {
        public delegate void InitActorHandler(Actor a);

        private static string s_ActorTemplateDirectory = "ActorTemplates\\";

        private static ActorFactory s_Instance;

        public static ActorFactory Instance 
        { 
            get 
            {
                if (s_Instance == null)
                    s_Instance = new ActorFactory();
                return s_Instance; 
            } 
        }

        public string ActorTemplateDirectory
        {
            get { return @"Config\" + s_ActorTemplateDirectory; }
        }

        private ActorFactory()
        {
            DeveloperConsole.Instance.ItemManager.AddCommand("BeginActor", x => {
                DeveloperConsole.VerifyArgs(x, typeof(string));
                BeginActor((string)x[0]);
                return null;
            });

            DeveloperConsole.Instance.ItemManager.AddCommand("EndActor", x => {
                return EndActor(true);
            });

            DeveloperConsole.Instance.ItemManager.AddCommand("EndScope", x =>
            {
                EndScope();
                return null;
            });

            DeveloperConsole.Instance.ItemManager.AddCommand("SaveLevel", x => {
                DeveloperConsole.VerifyArgs(x, typeof(string));
                WriteLevel((string)x[0]);
                return null;
            });
        }

        public Actor CreateActor(string actorDefFile)
        {
            return CreateActor(actorDefFile, "", 0, null);
        }

        public Actor CreateActor(string actorDefFile, string name, int layer, InitActorHandler customInitActor)
        {
            BeginActor(actorDefFile);
            Actor retVal = EndActor(false);
            
            if (customInitActor != null)
                customInitActor(retVal);
            retVal.Name = name;

            if (layer != retVal.Layer)
                World.Instance.UpdateLayer(retVal, layer);

            return retVal;
        }

        public void LoadLevel(string levelfile)
        {
            levelfile = levelfile.Trim();

            if (levelfile.Length == 0)
                return;

            //Execute file
            //TODO: get accessor for Level dir
            StringBuilder sb = new StringBuilder(@"Config\Levels\");
            sb.Append(levelfile);
            sb.Append(".lvl");

            DeveloperConsole.Instance.ExecuteFile(sb.ToString());
        }

        public void WriteLevel(string levelfile)
        {
            levelfile = levelfile.Trim();

            if (levelfile.Length == 0)
                return;

            //Execute file
            StringBuilder sb = new StringBuilder(@"Config\Levels\");
            sb.Append(levelfile);
            sb.Append(".lvl");

            using(StreamWriter stream = new StreamWriter(sb.ToString()))
            {
                World.Instance.ForEachActor( actor => {
                    SerializeActor(stream, actor);
                });
            }
        }

        private void BeginActor(string actorDefFile)
        {
            actorDefFile = actorDefFile.Trim();

            if (actorDefFile.Length == 0)
                return;

            //Execute file
            //TODO: get accessor for actor def dir
            StringBuilder sb = new StringBuilder(@"Config\");
            sb.Append(s_ActorTemplateDirectory);
            sb.Append(actorDefFile);
            sb.Append(".adf");

            DeveloperConsole.Instance.ExecuteFile(sb.ToString());

            // Set the actor's template deinition for seralization
            Actor theActor = (Actor)DeveloperConsole.Instance.CurrentScope;
            theActor.ActorDefinition = actorDefFile;
        }

        private void EndScope()
        {
            DeveloperConsole.Instance.EndUsing();
        }

        private Actor EndActor(bool addToWorld)
        {
            Actor theActor = (Actor)DeveloperConsole.Instance.CurrentScope;
            DeveloperConsole.Instance.EndUsing();

            // Only add to the world if EndActor is called from the console, 
            // not if it is called from CreateActor.  This allows level files
            // to specify actors in Begin/EndActor() blocks, but keeps
            // CreateActor from having the wierd side effect of also adding your
            // actor to the world.
            if(addToWorld)
                World.Instance.Add(theActor, theActor.Layer);

            return theActor;
        }

        public void SerializeActorDefinition(string definitionName, Actor myActor)
        {
            definitionName = definitionName.Trim();

            if (definitionName.Length == 0)
                return;

            StringBuilder sb = new StringBuilder(@"Config\");
            sb.Append(s_ActorTemplateDirectory);
            sb.Append(definitionName);
            sb.Append(".adf");

            ConstructorInfo constructor = myActor.GetType().GetConstructor(new Type[] {});
            if (constructor == null)
            {
                Log.Instance.Log("[WARN] Could not serialize actor definition. Actor type '" + myActor.GetType().Name + "' does not have a default constructuor.");
                return;
            }

            Actor template = (Actor)constructor.Invoke(null);

            using (StreamWriter stream = new StreamWriter(sb.ToString()))
            {
                stream.WriteLine("ActorFactory.InitializeActor(" + myActor.GetType().Name + ".Create())");
                SerializeActorProperties(stream, template, myActor);
            }

            // This actor is now of this definition
            myActor.ActorDefinition = definitionName;
        }

        public void SerializeActor(TextWriter stream, Actor myActor)
        {
            Actor template = null;
            if(myActor.ActorDefinition != null)
            {
                stream.WriteLine("BeginActor(\"" + myActor.ActorDefinition + "\")");
                template = CreateActor(myActor.ActorDefinition);
            }
            else
            {
                ConstructorInfo constructor = myActor.GetType().GetConstructor(new Type[] { });
                if (constructor != null)
                {
                    stream.WriteLine("ActorFactory.InitializeActor(" + myActor.GetType().Name + ".Create())");
                    template = (Actor)constructor.Invoke(null);
                }
            }

            if(template == null)
            {
                Log.Instance.Log("[WARN] Could not serialize actor '" + myActor.Name + "'.  Could not create a template for the actor.");
                return;
            }

            SerializeActorProperties(stream, template, myActor);

            // Write out all tags for the actor
            foreach (string tag in myActor.GetTags())
                stream.WriteLine("\tTag(\"" + tag + "\")");

            stream.WriteLine("EndActor()");
            stream.WriteLine();
        }

        private void SerializeActorProperties(TextWriter stream, Actor template, Actor myActor)
        {
            // Compare all console properties in the current Actor to those of a created version
            PropertyInfo[] properties = template.GetType().GetProperties();
            foreach (PropertyInfo info in properties)
            {
                if (info.GetCustomAttributes(typeof(ConsolePropertyAttribute), true).Length > 0)
                {
                    if (!info.GetValue(template, null).Equals(info.GetValue(myActor, null)))
                    {
                        object newValue = info.GetValue(myActor, null);
                        stream.Write('\t');
                        stream.Write(info.Name);
                        stream.Write(" = ");
                        // String is a special case as it needs to be enclosed in quotes
                        if (newValue is string)
                        {
                            stream.Write('"');
                            stream.Write(newValue.ToString());
                            stream.Write('"');
                        }
                        // Check if the console has this type registered, meaning it will have
                        // special syntax to write it out.
                        else
                        {
                            ConsoleType type = DeveloperConsole.Instance.ItemManager.GetConsoleType(newValue.GetType());
                            if (type != null)
                            {
                                stream.Write(type.Serialize(newValue));
                            }
                            else
                                stream.Write(newValue.ToString());
                        }
                        stream.WriteLine();
                    }
                }
            }            
        }

        [ConsoleMethod]
        public static void InitializeActor(Actor aTheActor)
        {
            DeveloperConsole.Instance.Using(aTheActor);
        }
    }
}
