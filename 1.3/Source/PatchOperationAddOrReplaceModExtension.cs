using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace mMEPO
{
    // PatchOp intent:
    // Will add modExt if modExtensions tag doesn't exist, or if modExt w/ Class name matching that of the class name in the value node
    // Will replace modExt if it finds any modExt w/ matching class name attribute @ xpath node
    // This whole patch operation is applied for each matching node the xpath points to
    internal class PatchOperationAddOrReplaceModExtension : PatchOperationPathed
    {
        public XmlContainer value;

        //replacing/adding multiple Mod Extensions per matching xpath node needs a new PatchOpAddOrReplaceModExt for each
        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlNode valNode = value.node;

            XmlNodeList matchingNodes = xml.SelectNodes(xpath);
            bool result = matchingNodes != null ? true : false;

            //get value's li element's Class attribute
            XmlAttribute valueClassAttributes = valNode.FirstChild.Attributes["Class"];
            if (valueClassAttributes == null)
            {
                Log.Error("Null Class Attribute for modExt");
                return false;
            }

            //Log.Message("modExt Class Attribute: " + valueClassAttributes.Value);

            for (int i = 0; i < matchingNodes.Count; ++i)
            {
                XmlNode matchingNode = matchingNodes[i];
                XmlNode modExtensionsNode = matchingNode["modExtensions"];
                //if def doesn't have a modExt list, add it and add the modExt
                if (modExtensionsNode == null)
                {
                    Log.Warning("Added modExt & modExts list (node didn't exist) @ def: " + matchingNode.Name);
                    modExtensionsNode = matchingNode.OwnerDocument.CreateElement("modExtensions");
                    modExtensionsNode.AppendChild(modExtensionsNode.OwnerDocument.ImportNode(valNode.FirstChild, deep: true));
                }
                else
                {
                    //Class Attribute has to match one of the modExts in each xpathnode
                    //search through def's entire modExt list until match
                    bool anyMatchingModExt = false;
                    for (int y = 0; y < modExtensionsNode.ChildNodes.Count; ++y)
                    {
                        XmlNode refNode = modExtensionsNode.ChildNodes[y];
                        //find a matching modExt, now replace it
                        if (refNode.Attributes["Class"].Value == valueClassAttributes.Value)
                        {
                            for(int z = 0; z < valNode.ChildNodes.Count; ++z)
                            {
                                modExtensionsNode.InsertBefore(modExtensionsNode.OwnerDocument.ImportNode(valNode.ChildNodes[z], deep: true), refNode);
                            }
                            modExtensionsNode.RemoveChild(refNode);
                            //Log.Message("Replaced modExt @ def: " + matchingNode.Name);
                            anyMatchingModExt = true;
                            break;
                        }
                    }

                    //patchop adding modExt case instead of replacing
                    if (!anyMatchingModExt)
                    {
                        //Log.Message("Added modExt @ def: " + matchingNode.Name);
                        modExtensionsNode.AppendChild(modExtensionsNode.OwnerDocument.ImportNode(valNode.FirstChild, deep: true));
                    }
                }
            }

            return result;
        }
    }
}