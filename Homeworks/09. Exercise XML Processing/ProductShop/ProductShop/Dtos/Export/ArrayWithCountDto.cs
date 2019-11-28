using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("products")]
    public class ArrayWithCountDto
    {
        [XmlElement("count")]
        public int Count { get; set; }

        [XmlArray("products")]
        public UserProductArrayDto[] SoldProduct { get; set; }
    }
}
