﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace mMEPO
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
                    for (int y = 0; y < xpathXmlNodeModExt.ChildNodes.Count; ++y)
                    {
                        XmlNode refNode = xpathXmlNodeModExt.ChildNodes[y];
                        //find a matching modExt, now replace it
                        if (refNode.Attributes["Class"].Value == valueClassAttributes.Value)
                        {
                            for(int z = 0; z < valNode.ChildNodes.Count; ++z)
                            {
                                xpathXmlNodeModExt.InsertBefore(xpathXmlNodeModExt.OwnerDocument.ImportNode(valNode.ChildNodes[z], deep: true), refNode);
                            }
                            xpathXmlNodeModExt.RemoveChild(refNode);
                            //Log.Message("Replaced modExt @ def: " + xpathXmlNode.Name);
                            anyMatchingModExt = true;
                            break;
                        }
                    }

                    //patchop adding modExt case instead of replacing
                    if (!anyMatchingModExt)
                    {
                        //Log.Message("Added modExt @ def: " + xpathXmlNode.Name);
                        xpathXmlNodeModExt.AppendChild(xpathXmlNodeModExt.OwnerDocument.ImportNode(valNode.FirstChild, deep: true));
                    }
                }
            }

            return result;
        }
    }
}