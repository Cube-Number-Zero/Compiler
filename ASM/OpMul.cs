namespace Asm {
    public class OpMul(IntRegister left, IntRegister right) : Op {
        readonly IntRegister left = left;
        readonly IntRegister right = right;

        public override string ToString() {
            return $"imul %{this.left}, %{this.right}";
        }
    }
}