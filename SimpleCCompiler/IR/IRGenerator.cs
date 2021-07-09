using SimpleCCompiler.AST.Decl;
using SimpleCCompiler.AST.Stmt;
using SimpleCCompiler.AST.Expr;
using SimpleCCompiler.AST;
using SimpleCCompiler.IR.Instrunction;
using System.Collections.Generic;
using System;
using System.Text;

namespace SimpleCCompiler.IR
{
    public static class IRGenerator
    {
        public static Module GenerateIRForTranslationUnit(TranslationUnitDecl translationUnitDecl)
        {
            
            Module module = new();
            // global variable
            // TODO
            foreach (var variable in translationUnitDecl.VarDeclList)
            {
                ;
            }
            foreach (var functionDecl in translationUnitDecl.FunctionDeclList)
            {
                Function function = new();
                function.Name = functionDecl.Name;
                function.Type = functionDecl.Type switch
                {
                    AST.Type.IntFunc => IR.Type.I32,
                    AST.Type.CharFunc => IR.Type.I32,
                    AST.Type.VoidFunc => IR.Type.Void,
                    _ => throw new NotImplementedException()
                };
                // arguments
                foreach (var item in functionDecl.SymbolTable.Symbols.Values)
                {
                    var argItem = item as ParameterSymbolTableItem;
                    Variable v = new();
                    v.Guid = argItem.Guid;
                    v.Name = argItem.Name;
                    v.Type = Type.I32;
                    v.OffsetEBP = (argItem.ParameterNumber + 1) * 4;
                    function.IRSymbolTable.VariableDictionary.Add(v.Guid, v);
                }
                // auto variables
                var symbols = CollectSymbolsFromFunction(functionDecl);
                foreach (var symbol in symbols)
                {
                    switch (symbol.Type)
                    {
                        case AST.Type.Int:
                        case AST.Type.Char:
                            Variable vv = new();
                            vv.Guid = symbol.Guid;
                            vv.Name = symbol.Name;
                            vv.Type = symbol.Type switch
                            {
                                AST.Type.Int => Type.I32,
                                AST.Type.Char => Type.I8,
                                _ => throw new InternalErrorException($"Unknown type {symbol.Type}")
                            };
                            function.AddAutoVariable(vv);
                            break;
                        case AST.Type.IntArray:
                        case AST.Type.CharArray:
                            ArrayVariable av = new();
                            av.Name = symbol.Name;
                            av.Guid = symbol.Guid;
                            av.Size = (symbol as ArraySymbolTableItem).ArraySize;
                            av.Type = symbol.Type switch
                            {
                                AST.Type.IntArray => Type.I32Array,
                                AST.Type.CharArray => Type.I8Array,
                                _ => throw new InternalErrorException($"Unknown type {symbol.Type}")
                            };
                            function.AddAutoArrayVariable(av);
                            break;
                        default:
                            throw new InternalErrorException($"Unknown type {symbol.Type}");
                    }
                }
                LabelInstruction functionLabel = new();
                functionLabel.Name = function.Name;
                function.IRs.Add(functionLabel);
                var alloc = new AllocateFrameInstrunction();
                function.IRs.Add(alloc);
                function.IRs.AddRange(GenerateIRForStmt(functionDecl.Body, function));
                alloc.FrameSize = function.AutoVariableCount * 4 + 16;
                module.Functions.Add(function);
            }
            foreach (Function item in module.Functions)
            {
                module.Instructions.AddRange(item.IRs);
            }
            return module;
        }

