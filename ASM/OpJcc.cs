namespace Asm {
    public class OpJcc(string condition, Label label) : Op {
        readonly string condition = condition;
        readonly Label label = label;

        public override string ToString() {
            return $"j{this.condition} {this.label}";
        }
    }
}