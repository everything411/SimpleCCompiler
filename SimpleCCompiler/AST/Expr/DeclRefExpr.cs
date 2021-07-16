using SimpleCCompiler.IR;
using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST
{
    public class DeclRefExpr : Expr
    {
        public SymbolTableItem Ref { get; set; }
        public override string ToString() => $"{base.ToString()}:{Ref}";
        public override IList<Instruction> GenerateIR(Function parentFunction)
        {
            List<Instruction> irList = new();
            var variable = parentFunction.IRSymbolTable.VariableDictionary[Ref.Guid];
            ResultVariable = variable;
            return irList;
        }
    }
}
