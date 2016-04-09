#include <stdio.h>
#include <stdlib.h>
#include <math.h>
// Array with characters to use in Brute Force Algorithm.
// You can remove or add more characters in this array.
//compile: gcc -O1 bfnew.c -o bfnew -lm
char CharList[] = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
long int CharListLength = sizeof(CharList);//we make it long long int, so we don't have to box it later
static int min = 0;
static int max = 7;
int q = 0;
//Main routine
int main()
{
	long long int powers[7];
	while (q<max)
	{
		powers[q] = pow(CharListLength, q);
		printf("%d %lldn", q, powers[q]);
		q++;
	}
	while (min<max)
	{
		long long int counter = pow(CharListLength, (min - 1));
		long long int c = 0;
		for (c; c <= counter; c++)
		{
			long long int s = c;
			int i = min - 1;
			for (i; i >= 0; i--)
			{
				char myChar = CharList[(s / powers[i])];
				//printf ("%c",myChar);//do your stuff here
				s = s % (powers[i]);
			}
			//printf("n");//just for clean printing/.test
		}
		min++;
	}
	return 0;//and we return
}