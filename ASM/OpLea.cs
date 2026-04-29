namespace Asm {
    public class OpLea(int offset, Register src, Register dst) : Op {
        readonly int offset = offset;
        readonly Register src = src;
        readonly Register dst = dst;

        public override string ToString() {
            return $"leaq {this.offset}(%{this.src}), %{this.dst}";
        }
    }
}