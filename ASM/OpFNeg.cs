namespace Asm {
    public class OpFNeg(FloatRegister reg) : Op {
        readonly FloatRegister reg = reg;

        public override string ToString() {
            return $"fchs";
        }
    }
}