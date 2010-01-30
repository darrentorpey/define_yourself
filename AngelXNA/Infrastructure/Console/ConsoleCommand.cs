using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using AngelXNA.Infrastructure.Logging;

namespace AngelXNA.Infrastructure.Console
{
    public delegate object ConsoleCommandHandler(object[] input);

    public class ConsoleCommand : ConsoleItem
    {
        [Flags]
        public enum Flags
        {
            None = 0,
            Config
        }

        private ConsoleCommandHandler _command;
        private MethodInfo _methodInfo;

        public bool IsStatic
        {
            get
            {
                if (_command != null)
                    return false;
                else
                    return _methodInfo.IsStatic;
            }
        }

        public override bool IgnoreForAutoComplete
        {
            get { return HasFlag((int)Flags.Config); }
        }

        internal ConsoleCommand(string id, ConsoleCommandHandler command)
            : base(id)
        {
            _command = command;
        }

        internal ConsoleCommand(string id, MethodInfo command)
            : base(id)
        {
            _methodInfo = command;
        }

        public object Execute(object executeOn, object[] parameters)
        {
            if (_command != null)
                return _command(parameters);
            else
            {
                ParameterInfo[] parameterInfo = _methodInfo.GetParameters();
                if (parameterInfo.Length != parameters.Length)
                {
                    Log.Instance.Log("[Console] Execute(): Invalid number of parameters.");
                    return null;
                }

                // Check for down conversion of parameters (float to int, int to short, etc)
                object[] actualParmaters = new object[parameters.Length];
                for (int i = 0; i < actualParmaters.Length; ++i)
                {
                    if (parameterInfo[i].ParameterType.IsAssignableFrom(parameters[i].GetType()))
                        actualParmaters[i] = parameters[i];
                    else if (parameters[i].GetType() == typeof(float))
                    {
                        if (parameterInfo[i].ParameterType == typeof(int))
                            actualParmaters[i] = Convert.ToInt32(parameters[i]);
                        else
                        {
                            Log.Instance.Log("[Console] Execute(): Invalid number parameter at index " + i);
                            return null;
                        }
                    }
                }
                return _methodInfo.Invoke(executeOn, actualParmaters);
            }
        }
    }
}
