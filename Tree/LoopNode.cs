public class LoopNode : StmtNode {
    
    public ExprNode expr;
    public StmtsNode stmts;
    public readonly Asm.Label endLoop;

    private LoopNode(ExprNode expr, StmtsNode stmts) {
        this.expr = expr;
        this.stmts = stmts;
        this.endLoop = new Asm.Label();
    }
    public override List<TreeNode> GetChildren() {
        return [expr, stmts];
    }
    public new static LoopNode Parse(Tokenizer T){
        _ = T.Expect("WHILE");
        ExprNode cond;
        if (T.Peek() == "LPAREN") {
            _ = T.Expect("LPAREN");
            cond = ExprNode.Parse(T);
            _ = T.Expect("RPAREN");
        } else {
            cond = ExprNode.Parse(T);
        }
        SymbolTable.AddScope();
        StmtsNode statements = StmtsNode.Parse(T);
        SymbolTable.RemoveScope();
        return new LoopNode(cond, statements);
    }

    public static bool CanParse(Tokenizer T) {
        return T.Peek() == "WHILE";
    }


    public override void TypeCheck() {
        if (expr.type! != VarType.Bool) {
            Utils.Error($"Expecting boolean expression in while loop at line {expr.token.line} column {expr.token.column} but got {expr.type}");
        }
    }

    public override void GenCode() {
        Asm.Label beginLoop = new();
        this.expr.GenCode();
        this.expr.temporary!.CopyToRegister(Asm.Register.rax);
        Asm.Asm.Emit(   new Asm.OpTest(Asm.Register.rax, Asm.Register.rax),
                        new Asm.OpJcc("z", endLoop),
                        beginLoop);
        this.stmts.GenCode();
        this.expr.GenCode();
        this.expr.temporary!.CopyToRegister(Asm.Register.rax);
        Asm.Asm.Emit(   new Asm.OpTest(Asm.Register.rax, Asm.Register.rax),
                        new Asm.OpJcc("nz", beginLoop),
                        endLoop);
    }
    public override void SetupCFG() {
        CFGNode testNode = new("test", this);
        this.entry.AddNext(testNode);
        testNode.AddNext(this.expr.entry);
        this.expr.exit.AddNext(this.stmts.entry);
        this.expr.exit.AddNext(this.exit);
        this.stmts.exit.AddNext(testNode);
    }
}