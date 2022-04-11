#include <stdio.h>

typedef unsigned char byte;

int arg1;
int arg2;
int res1;

byte buf[1<<16];

typedef void (*pfunc)(void);

union funcptr {
  pfunc x;
  byte* y;
};

int main( void ) {

  byte* p = buf;

  *p++ = 0x60; // pusha

  *p++ = 0xA1; // mov eax, [arg1]
  (int*&)p[0] = &arg1; p+=sizeof(int);

  // 0FAF05 34120000  imul eax,[arg2]
  *p++ = 0x0F;
  *p++ = 0xAF;
  *p++ = 0x05;
  (int*&)p[0] = &arg2; p+=sizeof(int);

  *p++ = 0xA3; // mov [res1],eax
  (int*&)p[0] = &res1; p+=sizeof(int);

  *p++ = 0x61; // popa
  *p++ = 0xC3; // ret

  funcptr func;
  func.y = buf;

  arg1 = 123; arg2 = 321; res1 = 0;

  func.x(); // call generated code

  printf( "arg1=%i arg2=%i arg1*arg2=%i func(arg1,arg2)=%i\n", arg1,arg2,arg1*arg2,res1 );

}
