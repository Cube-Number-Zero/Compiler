namespace Asm {
    public class OpMoveRegRegInd(Register src, Register dst, int offset) : Op {
        readonly Register src = src; // should be intregister
        readonly Register dst = dst;
        readonly int offset = offset;

        public override string ToString(){
            return $"movq %{this.src}, {this.offset}(%{this.dst})";
        }
    }
}