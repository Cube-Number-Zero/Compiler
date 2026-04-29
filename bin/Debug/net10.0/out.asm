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
/* Allocate space for 5 temporaries */
subq $80, %rsp
movabsq $4636744328229054054, %rax
/* Copy register to temporary 2 */
movq %rax, -48(%rbp)
/* Set storage class of temporary 2 */
movq $12345, -40(%rbp)
/* copy register to temporary 2 */
movq %rax, -24(%rbp)
/* Copy temporary 2 value to register */
movq -48(%rbp), %xmm0
fchs
/* Copy register to temporary 1 */
movq %xmm0, -32(%rbp)
/* Set storage class of temporary 1 */
movq $12345, -24(%rbp)
movabsq $4636751365103471821, %rax
/* Copy register to temporary 4 */
movq %rax, -80(%rbp)
/* Set storage class of temporary 4 */
movq $12345, -72(%rbp)
/* copy register to temporary 4 */
movq %rax, -40(%rbp)
/* Copy temporary 4 value to register */
movq -80(%rbp), %xmm0
fchs
/* Copy register to temporary 3 */
movq %xmm0, -64(%rbp)
/* Set storage class of temporary 3 */
movq $12345, -56(%rbp)
/* Copy temporary 1 value to register */
movq -32(%rbp), %xmm0
/* Copy temporary 3 value to register */
movq -64(%rbp), %xmm1
cmpnlesd %xmm1, %xmm0
movq %xmm0, %rax
andq $1, %rax
/* Copy register to temporary 0 */
movq %rax, -16(%rbp)
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
