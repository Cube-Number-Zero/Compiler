public class CommaNode(Token tok, ExprNode left, ExprNode right) : BinaryOperator(tok, left, right) {
    public override void SetType() {
        type = VarType.NotImplemented;
    }
    public override void TypeCheck() {} // no type checking needed
    protected override Dictionary<VarType, VarType> TypeRules { get; } = [];
}
