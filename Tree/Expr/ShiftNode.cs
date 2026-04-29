public class ShiftNode(Token tok, ExprNode left, ExprNode right) : BinaryOperator(tok, left, right) {
    protected override Dictionary<VarType, VarType> TypeRules { get; } = new() {
        {VarType.Int, VarType.Int}
    };
    public override void GenCode() {
        if (type is null) {
            throw new Exception();
        }
        if (this.type == VarType.Int) {
            this.left.GenCode();
            this.right.GenCode();
            this.left.temporary!.CopyToRegister(Asm.Register.rax);
            this.right.temporary!.CopyToRegister(Asm.Register.rcx);
            switch (this.token.lexeme) {
                case "<<":
                    Asm.Asm.Emit(
                        new Asm.OpShiftLeft(Asm.Register.rax),
                        new Asm.OpXor(Asm.Register.rdx, Asm.Register.rdx),
                        new Asm.OpCmpRegConst(Asm.Register.rcx, 63),
                        new Asm.OpCmovCC("g", Asm.Register.rdx, Asm.Register.rax)
                    );
                    break;
                case ">>":
                    Asm.Asm.Emit(
                        new Asm.OpShiftRight(Asm.Register.rax),
                        new Asm.OpXor(Asm.Register.rdx, Asm.Register.rdx),
                        new Asm.OpCmpRegConst(Asm.Register.rax, 0),
                        new Asm.OpMoveConstReg(-1, Asm.Register.r8),
                        new Asm.OpCmovCC("s", Asm.Register.r8, Asm.Register.rdx),
                        new Asm.OpCmpRegConst(Asm.Register.rcx, 63),
                        new Asm.OpCmovCC("g", Asm.Register.rdx, Asm.Register.rax)
                    );
                    break;
                case ">>>": // right shift logical
                    Asm.Asm.Emit(
                        new Asm.OpShiftRightLogical(Asm.Register.rax),
                        new Asm.OpXor(Asm.Register.rdx, Asm.Register.rdx),
                        new Asm.OpCmpRegConst(Asm.Register.rcx, 63),
                        new Asm.OpCmovCC("g", Asm.Register.rdx, Asm.Register.rax)
                    );
                    break;
                default:
                    throw new Exception();
            }
            this.temporary!.CopyFromRegister(Asm.Register.rax, StorageClass.STATIC);
        } else {
            throw new NotImplementedException();
        }
    }
}
