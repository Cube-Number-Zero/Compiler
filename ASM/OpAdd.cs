namespace Asm {
    public class OpAdd(IntRegister left, IntRegister right) : Op {
        readonly IntRegister left = left, right = right;

        public override string ToString() {
            // Intentional ordering
            return $"addq %{this.right}, %{this.left}";
        }
    }
}