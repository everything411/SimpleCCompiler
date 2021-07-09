using Antlr4.Runtime.Misc;
using SimpleCCompiler.AST;
using SimpleCCompiler.AST.Decl;
using SimpleCCompiler.AST.Expr;
using SimpleCCompiler.AST.Stmt;
using System;
using System.Collections.Generic;

namespace SimpleCCompiler.Parser
{
    public class DebugStack<T> : Stack<T>
    {
        public new void Push(T item)
        {
            Console.WriteLine($"Push {item}");
            base.Push(item);
        }
        public new T Pop()
        {
            Console.WriteLine($"Pop {base.Peek()}");
            return base.Pop();
        }
    }
    public class SimpleCASTListener : CBaseListener
    {
        TranslationUnitDecl translationUnitDecl;
        public Stack<INode> NodeStack { get; }
        public Stack<INode> GetNodeStackDebug()
        {
            return NodeStack;
        }

        public TranslationUnitDecl GetASTRoot()
        {
            return translationUnitDecl;
        }
        public SimpleCASTListener() : base()
        {
            translationUnitDecl = new TranslationUnitDecl();
            NodeStack = new Stack<INode>();
        }


        public override void EnterCompilationUnit([NotNull] CParser.CompilationUnitContext context)
        {
            base.EnterCompilationUnit(context);
            translationUnitDecl.SymbolTable.AddOrException("Mars_GetInt", AST.Type.IntFunc);
            translationUnitDecl.SymbolTable.AddOrException("Mars_PrintInt", AST.Type.VoidFunc);
            translationUnitDecl.SymbolTable.AddOrException("Mars_PrintStr", AST.Type.VoidFunc);
            NodeStack.Push(translationUnitDecl);
        }

