public class AssignNode(Token tok, ExprNode left, ExprNode right) : BinaryOperator(tok, left, right) {
    public override void SetType() {
        this.type = VarType.Void;
    }
    public override void TypeCheck() {
        if (left.type! != right.type!) {
            Utils.Error($"Cannot assign {left.type} to {right.type} at line {token.line} column {token.column}");
        }
        if (left.type is ClassType) {
            if (((ClassType)left.type!).name != ((ClassType)right.type!).name) {
                Utils.Error($"Cannot assign {((ClassType)left.type!).name} to type {((ClassType)right.type!).name} at line {token.line} column {token.column}");
            }
        }
    }
    protected override Dictionary<VarType, VarType> TypeRules { get; } = [];
    public override void GenCode() {
        this.right.GenCode();
        Variable? V = this.left as Variable;
        if (V is null) {
            Utils.Error($"Trying to assign to non-variable at line {token.line} column {token.column}");
        }
        // get the address of our desitination variable to rax
        V!.CopyAddressToRegister(Asm.Register.rax);

        // move the value of the temporary into rbx
        this.right.temporary!.CopyToRegister(Asm.Register.rbx);

        // copy value of temporary into our destination variable
        Asm.Asm.Emit(new Asm.OpMoveRegRegInd(src: Asm.Register.rbx, dst: Asm.Register.rax, offset: 0));

        Console.Error.WriteLine("FixMe: Not handling storage class");
    }
}
