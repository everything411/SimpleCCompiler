namespace SimpleCCompiler.AST.Decl
{

    public class ValueDecl : Decl
    {
        public string Name { get; set; } = "<no name>";
        public Type Type { get; set; } = AST.Type.Undefined;
        public override string ToString() => $"{base.ToString()}:{Name}:{Type}";
    }
}
