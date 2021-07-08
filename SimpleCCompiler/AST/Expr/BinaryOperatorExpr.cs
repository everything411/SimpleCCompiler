using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST.Expr
{
    public class BinaryOperatorExpr : Expr
    {
        public BinaryOperator Operator { get; set; }
        public IExpr LeftExpression { get; set; }
        public IExpr RightExpression { get; set; }
        public override string ToString() => $"{base.ToString()}:({LeftExpression}){Operator}({RightExpression})";
    }
}
