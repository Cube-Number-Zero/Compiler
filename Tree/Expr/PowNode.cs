public class PowNode(Token tok, ExprNode left, ExprNode right) : BinaryOperator(tok, left, right) {
    protected override Dictionary<VarType, VarType> TypeRules { get; } = new() {
        {VarType.Int,   VarType.Int  },
        {VarType.Float, VarType.Float}
    };
}
