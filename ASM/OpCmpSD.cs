namespace Asm {
    public class OpCmpSD(string cc, FloatRegister left, FloatRegister right) : Op {
        readonly string cc = cc;
        readonly FloatRegister left = left, right = right;

        public override string ToString() {
            return $"cmp{cc}sd %{this.right}, %{this.left}";
        }
    }
}