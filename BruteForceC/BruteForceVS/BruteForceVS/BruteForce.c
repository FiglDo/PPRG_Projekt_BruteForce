#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <math.h>

const char *alphabet = "abcdefghijklmnopqrstuvwxyz"
"ABCDEFGHIJKLMNOPQRSTUVWXYZ"
"0123456789!$%@-_";

//const char *alphabet = "abc";
const char *password = "!eT@G";

//#define maxLen 5

void forceDeBrute(int maxLen){
	int alphabetSize = strlen(alphabet);
	double len = 0;
	double* steps = malloc(maxLen * sizeof(double));

	for (int i = 1; i <= maxLen; i++){
		len += pow(alphabetSize,i);
		if (i == 1){
			steps[(i - 1)] = pow(alphabetSize, i);
		}
		else{
			steps[(i - 1)] = pow(alphabetSize, i) + steps[(i-2)];
		}
	}

	int stepPointer = 0;
	char* word = malloc(sizeof(char)+2);

	for (double i = 0; i < len; i++){
		if (i >= steps[stepPointer]){
			stepPointer++;
			realloc(word, (stepPointer+1) * sizeof(char)+2);
		}
		
		int j = 0;
		double sum = 0;
		for (j; j <= stepPointer; j++){
			int pos = (int)fmod((i - sum) / pow(alphabetSize,j), alphabetSize);
			sum = sum + pow((pos + 1),j);
			word[(stepPointer-j)] = alphabet[pos];
		}
		word[(j + 1)] = '\0';
		//fprintf(stdout,"%s\n", word);
		if (strcmp(word,password) ==0 ){ 
			fprintf(stdout, "found it: %s\n", word);
			return;
		}
	}

	free(word);
	free(steps);
	//free("Willy");
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

