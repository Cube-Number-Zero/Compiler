namespace Asm {
    public class OpInc(IntRegister reg) : Op {
        readonly IntRegister reg = reg;

        public override string ToString() {
            return $"incq %{this.reg}";
        }
    }
}