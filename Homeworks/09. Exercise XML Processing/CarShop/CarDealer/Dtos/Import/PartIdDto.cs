using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer.Dtos.Import
{
    [XmlRoot("parts")]
   public class PartIdDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
    }
}
