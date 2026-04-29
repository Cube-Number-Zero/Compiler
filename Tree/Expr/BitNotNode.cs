public class BitNotNode(Token tok, ExprNode operand) : UnaryOperator(tok, operand) {
    protected override Dictionary<VarType, VarType> TypeRules { get; } = new() {
        {VarType.Int, VarType.Int}
    };

    public override void GenCode() {
        operand.GenCode();
        operand.temporary!.CopyToRegister(Asm.Register.rax);
        Asm.Asm.Emit(
            new Asm.OpNot(Asm.Register.rax));
        this.temporary!.CopyFromRegister(Asm.Register.rax, StorageClass.STATIC);
    }
}
