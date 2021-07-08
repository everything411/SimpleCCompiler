using SimpleCCompiler.AST;
using System.Collections.Generic;
using System.Text.Json;

namespace SimpleCCompiler
{
    public class SymbolTableItem
    {
        public AST.StorageClass StorageClass { get; set; }
        public AST.Type Type { get; set; }
        public string Name { get; set; }
        public SymbolTableItem(AST.Type type, string name)
        {
            Type = type;
            Name = name;
        }
    }
    public class SymbolTable
    {
        public Dictionary<string, SymbolTableItem> Symbols { get; set; } = new();

        private void Add(string name, AST.Type type)
        {
            Symbols.Add(name, new SymbolTableItem(type, name));
        }
        public void AddOrException(string name, AST.Type type)
        {
            if (Symbols.GetValueOrDefault(name) is not null)
            {
                throw new SemanticErrorException($"Symbol {name} redefined");
            }
            Add(name, type);
        }

        public bool TryGetValue(string name, out SymbolTableItem symbolTableItem)
        {
            return Symbols.TryGetValue(name, out symbolTableItem);
        }
        public override string ToString() => $"{base.ToString()}:{JsonSerializer.Serialize(Symbols)}";
    }
}
