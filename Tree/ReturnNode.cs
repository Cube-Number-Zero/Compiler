abstract class ReturnNode(Token t) : StmtNode {
    public Token RETURN = t;    //for error reporting

    public new static ReturnNode Parse(Tokenizer T){
        Token ret = T.Expect("RETURN");
        if (T.Peek() == "$$")
        {
            _ = T.Expect("$$");
            return new ReturnVoidNode(ret);
        } else {
            ExprNode expr = ExprNode.Parse(T);
            _ = T.Expect("$$");
            return new ReturnExprNode(ret, expr);
        }
    }

    public static bool CanParse(Tokenizer T) {
        return T.Peek() == "RETURN";
    }

    public class ReturnExprNode(Token ret, ExprNode e) : ReturnNode(ret) {
        public ExprNode expr = e;

        public override List<TreeNode> GetChildren() {
            return [expr];
        }
        public override void GenCode() {
            expr.GenCode();
            expr.temporary!.CopyToRegister(Asm.Register.rax);
            Asm.Asm.Emit(
                new Asm.Comment($"return from {((FuncdefNode)parent!.parent!).name.lexeme}"),
                new Asm.OpMoveRegReg(src: Asm.Register.rbp, dst: Asm.Register.rsp),
                new Asm.OpPopReg(Asm.Register.rbp),
                new Asm.OpRet()
            );
        }
        public override void SetupCFG() {
            throw new NotImplementedException();
        }
    }
    public class ReturnVoidNode(Token ret) : ReturnNode(ret) {
        public override List<TreeNode> GetChildren() {
            return [];
        }
        public override void GenCode() {
            Asm.Asm.Emit(
                new Asm.Comment($"return from {((FuncdefNode)parent!.parent!).name.lexeme}"),
                new Asm.OpMoveRegReg(src: Asm.Register.rbp, dst: Asm.Register.rsp),
                new Asm.OpPopReg(Asm.Register.rbp),
                new Asm.OpRet()
            );
        }
        public override void SetupCFG() {
            TreeNode? p = this.parent;
            while (p != null) {
                if (p is FuncdefNode) {
                    break;
                } else {
                    p = p.parent;
                }
            }
            this.entry.AddNext(p!.exit);
        }
    }

    public override void TypeCheck() {} // no type checking needed
}