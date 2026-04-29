public class StmtsNode : TreeNode {
    public List<StmtNode> stmts = [];
    public static StmtsNode Parse(Tokenizer T) {
        _ = T.Expect("LBRACE");

        _ = T.Expect("$$");

        StmtsNode s = new StmtsNode();

        while (T.Peek() != "RBRACE") {
            if (T.Peek() == "$$") {
                _ = T.Expect("$$");
                continue;
            }
            s.stmts.Add(StmtNode.Parse(T));
        }
        _ = T.Expect("RBRACE");

        return s;
    }
    public override List<TreeNode> GetChildren() {
        List<TreeNode> tmp = [.. stmts];
        return tmp;
    }

    public override void TypeCheck() {} // no type checking needed
    public override void SetupCFG() {
        if (stmts.Count == 0) {
             this.entry.AddNext(this.exit);
             return;
        }
        this.entry.AddNext(this.stmts[0].entry);
        for(int i = 1; i < this.stmts.Count; ++i)
            this.stmts[i - 1].exit.AddNext(this.stmts[i].entry);
        this.stmts[^1].exit.AddNext(this.exit);
    }
}