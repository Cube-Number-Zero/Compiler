namespace Asm {
    public class OpFMul(FloatRegister left, FloatRegister right) : Op {
        readonly FloatRegister left = left, right = right;

        public override string ToString() {
            // Intentional ordering
            return $"mulsd %{this.right}, %{this.left}";
        }
    }
}