using System.Security.Cryptography.X509Certificates;
using Asm;

public abstract class ExprNode(Token tok) : StmtNode {

    public Temporary? temporary;
    public Token token = tok;
    public readonly string unique = $"n{ctr++}";
    private static int ctr = 0;

    class OpInfo(int precedence, Func<Token, ExprNode, ExprNode?, ExprNode> maker, OpInfo.Associativity assoc, OpInfo.Operands operands)
    {
        public readonly int precedence = precedence;
        // this represents a function that takes three arguments
        //(token, exprnode, exprnode), and returns an exprnode
        public readonly Func<Token, ExprNode, ExprNode?, ExprNode> maker = maker;
        
        public enum Associativity {
            LEFT,
            NONE,
            RIGHT
        }
        public enum Operands {
            UNARY,
            BINARY,
            SPECIAL
        }
        public readonly Associativity assoc = assoc;
        public readonly Operands operands = operands;
    }
    static readonly Dictionary<string, OpInfo> operators = new()
    {
        {"LPAREN",   new(1400, (Token tok, ExprNode left,    ExprNode? right) => {return new FuncNode(     tok, left, right!);}, OpInfo.Associativity.NONE,  OpInfo.Operands.SPECIAL)},
        {"LBRACKET", new(1400, (Token tok, ExprNode left,    ExprNode? right) => {return new ArrayNode(    tok, left, right!);}, OpInfo.Associativity.NONE,  OpInfo.Operands.SPECIAL)},
        {"DOTOP",    new(1300, (Token tok, ExprNode left,    ExprNode? right) => {return new DotNode(      tok, left, right!);}, OpInfo.Associativity.LEFT,  OpInfo.Operands.BINARY )},
        {"NEW",      new(1200, (Token tok, ExprNode operand, ExprNode? dummy) => {return new NewNode(      tok, operand     );}, OpInfo.Associativity.RIGHT, OpInfo.Operands.UNARY  )},
        {"CAST",     new(1100, (Token tok, ExprNode left,    ExprNode? right) => {return new CastNode(     tok, left, right!);}, OpInfo.Associativity.LEFT,  OpInfo.Operands.BINARY )},
        {"POWOP",    new(1000, (Token tok, ExprNode left,    ExprNode? right) => {return new PowNode(      tok, left, right!);}, OpInfo.Associativity.RIGHT, OpInfo.Operands.BINARY )},
        {"NEGATE",   new( 900, (Token tok, ExprNode operand, ExprNode? dummy) => {return new NegateNode(   tok, operand     );}, OpInfo.Associativity.RIGHT, OpInfo.Operands.UNARY  )},
        {"UNARYPLUS",new( 900, (Token tok, ExprNode operand, ExprNode? dummy) => {return new UnaryPlusNode(tok, operand     );}, OpInfo.Associativity.RIGHT, OpInfo.Operands.UNARY  )},
        {"BITNOT",   new( 900, (Token tok, ExprNode operand, ExprNode? dummy) => {return new BitNotNode(   tok, operand     );}, OpInfo.Associativity.RIGHT, OpInfo.Operands.UNARY  )},
        {"BOOLNOT",  new( 900, (Token tok, ExprNode operand, ExprNode? dummy) => {return new BoolNotNode(  tok, operand     );}, OpInfo.Associativity.RIGHT, OpInfo.Operands.UNARY  )},
        {"MULOP",    new( 800, (Token tok, ExprNode left,    ExprNode? right) => {return new MulNode(      tok, left, right!);}, OpInfo.Associativity.LEFT,  OpInfo.Operands.BINARY )},
        {"ADDOP",    new( 700, (Token tok, ExprNode left,    ExprNode? right) => {return new AddNode(      tok, left, right!);}, OpInfo.Associativity.LEFT,  OpInfo.Operands.BINARY)},
        {"SUBOP",    new( 700, (Token tok, ExprNode left,    ExprNode? right) => {return new SubNode(      tok, left, right!);}, OpInfo.Associativity.LEFT,  OpInfo.Operands.BINARY)},
        {"SHIFTOP",  new( 600, (Token tok, ExprNode left,    ExprNode? right) => {return new ShiftNode(    tok, left, right!);}, OpInfo.Associativity.LEFT,  OpInfo.Operands.BINARY )},
        {"RELOP",    new( 500, (Token tok, ExprNode left,    ExprNode? right) => {return new RelNode(      tok, left, right!);}, OpInfo.Associativity.NONE,  OpInfo.Operands.BINARY )},
        {"EQOP",     new( 500, (Token tok, ExprNode left,    ExprNode? right) => {return new EqNode(       tok, left, right!);}, OpInfo.Associativity.NONE,  OpInfo.Operands.BINARY )},
        {"BITOP",    new( 400, (Token tok, ExprNode left,    ExprNode? right) => {return new BitNode(      tok, left, right!);}, OpInfo.Associativity.LEFT,  OpInfo.Operands.BINARY )},
        {"BOOLOP",   new( 300, (Token tok, ExprNode left,    ExprNode? right) => {return new BoolNode(     tok, left, right!);}, OpInfo.Associativity.LEFT,  OpInfo.Operands.BINARY )},
        {"EQ",       new( 200, (Token tok, ExprNode left,    ExprNode? right) => {return new AssignNode(   tok, left, right!);}, OpInfo.Associativity.RIGHT, OpInfo.Operands.BINARY )},
        {"COMMA",    new( 100, (Token tok, ExprNode left,    ExprNode? right) => {return new CommaNode(    tok, left, right!);}, OpInfo.Associativity.LEFT,  OpInfo.Operands.BINARY )},
    };

