using SimpleCCompiler.IR;
using SimpleCCompiler.IR.Instrunction;
using System.Collections.Generic;

namespace SimpleCCompiler.AST
{
    public class ReturnStmt : Stmt
    {
        public Expr ReturnValueExpr { get; set; }
        public FunctionDecl FunctionReturnedFrom { get; set; }
        public override SymbolTableItem LookupSymbolTable(string name)
        {
            return Parent.LookupSymbolTable(name);
        }
        public override IList<Instruction> GenerateIR(Function parentFunction)
        {
            List<Instruction> irList = new();
            irList.AddRange(ReturnValueExpr.GenerateIR(parentFunction));
            irList.Add(new ReturnInstrunction(ReturnValueExpr.ResultVariable));
            return irList;
        }
    }
}
