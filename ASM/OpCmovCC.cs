namespace Asm {
    public class OpCmovCC(string cc, IntRegister src, IntRegister dest) : Op {
        readonly string cc = cc;
        readonly IntRegister src = src, dest = dest;

        public override string ToString() {
            return $"cmov{this.cc} %{this.src}, %{this.dest}";
        }
    }
}