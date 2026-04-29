namespace Asm {
    public class OpMovLabelAddrReg(Label lbl, IntRegister dst) : Op {
        readonly Label lbl = lbl;
        readonly IntRegister dst = dst;

        public override string ToString() {
            return $"movabsq ${this.lbl}, %{this.dst}";
        }
    }
}