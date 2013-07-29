using System.Xml;

namespace iConnect.Common
{
    public interface IXmlSerializable
    {
        void Serialize(XmlWriter xmlWriter);
        void Deserialize(XmlReader xmlReader);
    }
}