using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using System.Xml;

namespace ExpandedMaterials_Plastics
{
    class VMEu_PatchOperationAdjustPawnMoney : PatchOperationPathed
    {
		protected int amount;
		protected override bool ApplyWorker(XmlDocument xml)
		{
			bool result = false;
			XmlNode[] array = xml.SelectNodes(xpath).Cast<XmlNode>().ToArray();
			foreach (XmlNode xmlNode in array)
			{
				result = true;
				Log.Message("The selected node from " + xmlNode.ParentNode.FirstChild.InnerXml + " says: " + xmlNode.InnerText + ".");

				string minimumValue = xmlNode.InnerText.Split('~')[0];
				Log.Message("The minimum value obtained is " + minimumValue);
				int minimumValueNumber = Convert.ToInt32(minimumValue);
				XmlNode materialXmlNode = xmlNode.OwnerDocument.CreateElement(amount.ToString());
				int newMinimumValueNumber = minimumValueNumber + amount;

				if (xmlNode.InnerText.Contains('~'))
				{
					xmlNode.InnerText = xmlNode.InnerText.Replace(minimumValue, newMinimumValueNumber.ToString());
					Log.Message("Now the inner text says: " + xmlNode.InnerText);
				}
			}
			return result;
		}
	}
}
