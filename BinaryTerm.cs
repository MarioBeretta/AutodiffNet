using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace AutoDiffNet
{
    abstract class BinaryTerm : Term
    {
        //public override TermTypes TermType {  get { return TermTypes.BinaryTerm; } }
        protected Term Left;
        protected Term Right;
    }
}
