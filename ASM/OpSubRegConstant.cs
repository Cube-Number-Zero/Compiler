namespace Asm {
    public class OpSubRegConstant(IntRegister reg, int value) : Op {
        readonly IntRegister reg = reg;
        readonly int value = value;

        public override string ToString(){
            return $"subq ${this.value}, %{this.reg}";
        }
    }
}