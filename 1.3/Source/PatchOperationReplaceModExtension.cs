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
    // Will error if modExtensions tag doesn't exist (no modExts to replace), or if there is no modExt list item w/ Class name matching that given in value node.
    // Will replace modExt if it finds any modExt w/ matching class name attribute @ xpath node
    // This whole patch operation is applied for each matching node the xpath points to
    internal class PatchOperationReplaceModExtension : PatchOperationPathed
    {
        //xpath variable comes from PatchOperationPathed parent
        //value contains the modExtension list item
        public XmlContainer value;

        //replacing multiple Mod Extensions per matching xpath node needs a new PatchOpReplaceModExt for each
        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlNode valNode = value.node;

            XmlNodeList matchingNodes = xml.SelectNodes(xpath);
            bool result = matchingNodes != null ? true : false;

            //get value's list item element's Class attribute
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

                //Class Attribute has to match one of the modExts in each xpathnode
                bool anyMatchingModExt = false;
                if (modExtensionsNode != null)
                {
                    //search through def's entire modExt list until match
                    for (int y = 0; y < modExtensionsNode.ChildNodes.Count; ++y)
                    {
                        XmlNode refNode = modExtensionsNode.ChildNodes[y];
                        //find a matching modExt, now replace it
                        if (refNode.Attributes["Class"].Value == valueClassAttributes.Value)
                        {
                            for (int z = 0; z < valNode.ChildNodes.Count; ++z)
                            {
                                modExtensionsNode.InsertBefore(modExtensionsNode.OwnerDocument.ImportNode(valNode.ChildNodes[z], deep: true), refNode);
                            }
                            modExtensionsNode.RemoveChild(refNode);
                            //Log.Message("Replaced modExt @ def: " + matchingNode.Name);
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
