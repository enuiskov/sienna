//#include <iostream>
//#include <complex>
#include <stdlib.h>
#include <stdio.h>
#include <windows.h>

//#define MAX 200

//using namespace std;

//#define M_PI 3.1415926535897932384


int main()
{
	for(;;)
	{
		int _StateA = GetKeyState('A') & 0x8000;

		printf("StateA=%d\n",_StateA);
		Sleep(100);
	}
	
	return 0;
}