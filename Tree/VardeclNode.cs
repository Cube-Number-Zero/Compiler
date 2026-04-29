public class VardeclNode : StmtNode {
    public Token token;
    public VarType type;

    protected VardeclNode(Token token, VarType type) {
        this.token = token;
        this.type = type;
    }

    public new static VardeclNode Parse(Tokenizer T) {
        if (FuncdefNode.currentFunction is null) {
            // we are global scope
        } else {
            FuncdefNode.currentFunction.numLocals++;
        }
        _ = T.Expect("VAR");
        Token id = T.Expect("ID");
        _ = T.Expect("COLON");
        Token type;
        if (T.Peek() == "TYPE") {
            type = T.Expect("TYPE");
        } else {
            type = T.Expect("ID");
        }
        return new VardeclNode(id, VarType.FromToken(type));
    }

    public static bool CanParse(Tokenizer T) {
        return T.Peek() == "VAR";
    }

    public override List<TreeNode> GetChildren() {
        return [];
    }

    public override void TypeCheck() {}
    public override void SetupCFG() {
        this.entry.AddNext(this.exit);
    }
}