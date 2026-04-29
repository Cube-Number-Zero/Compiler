public class BoolNotNode(Token tok, ExprNode operand) : UnaryOperator(tok, operand) {
    protected override Dictionary<VarType, VarType> TypeRules { get; } = new() {
        {VarType.Bool, VarType.Bool}
    };
    public override void GenCode() {
        operand.GenCode();
        operand.temporary!.CopyToRegister(Asm.Register.rax);
        Asm.Asm.Emit(
            new Asm.OpAddConstReg(-1, Asm.Register.rax),
            new Asm.OpAndRegConst(Asm.Register.rax, 1));
        this.temporary!.CopyFromRegister(Asm.Register.rax, StorageClass.STATIC);
    }
}
