using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;



namespace POME
{
    internal class PatchOperationReplaceModExtension : PatchOperationPathed
    {
        public XmlContainer value;

        //replacing multiple Mod Extensions needs a new PatchOpReplaceModExt for each,
        //make sure thats explicitly mentioned in the description
        protected override bool ApplyWorker(XmlDocument xml)
        {
            //implement
            XmlNode valNode = value.node;

            XmlNodeList xpathNodes = xml.SelectNodes(xpath);
            bool result = xpathNodes != null ? true : false;

            for (int i = 0; i < xpathNodes.Count; ++i)
            {
                XmlNode xpathXmlNode = xpathNodes[i];
                //get value's li element's Class attribute
                XmlAttribute valueClassAttributes = valNode.FirstChild.Attributes["Class"];
                if (valueClassAttributes != null)
                    Log.Message("modExt Class Attribute: " + valueClassAttributes.Value);
                else
                {
                    Log.Error("Null Class Attribute for modExt");
                    return false;
                }

                XmlNode xpathXmlNodeModExt = xpathXmlNode["modExtensions"];

                //Class Attribute has to match one of the modExts in each xpathnode
                if (xpathXmlNodeModExt != null)
                {
                    //search through def's entire modExt list until match
                    foreach (XmlNode refNode in xpathXmlNodeModExt.ChildNodes)
                    {
                        //find a matching modExt, now replace it
                        if (refNode.Attributes["Class"] == valueClassAttributes)
                        {
                            result = true;
                            foreach (XmlNode valChildNode in valNode.ChildNodes)
                            {
                                xpathXmlNodeModExt.InsertBefore(xpathXmlNodeModExt.OwnerDocument.ImportNode(valChildNode, deep: true), refNode);
                            }
                            xpathXmlNodeModExt.RemoveChild(refNode);
                            Log.Message("Replaced modExt @ def: " + xpathXmlNode.Name);
                            break;
                        }

                    }
                }
                else
                {
                    Log.Error("modExtensions xmlNode not found for at least one matching xpath node");
                    return false;
                }


            }
            return result;
        }
    }
}
