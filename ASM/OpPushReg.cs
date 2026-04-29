namespace Asm {
    public class OpPushReg(IntRegister reg) : Op {
        readonly IntRegister reg = reg;

        public override string ToString(){
            return $"pushq %{this.reg}";
        }
    }
}