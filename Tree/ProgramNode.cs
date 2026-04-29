public class ProgramNode : TreeNode {
    private static readonly Dictionary<string, ClassType> classes = [];

    public static ClassType GetClassTypeFromClassName(Token className) {
        if (className.sym != "ID")
            throw new Exception();
        if (!classes.ContainsKey(className.lexeme))
            classes[className.lexeme] = new ClassType(className);
        return classes[className.lexeme];
    }

    public List<FuncdefNode> funcs = [];
    public static ProgramNode Parse(Tokenizer T) {
        ProgramNode p = new();
        while (true) {
            if (FuncdefNode.CanParse(T)) {
                p.funcs.Add(FuncdefNode.Parse(T, new GlobalLocation()));
            } else if (ClassDeclNode.CanParse(T)) {
                ClassDeclNode decl = ClassDeclNode.Parse(T);
                classes[decl.classType.name.lexeme] = decl.classType;
            } else if (T.Peek() == "$") {
                break;
            } else if (T.Peek() == "$$") {
                _ = T.Expect("$$");
            } else if (VardeclNode.CanParse(T)) {
                VardeclNode decl = VardeclNode.Parse(T);
                SymbolTable.Declare(decl.token, decl.type, new GlobalLocation());
            } else {
                Utils.Error("Unexpected thing: " + T.Peek() + " at line " + T.GetCurrentLine() + " column " + T.GetCurrentColumn());
            }
        }
        return p;
    }
    public override List<TreeNode> GetChildren() {
        List<TreeNode> tmp = [];
        tmp.AddRange(funcs);
        foreach (var c in classes.Values) {
            if (c.declarer is null)
                Utils.Error($"Class {c.name.lexeme} declared but not defined at line {c.name.line} column {c.name.column}");
            tmp.Add(c.declarer!);
        }
        return tmp;
    }
    public override void TypeCheck() {} // no type checking needed

    public override void SetupCFG() {
        throw new NotImplementedException();
    }
}