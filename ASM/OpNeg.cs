namespace Asm {
    public class OpNeg(IntRegister reg) : Op {
        readonly IntRegister reg = reg;

        public override string ToString() {
            return $"negq %{this.reg}";
        }
    }
}