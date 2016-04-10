#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <math.h>
#include <omp.h>
#include <limits.h>

const char *alphabet = "abcdefghijklmnopqrstuvwxyz"
"ABCDEFGHIJKLMNOPQRSTUVWXYZ"
"0123456789!$%@-_";

//const char *alphabet = "abc";
//const char *password = "bb";
const char *password = "!eT@";

//#define maxLen 5

void forceDeBrute(int maxLen){
	int alphabetSize = strlen(alphabet);
	double len = 0;
	double* steps = malloc(maxLen * sizeof(double));

	for (int i = 1; i <= maxLen; i++){
		len += pow(alphabetSize, i);
		if (i == 1){
			steps[(i - 1)] = pow(alphabetSize, i);
		}
		else{
			steps[(i - 1)] = pow(alphabetSize, i) + steps[(i - 2)];
		}
	}
	int junkS = 20000;
	// calculate num of int arrays
	int junkSize = len / junkS + 1;


	printf("junkSize: %d\n", junkSize);
	printf("INT_MAX: %d\n", junkS);
	printf("len: %f\n", len);

	int stepPointer = 0;
	int threadNr = 0;
	omp_set_num_threads(1);

	for (int x = 0; x < junkSize; x++)
	{
		#pragma omp parallel for shared(stepPointer)
		for (int y = 0; y < junkS; y++)
		{
			int i = x*junkSize + y;
			char* word = malloc(sizeof(char)+1);
			if (threadNr == 0){
				threadNr = omp_get_num_threads();
				printf("Threads: %d\n", threadNr);
			}
			int stepPointer = 0;
			if (i > steps[0]){
				for (int z = 0; z < maxLen; z++)
				{
					if (i >= steps[z] && i < steps[(z+1)]){
						stepPointer = z+1;
						break;
					}
				}
			}
			int j = 0;
			double sum = 0;
			int pos = 0;
			for (j; j <= stepPointer; j++){
				pos = (int)fmod((i - sum) / pow(alphabetSize, j), alphabetSize);
				sum = sum + pow((pos + 1), j);
				word[(stepPointer - j)] = alphabet[pos];
			}
			word[j] = '\0';
			 //fprintf(stdout, "%s\n", word);
			//getchar();
			if (strcmp(word, password) == 0){
				fprintf(stdout, "found it: %s\n", word);
				exit(0);
				//return;
			}
			free(word);
		}
	}
	free(steps);
	//free("willy");
}

int main(int argc, char *argv[])
{

	clock_t start = clock();
	forceDeBrute(7);
	clock_t end = clock();

	double timeTaken = (end - start);
	double timeTakenSec = timeTaken / 1000;
	if (timeTakenSec < 60){
		printf("Time taken: %f Sec", timeTakenSec);
	}
	else{
		double timeTakenMin = timeTakenSec / 60;
		printf("Time taken: %f Min", timeTakenMin);
	}
	getchar();
}

