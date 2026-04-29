public class BitNode(Token tok, ExprNode left, ExprNode right) : BinaryOperator(tok, left, right) {
    protected override Dictionary<VarType, VarType> TypeRules { get; } = new() {
        {VarType.Int, VarType.Int}
    };
    public override void GenCode() {
        if (type is null) {
            throw new Exception();
        }
        Asm.Asm.Emit(new Asm.Comment($"Doing bitwise at {this.token.line}:{this.token.column}"));
        this.left.GenCode();
        this.right.GenCode();
        this.left.temporary!.CopyToRegister(Asm.Register.rax);
        this.right.temporary!.CopyToRegister(Asm.Register.rbx);
        if (this.type == VarType.Int) {
            switch (this.token.lexeme) {
                case "&":
                    Asm.Asm.Emit(new Asm.OpAnd(left: Asm.Register.rax, right: Asm.Register.rbx));
                    break;
                case "|":
                    Asm.Asm.Emit(new Asm.OpOr(left: Asm.Register.rax, right: Asm.Register.rbx));
                    break;
                case "^":
                    Asm.Asm.Emit(new Asm.OpXor(left: Asm.Register.rax, right: Asm.Register.rbx));
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
