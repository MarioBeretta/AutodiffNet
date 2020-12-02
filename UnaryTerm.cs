using System;
using System.Collections.Generic;
using System.Text;

namespace AutoDiffNet
{
    abstract class UnaryTerm:Term
    {
        //public override TermTypes TermType { get { return TermTypes.UnaryTerm; } }
        protected Term body;
    }
}
