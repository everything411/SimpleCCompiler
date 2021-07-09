using System.Collections.Generic;

namespace SimpleCCompiler.AST.Decl
{
    public class VarDecl : ValueDecl
    {
        public IExpr InitializerExpr { get; set; }
        public override SymbolTableItem LookupSymbolTable(string name)
        {
            return Parent.LookupSymbolTable(name);
        }
        public override string ToString() => $"{base.ToString()}:{InitializerExpr}";
    }
    public class ArrayVarDecl : VarDecl
    {
        public int Size { get; set; }
        public override string ToString() => $"{base.ToString()}:Array[{Size}]";
    }
    public class VarDeclList : VarDecl
    {
        public List<VarDecl> VarDecls { get; set; } = new();
    }
}
