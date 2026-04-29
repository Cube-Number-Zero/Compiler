using Asm;

public class Temporary(int number)
{
    public readonly int number = number;

    public void CopyToRegister(Asm.IntRegister reg) {
        Asm.Asm.Emit(   new Comment($"Copy temporary {number} value to register"),
                        new OpMoveRegIndReg(src: Register.rbp, dst: reg, offset: -16 * (number + 1)));
    }
    public void CopyToRegister(Asm.FloatRegister reg) {
        Asm.Asm.Emit(   new Comment($"Copy temporary {number} value to register"),
                        new OpMoveRegIndReg(src: Register.rbp, dst: reg, offset: -16 * (number + 1)));
    }
    public void CopyFromRegister(Asm.IntRegister reg, StorageClass klass) {
        Asm.Asm.Emit(   new Comment($"Copy register to temporary {number}"),
                        new OpMoveRegRegInd(src: reg, dst: Register.rbp, offset: -16 * (number + 1)),
                        new Comment($"Set storage class of temporary {number}"),
                        new OpMoveConstRegInd(value: (int)klass, dst: Register.rbp, offset: 8 - 16 * (number + 1))/*,
                        new Comment($"Copy temporary {number} value to register"),
                        new OpMoveRegIndReg(src: Register.rbp, dst: Register.rbp, offset: -16 * (number + 1))*/);
    }
    public void CopyFromRegister(IntRegister reg, IntRegister klass) {
        
    }
    public void CopyFromRegister(Asm.FloatRegister reg, StorageClass klass) {
        Asm.Asm.Emit(   new Comment($"Copy register to temporary {number}"),
                        new OpMoveRegRegInd(src: reg, dst: Register.rbp, offset: -16 * (number + 1)),
                        new Comment($"Set storage class of temporary {number}"),
                        new OpMoveConstRegInd(value: (int)klass, dst: Register.rbp, offset: 8 - 16 * (number + 1))/*,
                        new Comment($"Copy temporary {number} value to register"),
                        new OpMoveRegIndReg(src: Register.rbp, dst: Register.rbp, offset: -16 * (number + 1))*/);
    }
}