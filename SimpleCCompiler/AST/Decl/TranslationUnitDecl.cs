using SimpleCCompiler.IR.Instrunction;
using System;
using System.Collections.Generic;
using SimpleCCompiler.IR;

namespace SimpleCCompiler.AST
{
    public class TranslationUnitDecl : Decl
    {
        public SymbolTable SymbolTable { get; set; } = new();
        // for json serialize change to ValueDecl
        public List<FunctionDecl> FunctionDeclList { get; set; } = new();
        public List<VarDecl> VarDeclList { get; set; } = new();
        public override SymbolTableItem LookupSymbolTable(string name)
        {
            if (SymbolTable.TryGetValue(name, out SymbolTableItem item))
            {
                return item;
            }
            else
            {
                throw new SemanticErrorException($"Unknown symbol {name}");
            }
        }
        public override void AddDeclaration(Decl decl)
        {
            switch (decl)
            {
                case VarDecl varDecl:
                    if (varDecl.Parent is null)
                    {
                        varDecl.Parent = this;
                    }
                    else if (varDecl.Parent != this)
                    {
                        throw new InternalErrorException("Different parent detacted");
                    }
                    VarDeclList.Add(varDecl);
                    SymbolTable.AddOrException(varDecl.Name, varDecl.Type);
                    break;
                case FunctionDecl varDecl:
                    if (varDecl.Parent is null)
                    {
                        varDecl.Parent = this;
                    }
                    else if (varDecl.Parent != this)
                    {
                        throw new InternalErrorException("Different parent detacted");
                    }
                    FunctionDeclList.Add(varDecl);
                    SymbolTable.AddOrException(varDecl.Name, varDecl.Type);
                    break;
                default:
                    throw new SemanticErrorException($"Unexpected token {decl}, expected VarDecl");
            }
        }
        public Module GenerateModule()
        {

            Module module = new();
            // global variable
            // TODO
            foreach (var variable in VarDeclList)
            {
                ;
            }
            foreach (var functionDecl in FunctionDeclList)
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
                    v.Type = IR.Type.I32;
                    v.OffsetEBP = (argItem.ParameterNumber + 1) * 4;
                    function.IRSymbolTable.VariableDictionary.Add(v.Guid, v);
                }
                // auto variables
                var symbols = functionDecl.CollectSymbolsFromFunction();
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
                                AST.Type.Int => IR.Type.I32,
                                AST.Type.Char => IR.Type.I8,
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
                                AST.Type.IntArray => IR.Type.I32Array,
                                AST.Type.CharArray => IR.Type.I8Array,
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
                function.IRs.AddRange(functionDecl.Body.GenerateIR(function));
                alloc.FrameSize = function.AutoVariableCount * 4 + 16;
                module.Functions.Add(function);
            }
            foreach (Function item in module.Functions)
            {
                module.Instructions.AddRange(item.IRs);
            }
            return module;
        }
    }
}