        public override void ExitCompilationUnit([NotNull] CParser.CompilationUnitContext context)
        {
            base.ExitCompilationUnit(context);
            if (NodeStack.Count != 1)
            {
                throw new SyntaxErrorException("Unexpected token in parse tree");
            }
        }
        public override void EnterDeclaration([NotNull] CParser.DeclarationContext context)
        {
            base.EnterDeclaration(context);
            var node = NodeStack.Peek();
            VarDeclList v = new();
            v.Parent = node;
            switch (node)
            {
                case TranslationUnitDecl _:
                    NodeStack.Push(v);
                    break;
                case CompoundStmt _:
                    NodeStack.Push(v);
                    break;
                case ForStmt _:
                    NodeStack.Push(v);
                    break;
                default:
                    throw new SyntaxErrorException($"Unexpected token {node}, expected TranslationUnitDecl, ForStmt or CompoundStmt");
            }
        }
        public override void ExitDeclaration([NotNull] CParser.DeclarationContext context)
        {
            base.ExitDeclaration(context);
            var node = NodeStack.Pop();
            switch (node)
            {
                case VarDeclList decls:
                    var parent = NodeStack.Peek();
                    foreach (var decl in decls.VarDecls)
                    {
                        switch (parent)
                        {
                            case TranslationUnitDecl translation:
                                translation.AddDeclaration(decl);
                                break;
                            case CompoundStmt compoundStmt:
                                compoundStmt.AddDeclaration(decl);
                                if (decl.InitializerExpr is not null)
                                {
                                    var es = new ExpressionStmt
                                    {
                                        Parent = compoundStmt
                                    };
                                    var e = new BinaryOperatorExpr
                                    {
                                        Operator = BinaryOperator.Assignment,
                                        Parent = es,
                                        RightExpression = decl.InitializerExpr
                                    };
                                    es.Expr = e;
                                    var d = new DeclRefExpr
                                    {
                                        Parent = e,
                                        Ref = compoundStmt.LookupSymbolTable(decl.Name)
                                    };
                                    e.LeftExpression = d;
                                    compoundStmt.Stmts.Add(es);
                                }
                                break;
                            case ForStmt forStmt:
                                forStmt.AddDeclaration(decl);
                                if (decl.InitializerExpr is not null)
                                {
                                    var e = new BinaryOperatorExpr
                                    {
                                        Operator = BinaryOperator.Assignment,
                                        Parent = forStmt,
                                        RightExpression = decl.InitializerExpr
                                    };
                                    var d = new DeclRefExpr
                                    {
                                        Parent = e,
                                        Ref = forStmt.LookupSymbolTable(decl.Name)
                                    };
                                    e.LeftExpression = d;
                                    forStmt.InitExpr = e;
                                }
                                
                                break;
                            default:
                                throw new SyntaxErrorException($"Unexpected token {parent}, expected TranslationUnitDecl, ForStmt or CompoundStmt");
                        }
                    }
                    break;
                default:
                    throw new SyntaxErrorException($"Unexpected token {node}, expected VarDecl");
            }
        }
        public override void EnterTypeSpecifier([NotNull] CParser.TypeSpecifierContext context)
        {
            base.EnterTypeSpecifier(context);
            var node = NodeStack.Peek();
            switch (node)
            {
                case FunctionDecl functionDecl:
                    {
                        string typeStr = context.GetText();
                        var type = typeStr switch
                        {
                            "int" => AST.Type.IntFunc,
                            "void" => AST.Type.VoidFunc,
                            "char" => AST.Type.CharFunc,
                            _ => throw new SyntaxErrorException($"Unexpected type {typeStr}"),
                        };
                        functionDecl.Type = type;
                        break;
                    }
                case VarDecl varDecl:
                    {
                        string typeStr = context.GetText();
                        var type = typeStr switch
                        {
                            "int" => AST.Type.Int,
                            "char" => AST.Type.Char,
                            _ => throw new SyntaxErrorException($"Unexpected type {typeStr}"),
                        };
                        varDecl.Type = type;
                        break;
                    }

                default:
                    throw new SyntaxErrorException($"Unexpected token {node}, expected FunctionDecl, VarDecl");
            }
        }
        public override void ExitTypeSpecifier([NotNull] CParser.TypeSpecifierContext context)
        {
            base.ExitTypeSpecifier(context);
        }
        public override void EnterDirectDeclarator([NotNull] CParser.DirectDeclaratorContext context)
        {
            base.EnterDirectDeclarator(context);
            // TODO: too ugly
            if (context.ChildCount == 1)
            {
                // Console.WriteLine($"debug DirectDeclarator {context.GetText()}");
                var node = NodeStack.Peek();
                switch (node)
                {
                    case VarDecl valueDecl:
                        valueDecl.Name = context.GetText();
                        break;
                    case FunctionDecl functionDecl:
                        functionDecl.Name = context.GetText();
                        // hack
                        functionDecl.Parent.AddDeclaration(functionDecl);
                        break;
                    default:
                        throw new SyntaxErrorException($"Unexpected token {node}, expected VarDecl or FunctionDecl");
                }
            }
            // array or func
            else
            {
                // array, change type
                if (context.LeftBracket() is not null)
                {
                    // Console.WriteLine("array");
                    var node = NodeStack.Pop();
                    switch (node)
                    {
                        case VarDecl valueDecl:
                            ArrayVarDecl arrayVarDecl = new();

                            arrayVarDecl.InitializerExpr = valueDecl.InitializerExpr;
                            arrayVarDecl.Name = valueDecl.Name;
                            arrayVarDecl.Parent = valueDecl.Parent;
                            
                            arrayVarDecl.Type = AST.Type.IntArray;
                            var parent = NodeStack.Peek();
                            if (parent is VarDeclList)
                            {
                                (parent as VarDeclList).VarDecls.Remove(valueDecl);
                                (parent as VarDeclList).VarDecls.Add(arrayVarDecl);
                            }
                            NodeStack.Push(arrayVarDecl);
                            break;
                        default:
                            throw new SyntaxErrorException($"Unexpected token {node}, expected VarDecl");
                    }
                }
            }

        }
        public override void ExitDirectDeclarator([NotNull] CParser.DirectDeclaratorContext context)
        {
            base.ExitDirectDeclarator(context);
        }

