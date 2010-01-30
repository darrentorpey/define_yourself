#if WINDOWS

using System.ComponentModel;
using AngelXNA.Infrastructure;

namespace AngelXNA.Editor
{
    public class FontConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(FontCache.Instance.GetRegisteredFonts());
        }
    }
}

#endif