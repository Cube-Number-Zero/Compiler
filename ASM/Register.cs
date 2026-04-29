namespace Asm {
    public abstract class Register(string name)
    {
        public static readonly IntRegister rax = new("rax");
        public static readonly IntRegister rbx = new("rbx");
        public static readonly IntRegister rcx = new("rcx");
        public static readonly IntRegister rdx = new("rdx");
        public static readonly IntRegister rsi = new("rsi");
        public static readonly IntRegister rdi = new("rdi");
        public static readonly IntRegister rsp = new("rsp");
        public static readonly IntRegister rbp = new("rbp");
        public static readonly IntRegister r8  = new("r8" );
        public static readonly IntRegister r9  = new("r9" );
        public static readonly IntRegister r10 = new("r10");
        public static readonly IntRegister r11 = new("r11");
        public static readonly IntRegister r12 = new("r12");
        public static readonly IntRegister r13 = new("r13");
        public static readonly IntRegister r14 = new("r14");
        public static readonly IntRegister r15 = new("r15");

        public static readonly FloatRegister xmm0 =  new(0);
        public static readonly FloatRegister xmm1 =  new(1);
        public static readonly FloatRegister xmm2 =  new(2);
        public static readonly FloatRegister xmm3 =  new(3);
        public static readonly FloatRegister xmm4 =  new(4);
        public static readonly FloatRegister xmm5 =  new(5);
        public static readonly FloatRegister xmm6 =  new(6);
        public static readonly FloatRegister xmm7 =  new(7);
        public static readonly FloatRegister xmm8 =  new(8);
        public static readonly FloatRegister xmm9 =  new(9);
        public static readonly FloatRegister xmm10 = new(10);
        public static readonly FloatRegister xmm11 = new(11);
        public static readonly FloatRegister xmm12 = new(12);
        public static readonly FloatRegister xmm13 = new(13);
        public static readonly FloatRegister xmm14 = new(14);
        public static readonly FloatRegister xmm15 = new(15);
        
        public readonly string name = name;


        public override string ToString( ){
            return name;
        }
    }

    public class IntRegister(string name) : Register(name) {
    }

    public class FloatRegister(int num) : Register($"xmm{num}") {
    }

}