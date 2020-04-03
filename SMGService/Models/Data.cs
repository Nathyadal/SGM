using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SMGService.Models
{
	[XmlRoot(ElementName = "data")]
	public class Data
	{
		[XmlAttribute(AttributeName = "name")]
		public string Name { get; set; }
		[XmlText]
		public string Text { get; set; }
	}

	[XmlRoot(ElementName = "tss_loan_request")]
	public class Tss_loan_request
	{
		[XmlElement(ElementName = "data")]
		public List<Data> Data { get; set; }
	}
}
