using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace POME
{
    //make sure description says it will add if no modExtension of type given currently exists
    //(so don't mispell your modExt's Class name, or miss the namespace)}?"

    internal class PatchOperationAddOrReplaceModExtension : PatchOperationPathed
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
                //if def doesn't have a modExt list, add it and add the modExt
                if (xpathXmlNodeModExt == null)
                {
                    Log.Warning("Added modExt & modExts list (node didn't exist) @ def: " + xpathXmlNode.Name);
                    xpathXmlNodeModExt = xpathXmlNode.OwnerDocument.CreateElement("modExtensions");
                    xpathXmlNodeModExt.AppendChild(xpathXmlNodeModExt.OwnerDocument.ImportNode(valNode.FirstChild, deep: true));
                }
                else
                {
                    //Class Attribute has to match one of the modExts in each xpathnode
                    //search through def's entire modExt list until match
                    bool anyMatchingModExt = false;
                    foreach (XmlNode refNode in xpathXmlNodeModExt.ChildNodes)
                    {
                        //find a matching modExt, now replace it
                        if (refNode.Attributes["Class"] == valueClassAttributes)
                        {
                            xpathXmlNodeModExt.InsertBefore(xpathXmlNodeModExt.OwnerDocument.ImportNode(valNode, deep: true), refNode);
                            xpathXmlNodeModExt.RemoveChild(refNode);
                            Log.Message("Replaced modExt @ def: " + xpathXmlNode.Name);
                            anyMatchingModExt = true;
                            break;
                        }
                    }

                    //patchop adding modExt case instead of replacing
                    if (!anyMatchingModExt)
                    {
                        Log.Message("Added modExt @ def: " + xpathXmlNode.Name);
                        xpathXmlNodeModExt.AppendChild(xpathXmlNodeModExt.OwnerDocument.ImportNode(valNode.FirstChild, deep: true));
                    }
                }
            }

            return result;
        }
    }
}