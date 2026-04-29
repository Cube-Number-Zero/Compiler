using Asm;

public class Variable : Term, ResultLocation, LValue {

    public Variable(Token token): base(token) {
        FigureOutVarInfo();
    }
    public VarInfo? info;

    public override void SetType(){
        if (token.sym == "THIS") {
            TreeNode t = this;
            while (t is not ClassDeclNode) {
                t = t.parent!;
            }
            this.type = (t as ClassDeclNode)!.classType;
            return;
        }
        this.type = info!.type; // type in ExprNode
    }

    public void FigureOutVarInfo() {
        if (this.info == null) {
            string varname = this.token.lexeme;
            this.info = SymbolTable.LookupIfExists(varname);
        }
    }
    public void MoveFromRegister(Asm.IntRegister reg, StorageClass cls)
    {
        //we should never try to do this (bug!)
        throw new Exception();
    }
    public void MoveToRegister(Asm.IntRegister reg)
    {
        this.info!.location.CopyAddressToRegister(reg);
        Asm.Asm.Emit(new Asm.OpMoveRegIndReg(offset:0, src: reg, dst: reg));
    }
    public override void GenCode() {
        // do nothing, handled in GenCode of parent node
    }

    public void CopyAddressToRegister(IntRegister reg) {
        this.info!.location.CopyAddressToRegister(reg);
    }
}
