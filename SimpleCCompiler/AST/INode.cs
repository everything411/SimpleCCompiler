namespace SimpleCCompiler.AST
{
    public interface INode
    {
        public INode Parent { get; set; }
        public void AddDeclaration(IDecl decl);
        public SymbolTableItem LookupSymbolTable(string name);
    }
}
