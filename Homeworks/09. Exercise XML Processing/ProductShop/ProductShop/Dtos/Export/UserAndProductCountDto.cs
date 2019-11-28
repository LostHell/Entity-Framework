using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("Users")]
    public class ExportUserAndProductDto
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("users")]
        public UserProductDto[] Users { get; set; }
    }
}
