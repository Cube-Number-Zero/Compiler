namespace Asm {
    public class OpMoveConstReg(long value, IntRegister dst) : Op {
        readonly IntRegister dst = dst;
        readonly long value = value;

        public override string ToString() {
            return $"movabsq ${this.value}, %{this.dst}";
        }
    }
}