namespace Asm {
    public class OpAddConstReg(int left, IntRegister right) : Op {
        readonly IntRegister right = right;

        public override string ToString() {
            // Intentional ordering
            return $"addq %{this.right}, ${left}";
        }
    }
}