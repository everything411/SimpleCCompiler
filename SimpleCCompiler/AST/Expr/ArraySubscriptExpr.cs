using SimpleCCompiler.IR;
using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST
{
    public class ArraySubscriptExpr : Expr 
    {
        public Expr SubscriptExpr { get; set; }
        public Expr ArrayRefExpr { get; set; }
        public override IList<Instruction> GenerateIR(Function parentFunction)
        {
            List<Instruction> irList = new();
            if (SubscriptExpr.ResultVariable is null)
            {
                irList.AddRange(SubscriptExpr.GenerateIR(parentFunction));
            }
            if (ArrayRefExpr is DeclRefExpr arrayRef && arrayRef.Ref.Type is AST.Type.IntArray)
            {
                Variable resultTempVar = parentFunction.AllocateTempVariable(IR.Type.I32);
                var arrayVar = parentFunction.IRSymbolTable.VariableDictionary[arrayRef.Ref.Guid] as ArrayVariable;
                ArrayIndexVariable arrayIndexVariable = new(arrayVar, SubscriptExpr.ResultVariable);
                LoadArrayInstruction instruction = new(arrayIndexVariable, resultTempVar);
                ResultVariable = resultTempVar;
                irList.Add(instruction);
            }
            else
            {
                throw new SemanticErrorException("Array DeclRefExpr expected");
            }
            return irList;
        }
    }
}
