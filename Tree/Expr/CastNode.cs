using Asm;

public class CastNode(Token tok, ExprNode left, ExprNode right) : BinaryOperator(tok, left, right) {
    public override void SetType() {
        type = VarType.FromToken(right.token);
    }
    public override void TypeCheck() {
        if (right.token.sym != "TYPE") {
            Utils.Error($"Expected type after 'as' at line {right.token.line} column {right.token.column}");
        }
        if (left.type! == type!) {
            // no-op
            return;
        }
        if (!PossibleCasts.ContainsKey(left.type!)) {
            Utils.Error($"Cannot cast {left.type} to {type} at line {left.token.line} column {left.token.column}");
        }
        if (!PossibleCasts[left.type!].Contains(type!)) {
            Utils.Error($"Cannot cast {left.type} to {type} at line {left.token.line} column {left.token.column}");
        }
    }
    readonly Dictionary<VarType, VarType[]> PossibleCasts = new() {
        {VarType.Int, [VarType.Float, VarType.String]},
        {VarType.Float, [VarType.Int, VarType.String]},
        {VarType.Bool, []},
        {VarType.String, [VarType.Int, VarType.Float]}
    };
    protected override Dictionary<VarType, VarType> TypeRules { get; } = [];

    public override void GenCode() {
        if (this.left.type! == VarType.Int && this.right.token.lexeme == "string")
        {
            // first arg to C function -> rcx
            // second arg to C function -> rdx
            // third arg to C function -> r8
            this.left.GenCode();
            //String* intToString(int64_t number, uint64_t* rsp, uint64_t* r15);
            this.left.temporary!.CopyToRegister(Asm.Register.rcx);
            Asm.Asm.Emit(
                new OpMoveRegReg(src: Register.rsp, dst: Register.rdx),
                new OpMoveRegReg(src: Register.rbp, dst: Register.r8),
                new OpSubRegConstant(Register.rsp, 32),
                new OpCall(new Label("intToString")),
                new OpAddConstReg(32, Register.rsp) // i think
            );
            // more?
        }
    }
}
