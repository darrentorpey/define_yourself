using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace AngelXNA.Infrastructure.Console
{
    public class ConsoleItemManager
    {
        private Dictionary<string, ConsoleVariable> _cvarTable = new Dictionary<string,ConsoleVariable>();

        private Dictionary<string, Dictionary<string, ConsoleCommand>> _commandTable = new Dictionary<string, Dictionary<string, ConsoleCommand>>();
        private Dictionary<string, Dictionary<string, PropertyInfo>> _objPropertyTable = new Dictionary<string, Dictionary<string, PropertyInfo>>();
        private Dictionary<Type, ConsoleType> _typeTable = new Dictionary<Type, ConsoleType>();

        public ConsoleItemManager()
        {
            // Global namespace always exists
            _commandTable.Add("", new Dictionary<string, ConsoleCommand>());
        }

        public ConsoleVariable GetCVar( string id )
        {
            ConsoleVariable cvar = GetCVarInternal(id);

            if (cvar != null)
                return cvar;

            return CreateConsoleVariable(id);
        }

        public ConsoleVariable FindCVar(string id)
        {
            return GetCVarInternal(id);
        }

        public ConsoleCommand FindCommand(object scope, string command, bool allowGlobal)
        {
            string actualScope;

            if(scope is string)
                actualScope = (string)scope;
            else if(scope == null)
                actualScope = "";
            else
                actualScope = scope.GetType().FullName;

            ConsoleCommand retCommand = null;
            if (_commandTable.ContainsKey(actualScope))
            {
                if (_commandTable[actualScope].ContainsKey(command))
                    retCommand = _commandTable[actualScope][command];
            }

            // If we were searching non-global but we didn't find a command
            // and we're allowing global calls to be made...
            if (actualScope != "" && retCommand == null && allowGlobal)
            {
                // Search for the global call
                if (_commandTable[""].ContainsKey(command))
                    retCommand = _commandTable[""][command];
            }

            return retCommand;
        }

        public void LoadMembersFromAssembly(Assembly assm)
        {
#if WINDOWS
            // Don't bother with GAC entries
            if (assm.GlobalAssemblyCache == true)
                return;
#endif
            foreach (Type type in assm.GetTypes())
            {
                string sshortTypeName = type.Name;
                string stypeName = type.FullName;
                if (type.IsGenericType || type.IsGenericTypeDefinition || type.IsInterface || type.BaseType.IsGenericType)
                    continue;
                
                MemberInfo[] info = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static);

                for (int i = 0; i < info.Length; ++i)
                {
                    if (info[i].MemberType == MemberTypes.Method)
                    {
                        ConsoleMethodAttribute[] method = (ConsoleMethodAttribute[])info[i].GetCustomAttributes(typeof(ConsoleMethodAttribute), true);
                        if (method != null && method.Length > 0)
                        {
                            string name = method[0].Name;
                            if (name == null)
                                name = info[i].Name;

                            ConsoleCommand newCommand = new ConsoleCommand(name, (MethodInfo)info[i]);
                            newCommand.SetFlag((int)method[0].Flags);

                            MethodInfo myinfo = (MethodInfo)info[i];
                            string tableKey = stypeName;
                            if (myinfo.IsStatic)
                                tableKey = sshortTypeName;

                            if (!_commandTable.ContainsKey(tableKey))
                                _commandTable.Add(tableKey, new Dictionary<string, ConsoleCommand>());

                            if (_commandTable[tableKey].ContainsKey(name))
                                _commandTable[tableKey][name] = newCommand;
                            else
                                _commandTable[tableKey].Add(name, newCommand);
                        }
                    }
                    else if (info[i].MemberType == MemberTypes.Property)
                    {
                        ConsolePropertyAttribute[] method = (ConsolePropertyAttribute[])info[i].GetCustomAttributes(typeof(ConsolePropertyAttribute), true);
                        if (method != null && method.Length > 0)
                        {
                            string name = method[0].Name;
                            if (name == null)
                                name = info[i].Name;

                            PropertyInfo myinfo = (PropertyInfo)info[i];
                            if (!_objPropertyTable.ContainsKey(stypeName))
                                _objPropertyTable.Add(stypeName, new Dictionary<string, PropertyInfo>());

                            if (_objPropertyTable.ContainsKey(name))
                                _objPropertyTable[stypeName][name] = myinfo;
                            else
                                _objPropertyTable[stypeName].Add(name, myinfo);
                        }
                    }
                }
            }
        }

        public void AddCommand(string id, ConsoleCommandHandler handler)
        {
            ConsoleCommand newCommand = new ConsoleCommand(id, handler);
            //newCommand.SetFlag( (int)flags );

            if (_commandTable[""].ContainsKey(id))
                _commandTable[""][id] = newCommand;
            else
                _commandTable[""].Add(id, newCommand);
        }

        public void AddType(ConsoleType type)
        {
            if (_typeTable.ContainsKey(type.RealType))
                _typeTable[type.RealType] = type;
            else
                _typeTable.Add(type.RealType, type);

            // Add the console command for deserializing
            AddCommand(type.ConsoleName, type._deserializer);
        }

        public ConsoleType GetConsoleType(Type realType)
        {
            if (_typeTable.ContainsKey(realType))
                return _typeTable[realType];

            return null;
        }

        public object GetPropertyValue(object var, string property)
        {
            string type = var.GetType().FullName;
            if (!_objPropertyTable.ContainsKey(type))
                return null;

            if (!_objPropertyTable[type].ContainsKey(property))
                return null;

            return _objPropertyTable[type][property].GetGetMethod().Invoke(var, null);
        }

        public void SetPropertyValue(object var, string property, object value)
        {
            string type = var.GetType().FullName;
            if (!_objPropertyTable.ContainsKey(type))
                return;

            if (!_objPropertyTable[type].ContainsKey(property))
                return;

            if (!_objPropertyTable[type][property].CanWrite)
                return;

            // If this in an integer property, convert the value
            if (_objPropertyTable[type][property].PropertyType == typeof(int))
                value = Convert.ToInt32(value);

            _objPropertyTable[type][property].GetSetMethod().Invoke(var, new object[] { value });
        }

        public void GetConsoleItemIds(string againstSubstring, ref List<string> out_ids )
        {
            if (againstSubstring.Length == 0)
                return;

            if (out_ids == null)
                out_ids = new List<string>();

            GetConsoleCommandIds(againstSubstring, ref out_ids);
            GetConsoleVariableIds(againstSubstring, ref out_ids);            
        }

        public void GetConsoleVariableIds( string againstSubstring, ref List<string> out_ids )
        {
            if(out_ids == null)
                out_ids = new List<string>();

	        foreach(ConsoleItem item in _cvarTable.Values)
	        {
                TryWriteConsoleItemsToList(item, againstSubstring, ref out_ids);
	        }
        }

        public void GetConsoleCommandIds( string againstSubstring, ref List<string> out_ids )
        {
            if(out_ids == null)
                out_ids = new List<string>();

	        //check against cmdTable
            foreach (ConsoleItem item in _commandTable[""].Values)
	        {
                TryWriteConsoleItemsToList(item, againstSubstring, ref out_ids);
	        }
        }

        public void SerializeConfigCVars(List<string> configCVars)
        {
            //check against cvarTable
	        foreach( ConsoleVariable cvar in _cvarTable.Values )
	        {
		        if( cvar != null && cvar.HasFlag((int)ConsoleVariable.Flags.Config) )
		        {
                    configCVars.Add(cvar.Serialize());
		        }
	        }
        }

        private ConsoleVariable GetCVarInternal(string useId)
        {
            if (_cvarTable.ContainsKey(useId))
                return _cvarTable[useId];
            else
                return null;
        }

        private ConsoleVariable CreateConsoleVariable(string useId)
        {
            ConsoleVariable newVar = new ConsoleVariable(useId);
            //Add to map
            _cvarTable.Add(useId, newVar);

            return newVar;
        }

        private void TryWriteConsoleItemsToList( ConsoleItem item, string againstSubstring, ref List<string> out_ids )
        {
	        if( item != null && !item.IgnoreForAutoComplete)
	        {
		        string id = item.Id.Substring(0, Math.Min(againstSubstring.Length, item.Id.Length) );
		        if( againstSubstring == id )
		        {
			        string compareText = item.Id;
			        string insertText = item.Description;
			        //Insert sorted
			        for( int i = 0; i < out_ids.Count; ++i) 
			        {
				        string val = out_ids[i];
				        if( compareText.CompareTo(val) < 0  )
				        {
					        out_ids.Insert(i, insertText );
					        return;
				        }
			        }
			        //otherwise, push it on the back
			        out_ids.Add( insertText );
		        }
	        }
        }
    }
}
