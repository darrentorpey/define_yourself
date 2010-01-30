using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace AngelXNA.Infrastructure.Console
{
    public class ConsoleVariable : ConsoleItem
    {
        [Flags]
        public enum Flags
        {
            Config,
            ReadOnly
        }

        private object _value;

        private ValueCache<string> _stringCache;
        private ValueCache<float> _floatCache;
        private ValueCache<int> _intCache;
        private ValueCache<Vector2> _vectorCache;

        public override string Description
        {
            get
            {
                return GetDescriptionInternal(false);
            }
        }

        public object Value
        {
            get { return _value; }
            set
            {
                if (HasFlag((int)Flags.ReadOnly))
                    return;
                _value = value;
                DirtyCache();

                if (HasFlag((int)Flags.Config))
                    DeveloperConsole.Instance.WriteConfigCvars();
            }
        }

        public string StringValue 
        {
            get { return _stringCache.Value; }
            set { Value = value; }
        }

        public int IntValue 
        { 
            get { return _intCache.Value; }
            set { Value = value.ToString(); }
        }

        public float FloatValue 
        { 
            get { return _floatCache.Value; }
            set { Value = value.ToString(); }
        }

        public Vector2 VectorValue 
        { 
            get { return _vectorCache.Value; }
            set { Value = value.ToString(); }
        }

        public bool HasValue { get { return _value != null; } }

        internal ConsoleVariable(string id)
            : base(id)
        {
            _stringCache = new ValueCache<string>(new ValueCache<string>.UpdateHandler(UpdateCachedString));
            _floatCache = new ValueCache<float>(new ValueCache<float>.UpdateHandler(UpdateCachedFloat));
            _intCache = new ValueCache<int>(new ValueCache<int>.UpdateHandler(UpdateCachedInt));
            _vectorCache = new ValueCache<Vector2>(new ValueCache<Vector2>.UpdateHandler(UpdateCachedVector));
        }

        public string Serialize()
        {
            return GetDescriptionInternal(true);
        }

        public void DeserializeParams(string[] paramList)
        {
            //Zero'th entry is the cvar decl. We want the next entry
	        for( int i = 1; i < paramList.Length; i++ )
	        {
		        string par = paramList[i].Trim().ToUpper();
		        if( par == "FLAGS=" )
		        {
			        //If we found flags at the start
#if XBOX360
                    string[] flags = par.Substring("FLAGS=".Length).Split(new char[] { ' ' });
#else
			        string[] flags = par.Substring("FLAGS=".Length).Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
#endif
			        for( int j = 1; j < flags.Length; j++ )
			        {
                        // TODO: Put in a TryParse?
                        try
                        {
                            Flags flag = (Flags)Enum.Parse(typeof(Flags), flags[j], true);
                            SetFlag((int)flag);
                        }
                        catch(ArgumentException) { }
			        }
		        }
	        }
        }

        private void UpdateCachedString(ref string val)
        {
            val = _value.ToString();
        }

        private void UpdateCachedFloat(ref float val)
        {
            val = Convert.ToSingle(_value);
        }

        private void UpdateCachedInt(ref int val)
        {
            val = Convert.ToInt32(_value);
        }

        private void UpdateCachedVector(ref Vector2 val)
        {
            // TODO: Vector2 parsing
            //val = Vector2.Parse(_stringVal);
        }

        private void DirtyCache()
        {
            _stringCache.Dirty();
            _floatCache.Dirty();
            _intCache.Dirty();
            _vectorCache.Dirty();
        }

        private string GetDescriptionInternal( bool toFile )
        {
            StringBuilder sb = new StringBuilder();
            if (toFile)
                sb.Append("DeclareCVar ");

            sb.AppendFormat("{0,-25} = {1,-20}", Id, StringValue);

            if( _flags != 0)
	        {                
		        if( toFile )
			        sb.Append("| ");
		        sb.Append("FLAGS= ");
		        if( HasFlag((int)Flags.Config) )
                {
			        sb.Append(Flags.Config.ToString());
                    sb.Append(" ");
                }
		        if( HasFlag((int)Flags.ReadOnly) )
                {
                    sb.Append(Flags.ReadOnly.ToString());
                    sb.Append(" ");
                }
	        }

            return sb.ToString();
        }
    }
}
