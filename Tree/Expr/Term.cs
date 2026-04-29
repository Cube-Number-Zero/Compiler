using Asm;

public class Term(Token term) : ExprNode(term) {
    protected override Dictionary<VarType, VarType> TypeRules => throw new NotImplementedException();

    public override List<TreeNode> GetChildren() {
        return [];
    }

    public override void SetType() {
        switch(token.sym) {
            case "NUM": type = VarType.Int; break;
            case "FNUM": type = VarType.Float; break;
            case "STRINGCONST": type = VarType.String; break;
            case "BOOLCONST": type = VarType.Bool; break;
            case "TYPE": type = VarType.Type; break;
            case "NOARGS": break;
            default: Utils.Error($"Unexpected token {token} at line {token.line} column {token.column}"); break;
        }
    }

    public override void TypeCheck() {} // no type checking needed

    public override void GenCode() {
        long v;
        switch (this.token.sym) {
            case "NUM":
                v = Int64.Parse(this.token.lexeme);
                Asm.Asm.Emit(new Comment($"Constant {this.token}"), new OpMoveConstReg(value: v, dst: Register.rax));
                this.temporary!.CopyFromRegister(Register.rax, StorageClass.STATIC);
                /*Asm.Asm.Emit(   new Comment($"copy register to temporary {this.temporary.number}"),
                                new OpMoveRegRegInd(
                                    src: Asm.Register.rax,
                                    offset: -(this.temporary.number + 1) * 8,
                                    dst: Register.rbp));*/
                return;
            case "FNUM":
                double fv = Double.Parse(this.token.lexeme);
                v = BitConverter.DoubleToInt64Bits(fv);
                Asm.Asm.Emit(new OpMoveConstReg(value: v, dst: Register.rax));
                this.temporary!.CopyFromRegister(Register.rax, StorageClass.STATIC);
                Asm.Asm.Emit(   new Comment($"copy register to temporary {this.temporary.number}"),
                                new OpMoveRegRegInd(
                                    src: Register.rax,
                                    offset: -(this.temporary.number + 1) * 8,
                                    dst: Register.rbp));
                return;
            default:
                throw new NotImplementedException();
        }
    }
}