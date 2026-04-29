public class ClassDeclNode : TreeNode {
    public ClassType classType;
    public List<TreeNode> contents = [];
    public ClassDeclNode(Token className) {
        this.classType = ProgramNode.GetClassTypeFromClassName(className);
        this.classType.declarer = this;
    }
    public static ClassDeclNode Parse(Tokenizer T) {
        T.Expect("CLASS");
        Token name = T.Expect("ID");
        ClassDeclNode declNode = new ClassDeclNode(name);
        if (ProgramNode.GetClassTypeFromClassName(name).declarer is not null) {
            Utils.Error($"Class {name.lexeme} already declared at line {name.line} column {name.column}");
        }
        T.Expect("LBRACE");
        SymbolTable.AddScope();
        List<TreeNode> children = [];
        while (true) {
            if (VardeclNode.CanParse(T)) {
                VardeclNode decl = VardeclNode.Parse(T);
                ProgramNode.GetClassTypeFromClassName(name).members.Add(new ClassType.Member(decl.token.lexeme, decl.type));
                decl.token.lexeme = name.lexeme + "." + decl.token.lexeme;
                SymbolTable.Declare(decl.token, decl.type, new MemberLocation(declNode));
                children.Add(decl);
            } else if (FuncdefNode.CanParse(T)) {
                FuncdefNode decl = FuncdefNode.Parse(T, new MemberLocation(declNode), name.lexeme);
                FuncType F = (SymbolTable.Lookup(decl.name.lexeme, decl.name.line).type as FuncType)!;
                ProgramNode.GetClassTypeFromClassName(name).members.Add(new ClassType.Member(decl.name.lexeme, F));
                children.Add(decl);
            } else if (T.Peek() == "$") {
                break;
            } else if (T.Peek() == "$$") {
                _ = T.Expect("$$");
            } else if (T.Peek() == "RBRACE") {
                break;
            } else {
                Utils.Error("Unexpected thing: " + T.Peek() + " at line " + T.GetCurrentLine() + " column " + T.GetCurrentColumn());
            }
        }
        T.Expect("RBRACE");
        SymbolTable.RemoveScope();
        declNode.contents = children;
        return declNode;
    }
    public static bool CanParse(Tokenizer T) {
        return T.Peek() == "CLASS";
    }
    public override List<TreeNode> GetChildren() {
        return contents;
    }
    public override void TypeCheck() {
        if (classType.declarer is null)
            Utils.Error($"Class {classType.name.lexeme} declared but not defined at line {classType.name.line} column {classType.name.column}");
    }
    public override void SetupCFG() {
        throw new NotImplementedException();
    }
}