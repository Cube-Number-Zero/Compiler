namespace Asm {
    public class OpFAdd(FloatRegister left, FloatRegister right) : Op {
        readonly FloatRegister left = left, right = right;

        public override string ToString() {
            // Intentional ordering
            return $"addsd %{this.right}, %{this.left}";
        }
    }
}