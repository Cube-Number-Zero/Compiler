public class CondNode : StmtNode {
    
    public ExprNode expr;
    public StmtsNode stmts;
    public TreeNode? elseResult = null;

    private CondNode(ExprNode expr, StmtsNode stmts) {
        this.expr = expr;
        this.stmts = stmts;
    }
    private CondNode(ExprNode expr, StmtsNode stmts, TreeNode elseResult) {
        this.expr = expr;
        this.stmts = stmts;
        this.elseResult = elseResult;
    }

    public override List<TreeNode> GetChildren() {
        if (elseResult == null)
            return [expr, stmts];
        else
            return [expr, stmts, elseResult];
    }
    public new static CondNode Parse(Tokenizer T){
        _ = T.Expect("IF");
        ExprNode cond = ExprNode.Parse(T);
        SymbolTable.AddScope();
        StmtsNode statements = StmtsNode.Parse(T);
        SymbolTable.RemoveScope();
        if (T.Peek() == "ELSE") {
            _ = T.Expect("ELSE");
            if (T.Peek() == "LBRACE") {
                StmtsNode statements2 = StmtsNode.Parse(T);
                return new CondNode(cond, statements, statements2);
            } else {
                CondNode elseif = CondNode.Parse(T);
                return new CondNode(cond, statements, elseif);
            }
        } else
            return new CondNode(cond, statements);
    }

    public static bool CanParse(Tokenizer T) {
        return T.Peek() == "IF";
    }

    public override void TypeCheck() {
        if (expr.type! != VarType.Bool) {
            Utils.Error($"Expecting boolean expression in conditional at line {expr.token.line} column {expr.token.column} but got {expr.type}");
        }
    }

    public override void GenCode() {
        this.expr.GenCode();
        this.expr.temporary!.CopyToRegister(Asm.Register.rax);
        Asm.Label endifL = new();
        if (elseResult is null) {
        
            Asm.Asm.Emit(
                new Asm.OpCmpRegConst(Asm.Register.rax, 0),
                new Asm.OpJcc("e", endifL)
            );
            this.stmts.GenCode();
            Asm.Asm.Emit(endifL);
        } else {
            Asm.Label elseL = new();
            Asm.Asm.Emit(
                new Asm.OpCmpRegConst(Asm.Register.rax, 0),
                new Asm.OpJcc("e", elseL)
            );
            this.stmts.GenCode();
            Asm.Asm.Emit(new Asm.OpJmp(endifL), elseL);
            this.elseResult.GenCode();
            Asm.Asm.Emit(endifL);
        }
    }
    public override void SetupCFG() {
        throw new NotImplementedException();
    }
}