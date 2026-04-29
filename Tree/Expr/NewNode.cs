public class NewNode: UnaryOperator {
    public NewNode(Token tok, ExprNode operand) : base(tok, new TypeOfThingIAmCreating((operand as FuncNode)!.left.token)) {
        var F = operand as FuncNode;
        if (F is null)
            Utils.Error($"Expected class name after 'new' at line {operand.token.line} column {operand.token.column}");
        var className = F!.left.token;
        if (className.sym != "ID")
            Utils.Error($"Expected class name after 'new' at line {className.line} column {className.column}");
    }
    public override void SetType() {
        type = new ClassType((operand as TypeOfThingIAmCreating)!.token);
    }
    public override void TypeCheck() {
    }
    public class TypeOfThingIAmCreating(Token className) : ExprNode(className) {
        public override void SetType() {}
        public override void TypeCheck() {}
        public override List<TreeNode> GetChildren() {
            return [];
        }
        protected override Dictionary<VarType, VarType> TypeRules { get; } = [];
    }
    protected override Dictionary<VarType, VarType> TypeRules { get; } = [];

}
