namespace Asm {
    public class OpShiftRight(IntRegister reg) : Op {
        readonly IntRegister reg = reg;

        public override string ToString() {
            return $"sar %cl, %{this.reg}";
        }
    }
}