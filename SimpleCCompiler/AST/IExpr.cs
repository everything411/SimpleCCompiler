using SimpleCCompiler.IR;

namespace SimpleCCompiler.AST
{
    public interface IExpr : INode
    {
        public Variable ResultVariable { get; set; }
    }
}
