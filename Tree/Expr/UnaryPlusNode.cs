public class UnaryPlusNode(Token tok, ExprNode operand) : UnaryOperator(tok, operand) {
    protected override Dictionary<VarType, VarType> TypeRules { get; } = new() {
        {VarType.Int, VarType.Int},
        {VarType.Float, VarType.Float}
    };
}