        internal static List<IInstruction> GenerateIRForStmt(IStmt stmt, Function parentFunction)
        {
            List<IInstruction> irList = new();
            switch (stmt)
            {
                case ForStmt forStmt:
                    {
                        irList.AddRange(GenerateIRForExpr(forStmt.InitExpr, parentFunction));
                        irList.Add(forStmt.StartLabel);
                        var cmpIRList = GenerateIRForExpr(forStmt.ConditionalExpr, parentFunction);
                        irList.AddRange(cmpIRList);
                        var cmpIR = irList[^1] as CmpInstruction;
                        Instruction jumpInstruction = cmpIR.BinaryOperator switch
                        {
                            BinaryOperator.Less => new JgeInstruction(forStmt.EndLabel),
                            BinaryOperator.Greater => new JlInstruction(forStmt.EndLabel),
                            BinaryOperator.LessEqual => new JgInstruction(forStmt.EndLabel),
                            BinaryOperator.GreaterEqual => new JlInstruction(forStmt.EndLabel),
                            BinaryOperator.Equal => new JneInstruction(forStmt.EndLabel),
                            BinaryOperator.NotEqual => new JeInstruction(forStmt.EndLabel),
                            _ => throw new NotImplementedException($"{cmpIR.BinaryOperator} not implemented"),
                        };
                        irList.Add(jumpInstruction);
                        irList.AddRange(GenerateIRForStmt(forStmt.LoopBodyStmt, parentFunction));
                        irList.Add(forStmt.EndExprLabel);
                        irList.AddRange(GenerateIRForExpr(forStmt.EndExpr, parentFunction));
                        irList.Add(new JmpInstruction(forStmt.StartLabel));
                        irList.Add(forStmt.EndLabel);
                        break;
                    }
                case IfStmt ifStmt:
                    {
                        var cmpIRList = GenerateIRForExpr(ifStmt.ConditionalExpr, parentFunction);
                        irList.AddRange(cmpIRList);
                        var cmpIR = irList[^1] as CmpInstruction;
                        Instruction jumpInstruction = cmpIR.BinaryOperator switch
                        {
                            BinaryOperator.Less => new JgeInstruction(ifStmt.IfFalseLabel),
                            BinaryOperator.Greater => new JlInstruction(ifStmt.IfFalseLabel),
                            BinaryOperator.LessEqual => new JgInstruction(ifStmt.IfFalseLabel),
                            BinaryOperator.GreaterEqual => new JlInstruction(ifStmt.IfFalseLabel),
                            BinaryOperator.Equal => new JneInstruction(ifStmt.IfFalseLabel),
                            BinaryOperator.NotEqual => new JeInstruction(ifStmt.IfFalseLabel),
                            _ => throw new NotImplementedException($"{cmpIR.BinaryOperator} not implemented"),
                        };
                        irList.Add(jumpInstruction);
                        irList.AddRange(GenerateIRForStmt(ifStmt.IfTrueBody, parentFunction));
                        irList.Add(new JmpInstruction(ifStmt.IfEndLabel));
                        irList.Add(ifStmt.IfFalseLabel);
                        irList.AddRange(GenerateIRForStmt(ifStmt.IfFalseBody, parentFunction));
                        irList.Add(ifStmt.IfEndLabel);
                        break;
                    }
                case EmptyStmt e:
                    {
                        break;
                    }
                case ExpressionStmt e:
                    {
                        irList.AddRange(GenerateIRForExpr(e.Expr, parentFunction));
                        break;
                    }
                case ReturnStmt r:
                    {
                        irList.AddRange(GenerateIRForExpr(r.ReturnValueExpr, parentFunction));
                        irList.Add(new ReturnInstrunction(r.ReturnValueExpr.ResultVariable));
                        break;
                    }
                case BreakStmt b:
                    {
                        irList.Add(new JmpInstruction(b.ParentIterationStmt.EndLabel));
                        break;
                    }
                case CompoundStmt compoundStmt:
                    {
                        foreach (var item in compoundStmt.Stmts)
                        {
                            irList.AddRange(GenerateIRForStmt(item, parentFunction));
                        }
                        break;
                    }
                default:
                    throw new NotImplementedException($"Stmt {stmt} not implemented");
            }
            return irList;
        }
        internal static List<IInstruction> GenerateIRForExpr(IExpr expr, Function parentFunction)
        {
            List<IInstruction> irList = new();
            if (expr.ResultVariable is not null)
            {
                return irList;
            }
            switch (expr)
            {
                case UnaryOperatorExpr unaryOperatorExpr:
                    {
                        switch (unaryOperatorExpr.Operator)
                        {
                            case UnaryOperator.PostfixDecrement:
                                {
                                    if (unaryOperatorExpr.Expression is not DeclRefExpr decl)
                                    {
                                        throw new SemanticErrorException("Decrement to rvalue");
                                    }
                                    Variable resultTempVar = parentFunction.AllocateTempVariable(Type.I32);
                                    var arg1 = parentFunction.IRSymbolTable.VariableDictionary[decl.Ref.Guid];
                                    DecInstruction dec = new(arg1, resultTempVar);
                                    unaryOperatorExpr.ResultVariable = resultTempVar;
                                    irList.Add(dec);
                                    break;
                                }
                            case UnaryOperator.PostfixIncrement:
                                {
                                    if (unaryOperatorExpr.Expression is not DeclRefExpr decl)
                                    {
                                        throw new SemanticErrorException("Increment to rvalue");
                                    }
                                    Variable resultTempVar = parentFunction.AllocateTempVariable(Type.I32);
                                    var arg1 = parentFunction.IRSymbolTable.VariableDictionary[decl.Ref.Guid];
                                    IncInstruction inc = new(arg1, resultTempVar);
                                    unaryOperatorExpr.ResultVariable = resultTempVar;
                                    irList.Add(inc);
                                    break;
                                }
                            default:
                                throw new NotImplementedException($"{unaryOperatorExpr.Operator}");
                        }
                        break;
                    }
                case BinaryOperatorExpr binaryOperatorExpr:
                    {
                        switch (binaryOperatorExpr.Operator)
                        {
                            case BinaryOperator.Assignment:
                                {
                                    var right = binaryOperatorExpr.RightExpression as Expr;
                                    if (right.ResultVariable is null)
                                    {
                                        var list = GenerateIRForExpr(right, parentFunction);
                                        irList.AddRange(list);
                                    }
                                    var left = binaryOperatorExpr.LeftExpression;
                                    switch (left)
                                    {
                                        case DeclRefExpr decl:
                                            {
                                                // left
                                                Variable resultTempVar = parentFunction.AllocateTempVariable(Type.I32);
                                                var lvalue = parentFunction.IRSymbolTable.VariableDictionary[decl.Ref.Guid];
                                                AssignmentInstruction assignment = new(lvalue, right.ResultVariable, resultTempVar);
                                                irList.Add(assignment);
                                                break;
                                            }
                                        case ArraySubscriptExpr arraySubscript:
                                            {
                                                if (arraySubscript.SubscriptExpr.ResultVariable is null)
                                                {
                                                    var list = GenerateIRForExpr(arraySubscript.SubscriptExpr as Expr, parentFunction);
                                                    irList.AddRange(list);
                                                }
                                                if (arraySubscript.ArrayRefExpr is DeclRefExpr arrayRef && arrayRef.Ref.Type is AST.Type.IntArray)
                                                {
                                                    Variable resultTempVar = parentFunction.AllocateTempVariable(Type.I32);
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
                                    var left = binaryOperatorExpr.LeftExpression as Expr;
                                    if (left.ResultVariable is null)
                                    {
                                        var list = GenerateIRForExpr(left, parentFunction);
                                        irList.AddRange(list);
                                    }
                                    var right = binaryOperatorExpr.RightExpression as Expr;
                                    if (right.ResultVariable is null)
                                    {
                                        var list = GenerateIRForExpr(right, parentFunction);
                                        irList.AddRange(list);
                                    }
                                    // Variable resultTempVar = parentFunction.AllocateTempVariable(IR.Type.I32);
                                    Variable resultTempVar = null;
                                    CmpInstruction instruction = new(left.ResultVariable, right.ResultVariable,
                                        resultTempVar, binaryOperatorExpr.Operator);
                                    binaryOperatorExpr.ResultVariable = resultTempVar;
                                    irList.Add(instruction);
                                    break;
                                }
                            case BinaryOperator.Plus:
                            case BinaryOperator.Minus:
                            case BinaryOperator.Mul:
                            case BinaryOperator.Div:
                            case BinaryOperator.Mod:
                                {
                                    var left = binaryOperatorExpr.LeftExpression as Expr;
                                    if (left.ResultVariable is null)
                                    {
                                        var list = GenerateIRForExpr(left, parentFunction);
                                        irList.AddRange(list);
                                    }
                                    var right = binaryOperatorExpr.RightExpression as Expr;
                                    if (right.ResultVariable is null)
                                    {
                                        var list = GenerateIRForExpr(right, parentFunction);
                                        irList.AddRange(list);
                                    }
                                    Variable resultTempVar = parentFunction.AllocateTempVariable(IR.Type.I32);
                                    binaryOperatorExpr.ResultVariable = resultTempVar;
                                    Instruction instruction = binaryOperatorExpr.Operator switch
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
                                throw new NotImplementedException($"{binaryOperatorExpr.Operator}");
                        }
                        break;
                    }
                case CallExpr callExpr:
                    {
                        if (callExpr.FunctionRefExpr is not DeclRefExpr func ||
                            (func.Ref.Type != AST.Type.IntFunc &&
                            func.Ref.Type != AST.Type.VoidFunc))
                        {
                            throw new SemanticErrorException("Function DeclRefExpr expected");
                        }
                        List<Variable> args = new();
                        foreach (var item in callExpr.ArgumentExprList)
                        {
                            if (item.ResultVariable is null)
                            {
                                var ir = GenerateIRForExpr(item as Expr, parentFunction);
                                irList.AddRange(ir);
                            }
                            args.Add(item.ResultVariable);
                        }
                        Variable resultTempVar = parentFunction.AllocateTempVariable(IR.Type.I32);
                        callExpr.ResultVariable = resultTempVar;
                        CallInstrunction instruction = new(func.Ref.Name, args, resultTempVar);
                        irList.Add(instruction);
                        break;
                    }
                case DeclRefExpr declRefExpr:
                    {
                        var variable = parentFunction.IRSymbolTable.VariableDictionary[declRefExpr.Ref.Guid];
                        declRefExpr.ResultVariable = variable;
                        break;
                    }
                case StringLiteral literal:
                    {
                        Variable resultTempVar = parentFunction.AllocateTempVariable(IR.Type.I32);
                        literal.ResultVariable = resultTempVar;
                        IRStringLiteral iRStringLiteral = new(literal);
                        parentFunction.StringLiterals.Add(iRStringLiteral);
                        LoadStringLiternalInstruction instruction = new(resultTempVar, iRStringLiteral);
                        irList.Add(instruction);
                        break;
                    }
                case IntegerLiteral literal:
                    {
                        Variable resultTempVar = parentFunction.AllocateTempVariable(IR.Type.I32);
                        literal.ResultVariable = resultTempVar;
                        LoadIntLiternalInstruction instruction = new(resultTempVar, literal.Value);
                        irList.Add(instruction);
                        break;
                    }
                case ArraySubscriptExpr arraySubscript:
                    {
                        if (arraySubscript.SubscriptExpr.ResultVariable is null)
                        {
                            var list = GenerateIRForExpr(arraySubscript.SubscriptExpr, parentFunction);
                            irList.AddRange(list);
                        }
                        if (arraySubscript.ArrayRefExpr is DeclRefExpr arrayRef && arrayRef.Ref.Type is AST.Type.IntArray)
                        {
                            Variable resultTempVar = parentFunction.AllocateTempVariable(Type.I32);
                            var arrayVar = parentFunction.IRSymbolTable.VariableDictionary[arrayRef.Ref.Guid] as ArrayVariable;
                            ArrayIndexVariable arrayIndexVariable = new(arrayVar, arraySubscript.SubscriptExpr.ResultVariable);
                            LoadArrayInstruction instruction = new(arrayIndexVariable, resultTempVar);
                            arraySubscript.ResultVariable = resultTempVar;
                            irList.Add(instruction);
                        }
                        else
                        {
                            throw new SemanticErrorException("Array DeclRefExpr expected");
                        }
                        break;
                    }
                case EmptyExpr e:
                    {
                        break;
                    }
                default:
                    break;
            }
            return irList;
        }
        internal static List<SymbolTableItem> CollectSymbolsFromFunction(FunctionDecl functionDecl)
        {
            List<SymbolTableItem> symbolTableItems = new();
            // symbolTableItems.AddRange(functionDecl.SymbolTable.Symbols.Values);
            symbolTableItems.AddRange(functionDecl.Body.CollectSymbolTableItems());
            /*
            Console.WriteLine("CollectSymbolsFromFunction:");
            foreach (var item in symbolTableItems)
            {
                Console.WriteLine(item);
            }
            */
            return symbolTableItems;
        }
    }
}
