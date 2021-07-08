using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST.Expr
{
    public class ArraySubscriptExpr : Expr 
    {
        public IExpr SubscriptExpr { get; set; }
        public IExpr ArrayRefExpr { get; set; }
    }
}
