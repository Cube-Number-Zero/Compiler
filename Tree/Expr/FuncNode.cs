using Asm;

public class FuncNode(Token tok, ExprNode left, ExprNode right) : BinaryOperator(tok, left, right) {
    public override void SetType() {
        string name = left.token.lexeme;
        if (left is DotNode) {
            name = ((ClassType)((DotNode)left).left.type!).name.lexeme + "." + ((DotNode)left).right.token.lexeme;
        }
        FuncType funcType = SymbolTable.GetFunc(name, left.token.line);
        type = funcType.returnType;
    }
    public override void TypeCheck() {
        if (left is DotNode) {
            FuncType? F = left.type as FuncType;
            if (F is null) {
                Utils.Error($"Trying to call a non-function {left.token.lexeme} at line {left.token.line} column {left.token.column}");
            }
        }
        string name = left.token.lexeme;
        if (left is DotNode) {
            name = ((ClassType)((DotNode)left).left.type!).name.lexeme + "." + ((DotNode)left).right.token.lexeme;
        }
        
        FuncType funcType = SymbolTable.GetFunc(name, left.token.line);
        List<VarType> paramTypes = [];
        if (right is CommaNode) {
            ExprNode r = right;
            while (r is CommaNode c) {
                paramTypes.Add(c.right.type!);
                r = c.left;
            }
            paramTypes.Add(r.type!);
        } else if (right is Term && right.token.sym != "NOARGS") {
            paramTypes = [right.type!];
        }
        if (paramTypes.Count != funcType.parameters.Count) {
            Utils.Error($"Function {name}() expects {funcType.parameters.Count} arguments but got {paramTypes.Count} arguments (line {left.token.line} column {left.token.column})");
        }
        for (int i = 0; i < paramTypes.Count; i++) {
            if (paramTypes[paramTypes.Count - 1 - i] != funcType.parameters[i].type) {
                Utils.Error($"Function {name}() expects argument {i + 1} to be of type {funcType.parameters[i].type} but got type {paramTypes[i]} (line {left.token.line} column {left.token.column})");
            }
        }
    }
    protected override Dictionary<VarType, VarType> TypeRules { get; } = [];
    public override void GenCode() {
        if (this.right.token.sym != "NOARGS") {
            ExprNode n = this.left;
            while(n as CommaNode is not null) {
                // push right child's value to stack
                (n as CommaNode)!.right.GenCode();
                (n as CommaNode)!.right.temporary!.CopyToRegister(Asm.Register.rax);
                Asm.Asm.Emit(new Asm.OpPushReg(Asm.Register.rbx)); // storage class
                Asm.Asm.Emit(new Asm.OpPushReg(Asm.Register.rax)); // value
                n = (n as CommaNode)!.left;
            }
            throw new NotImplementedException("FINISH THIS");
            // push n's value to stack
        }
        Variable? fname = this.left as Variable;
        if (fname is null) {
            Utils.Error($"This should not happen yet");
        }
        FuncType? ftype = fname!.info!.type as FuncType;
        if (ftype is null) {
            Utils.Error($"Trying to call a non-function {fname.token.lexeme} at line {fname.token.line} column {fname.token.column}");
        }

        if (ftype!.builtin) {
            Asm.Asm.Emit(   new OpMoveRegReg(src: Register.rsp, dst: Register.rcx),
                            new OpSubRegConstant(Register.rsp, 32) // shadow space
            );
        }
        Asm.Asm.Emit(new OpCall(ftype!.declarer!.lbl));
        if (ftype.returnType == VarType.Void) {
            // nothing to do
        }
        else if (ftype.returnType == VarType.Int || ftype.returnType == VarType.Bool || ftype.returnType == VarType.String) {
            if (ftype.builtin)
                this.temporary!.CopyFromRegister(Register.rax, StorageClass.STATIC);
            else
            this.temporary!.CopyFromRegister(Register.rax, Register.rbx);
        } else if (ftype.returnType == VarType.Float) {
            //this.temporary!.CopyFromRegister();
        }
        if (ftype.builtin) {
            Asm.Asm.Emit(new OpAddConstReg(32, Register.rsp)); // clean up shadow space
        }
        if (ftype.parameters.Count > 0) {
            Asm.Asm.Emit(new OpAddConstReg(16 * ftype.parameters.Count, Register.rsp));
        }
    }
}
