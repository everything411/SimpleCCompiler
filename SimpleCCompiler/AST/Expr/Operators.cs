using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST
{
    public enum BinaryOperator
    {
        Assignment,
        Less,
        Greater,
        LessEqual,
        GreaterEqual,
        Equal,
        NotEqual,
        Plus,
        Minus,
        Mul,
        Div,
        Mod,

    }
    public enum UnaryOperator
    {
        PostfixIncrement,
        PrefixIncrement,
        PostfixDecrement,
        PrefixDecrement,
        FunctionCall,
        ArraySubscript
    }
}
