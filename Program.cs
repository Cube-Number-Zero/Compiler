using System.Text.Encodings.Web;
using System.Text.Unicode;
using lab;
COMPILER="bin/Debug/net10.0/lab.exe"

public class StopIteration : Exception {}
public class Program {
    static void WalkPreOrder(TreeNode n, Action<TreeNode> callback) {
        try {
            WalkPreOrderHelper(n, callback);
        } catch(StopIteration) {}
    }
    static void WalkPreOrderHelper(TreeNode n, Action<TreeNode> callback)
    {
        callback(n);
        foreach (TreeNode c in n.GetChildren()) {
            WalkPreOrderHelper(c, callback);
        }
    }
    static void WalkPostorder(TreeNode n, Action<TreeNode> callback) {
        try {
            WalkPostorderHelper(n, callback);
        } catch(StopIteration) {}
    }

    static void WalkPostorderHelper(TreeNode n, Action<TreeNode> callback) {
        foreach (TreeNode c in n.GetChildren()) {
            if (c == null) {
                throw new Exception($"Child of {n} is null");
            }
            WalkPostorderHelper(c, callback);
        }
        callback(n);
    }
    static void WalkPreAndPost(TreeNode n, Action<TreeNode> precallback, Action<TreeNode> postcallback) {
        try {
            WalkPreAndPostHelper(n, precallback, postcallback);
        } catch(StopIteration) {}
    }

    static void WalkPreAndPostHelper(TreeNode n, Action<TreeNode> precallback, Action<TreeNode> postcallback) {
        precallback(n);
        foreach (TreeNode c in n.GetChildren()) {
            if (c == null) {
                throw new Exception($"Child of {n} is null");
            }
            WalkPreAndPostHelper(c, precallback, postcallback);
        }
        postcallback(n);
    }

    static void MakeJSON(ExprNode e, StreamWriter w) {
        w.Write("{");
        w.Write($"\"token\" : \"{e.token.lexeme}\",\"children\" : [");
        List<TreeNode> children = e.GetChildren();
        for(int i = 0; i < children.Count; ++i) {
            MakeJSON((ExprNode)children[i], w);
            if (i != children.Count - 1) {
                w.Write(",");
            }
        }
        w.Write("]}");
    }

    static void FallOffEndCheck(CFGNode n, CFGNode funcExit, HashSet<CFGNode> visited) {
        visited.Add(n);
        if (n == funcExit) {
            Utils.Error("Did not use return in non-void function");
        }
        if (n.owner as ReturnNode != null) {
            return;
        }
        foreach(var next in n.next) {
            if (visited.Contains(next)) {
                visited.Add(next);
                FallOffEndCheck(next, funcExit, visited);
            }
        }
    }
    public static void Main(string[] args)
    {
        SymbolTable.PopulateBuiltins();
        var T = new Tokenizer();
        // using(StreamReader r = new StreamReader("../../../tests/inputs/float-26.txt")) {
        //     T.SetInput(r.ReadToEnd());
        // }
        using(StreamReader r = new StreamReader(args[0])) {
            T.SetInput(r.ReadToEnd());
        }

        // skip json at beginning
        if (T.Peek() == "LBRACE") {
            _ = T.Expect("LBRACE");
            while (T.Peek() != "RBRACE") {
                _ = T.Next();
            }
            _ = T.Expect("RBRACE");
        }



        // Phase 1: Make tree
        ProgramNode p = ProgramNode.Parse(T);

        // Phase 2: Hoist
        WalkPreOrder(p, (n) => {
            // if this node is a variable node and
            // its VarInfo is null, look up in the
            // global system table
            Variable? v = n as Variable;
            if (v == null) {
                return;
            }
            if (v.info != null) {
                return;
            }
        
            if (SymbolTable.LookupIfExists(v.token.lexeme) == null && v.token.lexeme != "this") {
                Utils.Error($"Variable {v.token.lexeme} not declared at line {v.token.line}, column {v.token.column}");
            }

            v.FigureOutVarInfo();
        });

        // set parents
        WalkPreOrder(p, (TreeNode n) => {
            foreach (TreeNode c in n.GetChildren())
                c.parent = n;
        });

        // type checking
        WalkPostorder(p, (n) => {
            (n as ExprNode)?.SetType();
        });
        WalkPostorder(p, (n) => {
            n.TypeCheck();
        });

        // check return types
        WalkPreOrder(p, (n) => {
            ReturnNode? e = n as ReturnNode;
            if (e == null)
                return;
            
            VarType type;
            if (e is ReturnNode.ReturnVoidNode) {
                type = VarType.Void;
            } else {
                type = (e as ReturnNode.ReturnExprNode)!.expr.type!;
            }

            TreeNode? f = e.parent;
            while (f != null && f is not FuncdefNode) {
                f = f.parent;
            }
            if (f == null) {
                Utils.Error($"Return statement at line {e.RETURN.line} column {e.RETURN.column} is not inside a function");
            }
            if ((f as FuncdefNode)!.returnType != type) {
                Utils.Error($"Return type mismatch at line {e.RETURN.line} column {e.RETURN.column}: function {((f as FuncdefNode)!.name.lexeme)} returns {((f as FuncdefNode)!.returnType)} but return statement returns {type}");
            }
        });


        ExprNode? topLevel = null;
        int numTemporaries = 0;
        FuncdefNode? currentFunction = null;
        WalkPreAndPost(p,
            (n) => {
                if (n is FuncdefNode f)
                    currentFunction = f;
                if (n is not ExprNode e)
                    return;
                if (e is Variable v)
                    return; // variables don't get temporaries
                if (topLevel == null) {
                    topLevel = e;
                    numTemporaries = 0;
                }
                e.temporary = new Temporary(numTemporaries);
                numTemporaries++;
            }, (n) => {
                if (n == topLevel) {
                    currentFunction!.maxTemporaries = Math.Max(numTemporaries, currentFunction!.maxTemporaries);
                    topLevel = null;
                }
            }
        );

        /*WalkPostorder(p, (n) => {
            n.SetupCFG();
        });*/

        // done with tree!

        p.GenCode();
        Asm.Label? mainLabel = null;
        foreach(var n in p.GetChildren()) {
            if (n is FuncdefNode f) {
                if (f.name.lexeme == "main") {
                    mainLabel = f.lbl;
                }
            }
        }
        // get mainlabel
        using(var w = new StreamWriter("out.asm")){
            Asm.Asm.Write(w, mainLabel!);
        }
        lab.Run.compile("out.asm");     
        
        Environment.Exit(0);
    }
}