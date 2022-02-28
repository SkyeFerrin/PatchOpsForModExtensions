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
        private XmlContainer value;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            //implement

        }
    }
}
