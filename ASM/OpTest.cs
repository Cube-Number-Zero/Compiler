namespace Asm {
    public class OpTest(IntRegister left, IntRegister right) : Op {
        readonly IntRegister left = left;
        readonly IntRegister right = right;

        public override string ToString() {
            return $"testq %{this.right}, %{this.left}";
        }
    }
}