public class AddNode(Token tok, ExprNode left, ExprNode right) : BinaryOperator(tok, left, right) {
    protected override Dictionary<VarType, VarType> TypeRules { get; } = new() {
        {VarType.Int,    VarType.Int   },
        {VarType.Float,  VarType.Float },
        {VarType.String, VarType.String}
    };
    public override void GenCode() {
        if (type is null) {
            throw new Exception();
        }
        if (this.type == VarType.Int) {
            this.left.GenCode();
            this.right.GenCode();
            this.left.temporary!.CopyToRegister(Asm.Register.rax);
            this.right.temporary!.CopyToRegister(Asm.Register.rbx);
            Asm.Asm.Emit(new Asm.OpAdd(left: Asm.Register.rax, right: Asm.Register.rbx));
            this.temporary!.CopyFromRegister(Asm.Register.rax, StorageClass.STATIC);
        } else if (this.type == VarType.Float) {
            this.left.GenCode();
            this.right.GenCode();
            this.left.temporary!.CopyToRegister(Asm.Register.xmm0);
            this.right.temporary!.CopyToRegister(Asm.Register.xmm1);
            Asm.Asm.Emit(new Asm.OpFAdd(left: Asm.Register.xmm0, right: Asm.Register.xmm1));
            this.temporary!.CopyFromRegister(Asm.Register.xmm0, StorageClass.STATIC);
        } else {
            throw new NotImplementedException();
        }
    }
}