    protected abstract Dictionary<VarType, VarType> TypeRules { get; }
    public VarType? type = null;
    public abstract void SetType();
    static bool isOperator(string sym) {
        return operators.ContainsKey(sym);
    }
    public static new ExprNode Parse(Tokenizer T){
        List<Token> tokens = T.ReadUntil("LBRACE", "$$", "$");
        if (tokens.Count == 0) {
            Utils.Error($"Expected expression at line {T.GetCurrentLine()}");
        }
        Stack<Token> operatorStack = new();
        Stack<ExprNode> termStack = new();
        bool most_recent_was_operator = true;
        foreach (Token t in tokens) {
            if (t.sym == "ID" || t.sym == "THIS") {
                if (!most_recent_was_operator)
                    Utils.Error($"Missing operator at line {t.line} column {t.column}");
                termStack.Push(new Variable(t));
                most_recent_was_operator = false;
                if (operatorStack.Count > 0) {
                    if (operatorStack.Peek().sym == "DOTOP") {
                        ApplyOperator(operatorStack, termStack);
                    }
                }
            } else if (t.sym == "NUM" || t.sym == "FNUM" || t.sym == "STRINGCONST" || t.sym == "BOOLCONST" || t.sym == "TYPE") {
                if (!most_recent_was_operator)
                    Utils.Error($"Missing operator at line {t.line} column {t.column}");
                termStack.Push(new Term(t));
                most_recent_was_operator = false;
            } else if (t.sym == "LPAREN") {
                if (most_recent_was_operator) {
                    operatorStack.Push(t);
                } else {
                    operatorStack.Push(new Token("LPAREN", t.line, t.column, "func-call"));
                    most_recent_was_operator = true;
                }
            } else if (t.sym == "RPAREN") {
                while (operatorStack.Count > 0 && operatorStack.Peek().sym != "LPAREN") {
                    ApplyOperator(operatorStack, termStack);
                    most_recent_was_operator = false;
                }
                if (most_recent_was_operator) {
                    termStack.Push(new Term(new Token("NOARGS", t.line, t.column, "no-args")));
                }
                if (operatorStack.Count == 0) {
                    Utils.Error($"Mismatched parentheses at line {t.line} column {t.column}");
                }
                if (operatorStack.Peek().lexeme == "func-call") {
                    ApplyFunc(operatorStack, termStack);
                } else {
                    ApplyParen(operatorStack, termStack);
                }
                most_recent_was_operator = false;
            } else if (t.sym == "LBRACKET") {
                operatorStack.Push(t);
            } else if (t.sym == "RBRACKET") {
                while (operatorStack.Count > 0 && operatorStack.Peek().sym != "LBRACKET") {
                    ApplyOperator(operatorStack, termStack);
                }
                if (operatorStack.Count == 0) {
                    Utils.Error($"Mismatched brackets at line {t.line} column {t.column}");
                }
                ApplyBracket(operatorStack, termStack);
            } else if (isOperator(t.sym)) {
                if (most_recent_was_operator) {
                    // unary operator
                    if (t.sym == "ADDOP")
                        operatorStack.Push(new Token("UNARYPLUS", t.line, t.column, "+"));
                    else if (t.sym == "SUBOP")
                        operatorStack.Push(new Token("NEGATE", t.line, t.column, "-"));
                    else if (operators[t.sym].operands != OpInfo.Operands.UNARY)
                        Utils.Error($"Unexpected operator {t.sym} at line {t.line} column {t.column}");
                    else
                        operatorStack.Push(t);
                    continue;
                }
                while (operatorStack.Count > 0 && operators[operatorStack.Peek().sym].precedence >= operators[t.sym].precedence && operatorStack.Peek().sym != "LPAREN" && operatorStack.Peek().sym != "LBRACKET") {
                    if (operators[operatorStack.Peek().sym].precedence == operators[t.sym].precedence) {
                        if (operators[t.sym].assoc == OpInfo.Associativity.RIGHT) {
                            break;
                        } else if (operators[t.sym].assoc == OpInfo.Associativity.NONE) {
                            Utils.Error($"{t.sym} is non-associative, but was used in a chain at line {t.line} column {t.column}");
                        }
                    }
                    ApplyOperator(operatorStack, termStack);
                }
                operatorStack.Push(t);
                most_recent_was_operator = true;
            } else {
                Utils.Error($"Bad thing in expression: {t}");
            }
        }
        while (operatorStack.Count > 0) {
            ApplyOperator(operatorStack, termStack);
        }
        if (termStack.Count != 1) {
            Utils.Error($"Bad expression {tokens[^1]}");
        }
        return termStack.Pop();
    }
    public static void ApplyOperator(Stack<Token> operatorStack, Stack<ExprNode> termStack) {
        Token op = operatorStack.Pop();
        if (operators[op.sym].operands == OpInfo.Operands.UNARY) {
            ApplyOperatorUnary(op, termStack);
            return;
        }
        ExprNode right = termStack.Pop();
        if (termStack.Count == 0) {
            Utils.Error($"{op.sym} at line {op.line} column {op.column} is incomplete");
        }
        ExprNode left = termStack.Pop();
        ExprNode newNode = operators[op.sym].maker(op, left, right);
        termStack.Push(newNode);
    }
    public static void ApplyOperatorUnary(Token op, Stack<ExprNode> termStack) {
        ExprNode operand = termStack.Pop();
        if (op.sym == "ADDOP") {
            op = new Token("UNARYPLUS", op.line, op.column, "+");
        } else if (op.sym == "SUBOP") {
            op = new Token("NEGATE", op.line, op.column, "-");
        }
        ExprNode newNode = operators[op.sym].maker(op, operand, null);
        termStack.Push(newNode);
    }
    public static void ApplyFunc(Stack<Token> operatorStack, Stack<ExprNode> termStack) {
        Token op = operatorStack.Pop();
        ExprNode right = termStack.Pop();
        ExprNode left = termStack.Pop();
        ExprNode newNode = new FuncNode(new("FUNCCALL", op.line, op.column, "func-call"), left, right);
        termStack.Push(newNode);
    }
    public static void ApplyParen(Stack<Token> operatorStack, Stack<ExprNode> termStack) {
        _ = operatorStack.Pop();
        termStack.Push(termStack.Pop());
    }
    public static void ApplyBracket(Stack<Token> operatorStack, Stack<ExprNode> termStack) {
        Token op = operatorStack.Pop();
        ExprNode right = termStack.Pop();
        if (termStack.Count == 0) {
            Utils.Error($"Empty array access at line {op.line} column {op.column}");
        }
        ExprNode left = termStack.Pop();
        ExprNode newNode = new ArrayNode(new("ARRAY", op.line, op.column, "array-access"), left, right);
        termStack.Push(newNode);
    }
    public override void SetupCFG() {
        throw new NotImplementedException();
    }
}
public abstract class BinaryOperator(Token tok, ExprNode left, ExprNode right) : ExprNode(tok) {
    public readonly ExprNode left = left;
    public readonly ExprNode right = right;
    public override void SetType() {
        if (!TypeRules.TryGetValue(left.type!, out VarType? value)) {
            Utils.Error($"Cannot apply operator {token.sym} to {left.type} at line {token.line} column {token.column}");
        } else if (left.type! != right.type!) {
            Utils.Error($"Cannot apply operator {token.sym} to {left.type} and {right.type} at line {token.line} column {token.column}");
        } else {
            type = value;
        }
    }
    public override void TypeCheck() {
        if (!TypeRules.ContainsKey(left.type!) || left.type! != right.type!) {
            Utils.Error($"Cannot apply operator {token.sym} to {left.type} and {right.type} at line {token.line} column {token.column}");
        }
    }

    public override List<TreeNode> GetChildren() {
        return [left, right];
    }
    public override void SetupCFG() {
        this.entry.AddNext(this.left.entry);
        this.left.exit.AddNext(this.right.entry);
        this.right.exit.AddNext(this.exit);
    }
}

public abstract class UnaryOperator(Token tok, ExprNode operand) : ExprNode(tok) {
    public readonly ExprNode operand = operand;

    public override void SetType() {
        if (TypeRules.ContainsKey(operand.type!)) {
            type = TypeRules[operand.type!];
        } else {
            Utils.Error($"Cannot apply operator {token.sym} to {operand.type} at line {token.line} column {token.column}");
        }
    }
    public override void TypeCheck() {
        if (!TypeRules.ContainsKey(operand.type!)) {
            Utils.Error($"Cannot apply operator {token.sym} to {operand.type} at line {token.line} column {token.column}");
        }
    }
    public override List<TreeNode> GetChildren() {
        return [operand];
    }
    public override void SetupCFG() {
        this.entry.AddNext(this.operand.entry);
        this.operand.exit.AddNext(this.exit);
    }
}
