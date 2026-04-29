public abstract class StmtNode : TreeNode {
    public static StmtNode Parse(Tokenizer T) {
        if (ReturnNode.CanParse(T)) {
            return ReturnNode.Parse(T);
        }
        if (CondNode.CanParse(T)) {
            return CondNode.Parse(T);
        }
        if (LoopNode.CanParse(T)) {
            return LoopNode.Parse(T);
        }
        if (BreakNode.CanParse(T)) {
            return BreakNode.Parse(T);
        }
        if (VardeclNode.CanParse(T)) {
            VardeclNode decl = VardeclNode.Parse(T);
            SymbolTable.Declare(decl.token, decl.type, new LocalLocation(FuncdefNode.currentFunction!));
            return decl;
        }
        try {
            return ExprNode.Parse(T); // if nothing else, try an expression statement
        } catch (Exception) {
            Utils.Error($"Expecting statement at line {T.GetCurrentLine()}, column {T.GetCurrentColumn()} but did not get one. The next token is " + T.Peek());
            throw new Exception(); // bogus
        }
    }
}