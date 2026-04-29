namespace Asm {
    public class OpAnd(IntRegister left, IntRegister right) : Op {
        readonly IntRegister left = left, right = right;

        public override string ToString() {
            return $"and %{this.right}, %{this.left}";
        }
    }
}