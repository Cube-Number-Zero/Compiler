namespace Asm {
    public class OpFDiv(FloatRegister left, FloatRegister right) : Op {
        readonly FloatRegister left = left, right = right;

        public override string ToString() {
            // Intentional ordering
            return $"divsd %{this.right}, %{this.left}";
        }
    }
}