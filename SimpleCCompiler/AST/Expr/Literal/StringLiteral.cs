using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST.Expr
{
    class StringLiteral : Literal
    {
        public string Value { get; set; }
    }
}
