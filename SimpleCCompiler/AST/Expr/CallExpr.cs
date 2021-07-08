using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST.Expr
{
    public class CallExpr : Expr
    {
        public IExpr FunctionRefExpr { get; set; }
        public List<IExpr> ArgumentExprList { get; set; }
        public CallExpr()
        {
            ArgumentExprList = new List<IExpr>();
        }
    }
}
