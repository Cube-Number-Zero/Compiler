namespace Asm {
    public class OpAndRegConst(IntRegister left, int right) : Op {
        readonly IntRegister left = left;
        readonly int right = right;

        public override string ToString() {
            return $"andq ${this.right}, %{this.left}";
        }
    }
}