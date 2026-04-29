namespace Asm {
    public class Label: Op {
        private static int counter_ = 0;
        public readonly string lbl;
        public Label() {
            this.lbl = $"lbl{counter_++}";
        }
        public Label(string lbl) {
            this.lbl = lbl;
        }
        public override string ToString(){
            return $"{lbl}:";
        }
    }
}