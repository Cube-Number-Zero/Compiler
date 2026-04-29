public class MulNode(Token tok, ExprNode left, ExprNode right) : BinaryOperator(tok, left, right) {
    protected override Dictionary<VarType, VarType> TypeRules { get; } = new() {
        {VarType.Int,   VarType.Int  },
        {VarType.Float, VarType.Float}
    };
    public override void GenCode() {
        if (type is null) {
            throw new Exception();
        }
        Asm.Asm.Emit(new Asm.Comment($"Doing mulop at {this.token.line}:{this.token.column}"));
        this.left.GenCode();
        this.right.GenCode();
        if (this.type == VarType.Int) {
            switch (this.token.lexeme) {
                case "*":
                    this.left.temporary!.CopyToRegister(Asm.Register.rax);
                    this.right.temporary!.CopyToRegister(Asm.Register.rbx);
                    Asm.Asm.Emit(new Asm.OpMul(left: Asm.Register.rax, right: Asm.Register.rbx));
                    this.temporary!.CopyFromRegister(Asm.Register.rbx, StorageClass.STATIC);
                    break;
                case "/":
                    this.left.temporary!.CopyToRegister(Asm.Register.rax);
                    Asm.Asm.Emit(new Asm.OpCQO());
                    this.right.temporary!.CopyToRegister(Asm.Register.rbx);
                    Asm.Asm.Emit(new Asm.OpDiv(Asm.Register.rbx));
                    this.temporary!.CopyFromRegister(Asm.Register.rax, StorageClass.STATIC);
                    break;
                case "%":
                    this.left.temporary!.CopyToRegister(Asm.Register.rax);
                    Asm.Asm.Emit(new Asm.OpCQO());
                    this.right.temporary!.CopyToRegister(Asm.Register.rbx);
                    Asm.Asm.Emit(new Asm.OpDiv(Asm.Register.rbx));
                    this.temporary!.CopyFromRegister(Asm.Register.rdx, StorageClass.STATIC);
                    break;
                default:
                    throw new Exception();
            }
        } else if (this.type == VarType.Float) {
            switch (this.token.lexeme) {
                case "*":
                    this.left.temporary!.CopyToRegister(Asm.Register.xmm0);
                    this.right.temporary!.CopyToRegister(Asm.Register.xmm1);
                    Asm.Asm.Emit(new Asm.OpFMul(Asm.Register.xmm0, Asm.Register.xmm1));
                    this.temporary!.CopyFromRegister(Asm.Register.xmm0, StorageClass.STATIC);
                    break;
                case "/":
                    this.left.temporary!.CopyToRegister(Asm.Register.xmm0);
                    this.right.temporary!.CopyToRegister(Asm.Register.xmm1);
                    Asm.Asm.Emit(new Asm.OpFDiv(Asm.Register.xmm0, Asm.Register.xmm1));
                    this.temporary!.CopyFromRegister(Asm.Register.xmm0, StorageClass.STATIC);
                    break;
                default:
                    throw new Exception();
            }
        } else {
            throw new NotImplementedException();
        }
    }
}
