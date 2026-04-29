namespace Asm {
    public class OpCmpRegReg(IntRegister left, IntRegister right) : Op {
        readonly IntRegister left = left, right = right;

        public override string ToString() {
            return $"cmpq %{this.right}, %{this.left}";
        }
    }
}