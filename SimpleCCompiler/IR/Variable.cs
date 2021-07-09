using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleCCompiler.AST.Expr;

namespace SimpleCCompiler.IR
{
    public class Variable
    {
        public Guid Guid { get; set; }
        public IR.Type Type { get; set; }
        public string Name { get; set; }
        // auto variable only now
        public bool IsAutoVariable { get; set; } = true;
        public int OffsetEBP { get; set; }
        public override string ToString()
        {
            if (OffsetEBP > 0)
            {
                return $" dword ptr [ebp + {OffsetEBP}] ";
            }
            else
            {
                return $" dword ptr [ebp {OffsetEBP}] ";
            }
            
        }
    }
    public class ArrayIndexVariable : Variable
    {
        public Variable Index { get; set; }
        public ArrayIndexVariable(ArrayVariable arrayVariable, Variable index)
        {
            Guid = Guid.NewGuid();
            Name = "A_" + Guid.ToString("N").Substring(0, 8);
            Type = Type switch
            {
                Type.I32Array => Type.I32,
                Type.I8Array => Type.I8,
                _ => throw new Exception("")
            };
            OffsetEBP = arrayVariable.OffsetEBP;
            Index = index;
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
    public class ArrayVariable : Variable
    {
        public int Size { get; set; }
        public override string ToString()
        {
            return base.ToString();
        }
    }

    public class IRStringLiteral
    {
        // for string literal
        public Guid Guid { get; set; } = Guid.NewGuid();
        public string LiteralName { get; set; }
        public string Value { get; set; }
        public IRStringLiteral(StringLiteral sl)
        {
            LiteralName = "LN_" + Guid.ToString("N").Substring(0, 8);
            Value = sl.Value;
        }
    }
    // public class IRStringLiteral
}