        public override void EnterInitDeclarator([NotNull] CParser.InitDeclaratorContext context)
        {
            base.EnterInitDeclarator(context);
            var node = NodeStack.Peek();
            switch (node)
            {
                case VarDeclList varDeclList:
                    {
                        VarDecl varDecl = new();
                        varDecl.Type = varDeclList.Type;
                        varDecl.Parent = varDeclList.Parent;
                        varDeclList.VarDecls.Add(varDecl);
                        NodeStack.Push(varDecl);
                        break;
                    }
                default:
                    throw new SyntaxErrorException($"Unexpected token {node}, expected VarDeclList");
            }
        }
        public override void ExitInitDeclarator([NotNull] CParser.InitDeclaratorContext context)
        {
            base.ExitInitDeclarator(context);
            NodeStack.Pop();
        }
        public override void EnterParameterDeclaration([NotNull] CParser.ParameterDeclarationContext context)
        {
            base.EnterParameterDeclaration(context);
            var node = NodeStack.Peek();
            switch (node)
            {
                case FunctionDecl f:
                    ParmVarDecl varDecl = new();
                    varDecl.Parent = f;
                    NodeStack.Push(varDecl);
                    break;
                default:
                    throw new SyntaxErrorException($"Unexpected token {node}, expected FunctionDecl");
            }
        }
        public override void ExitParameterDeclaration([NotNull] CParser.ParameterDeclarationContext context)
        {
            base.ExitParameterDeclaration(context);
            var decl = NodeStack.Pop();
            var parent = NodeStack.Peek();
            parent.AddDeclaration(decl as VarDecl);
        }
        // FunctionDefinition
        public override void EnterFunctionDefinition([NotNull] CParser.FunctionDefinitionContext context)
        {
            base.EnterFunctionDefinition(context);
            var node = NodeStack.Peek();
            switch (node)
            {
                case TranslationUnitDecl parent:
                    FunctionDecl functionDecl = new();
                    functionDecl.Parent = parent;
                    NodeStack.Push(functionDecl);
                    break;
                default:
                    throw new SyntaxErrorException($"Unexpected token {node}, expected TranslationUnitDecl");
            }
        }
        public override void ExitFunctionDefinition([NotNull] CParser.FunctionDefinitionContext context)
        {
            base.ExitFunctionDefinition(context);
            // dirty hack for recursive
            var decl = NodeStack.Pop();
            // var parent = NodeStack.Peek();
            // parent.AddDeclaration(decl as FunctionDecl);
        }

        // CompoundStatement
        public override void EnterCompoundStatement([NotNull] CParser.CompoundStatementContext context)
        {
            base.EnterCompoundStatement(context);
            var node = NodeStack.Peek();
            switch (node)
            {
                // TODO split different stmt
                case Stmt stmt:
                    var compound = new CompoundStmt
                    {
                        Parent = stmt
                    };
                    NodeStack.Push(compound);
                    break;
                case FunctionDecl functionDecl:
                    var cc = new CompoundStmt
                    {
                        Parent = functionDecl
                    };
                    NodeStack.Push(cc);
                    break;
                default:
                    throw new SyntaxErrorException($"Unexpected token {node}, expected Stmt or FunctionDecl");
            }
        }
        public override void ExitCompoundStatement([NotNull] CParser.CompoundStatementContext context)
        {
            base.ExitCompoundStatement(context);
            var node = NodeStack.Pop();
            switch (node)
            {
                case CompoundStmt compound:
                    var parent = NodeStack.Peek();
                    switch (parent)
                    {
                        case ForStmt parentForStmt:
                            parentForStmt.LoopBodyStmt = compound;
                            break;
                        case IfStmt parentIfStmt:
                            if (parentIfStmt.IfTrueBody is null)
                            {
                                parentIfStmt.IfTrueBody = compound;
                            }
                            else
                            {
                                parentIfStmt.IfFalseBody = compound;
                            }
                            break;
                        case CompoundStmt parentCompoundStmt:
                            parentCompoundStmt.Stmts.Add(compound);
                            break;
                        case FunctionDecl functionDecl:
                            functionDecl.Body = compound;
                            break;
                        default:
                            throw new SyntaxErrorException($"Unexpected token {parent}, expected ForStmt, IfStmt, CompoundStmt or FunctionDecl");
                    }
                    break;
                default:
                    throw new SyntaxErrorException($"Unexpected token {node}, expected CompoundStmt");
            }
        }

