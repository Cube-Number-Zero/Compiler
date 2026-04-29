namespace Asm {
    public class OpCall : Op {
        public readonly Label name;
        public OpCall(Label name) {
            this.name = name;
        }
        public override string ToString() {
            return $"call {name}";
        }
    }
}