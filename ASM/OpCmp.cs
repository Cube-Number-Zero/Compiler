namespace Asm {
    public class OpCmp(IntRegister left, IntRegister right) : Op {
        readonly IntRegister left = left, right = right;

        public override string ToString() {
            return $"cmp %{this.right}, %{this.left}";
        }
    }
}