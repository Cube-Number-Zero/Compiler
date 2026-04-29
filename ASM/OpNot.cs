namespace Asm {
    public class OpNot(IntRegister reg) : Op {
        readonly IntRegister reg = reg;

        public override string ToString() {
            return $"notq %{this.reg}";
        }
    }
}