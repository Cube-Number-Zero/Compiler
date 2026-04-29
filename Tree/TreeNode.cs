public abstract class TreeNode {
    public TreeNode? parent = null;
    public abstract List<TreeNode> GetChildren();
    public abstract void TypeCheck();
    public virtual void GenCode() {
        foreach(var n in this.GetChildren()) {
            n.GenCode();
        }
    }
    public CFGNode entry;
    public CFGNode exit;
    public TreeNode() {
        this.entry = new("entry", this);
        this.exit = new("exit", this);
    }
    public abstract void SetupCFG();
}