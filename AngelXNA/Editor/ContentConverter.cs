#if WINDOWS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.IO;
using AngelXNA.Actors;

namespace AngelXNA.Editor
{
    public class ContentConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            string[] files = Directory.GetFiles("Content", "*.*", SearchOption.AllDirectories);

            List<string> simplifiedSet = new List<string>();
            foreach(string file in files)
            {
                // Strip "Content/" off the front and remove the extension
                string stripped = Path.ChangeExtension(file.Remove(0, 8), null);
 
                // Ignore fonts
                if (stripped.StartsWith("Fonts", StringComparison.InvariantCultureIgnoreCase))
                    continue;
                // Ignore Sounds
                if (stripped.StartsWith("Sounds", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                if (Actor.IsAnimFile(stripped))
                {
                    int numberSeparator = stripped.LastIndexOf("_");
                    int numDigits = stripped.Length - numberSeparator - 1;

	                // If we got this far, the filename format is correct.
	                // The number string is just the digits between the '_' and the file extension (i.e. 001).
                    string numberString = stripped.Substring(numberSeparator + 1, numDigits);

	                // Get our starting numberical value.
	                int number = int.Parse(numberString);
                    if(number > 1)
                        continue;
                }

                simplifiedSet.Add(stripped);
            }
            return new StandardValuesCollection(simplifiedSet.ToArray());
        }
    }
}

#endif