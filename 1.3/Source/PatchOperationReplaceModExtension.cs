using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;



namespace mMEPO
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

            //get value's li element's Class attribute
            XmlAttribute valueClassAttributes = valNode.FirstChild.Attributes["Class"];
            if (valueClassAttributes == null)
            {
                Log.Error("Null Class Attribute for modExt");
                return false;
            }

            //Log.Message("modExt Class Attribute: " + valueClassAttributes.Value);

            for (int i = 0; i < xpathNodes.Count; ++i)
            {
                XmlNode xpathXmlNode = xpathNodes[i];
                XmlNode xpathXmlNodeModExt = xpathXmlNode["modExtensions"];

                //Class Attribute has to match one of the modExts in each xpathnode
                bool anyMatchingModExt = false;
                if (xpathXmlNodeModExt != null)
                {
                    //search through def's entire modExt list until match
                    for (int y = 0; y < xpathXmlNodeModExt.ChildNodes.Count; ++y)
                    {
                        XmlNode refNode = xpathXmlNodeModExt.ChildNodes[y];
                        //find a matching modExt, now replace it
                        if (refNode.Attributes["Class"].Value == valueClassAttributes.Value)
                        {
                            for (int z = 0; z < valNode.ChildNodes.Count; ++z)
                            {
                                xpathXmlNodeModExt.InsertBefore(xpathXmlNodeModExt.OwnerDocument.ImportNode(valNode.ChildNodes[z], deep: true), refNode);
                            }
                            xpathXmlNodeModExt.RemoveChild(refNode);
                            //Log.Message("Replaced modExt @ def: " + xpathXmlNode.Name);
                            anyMatchingModExt = true;
                            break;
                        }
                    }

                    if (!anyMatchingModExt)
                    {
                        Log.Error("modExtensions list for def does not contain any matching modExt");
                        return false;
                    }

                }
                else
                {
                    Log.Error("modExtensions xmlNode not found for at least one matching xpath node");
                    return false;
                }


            } result = true;
            return result;
        }
    }
}
