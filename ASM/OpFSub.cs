namespace Asm {
    public class OpFSub(FloatRegister left, FloatRegister right) : Op {
        readonly FloatRegister left = left, right = right;

        public override string ToString() {
            // Intentional ordering
            return $"subsd %{this.right}, %{this.left}";
        }
    }
}