using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Media;
using System.Windows.Resources;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Drawing;
using System.Xml.Serialization;
using System.Net;
using IntelliStretch.UserControls;

namespace IntelliStretch
{
    class Utilities
    {
        public static AsyncSoundPlayer LoadSound(Uri soundUri)
        {
            StreamResourceInfo resourceInfo = System.Windows.Application.GetResourceStream(soundUri);
            Stream resourceStream = resourceInfo.Stream;
            AsyncSoundPlayer soundPlayer = new AsyncSoundPlayer(resourceStream);
            return soundPlayer;
        }

        public static ImageSource GetImage(string imageName)
        {            
            ImageSourceConverter imgSrcConverter = new ImageSourceConverter();
            return imgSrcConverter.ConvertFromString(@"pack://application:,,/images/" + imageName) as ImageSource;
        }

        public static int Round(float input)
        {
            return (int)((input < 0) ? (input - 0.5) : (input + 0.5));
        }

        // XML serialization
        public static void SaveToXML<T>(T info, string xmlFile)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            TextWriter textWriter = new StreamWriter(xmlFile);
            xmlSerializer.Serialize(textWriter, info);
            textWriter.Close();
        }

        public static T ReadFromXML<T>(string xmlFile, bool IsLocal)
        {
            XmlSerializer xmlDeSerializer = new XmlSerializer(typeof(T));
            if (IsLocal)
            {
                TextReader textReader = new StreamReader(xmlFile);
                T info = (T)xmlDeSerializer.Deserialize(textReader);
                textReader.Close();
                return info;
            }
            else
            {
                try
                {
                    WebClient webClient = new WebClient();
                    Stream xmlStream = webClient.OpenRead(xmlFile);
                    T info = (T)xmlDeSerializer.Deserialize(xmlStream);
                    xmlStream.Close();
                    return info;  

                }
                catch (WebException)
                {
                    System.Windows.MessageBox.Show("Cannot connect to server...", "Error", System.Windows.MessageBoxButton.OK);
                    return default(T);
                }             
            }
        }
    }
}
