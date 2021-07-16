using SimpleCCompiler.IR;
using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST
{
    public class ArgumentExprListExpr : Expr
    {
        public List<Expr> ArgumentExprList { get; set; } = new();
    }
    public class CallExpr : Expr
    {
        public Expr FunctionRefExpr { get; set; }
        public List<Expr> ArgumentExprList { get; set; } = new();
        public override IList<Instruction> GenerateIR(Function parentFunction)
        {
            List<Instruction> irList = new();
            if (FunctionRefExpr is not DeclRefExpr func ||
                            (func.Ref.Type != AST.Type.IntFunc &&
                            func.Ref.Type != AST.Type.VoidFunc))
            {
                throw new SemanticErrorException("Function DeclRefExpr expected");
            }
            List<Variable> args = new();
            foreach (var item in ArgumentExprList)
            {
                if (item.ResultVariable is null)
                {
                    irList.AddRange(item.GenerateIR(parentFunction));
                }
                args.Add(item.ResultVariable);
            }
            Variable resultTempVar = parentFunction.AllocateTempVariable(IR.Type.I32);
            ResultVariable = resultTempVar;
            CallInstrunction instruction = new(func.Ref.Name, args, resultTempVar);
            irList.Add(instruction);
            return irList;
        }
    }
}
