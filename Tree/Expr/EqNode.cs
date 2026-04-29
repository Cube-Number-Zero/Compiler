public class EqNode(Token tok, ExprNode left, ExprNode right) : BinaryOperator(tok, left, right) {
    protected override Dictionary<VarType, VarType> TypeRules { get; } = new() {
        {VarType.Int, VarType.Bool},
        {VarType.Float, VarType.Bool},
        {VarType.String, VarType.Bool},
        {VarType.Bool, VarType.Bool}
    };
    public override void GenCode()
    {
        left.GenCode();
        right.GenCode();
        if (this.left.type! == VarType.Int || this.left.type! == VarType.Bool) {
            this.left.temporary!.CopyToRegister(Asm.Register.rax);
            this.right.temporary!.CopyToRegister(Asm.Register.rbx);
            string cc = this.token.lexeme switch
            {
                "==" => "e",
                "!=" => "ne",
                _ => throw new Exception(),
            };
            Asm.Asm.Emit(
                new Asm.OpXor(Asm.Register.rcx, Asm.Register.rcx),
                new Asm.OpXor(Asm.Register.rdx, Asm.Register.rdx),
                new Asm.OpInc(Asm.Register.rdx),
                new Asm.OpCmpRegReg(Asm.Register.rax, Asm.Register.rbx),
                new Asm.OpCmovCC(cc, Asm.Register.rdx, Asm.Register.rcx));

            this.temporary!.CopyFromRegister(Asm.Register.rcx, StorageClass.STATIC);
        } else if (this.left.type! == VarType.Float) {
            this.left.temporary!.CopyToRegister(Asm.Register.xmm0);
            this.right.temporary!.CopyToRegister(Asm.Register.xmm1);
            string cc = this.token.lexeme switch
            {
                "==" => "eq",
                "!=" => "neq",
                _ => throw new Exception(),
            };
            Asm.Asm.Emit(
                new Asm.OpCmpSD(cc, Asm.Register.xmm0, Asm.Register.xmm1),
                new Asm.OpMoveRegReg(Asm.Register.xmm0, Asm.Register.rax),
                new Asm.OpAndRegConst(Asm.Register.rax, 1));
            this.temporary!.CopyFromRegister(Asm.Register.rax, StorageClass.STATIC);
        }
    }
}
