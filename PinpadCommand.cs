using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Server_Socket
{
    [XmlRoot]
    public class PinpadCommand
    {
        [XmlElement]
        public string Command { get; set; }
        [XmlElement]
        public bool IsCustomized { get; set; }
    }
}
