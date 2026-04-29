namespace Asm {
    public class OpShiftRightLogical(IntRegister reg) : Op {
        readonly IntRegister reg = reg;

        public override string ToString() {
            return $"shr %cl, %{this.reg}";
        }
    }
}