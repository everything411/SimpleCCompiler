using SimpleCCompiler.IR;
using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCCompiler.AST
{
    public class BinaryOperatorExpr : Expr
    {
        public BinaryOperator Operator { get; set; }
        public Expr LeftExpression { get; set; }
        public Expr RightExpression { get; set; }
        public override string ToString() => $"{base.ToString()}:({LeftExpression}){Operator}({RightExpression})";
        public override IList<Instruction> GenerateIR(Function parentFunction)
        {
            List<Instruction> irList = new();
            switch (Operator)
            {
                case BinaryOperator.Assignment:
                    {
                        var right = RightExpression as Expr;
                        if (right.ResultVariable is null)
                        {
                            var list = right.GenerateIR(parentFunction);
                            irList.AddRange(list);
                        }
                        var left = LeftExpression;
                        switch (left)
                        {
                            case DeclRefExpr decl:
                                {
                                    // left
                                    Variable resultTempVar = parentFunction.AllocateTempVariable(IR.Type.I32);
                                    var lvalue = parentFunction.IRSymbolTable.VariableDictionary[decl.Ref.Guid];
                                    AssignmentInstruction assignment = new(lvalue, right.ResultVariable, resultTempVar);
                                    irList.Add(assignment);
                                    break;
                                }
                            case ArraySubscriptExpr arraySubscript:
                                {
                                    if (arraySubscript.SubscriptExpr.ResultVariable is null)
                                    {
                                        var list = arraySubscript.SubscriptExpr.GenerateIR(parentFunction);
                                        irList.AddRange(list);
                                    }
                                    if (arraySubscript.ArrayRefExpr is DeclRefExpr arrayRef && arrayRef.Ref.Type is AST.Type.IntArray)
                                    {
                                        Variable resultTempVar = parentFunction.AllocateTempVariable(IR.Type.I32);
                                        var lvalue = parentFunction.IRSymbolTable.VariableDictionary[arrayRef.Ref.Guid] as ArrayVariable;
                                        ArrayIndexVariable arrayIndexVariable = new(lvalue, arraySubscript.SubscriptExpr.ResultVariable);

                                        ArrayAssignmentInstruction assignment = new(arrayIndexVariable, right.ResultVariable, resultTempVar);
                                        irList.Add(assignment);
                                    }
                                    else
                                    {
                                        throw new SemanticErrorException($"Unexpected {arraySubscript.ArrayRefExpr}, Array DeclRefExpr expected");
                                    }
                                }
                                break;
                            default:
                                throw new SemanticErrorException("Assignment to rvalue");
                        }
                        break;
                    }
                case BinaryOperator.Less:
                case BinaryOperator.Greater:
                case BinaryOperator.LessEqual:
                case BinaryOperator.GreaterEqual:
                case BinaryOperator.Equal:
                case BinaryOperator.NotEqual:
                    {
                        var left = LeftExpression;
                        if (left.ResultVariable is null)
                        {
                            irList.AddRange(left.GenerateIR(parentFunction));
                        }
                        var right = RightExpression;
                        if (right.ResultVariable is null)
                        {
                            irList.AddRange(right.GenerateIR(parentFunction));
                        }
                        Variable resultTempVar = null;
                        CmpInstruction instruction = new(left.ResultVariable, right.ResultVariable,
                            resultTempVar, Operator);
                        ResultVariable = resultTempVar;
                        irList.Add(instruction);
                        break;
                    }
                case BinaryOperator.Plus:
                case BinaryOperator.Minus:
                case BinaryOperator.Mul:
                case BinaryOperator.Div:
                case BinaryOperator.Mod:
                    {
                        var left = LeftExpression;
                        if (left.ResultVariable is null)
                        {
                            irList.AddRange(left.GenerateIR(parentFunction));
                        }
                        var right = RightExpression;
                        if (right.ResultVariable is null)
                        {
                            irList.AddRange(right.GenerateIR(parentFunction));
                        }
                        Variable resultTempVar = parentFunction.AllocateTempVariable(IR.Type.I32);
                        ResultVariable = resultTempVar;
                        Instruction instruction = Operator switch
                        {
                            BinaryOperator.Plus => new AddInstruction(left.ResultVariable, right.ResultVariable, resultTempVar),
                            BinaryOperator.Minus => new SubInstruction(left.ResultVariable, right.ResultVariable, resultTempVar),
                            BinaryOperator.Mul => new MulInstruction(left.ResultVariable, right.ResultVariable, resultTempVar),
                            BinaryOperator.Div => new DivInstruction(left.ResultVariable, right.ResultVariable, resultTempVar),
                            BinaryOperator.Mod => new ModInstruction(left.ResultVariable, right.ResultVariable, resultTempVar),
                            _ => throw new InternalErrorException("Bad Operator"),
                        };
                        irList.Add(instruction);
                        break;
                    }

                default:
                    throw new NotImplementedException($"{Operator}");
            }
            return irList;
        }
    }
}
