namespace Asm {
    public class OpMoveRegReg(Register src, Register dst) : Op {
        readonly Register src = src, dst = dst;

        public override string ToString(){
            return $"movq %{this.src}, %{this.dst}";
        }
    }
}