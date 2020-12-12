using System;
using System.IO;
using System.Reflection;

namespace LabDeCSharp
{
    public class Manager
    {
        const string XmlConfigName = @"config.xml";
        const string JsonConfigName = @"appsettings.json";
        private readonly string CurPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public Options GetOptions()
        {
            string JsonConfigPath = Path.Combine(CurPath, JsonConfigName);
            string XmlConfigPath = Path.Combine(CurPath, XmlConfigName);
            JsonParser jsonParser;
            try
            {
                jsonParser = new JsonParser(JsonConfigPath);
                Log("JSON was uploaded");
            }
            catch
            {
                jsonParser = null;
            }
            XmlParser xmlParser;
            try
            {
                xmlParser = new XmlParser(XmlConfigPath);
                Log("XML was uploaded");
            }
            catch
            {
                xmlParser = null;
            }
            Options options = new Options();
            bool OptionsIsFilled = false;
            if (!OptionsIsFilled && !(jsonParser is null))
            {
                try
                {
                    options.SourceDirectory = jsonParser.GetElementValue("SourceDirectory");
                    options.TargetDirectory = jsonParser.GetElementValue("TargetDirectory");
                    options.ArchiveDirectory = jsonParser.GetElementValue("ArchiveDirectory");
                    OptionsIsFilled = true;
                    Log("JSON was successfully used");
                }
                catch
                {
                    options = new Options();
                    Log("JSON was corrupted");
                }
            }
            if (!OptionsIsFilled && !(xmlParser is null))
            {
                try
                {
                    options.SourceDirectory = xmlParser.GetElementValue("SourceDirectory");
                    options.TargetDirectory = xmlParser.GetElementValue("TargetDirectory");
                    options.ArchiveDirectory = xmlParser.GetElementValue("ArchiveDirectory");
                    OptionsIsFilled = true;
                    Log("XML was successfully used");
                }
                catch
                {
                    options = new Options();
                    Log("XML was corrupted");
                }
            }
            if (!OptionsIsFilled)
            {
                Log("DefaultOptions was used");
                OptionsIsFilled = true;
            }
            return options;
        }

        const string LogFile = "logs.txt";
        private void Log(string info)
        {
            File.AppendAllText(Path.Combine(CurPath, LogFile), DateTime.Now + " | "+ info + '\n');
        }
    }
}
