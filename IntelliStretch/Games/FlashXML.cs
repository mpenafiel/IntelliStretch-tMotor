using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace IntelliStretch.Games
{
    class FlashXML
    {
        public static string GetXMLCmd(string methodName, string[] paramArray)
        {
            string cmd = "<invoke name=\"" + methodName + "\" returntype=\"xml\"><arguments>";

            foreach (string param in paramArray)
            {
                cmd += "<string>";
                cmd += param;
                cmd += "</string>";
            }

            cmd += "</arguments></invoke>";
            return cmd;
        }

        public static string GetMethod(string xmlCmd)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlCmd);
            XmlAttributeCollection attributes = document.FirstChild.Attributes;
            string method = attributes.Item(0).InnerText;
            return method;
        }

        public static string[] GetParams(string xmlCmd)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xmlCmd);
            XmlNodeList paramList = document.GetElementsByTagName("string");
            int paramNum = paramList.Count;
            string[] paramArray = new string[paramNum];
            for (int i = 0; i < paramNum; i++)
            {
                paramArray[i] = paramList[i].InnerText;
            }
            return paramArray;
        }
    }
}
