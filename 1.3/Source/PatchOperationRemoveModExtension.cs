using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Verse;

namespace mMEPO
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
                    for (int y = 0; y < xpathXmlNodeModExt.ChildNodes.Count; ++y)
                    {
                        XmlNode refNode = xpathXmlNodeModExt.ChildNodes[y];
                        //find a matching modExt, now replace it
                        if (refNode.Attributes["Class"].Value == modExtClassName)
                        {
                            xpathXmlNodeModExt.RemoveChild(refNode);
                            //Log.Message("removed modExt @ def: " + xpathXmlNode.Name);
                            anyMatchingModExt = true;
                            break;
                        }
                    }

                    if (!anyMatchingModExt)
                    {
                        Log.Error("Didn't find any matching modExt @ def: " + xpathXmlNode.Name);
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