namespace Asm {
    public class OpMoveRegIndReg(IntRegister src, Register dst, int offset) : Op {
        readonly IntRegister src = src;
        readonly Register dst = dst;
        readonly int offset = offset;

        public override string ToString(){
            return $"movq {this.offset}(%{this.src}), %{this.dst}";
        }
    }
}