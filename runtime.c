typedef unsigned int uint32_t;
typedef unsigned long long uint64_t;

_Static_assert(sizeof(uint32_t) == 4, "Bad size for uint32");
_Static_assert(sizeof(uint64_t) == 8, "Bad size for uint64");

typedef uint32_t DWORD;
typedef void* HANDLE;

#define NULL 0
static HANDLE stdin;
static HANDLE stdout;
static HANDLE stderr;

typedef struct StackArg_ {
    uint64_t value;
    uint64_t storageClass;
} StackArg;

__attribute__((ms_abi)) HANDLE GetStdHandle(DWORD d);
__attribute__((ms_abi)) void CloseHandle(HANDLE h);
__attribute__((ms_abi)) int WriteFile(HANDLE h, void* p, DWORD count, DWORD* written, void* o);

//runtime init
__attribute__((ms_abi)) void rtinit()
{
    stdin = GetStdHandle(0xfffffff6);
    stdout = GetStdHandle(0xfffffff5);
    stderr = GetStdHandle(0xfffffff4);
}

//runtime cleanup
__attribute__((ms_abi)) void rtcleanup()
{
    CloseHandle(stdin);
    CloseHandle(stdout);
    CloseHandle(stderr);
}

__attribute__((ms_abi)) uint64_t putc(StackArg* ptr) {
    DWORD count;
    char c = ptr[0].value;
    WriteFile(stdout, &c, 1, &count, NULL);
    return 1;
}
