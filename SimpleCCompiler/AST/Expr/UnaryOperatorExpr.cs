using SimpleCCompiler.IR;
using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST
{
    public class UnaryOperatorExpr : Expr
    {
        public UnaryOperator Operator { get; set; }
        public Expr Expression { get; set; }
        public override IList<Instruction> GenerateIR(Function parentFunction)
        {
            List<Instruction> irList = new();
            switch (Operator)
            {
                case UnaryOperator.PostfixDecrement:
                    {
                        if (Expression is not DeclRefExpr decl)
                        {
                            throw new SemanticErrorException("Decrement to rvalue");
                        }
                        Variable resultTempVar = parentFunction.AllocateTempVariable(IR.Type.I32);
                        var arg1 = parentFunction.IRSymbolTable.VariableDictionary[decl.Ref.Guid];
                        DecInstruction dec = new(arg1, resultTempVar);
                        ResultVariable = resultTempVar;
                        irList.Add(dec);
                        break;
                    }
                case UnaryOperator.PostfixIncrement:
                    {
                        if (Expression is not DeclRefExpr decl)
                        {
                            throw new SemanticErrorException("Increment to rvalue");
                        }
                        Variable resultTempVar = parentFunction.AllocateTempVariable(IR.Type.I32);
                        var arg1 = parentFunction.IRSymbolTable.VariableDictionary[decl.Ref.Guid];
                        IncInstruction inc = new(arg1, resultTempVar);
                        ResultVariable = resultTempVar;
                        irList.Add(inc);
                        break;
                    }
                default:
                    throw new NotImplementedException($"{Operator}");
            }
            return irList;
        }
    }
}