        // IterationStatement
        public override void EnterIterationStatement([NotNull] CParser.IterationStatementContext context)
        {
            base.EnterIterationStatement(context);
            var node = NodeStack.Peek();
            switch (node)
            {
                case Stmt stmt:
                    var forStmt = new ForStmt
                    {
                        Parent = stmt,
                    };
                    NodeStack.Push(forStmt);
                    break;
                case FunctionDecl functionDecl:
                    var forStmt2 = new ForStmt
                    {
                        Parent = functionDecl
                    };
                    NodeStack.Push(forStmt2);
                    break;
                default:
                    throw new SyntaxErrorException($"Unexpected token {node}, expected Stmt or FunctionDecl");
            }
        }
        public override void ExitIterationStatement([NotNull] CParser.IterationStatementContext context)
        {
            base.ExitIterationStatement(context);
            var node = NodeStack.Pop();
            switch (node)
            {
                case ForStmt forStmt:
                    var parent = NodeStack.Peek();
                    switch (parent)
                    {
                        case ForStmt parentFor:
                            parentFor.LoopBodyStmt = forStmt;
                            break;
                        case IfStmt parentIfStmt:
                            if (parentIfStmt.IfTrueBody is null)
                            {
                                parentIfStmt.IfTrueBody = forStmt;
                            }
                            else
                            {
                                parentIfStmt.IfFalseBody = forStmt;
                            }
                            break;
                        case CompoundStmt parentCompoundStmt:
                            parentCompoundStmt.Stmts.Add(forStmt);
                            break;
                        default:
                            throw new SyntaxErrorException($"Unexpected token {parent}, expected ForStmt, IfStmt or CompoundStmt");
                    }
                    break;
                default:
                    throw new SyntaxErrorException($"Unexpected token {node}, expected ForStmt");
            }
        }
        public override void EnterJumpStatement([NotNull] CParser.JumpStatementContext context)
        {
            base.EnterJumpStatement(context);
            // TODO: too ugly
            // break
            if (context.ChildCount == 2)
            {
                var node = NodeStack.Peek();
                switch (node)
                {
                    case Stmt stmt:
                        var breakStmt = new BreakStmt(stmt);
                        NodeStack.Push(breakStmt);
                        break;
                    default:
                        throw new SyntaxErrorException($"Unexpected token {node}, expected Stmt");
                }
            }
            // return
            else if (context.ChildCount == 3)
            {
                // Console.WriteLine($"return {context.GetText()}");
                var node = NodeStack.Peek();
                switch (node)
                {
                    case Stmt stmt:
                        var returnStmt = new ReturnStmt
                        {
                            Parent = stmt,
                        };
                        NodeStack.Push(returnStmt);
                        break;
                    default:
                        throw new SyntaxErrorException($"Unexpected token {node}, expected Stmt");
                }
            }
        }
        public override void ExitJumpStatement([NotNull] CParser.JumpStatementContext context)
        {
            base.ExitJumpStatement(context);
            var node = NodeStack.Pop();
            var parent = NodeStack.Peek();
            switch (node)
            {
                case ReturnStmt rstmt:
                    switch (parent)
                    {
                        case ForStmt parentFor:
                            parentFor.LoopBodyStmt = rstmt;
                            break;
                        case IfStmt parentIfStmt:
                            if (parentIfStmt.IfTrueBody is null)
                            {
                                parentIfStmt.IfTrueBody = rstmt;
                            }
                            else
                            {
                                parentIfStmt.IfFalseBody = rstmt;
                            }
                            break;
                        case CompoundStmt parentCompoundStmt:
                            parentCompoundStmt.Stmts.Add(rstmt);
                            break;
                        default:
                            throw new SyntaxErrorException($"Unexpected token {parent}, expected ForStmt, IfStmt or CompoundStmt");
                    }
                    break;
                case BreakStmt bstmt:
                    switch (parent)
                    {
                        case ForStmt parentFor:
                            parentFor.LoopBodyStmt = bstmt;
                            break;
                        case IfStmt parentIfStmt:
                            if (parentIfStmt.IfTrueBody is null)
                            {
                                parentIfStmt.IfTrueBody = bstmt;
                            }
                            else
                            {
                                parentIfStmt.IfFalseBody = bstmt;
                            }
                            break;
                        case CompoundStmt parentCompoundStmt:
                            parentCompoundStmt.Stmts.Add(bstmt);
                            break;
                        default:
                            throw new SyntaxErrorException($"Unexpected token {parent}, expected ForStmt, IfStmt or CompoundStmt");
                    }
                    break;
                default:
                    throw new SyntaxErrorException($"Unexpected token {node}, expected CompoundStmt");
            }
        }
        public override void EnterSelectionStatement([NotNull] CParser.SelectionStatementContext context)
        {
            base.EnterSelectionStatement(context);
            var node = NodeStack.Peek();
            switch (node)
            {
                case Stmt stmt:
                    var ifStmt = new IfStmt
                    {
                        Parent = stmt
                    };
                    NodeStack.Push(ifStmt);
                    break;
                case FunctionDecl functionDecl:
                    var ifStmt2 = new IfStmt
                    {
                        Parent = functionDecl
                    };
                    NodeStack.Push(ifStmt2);
                    break;
                default:
                    throw new SyntaxErrorException($"Unexpected token {node}, expected Stmt or FunctionDecl");
            }
        }
        public override void ExitSelectionStatement([NotNull] CParser.SelectionStatementContext context)
        {
            base.ExitSelectionStatement(context);
            var node = NodeStack.Pop();
            switch (node)
            {
                case IfStmt ifStmt:
                    var parent = NodeStack.Peek();
                    switch (parent)
                    {
                        case ForStmt parentForStmt:
                            parentForStmt.LoopBodyStmt = ifStmt;
                            break;
                        case IfStmt parentIfStmt:
                            if (parentIfStmt.IfTrueBody is null)
                            {
                                parentIfStmt.IfTrueBody = ifStmt;
                            }
                            else
                            {
                                
                                parentIfStmt.IfFalseBody = ifStmt;
                            }
                            break;
                        case CompoundStmt parentCompoundStmt:
                            parentCompoundStmt.Stmts.Add(ifStmt);
                            break;
                        default:
                            throw new SyntaxErrorException($"Unexpected token {parent}, expected ForStmt, IfStmt or CompoundStmt");
                    }
                    break;
                default:
                    throw new SyntaxErrorException($"Unexpected token {node}, expected CompoundStmt");
            }
        }

