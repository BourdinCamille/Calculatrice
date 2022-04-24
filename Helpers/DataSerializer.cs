using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace Calculatrice.Helpers
{
    public static class DataSerializer
    {
        public static void XmlSerialize(Type dataType, object data, string filePath)
        {
            if (data != null)
            {
                XmlSerializer xmlSerializer = new XmlSerializer(dataType);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                TextWriter writer = new StreamWriter(filePath);
                xmlSerializer.Serialize(writer, data);
                writer.Close();
            }
        }

        public static object XmlDeserialize(Type dataType, string filePath)
        {
            object obj = null;

            XmlSerializer xmlSerializer = new XmlSerializer(dataType);
            if (File.Exists(filePath))
            {
                TextReader textReader = new StreamReader(filePath);
                obj = xmlSerializer.Deserialize(textReader);
                textReader.Close();
            }
            return obj;
        }
    }
}
