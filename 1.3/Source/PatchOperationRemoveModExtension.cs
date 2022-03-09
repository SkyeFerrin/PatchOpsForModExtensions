using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace POME
{
    internal class PatchOperationRemoveModExtension : PatchOperationPathed
    {
        // modExt Class Name w/ namespace
        public string modExtClassName;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            //implement
            XmlNodeList xpathNodes = xml.SelectNodes(xpath);
            bool result = xpathNodes != null ? true: false;

            for (int i = 0; i < xpathNodes.Count; ++i)
            {
                XmlNode xpathXmlNode = xpathNodes[i];
                XmlNode xpathXmlNodeModExt = xpathXmlNode["modExtensions"];

                //Class Attribute has to match one of the modExts in each xpathnode
                if (xpathXmlNodeModExt != null)
                {
                    //search through def's entire modExt list until match
                    bool anyMatchingModExt = false;
                    foreach (XmlNode refNode in xpathXmlNodeModExt.ChildNodes)
                    {
                        //find a matching modExt, now replace it
                        Log.Message("looking for modExt Class: " + refNode.Attributes["Class"].ToString());
                        if (refNode.Attributes["Class"].ToString() == modExtClassName)
                        {
                            xpathXmlNodeModExt.RemoveChild(refNode);
                            Log.Message("removed modExt @ def: " + xpathXmlNode.Name);
                            break;
                        }
                    }

                    if (!anyMatchingModExt)
                    {
                        Log.Message("Didn't find any matching modExt @ def: " + xpathXmlNode.Name);
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