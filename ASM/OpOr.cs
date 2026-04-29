namespace Asm {
    public class OpOr(IntRegister left, IntRegister right) : Op {
        readonly IntRegister left = left, right = right;

        public override string ToString() {
            return $"or %{this.right}, %{this.left}";
        }
    }
}