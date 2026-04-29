public class VarType {
    public static VarType FromToken(Token t) {
        if (t.sym == "ID") // class
            return ProgramNode.GetClassTypeFromClassName(t);
        switch(t.lexeme){
            case "int": return new IntType();
            case "float": return new FloatType();
            case "string": return new StringType();
            case "bool": return new BoolType();
            default:
                Utils.Error($"Expected variable type at line {t.line} column {t.column} but got {t}");
                throw new Exception();  //dummy
        }
    }


    public override bool Equals(Object? other) {
        if (other == null) return false;
        return this.GetType() == other.GetType();
    }

    public static bool operator== (VarType v1, VarType v2) {
        if (Object.ReferenceEquals(v1, null)) return Object.ReferenceEquals(v2, null);
        return v1.Equals(v2);
    }
    public static bool operator!= (VarType v1, VarType v2) {
        if (Object.ReferenceEquals(v1, null)) return !Object.ReferenceEquals(v2, null);
        return !v1.Equals(v2);
    }
    public override int GetHashCode() {
        return this.GetType().GetHashCode();
    }

    public static readonly IntType Int = new();
    public static readonly FloatType Float = new();
    public static readonly BoolType Bool = new();
    public static readonly StringType String = new();
    public static readonly VoidType Void = new();
    public static readonly TypeType Type = new();
    public static readonly NotImplementedType NotImplemented = new();
}

public class IntType : VarType {
}
public class FloatType : VarType {
}
public class BoolType : VarType {
}
public class StringType : VarType {
}
public class VoidType : VarType {
}
public class FuncType(FuncdefNode declarer) : VarType {
    public VarType returnType = Void;
    public class Parameter(string name, VarType type) {
        public readonly string name = name;
        public readonly VarType type = type;
    }
    public List<Parameter> parameters = [];
    public FuncdefNode declarer = declarer;
    public bool builtin = false;
}
public class TypeType : VarType {
}
public class ClassType(Token name) : VarType {
    public ClassDeclNode? declarer = null;
    public Token name = name;

    public class Member(string name, VarType type) {
        public readonly string name = name;
        public readonly VarType type = type;
    }
    public VarType GetTypeOfField(Token fieldName) {
        for (int i = 0; i < members.Count; i++) {
            if (members[i].name.Split('.').Last() == fieldName.lexeme) {
                return members[i].type;
            }
        }
        Utils.Error($"Class {name.lexeme} has no member {fieldName.lexeme} at line {fieldName.line} column {fieldName.column}");
        throw new Exception(); // bogus
    }
    public List<Member> members = [];
}
public class NotImplementedType : VarType{
}