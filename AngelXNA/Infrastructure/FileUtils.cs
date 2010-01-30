using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace AngelXNA.Infrastructure
{
    public class FileUtils
    {
        public static bool GetLinesFromFile(string fileName, List<string> outList)
        {
            if(File.Exists(fileName))
            {
                using (StreamReader fileStream = File.OpenText(fileName))
                {
                    while(!fileStream.EndOfStream)
                        outList.Add(fileStream.ReadLine());
                }

                return true;
            }
            return false;
        }

        public static bool WriteLinesToFile(string fileName, List<string> outList)
        {
            try
            {
                using (FileStream fileStream = File.Open(fileName, FileMode.OpenOrCreate))
                {
                    using (TextWriter writer = new StreamWriter(fileStream))
                    {
                        for (int i = 0; i < outList.Count; ++i)
                            writer.WriteLine(outList[i]);
                    }
                }

                return true;
            }
            catch (UnauthorizedAccessException) { }

            return false;
        }

        public static bool MakeDirIfNeeded(string dirName)
        {
            DirectoryInfo dir = new DirectoryInfo(dirName);
            if (dir.Exists)
            {
                return false;
            }
            else
            {
                Directory.CreateDirectory(dirName);
                return true;
            }
        }

        public static bool AppendLineToFile( string fileName, List<string> lines )
        {
	        try
            {
                using (FileStream fileStream = File.Open(fileName, FileMode.Append))
                {
                    using (TextWriter writer = new StreamWriter(fileStream))
                    {
                        for (int i = 0; i < lines.Count; ++i)
                            writer.WriteLine(lines[i]);
                    }
                }

                return true;
            }
            catch (UnauthorizedAccessException) { }

	        return false;
        }
    }
}
