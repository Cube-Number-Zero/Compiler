public class CFGNode{
    //nodes which have edges leading into this one
    public List<CFGNode> prev = [];
    //outgoing edges to other nodes
    public List<CFGNode> next = [];
    //TreeNode that is associated with this graph node
    public TreeNode owner;
    //for debugging
    public string name;
    //make CFG node with outgoing edges to each thing in next_
    public CFGNode(string name, TreeNode owner, params CFGNode[] next_){
        this.name = name;
        this.owner = owner;
        foreach(var n in next_){
            AddNext(n);
        }
    }
    //add outgoing edge
    public void AddNext(CFGNode n){
        this.next.Add(n);
        n.prev.Add(this);
    }
    //convenience function
    public void AddNext(TreeNode n){
        this.next.Add(n.entry);
        n.entry.prev.Add(this);
    }
}