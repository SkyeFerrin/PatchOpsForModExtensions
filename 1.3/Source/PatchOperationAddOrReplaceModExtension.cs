using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace POME
{
    //add if no modExtension of type given currently exists
    //log typeof @Class of given value to see if you're grabbing it correctly 
    internal class PatchOperationAddOrReplaceModExtension : PatchOperationPathed
    {
        private XmlContainer value;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            //implement

        }
    }
}