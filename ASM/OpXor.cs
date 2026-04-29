namespace Asm {
    public class OpXor(IntRegister left, IntRegister right) : Op {
        readonly IntRegister left = left, right = right;

        public override string ToString() {
            return $"xor %{this.right}, %{this.left}";
        }
    }
}