namespace Asm {
    public class OpMoveConstRegInd(long value, IntRegister dst, int offset) : Op {
        readonly IntRegister dst = dst;
        readonly long value = value;
        readonly int offset = offset;

        public override string ToString() {
            return $"movq ${this.value}, {this.offset}(%{this.dst})";
        }
    }
}