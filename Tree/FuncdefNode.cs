public class FuncdefNode : TreeNode {
    
    public VarType returnType;
    public Token name;
    public StmtsNode stmts;
    public Asm.Label lbl = new();
    public int maxTemporaries = 0;
    public int numLocals = 0;
    
    public static FuncdefNode? currentFunction = null;

    public FuncdefNode(VarType returnType, Token name, StmtsNode stmts) {
        this.returnType = returnType;
        this.name = name;
        this.stmts = stmts;
    }
    private FuncdefNode(Token name) {
        this.returnType = null!;
        this.name = name;
        this.stmts = null!;
    }
    public static FuncdefNode Parse(Tokenizer T, VarLocation location, string ClassName = "") {
        _ = T.Expect("FUNC");
        Token name = T.Expect("ID");
        LocalLocation.ResetCounter();
        ParameterLocation.ResetCounter();
        if (ClassName != "") {
            name.lexeme = ClassName + "." + name.lexeme;
        }
        if (SymbolTable.LookupIfExists(name.lexeme) != null) {
            Utils.Error($"Function {name.lexeme} already declared at line {T.GetCurrentLine()} column {T.GetCurrentColumn()}");
        }
        FuncdefNode f = new(name);
        currentFunction = f;
        SymbolTable.AddScope();
        _ = T.Expect("LPAREN");
        FuncType F = new(f);
        if (T.Peek() != "RPAREN") {
            while (true) {
                Token paramName = T.Expect("ID");
                _ = T.Expect("COLON");
                Token paramType;
                if (T.Peek() == "ID") {
                    paramType = T.Expect("ID");
                } else {
                    paramType = T.Expect("TYPE");
                }
                SymbolTable.Declare(paramName, VarType.FromToken(paramType), new ParameterLocation(f));
                F.parameters.Add(new FuncType.Parameter(paramName.lexeme, VarType.FromToken(paramType)));
                if (T.Peek() == "COMMA") {
                    _ = T.Expect("COMMA");
                } else {
                    break;
                }
            }
        }
        _ = T.Expect("RPAREN");
        if (T.Peek() == "COLON") {
            _ = T.Expect("COLON");
            var tmp = T.Next();
            f.returnType = F.returnType = VarType.FromToken(tmp);
        } else {
            f.returnType = F.returnType = new VoidType();
        }
        f.stmts = StmtsNode.Parse(T);
        SymbolTable.RemoveScope();
        SymbolTable.DeclareFunc(name, F, location);
        currentFunction = null;
        return f;
    }
    public static bool CanParse(Tokenizer T) {
        return T.Peek() == "FUNC";
    }
    public override List<TreeNode> GetChildren() {
        List<TreeNode> L = [stmts];
        return L;
    }

    public override void TypeCheck() {} // no type checking needed

    public override void GenCode()
    {
        Asm.Asm.Emit(
            new Asm.Comment(this.name.lexeme),
            this.lbl,
            new Asm.Comment("function prologue"),
            new Asm.OpPushReg(Asm.Register.rbp),
            new Asm.OpMoveRegReg(Asm.Register.rsp, Asm.Register.rbp),
            new Asm.Comment($"Allocate space for {this.maxTemporaries} temporaries"),
            new Asm.OpSubRegConstant(Asm.Register.rsp, 16 * (this.maxTemporaries + this.numLocals))
        );
        this.stmts.GenCode();
        Asm.Asm.Emit(
            new Asm.Comment("return fall off end"),
            new Asm.OpMoveRegReg(src: Asm.Register.rbp, dst: Asm.Register.rsp),
            new Asm.OpPopReg(Asm.Register.rbp),
            new Asm.OpRet()
        );
        Asm.Asm.Emit(new Asm.Comment($"End of {this.name.lexeme}"));
    }
    public override void SetupCFG() {
        throw new NotImplementedException();
    }
}