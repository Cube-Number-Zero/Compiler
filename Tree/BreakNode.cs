public class BreakNode(Token token) : StmtNode {
    readonly Token token = token;

    public static new BreakNode Parse(Tokenizer T) {
        Token token = T.Expect("BREAK");
        return new BreakNode(token);
    }
    public static bool CanParse(Tokenizer T) {
        return T.Peek() == "BREAK";
    }
    public override List<TreeNode> GetChildren() {
        return [];
    }
    public override void TypeCheck() {} // Nothing to type check here
    public override void GenCode() {
        TreeNode? p = this.parent;
        while (p != null) {
            if (p is LoopNode L) {
                Asm.Asm.Emit(   new Asm.Comment($"Break at {this.token}"),
                                new Asm.OpJmp(L.endLoop));
            } else {
                p = p.parent;
            }
        }
        if (p == null) {
            Utils.Error($"Cannot use break statement outside of a loop at line {this.token.line} column {this.token.column}");
        }
    }
    public override void SetupCFG() {
        TreeNode? p = this.parent;
        while (p != null) {
            if (p is LoopNode) {
                break;
            } else {
                p = p.parent;
            }
        }
        this.entry.AddNext(p!.exit);
    }
}