using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Xml;

namespace LabDeCSharp
{
    class JsonParser
    {
        private readonly Dictionary<string, string> jsonElements;

        public JsonParser(string jsonDocPath)
        {
            if (jsonDocPath is null)
            {
                throw new ArgumentNullException(nameof(jsonDocPath));
            }
            jsonElements = new Dictionary<string, string>();
            string jsonDocContents = File.ReadAllText(jsonDocPath);
            using (JsonDocument doc = JsonDocument.Parse(jsonDocContents))
            {
                JsonElement root = doc.RootElement;
                var props = root.EnumerateObject();
                while (props.MoveNext())
                {
                    var prop = props.Current;
                    jsonElements.Add(prop.Name, prop.Value.ToString());
                }
            }
        }

        public string GetElementValue(string key)
        {
            return jsonElements[key];
        }
    }
    class XmlParser
    {
        private readonly Dictionary<string, string> xmlElements;

        public XmlParser(string xmlDocPath)
        {
            if (xmlDocPath is null)
            {
                throw new ArgumentNullException(nameof(xmlDocPath));
            }
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlDocPath);
            xmlElements = new Dictionary<string, string>();
            foreach (XmlNode node in doc.DocumentElement)
            {
                xmlElements.Add(node.Name, node.InnerText);
            }
        }

        public string GetElementValue(string key)
        {
            return xmlElements[key];
        }
    }
}
