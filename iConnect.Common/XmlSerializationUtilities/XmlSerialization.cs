using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml.XPath;

namespace iConnect.Common
{
    public static class XmlSerialization
    {
        public static void Serialize(IXPathNavigable toXmlDocument, object data)
        {
            if (data == null)
            {
                return;
            }
            if (toXmlDocument == null)
            {
                toXmlDocument = new XmlDocument();
            }
            using(var xmlStream = new MemoryStream())
            {
                Serialize(xmlStream, data);

                xmlStream.Position = 0;
                (toXmlDocument as XmlDocument).Load(XmlReader.Create(xmlStream));
            }
           
            
        }
        public static void Serialize(Stream toStream, object data)
        {
            if (data == null)
            {
                return;
            }
            XmlWriter xmlWriter = XmlWriter.Create(toStream);
            Serialize(xmlWriter, data);
            xmlWriter.Flush();
        }
        public static void Serialize(XmlWriter xmlWriter, object data)
        {
            if (xmlWriter == null || data == null)
            {
                return;
            }
            var xmlSerializable = data as IXmlSerializable;
            if (xmlSerializable == null)
            {
                try
                {
                    if (data == null)
                    {
                        return;
                    }
                    var serializer = new XmlSerializer(data.GetType());
                    serializer.Serialize(xmlWriter, data);
                    xmlWriter.Flush();
                }
                catch (Exception e)
                {
                    throw new SerializationException("Failed to serialize object.", e);
                }
            }
            else
            {
                try
                {
                    xmlSerializable.Serialize(xmlWriter);
                }
                catch (Exception e)
                {
                    throw new SerializationException("Failed to serialize object.", e);
                }
            }
        }
        public static string ToXmlString(Object data)
        {
            if(data == null)
            {
                throw new InvalidOperationException("The data cannot be null");
            }
            var xmlDoc = new XmlDocument();
            var xmlSerializer = new XmlSerializer(data.GetType());
            using (var xmlStream = new MemoryStream())
            {
                xmlSerializer.Serialize(xmlStream, data);
                xmlStream.Position = 0;
                xmlDoc.Load(xmlStream);
                return xmlDoc.InnerXml;
            }
        }
        public static string ToFormattedString(Stream stream)
        {
            if(stream == null)
            {
                throw new InvalidOperationException("The stream cannot be null");
            }
            Stream receiveStream = stream;
            var xmlSoapRequest = new XmlDocument();
            // Move to begining of input stream and read
            receiveStream.Position = 0;
            using (var readStream = new StreamReader(receiveStream, System.Text.Encoding.UTF8))
            {
                // Load into XML document
                xmlSoapRequest.Load(readStream);
            }
            return xmlSoapRequest.InnerXml;

        }
    }
}