public class SymbolTable {
    // The current symbol table.
    // Initialize with a global symbol table
    private static SymbolTable current = new SymbolTable(null);

    // linked list
    private readonly SymbolTable? prev;

    private readonly Dictionary<string, VarInfo> decls = [];

    // Create new table and link to previous one
    private SymbolTable(SymbolTable? prev) {
        this.prev = prev;
    }

    private static void DeclareBuiltin(string name, VarType rtype, List<VarType> argTypes) {
        Token t = new("ID", -1, -1, name);
        //FuncType wants a link to the FuncdefNode
        FuncdefNode FN = new(rtype, t, new StmtsNode());
        FuncType F = new(FN) {
            builtin = true
        };
        //Declare(t, F, new GlobalLocation("_"+name));
        Declare(t, F, new GlobalLocation());
    }

    public static void PopulateBuiltins() {
        DeclareBuiltin("putc", VarType.Bool, [VarType.Int]);
        DeclareBuiltin("newline", VarType.Void, []);
        DeclareBuiltin("putv", VarType.Bool, [VarType.Int, VarType.Int]);
        DeclareBuiltin("getc", VarType.Int, []);
    }

    public static void AddScope() {
        SymbolTable newScope = new SymbolTable(current);
        current = newScope;
    }

    public static void RemoveScope() {
        current = current.prev!;
    }

    // These check all symbol tables for the given name

    // Lookup a variable.
    // Raise error if variable does not exist.
    // 'line' is for error reporting only.
    public static VarInfo Lookup(string name, int line) {
        SymbolTable? s = current;
        do {
            if (s.decls.TryGetValue(name, out VarInfo? value)) {
                return value;
            }
            s = s.prev;
        } while (s != null);
        Utils.Error($"Undeclared variable {name} at line {line}");
        throw new Exception(); // bogus
    }

    // Lookup a variable; if it doesn't exist, return null
    public static VarInfo? LookupIfExists(string name) {
        SymbolTable? s = current;
        do {
            if (s.decls.TryGetValue(name, out VarInfo? value)) {
                return value;
            }
            s = s.prev;
        } while (s != null);
        return null;
    }

    public static void Declare(Token id, VarType type, VarLocation location) {
        // see if it's defined
        if (current.decls.TryGetValue(id.lexeme, out VarInfo? value)) {
            Utils.Error($"Variable {id.lexeme} already declared at line {id.line} column {id.column}");
        }
        // add decls
        current.decls[id.lexeme] = new VarInfo(id, type, location);
    }

    public static void DeclareFunc(Token id, FuncType type, VarLocation location) {
        if (current.decls.TryGetValue(id.lexeme.Split('.').Last(), out VarInfo? v)) {
            Utils.Error($"Variable {id.lexeme} already declared at line {id.line} column {id.column}");
        }
        SymbolTable s = current;
        while (s.prev != null) {
            s = s.prev;
        }
        if (s.decls.TryGetValue(id.lexeme, out VarInfo? value)) {
            Utils.Error($"Function {id.lexeme} already declared at line {id.line} column {id.column}");
        }
        // add decls
        s.decls[id.lexeme] = new VarInfo(id, type, location);
    }

    public static FuncType GetFunc(string name, int line) {
        SymbolTable s = current;
        while (s.prev != null) {
            s = s.prev;
        }
        VarInfo? info = s.decls[name];
        if (info == null)
            Utils.Error($"Nonexistent function {name}() at line {line}");
        if (info!.type is FuncType f) {
            return f;
        } else {
            Utils.Error($"Variable {name} at line {line} is not a function");
            throw new Exception(); // bogus
        }
    }
}

public class VarInfo(Token token, VarType type, VarLocation location)
{
    public readonly Token token = token; // for debugging/error reporting
    public readonly VarType type = type;
    public readonly VarLocation location = location;
}
