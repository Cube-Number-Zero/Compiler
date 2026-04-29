.section .text
.global _start

_start:
andq $~0xf, %rsp   //align the stack

sub $32, %rsp
call rtinit
add $32, %rsp


call lbl4

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
/* main */
lbl4:
/* function prologue */
pushq %rbp
movq %rsp, %rbp
/* Allocate space for 3 temporaries */
subq $48, %rsp
/* Constant [NUM at line 3 column 11: 3] */
movabsq $3, %rax
/* Copy register to temporary 1 */
movq %rax, -32(%rbp)
/* Set storage class of temporary 1 */
movq $12345, -24(%rbp)
/* Constant [NUM at line 3 column 16: 3] */
movabsq $3, %rax
/* Copy register to temporary 2 */
movq %rax, -48(%rbp)
/* Set storage class of temporary 2 */
movq $12345, -40(%rbp)
/* Copy temporary 1 value to register */
movq -32(%rbp), %rax
/* Copy temporary 2 value to register */
movq -48(%rbp), %rbx
xor %rcx, %rcx
xor %rdx, %rdx
incq %rdx
cmpq %rbx, %rax
cmovne %rdx, %rcx
/* Copy register to temporary 0 */
movq %rcx, -16(%rbp)
/* Set storage class of temporary 0 */
movq $12345, -8(%rbp)
/* Copy temporary 0 value to register */
movq -16(%rbp), %rax
/* return from main */
movq %rbp, %rsp
popq %rbp
ret
/* return fall off end */
movq %rbp, %rsp
popq %rbp
ret
/* End of main */