        public override void EnterAssignmentExpression([NotNull] CParser.AssignmentExpressionContext context)
        {
            base.EnterAssignmentExpression(context);
            // TODO: too ugly
            // =
            if (context.ChildCount == 3)
            {
                var operatorContext = context.assignmentOperator();
                if (operatorContext is not null)
                {
                    if (operatorContext.Assign() is not null)
                    {
                        BinaryOperatorExpr expr = new();
                        expr.Operator = BinaryOperator.Assignment;
                        var parent = NodeStack.Peek();
                        AddExpressionToParent(parent, expr);
                        NodeStack.Push(expr);
                    }
                    else
                    {
                        throw new NotImplementedException($"Unexpected token {operatorContext.GetText()}, expected BinaryOperator.Assignment");
                    }
                }
            }
        }
        public override void ExitAssignmentExpression([NotNull] CParser.AssignmentExpressionContext context)
        {
            base.ExitAssignmentExpression(context);
            if (context.ChildCount == 3)
            {
                NodeStack.Pop();
            }
        }
        public override void EnterEqualityExpression([NotNull] CParser.EqualityExpressionContext context)
        {
            base.EnterEqualityExpression(context);
            // TODO: too ugly
            // == !=
            if (context.ChildCount == 3)
            {
                BinaryOperatorExpr expr = new();
                if (context.Equal() is not null)
                {
                    expr.Operator = BinaryOperator.Equal;
                }
                else if (context.NotEqual() is not null)
                {
                    expr.Operator = BinaryOperator.NotEqual;
                }
                var parent = NodeStack.Peek();
                AddExpressionToParent(parent, expr);
                NodeStack.Push(expr);
            }
        }
        public override void ExitEqualityExpression([NotNull] CParser.EqualityExpressionContext context)
        {
            base.ExitEqualityExpression(context);
            if (context.ChildCount == 3)
            {
                NodeStack.Pop();
            }
            
        }
        public override void EnterRelationalExpression([NotNull] CParser.RelationalExpressionContext context)
        {
            base.EnterRelationalExpression(context);
            // TODO: too ugly
            // <= => < >
            if (context.ChildCount == 3)
            {
                BinaryOperatorExpr expr = new();
                if (context.LessEqual() is not null)
                {
                    expr.Operator = BinaryOperator.LessEqual;
                }
                else if (context.Less() is not null)
                {
                    expr.Operator = BinaryOperator.Less;
                }
                else if (context.GreaterEqual() is not null)
                {
                    expr.Operator = BinaryOperator.GreaterEqual;
                }
                else if (context.Greater() is not null)
                {
                    expr.Operator = BinaryOperator.Greater;
                }
                var parent = NodeStack.Peek();
                AddExpressionToParent(parent, expr);
                NodeStack.Push(expr);
            }
        }
        public override void ExitRelationalExpression([NotNull] CParser.RelationalExpressionContext context)
        {
            base.ExitRelationalExpression(context);
            if (context.ChildCount == 3)
            {
                NodeStack.Pop();
            }
        }
        public override void EnterAdditiveExpression([NotNull] CParser.AdditiveExpressionContext context)
        {
            base.EnterAdditiveExpression(context);
            // TODO: too ugly
            // + -
            if (context.ChildCount == 3)
            {
                BinaryOperatorExpr expr = new();
                if (context.Plus() is not null)
                {
                    expr.Operator = BinaryOperator.Plus;
                }
                else if (context.Minus() is not null)
                {
                    expr.Operator = BinaryOperator.Minus;
                }
                var parent = NodeStack.Peek();
                AddExpressionToParent(parent, expr);
                NodeStack.Push(expr);
            }
        }
        public override void ExitAdditiveExpression([NotNull] CParser.AdditiveExpressionContext context)
        {
            base.ExitAdditiveExpression(context);
            if (context.ChildCount == 3)
            {
                NodeStack.Pop();
            }
        }
        public override void EnterMultiplicativeExpression([NotNull] CParser.MultiplicativeExpressionContext context)
        {
            base.EnterMultiplicativeExpression(context);
            // TODO: too ugly
            // * /
            if (context.ChildCount == 3)
            {
                BinaryOperatorExpr expr = new();
                if (context.Star() is not null)
                {
                    expr.Operator = BinaryOperator.Mul;
                }
                else if (context.Mod() is not null)
                {
                    expr.Operator = BinaryOperator.Mod;
                }
                else if (context.Div() is not null)
                {
                    expr.Operator = BinaryOperator.Div;
                }
                var parent = NodeStack.Peek();
                AddExpressionToParent(parent, expr);
                NodeStack.Push(expr);
            }
        }
        public override void ExitMultiplicativeExpression([NotNull] CParser.MultiplicativeExpressionContext context)
        {
            base.ExitMultiplicativeExpression(context);
            if (context.ChildCount == 3)
            {
                NodeStack.Pop();
            }
        }
        public override void EnterPostfixExpression([NotNull] CParser.PostfixExpressionContext context)
        {
            base.EnterPostfixExpression(context);
            // TODO: too ugly
            // ++ --
            if (context.ChildCount == 2)
            {
                UnaryOperatorExpr expr = new();
                if (context.PlusPlus() is not null)
                {
                    expr.Operator = UnaryOperator.PostfixIncrement;
                }
                else if (context.MinusMinus() is not null)
                {
                    expr.Operator = UnaryOperator.PostfixDecrement;
                }
                var parent = NodeStack.Peek();
                AddExpressionToParent(parent, expr);
                NodeStack.Push(expr);
            }
            // ()
            else if(context.ChildCount == 3)
            {
                if (context.LeftParen() is not null)
                {
                    var expr = new CallExpr();
                    var parent = NodeStack.Peek();
                    AddExpressionToParent(parent, expr);
                    NodeStack.Push(expr);
                }
                else
                {
                    throw new SemanticErrorException($"Expected expression in {context.GetText()}");
                }
                // Console.WriteLine($"() {context.GetText()}");
            }
            // [] ()
            else if (context.ChildCount == 4)
            {
                if (context.LeftBracket() is not null)
                {
                    ArraySubscriptExpr expr = new();
                    var parent = NodeStack.Peek();
                    AddExpressionToParent(parent, expr);
                    NodeStack.Push(expr);
                }
                else if (context.LeftParen() is not null)
                {
                    CallExpr expr = new();
                    var parent = NodeStack.Peek();
                    AddExpressionToParent(parent, expr);
                    NodeStack.Push(expr);
                }
                else
                {
                    throw new SemanticErrorException($"Expected expression in {context.GetText()}");
                }
                // Console.WriteLine($"() [] {context.GetText()}");
            }
        }
        public override void ExitPostfixExpression([NotNull] CParser.PostfixExpressionContext context)
        {
            base.ExitPostfixExpression(context);
            // TODO: too ugly
            // ++ --
            if (context.ChildCount == 2)
            {
                NodeStack.Pop();
            }
            // ()
            else if (context.ChildCount == 3)
            {
                NodeStack.Pop();
            }
            // [] ()
            else if (context.ChildCount == 4)
            {
                NodeStack.Pop();
            }
        }

