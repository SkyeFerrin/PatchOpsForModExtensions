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
    // Gives a warning if modExtensions tag doesn't exist (no modExts to remove)
    // Will error if there is no modExt list item w/ Class name matching that given in value node.
    // Will remove modExt if it finds any modExt w/ matching class name attribute @ xpath node
    // This whole patch operation is applied for each matching node the xpath points to
    internal class PatchOperationRemoveModExtension : PatchOperationPathed
    {
        // modExt Class Name w/ namespace, has to match a modExt in def's list for each matching node
        public string modExtClassName;

        //removing multiple Mod Extensions per matching xpath node needs a new PatchOpRemoveModExt for each
        protected override bool ApplyWorker(XmlDocument xml)
        {
            XmlNodeList matchingNodes = xml.SelectNodes(xpath);
            bool result = matchingNodes != null ? true: false;

            for (int i = 0; i < matchingNodes.Count; ++i)
            {
                XmlNode matchingNode = matchingNodes[i];
                XmlNode modExtensionsNode = matchingNode["modExtensions"];

                //Class Attribute has to match one of the modExts in each xpathnode
                if (modExtensionsNode != null)
                {
                    //search through def's entire modExt list until match
                    bool anyMatchingModExt = false;
                    for (int y = 0; y < modExtensionsNode.ChildNodes.Count; ++y)
                    {
                        XmlNode refNode = modExtensionsNode.ChildNodes[y];
                        //find a matching modExt, now replace it
                        if (refNode.Attributes["Class"].Value == modExtClassName)
                        {
                            modExtensionsNode.RemoveChild(refNode);
                            //Log.Message("removed modExt @ def: " + matchingNode.Name);
                            anyMatchingModExt = true;
                            break;
                        }
                    }

                    if (!anyMatchingModExt)
                    {
                        Log.Error("Didn't find any matching modExt @ def: " + matchingNode.Name);
                        return false;
                    }
                }
                else
                    Log.Warning("modExtensions xmlNode not found for at least one matching xpath node w/ Remove ModExt");


            } result = true;
            return result;
        }
    }
}