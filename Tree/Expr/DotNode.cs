public class DotNode : BinaryOperator {
    public DotNode(Token tok, ExprNode left, ExprNode right) : base(tok, left, new Member(right.token)) {
        if (right.token.sym != "ID" && right.token.sym != "FUNCCALL")
            Utils.Error($"Expected member name after dot at line {right.token.line} column {right.token.column}, but got {right.token.sym}");
    }
    public override void SetType() {
        ClassType? C = left.type as ClassType;
        if (C is null)
            Utils.Error($"Using dot on something that's not a class at line {token.line} column {token.column}");
        if (C!.declarer is null)
            Utils.Error($"Undeclared class {C} at line {token.line} column {token.column}");
        Member? member = right as Member;
        if (member is null)
            throw new Exception();
        member.type = C.GetTypeOfField(member.token);
        member.declaringClassType = C;
        this.type = member.type;
    }
    public override void TypeCheck() {
        if (left.type is not ClassType) {
            Utils.Error($"Cannot access member {right.token.lexeme} of non-class type {left.type} at line {token.line} column {token.column}");
        }
        //ClassType classType = (ClassType)left.type!;
    }
    protected override Dictionary<VarType, VarType> TypeRules { get; } = [];
}
