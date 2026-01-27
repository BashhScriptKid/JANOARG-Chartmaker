using System.Xml.Serialization;
using JANOARG.Shared.Data.ChartInfo;

namespace JANOARG.Chartmaker.Data
{

    [XmlRoot("ItemList")]
    public class ClientSerializeProxyList : SerializeProxyList
    {
    }

    public class Storage : Storage<ClientSerializeProxyList>
    {
        public Storage(string path) : base(path) { }
    }
}