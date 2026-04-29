using Asm;

public interface ResultLocation {
    void MoveToRegister(Asm.IntRegister reg);
    void MoveFromRegister(Asm.IntRegister reg, StorageClass klass);
}