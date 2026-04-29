public class NegateNode(Token tok, ExprNode operand) : UnaryOperator(tok, operand) {
    protected override Dictionary<VarType, VarType> TypeRules { get; } = new() {
        {VarType.Int, VarType.Int},
        {VarType.Float, VarType.Float}
    };
    public override void GenCode() {
        this.operand.GenCode();
        if (this.type! == VarType.Int) {
            this.operand.temporary!.CopyToRegister(Asm.Register.rax);
            Asm.Asm.Emit(new Asm.OpNeg(Asm.Register.rax));
            this.temporary!.CopyFromRegister(Asm.Register.rax, StorageClass.STATIC);
        } else if (this.type! == VarType.Float) {
            this.operand.temporary!.CopyToRegister(Asm.Register.xmm0);
            Asm.Asm.Emit(new Asm.OpFNeg(Asm.Register.xmm0));
            this.temporary!.CopyFromRegister(Asm.Register.xmm0, StorageClass.STATIC);
        } else {
            throw new NotImplementedException();
        }
    }
}
