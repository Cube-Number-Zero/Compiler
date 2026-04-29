namespace Asm {
    public class OpPopReg(IntRegister reg) : Op {
        readonly IntRegister reg = reg;

        public override string ToString(){
            return $"popq %{this.reg}";
        }
    }
}
