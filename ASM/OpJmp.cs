namespace Asm {
    public class OpJmp(Label label) : Op {
        readonly Label label = label;

        public override string ToString() {
            return $"jmp {this.label}";
        }
    }
}