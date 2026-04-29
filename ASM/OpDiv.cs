namespace Asm {
    public class OpDiv(IntRegister reg) : Op {
        readonly IntRegister reg = reg;

        public override string ToString() {
            return $"idiv %{this.reg}";
        }
    }
}