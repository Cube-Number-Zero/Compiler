namespace Asm {
    public class OpSub(IntRegister left, IntRegister right) : Op {
        readonly IntRegister left = left, right = right;

        public override string ToString() {
            // Intentional ordering
            return $"subq %{this.right}, %{this.left}";
        }
    }
}