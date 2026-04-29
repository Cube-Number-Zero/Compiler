namespace Asm {
    public static class Asm {
        public static List<Op> opcodes = [];
        public static void Emit(List<Op> ops) {
            opcodes.AddRange(ops);
        }
        public static void Emit(params Op[] ops) {
            opcodes.AddRange(ops);
        }
        public static void Write(TextWriter outputfile, Label mailLabel) {
            outputfile.Write(
$@".section .text
.global _start

_start:
andq $~0xf, %rsp   //align the stack

sub $32, %rsp
call rtinit
add $32, %rsp


call {mailLabel.lbl}

push %rax
push %rax
sub $32, %rsp
call rtcleanup
add $32, %rsp

//main's return value comes back in rax
//Call ExitProcess(rax)
pop %rcx
pop %rcx
subq $32, %rsp     //shadow space
call ExitProcess
");
            foreach (Op op in opcodes) {
                outputfile.WriteLine(op);
            }
            //outputfile.WriteLine(".section .bss");
            // global1:
            // skip 16
            // global2:
            // skip 16
            // for loop with symboltable
        }
    }
}