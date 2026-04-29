public abstract class VarLocation {
    public abstract void CopyAddressToRegister(Asm.IntRegister reg);
}

public class GlobalLocation : VarLocation {
    static int counter = 0;
    readonly int number;
    public GlobalLocation() {
        this.number = counter++;
    }
    public override void CopyAddressToRegister(Asm.IntRegister reg)
    {
        //example output: mov $global0, %rax
        Asm.Asm.Emit(new Asm.OpMovLabelAddrReg(
            new Asm.Label($"global{this.number}"),
            reg
        ));
    }
    public override string ToString() {
        return $"global";
    }
}
public class LocalLocation : VarLocation {
    static int counter = 0;
    readonly int number;
    readonly FuncdefNode declarer;
    public LocalLocation(FuncdefNode F) {
        this.number = counter++;
        this.declarer = F;
    }
    
    public override void CopyAddressToRegister(Asm.IntRegister reg){
        int offset = 16 * this.number;
        Asm.Asm.Emit( new Asm.OpLea( offset: offset, src: Asm.Register.rbp, dst: reg) );
    }
    public static void ResetCounter() {
        counter = 0;
    }
    public override string ToString() {
        return $"local";
    }
}
public class ParameterLocation : VarLocation {
    static int counter = 0;
    readonly int number;
    readonly FuncdefNode declarer;
    public ParameterLocation(FuncdefNode F) {
        this.number = counter++;
        this.declarer = F;
    }
    public override void CopyAddressToRegister(Asm.IntRegister reg){
        int offset = 16 * this.number;
        Asm.Asm.Emit( new Asm.OpLea( offset: offset, src: Asm.Register.rbp, dst: reg) );
    }
    public static void ResetCounter() {
        counter = 0;
    }
    public override string ToString() {
        return $"parameter";
    }
}
public class MemberLocation : VarLocation {
    static int counter = 0;
    readonly int number;
    readonly ClassDeclNode declarer;
    public MemberLocation(ClassDeclNode C) {
        this.number = counter++;
        this.declarer = C;
    }
    public override void CopyAddressToRegister(Asm.IntRegister reg){
        int offset = 16 * this.number;
        Asm.Asm.Emit( new Asm.OpLea( offset: offset, src: Asm.Register.rbp, dst: reg) );
    }
    public static void ResetCounter() {
        counter = 0;
    }
    public override string ToString() {
        return $"parameter";
    }
}