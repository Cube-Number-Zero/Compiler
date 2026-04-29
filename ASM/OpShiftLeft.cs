namespace Asm {
    public class OpShiftLeft(IntRegister reg) : Op {
        readonly IntRegister reg = reg;

        public override string ToString() {
            return $"sal %cl, %{this.reg}";
        }
    }
}