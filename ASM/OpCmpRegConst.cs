namespace Asm {
    public class OpCmpRegConst(IntRegister left, int right) : Op {
        readonly IntRegister left = left;
        readonly int right = right;

        public override string ToString() {
            return $"cmp ${this.right}, %{this.left}";
        }
    }
}