        public override void EnterArgumentExpressionList([NotNull] CParser.ArgumentExpressionListContext context)
        {
            base.EnterArgumentExpressionList(context);
            var parent = NodeStack.Peek();
            /*
            if (parent is ArgumentExprListExpr)
            {
                return;
            }
            */
            var expr = new ArgumentExprListExpr();
            expr.Parent = parent;
            // AddExpressionToParent(parent, expr);
            NodeStack.Push(expr);
        }
        public override void ExitArgumentExpressionList([NotNull] CParser.ArgumentExpressionListContext context)
        {
            base.ExitArgumentExpressionList(context);
            var node = NodeStack.Pop() as ArgumentExprListExpr;
            var parent = NodeStack.Peek();
            // TODO: this may be buggy
            switch (parent)
            {
                case ArgumentExprListExpr e:
                    e.ArgumentExprList.AddRange(node.ArgumentExprList);
                    break;
                case CallExpr f:
                    f.ArgumentExprList = node.ArgumentExprList;
                    break;
                default:
                    throw new SyntaxErrorException($"Unexpected {parent}, ArgumentExprListExpr or CallExpr expected");
            }
        }

        // PrimaryExpression
        public override void EnterPrimaryExpression([NotNull] CParser.PrimaryExpressionContext context)
        {
            base.EnterPrimaryExpression(context);
            var constant = context.Constant();
            var stringLiteral = context.StringLiteral();
            var identifier = context.Identifier();
            if (identifier is not null)
            {
                var parent = NodeStack.Peek();
                string s = identifier.ToString();
                DeclRefExpr declRef = new();
                AddExpressionToParent(parent, declRef);
                declRef.Ref = parent.LookupSymbolTable(s);
                // Console.WriteLine($"Identifier {s}");
            }
            else if (constant is not null)
            {
                var parent = NodeStack.Peek();
                int value = int.Parse(constant.ToString());
                IntegerLiteral literal = new() { Value = value };
                AddExpressionToParent(parent, literal);
                // Console.WriteLine($"Constant {value}");
            }
            else if (stringLiteral is not null)
            {
                var parent = NodeStack.Peek();
                string value = stringLiteral[0].ToString();
                StringLiteral literal = new() { Value = value };
                AddExpressionToParent(parent, literal);
                // Console.WriteLine($"stringLiteral {value}");
            }
        }
        public override void ExitPrimaryExpression([NotNull] CParser.PrimaryExpressionContext context)
        {
            base.ExitPrimaryExpression(context);
        }
        internal static void AddStatementToParent(INode parent, IStmt child)
        {

        }
        internal static void AddExpressionToParent(INode parent, IExpr child)
        {
            switch (parent)
            {
                case ArraySubscriptExpr array:
                    child.Parent = array;
                    if (array.ArrayRefExpr is null)
                    {
                        array.ArrayRefExpr = child;
                    }
                    else if (array.SubscriptExpr is null)
                    {
                        array.SubscriptExpr = child;
                    }
                    else
                    {
                        throw new SyntaxErrorException($"Unexpected {child}");
                    }
                    break;
                case BinaryOperatorExpr binary:
                    child.Parent = binary;
                    if (binary.LeftExpression is null)
                    {
                        binary.LeftExpression = child;
                    }
                    else if (binary.RightExpression is null)
                    {
                        binary.RightExpression = child;
                    }
                    else
                    {
                        throw new SyntaxErrorException($"Unexpected {child}");
                    }
                    break;
                case UnaryOperatorExpr unary:
                    child.Parent = unary;
                    if (unary.Expression is null)
                    {
                        unary.Expression = child;
                    }
                    else
                    {
                        throw new SyntaxErrorException($"Unexpected {child}");
                    }
                    break;
                case CallExpr call:
                    child.Parent = call;
                    if (call.FunctionRefExpr is null)
                    {
                        call.FunctionRefExpr = child;
                    }
                    else
                    {
                        call.ArgumentExprList.Add(child);
                    }
                    break;
                case CompoundStmt compoundStmt:
                    {
                        ExpressionStmt exprStmt = new()
                        {
                            Parent = compoundStmt,
                            Expr = child
                        };
                        child.Parent = exprStmt;
                        compoundStmt.Stmts.Add(exprStmt);
                        break;
                    }
                case ForStmt forStmt:
                    child.Parent = forStmt;
                    if (forStmt.InitExpr is null)
                    {
                        forStmt.InitExpr = child;
                    }
                    else if (forStmt.ConditionalExpr is null)
                    {
                        forStmt.ConditionalExpr = child;
                    }
                    else if (forStmt.EndExpr is null)
                    {
                        forStmt.EndExpr = child;
                    }
                    else if (forStmt.LoopBodyStmt is null)
                    {
                        ExpressionStmt exprStmt = new()
                        {
                            Parent = forStmt,
                            Expr = child
                        };
                        forStmt.LoopBodyStmt = exprStmt;
                    }
                    else
                    {
                        throw new Exception("What?");
                    }
                    break;
                case IfStmt ifStmt:
                    child.Parent = ifStmt;
                    if (ifStmt.ConditionalExpr is null)
                    {
                        ifStmt.ConditionalExpr = child;
                    }
                    else if(ifStmt.IfTrueBody is null)
                    {
                        ExpressionStmt exprStmt = new()
                        {
                            Parent = ifStmt,
                            Expr = child
                        };
                        ifStmt.IfTrueBody = exprStmt;
                    }
                    else
                    {
                        ExpressionStmt exprStmt = new()
                        {
                            Parent = ifStmt,
                            Expr = child
                        };
                        ifStmt.IfFalseBody = exprStmt;
                    }
                    /*
                    else
                    {
                        throw new Exception("What?");
                    }
                    */
                    break;
                case ReturnStmt returnStmt:
                    child.Parent = returnStmt;
                    if (returnStmt.ReturnValueExpr is null)
                    {
                        returnStmt.ReturnValueExpr = child;
                    }
                    else
                    {
                        throw new Exception("What?");
                    }
                    break;
                case ArrayVarDecl arrayVarDecl:
                    child.Parent = arrayVarDecl;
                    /*
                    arrayVarDecl.Type = arrayVarDecl.Type switch
                    {
                        AST.Type.Int => AST.Type.IntArray,
                        AST.Type.Char => AST.Type.CharArray,
                        _ => throw new Exception("What?")
                    };
                    */
                    if (arrayVarDecl.Size == 0 && child is IntegerLiteral)
                    {
                        arrayVarDecl.Size = (child as IntegerLiteral).Value;
                        // Console.WriteLine($"Array Size {arrayVarDecl.Size}");
                    }
                    else
                    {
                        throw new Exception("What?");
                    }
                    break;
                case VarDecl varDecl:
                    child.Parent = varDecl;
                    if (varDecl.InitializerExpr is null)
                    {
                        varDecl.InitializerExpr = child;
                    }
                    else
                    {
                        throw new Exception("What?");
                    }
                    break;
                case ArgumentExprListExpr argumentExprList:
                    child.Parent = argumentExprList.Parent;
                    argumentExprList.ArgumentExprList.Add(child);
                    break; 
                default:
                    throw new SyntaxErrorException($"Unexpected parent node {parent}");
            }
        }
    }
}
