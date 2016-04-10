using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCLTemplate;

namespace OpenCL1
{
	class Program
	{
		static void Main(string[] args)
		{
			string brute = @"
				__kernel void
				bruteDeForce(global char * alphabet, global double * alphabetSize,  
				global int * maxLen, global double * steps, global char * password,
				global int * match)
				{

				// Vector element index
				int i = get_global_id(0);

				int stepPointer = 0;
				char word[7];
				int pos = 0;
				if (i >= steps[stepPointer]){
					stepPointer++;
				}
	
				int j = 0;
				double sum = 0;
				for (; j <= stepPointer; j++){
					pos = (int)fmod((i - sum) / pow(alphabetSize[0], (double)j), alphabetSize[0]);
					sum = sum + pow((pos + 1),(double)j);
					word[(stepPointer-j)] = alphabet[pos];
				}
				word[j] = '\0';
                      
				}
								";

			//Initializes OpenCL Platforms and Devices and sets everything up
			CLCalc.InitCL();

			//Compiles the source codes. The source is a string array because the user may want
			//to split the source into many strings.
			CLCalc.Program.Compile(new string[] { brute });

			//Gets host access to the OpenCL floatVectorSum kernel
			CLCalc.Program.Kernel VectorSum = new OpenCLTemplate.CLCalc.Program.Kernel("bruteDeForce");

			int[] maxLen = {3};
            char[] password = new char[10];
            int[] match = { 0 };
        //     char[] alphabet = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r',
        //'s', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R',
        //'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ','!','$','%','@','-','_'};

             char[] alphabet = new char[] { 'a', 'b', 'c'};

			 int[] alphabetSize = {alphabet.Length};

			double len = 0;
			double[] steps = new double[maxLen[0]];

			for (int i = 1; i <= maxLen[0]; i++)
			{
				len += Math.Pow(alphabetSize[0], i);
				if (i == 1)
				{
					steps[(i - 1)] = Math.Pow(alphabetSize[0], i);
				}
				else
				{
					steps[(i - 1)] = Math.Pow(alphabetSize[0], i) + steps[(i - 2)];
				}
			}

			char[] word = new char[maxLen[0]];


			//Creates vectors v1 and v2 in the device memory
			OpenCLTemplate.CLCalc.Program.Variable varAlphabet = new OpenCLTemplate.CLCalc.Program.Variable(alphabet);
			OpenCLTemplate.CLCalc.Program.Variable varAlphabetSize = new OpenCLTemplate.CLCalc.Program.Variable(alphabetSize);
			OpenCLTemplate.CLCalc.Program.Variable varMaxLen = new OpenCLTemplate.CLCalc.Program.Variable(maxLen);
			OpenCLTemplate.CLCalc.Program.Variable varSteps = new OpenCLTemplate.CLCalc.Program.Variable(steps);
			OpenCLTemplate.CLCalc.Program.Variable varPassword = new OpenCLTemplate.CLCalc.Program.Variable(password);
			OpenCLTemplate.CLCalc.Program.Variable varMatch = new OpenCLTemplate.CLCalc.Program.Variable(match);

			//Arguments of VectorSum kernel
			OpenCLTemplate.CLCalc.Program.Variable[] argsCL = new OpenCLTemplate.CLCalc.Program.Variable[] { varAlphabet, varAlphabetSize, varMaxLen, varSteps, varPassword, varMatch };

			int[] workers = new int[1] { 10 };

            //OpenCLTemplate.CLCalc.Program.DefaultCQ = 0;

			//Execute the kernel
			VectorSum.Execute(argsCL, workers);

			//Read device memory varV1 to host memory v1
            varMatch.ReadFromDeviceTo(match);
            varPassword.ReadFromDeviceTo(password);

            Console.WriteLine(match[0]);
            Console.ReadLine();
		}
	}
}

//int t=0;
//int help=9;
//while(t < j && password[t] != 'X'){
//if(word[t] != password[t]){
//    help=1;
//}
//t++;
//}
//match[0] = help;