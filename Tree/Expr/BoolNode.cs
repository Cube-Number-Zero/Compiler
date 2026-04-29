public class BoolNode(Token tok, ExprNode left, ExprNode right) : BinaryOperator(tok, left, right) {
    protected override Dictionary<VarType, VarType> TypeRules { get; } = new() {
        {VarType.Bool, VarType.Bool}
    };
    public override void GenCode() {
        if (this.type is null) throw new Exception();

        if (this.type == VarType.Int)
        {
            this.left.GenCode();
            this.left.temporary!.CopyToRegister(Asm.Register.rax);
            Asm.Label lbl = new();
            Asm.Asm.Emit(   new Asm.OpCmpRegConst(Asm.Register.rax, 0),
                            new Asm.OpJcc("e", lbl)
            );
            this.right.GenCode();
            this.right.temporary!.CopyToRegister(Asm.Register.rax);
            Asm.Asm.Emit(lbl);
            this.temporary!.CopyFromRegister(Asm.Register.rax, StorageClass.STATIC);
        }
    }
}
