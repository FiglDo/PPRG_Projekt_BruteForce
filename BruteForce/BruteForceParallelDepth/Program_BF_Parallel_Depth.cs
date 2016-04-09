using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BruteForceParallelDepth
{
    class Program_BF_Parallel_Depth
    {
        static char[] attackVector = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r',
        's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R',
        'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ','!','$','%','@','-','_'};

        static bool showOutput = true;
        const string password = "!eT@G";
        static int maxLength = password.Length;
        static Stopwatch sw;

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to C# - Brute Force - PARALLEL Depth!");
            //Console.Write("Please enter a password: ");
            //password = Console.ReadLine();
            Console.Write("Max. Length [{0}]: ", maxLength);
            var maxLengthS = Console.ReadLine();
            maxLength = String.IsNullOrEmpty(maxLengthS) ? maxLength : Int32.Parse(maxLengthS);
            Console.Write("Show Steps (true/[false]): ");
            var showOutputS = Console.ReadLine();
            showOutput = String.IsNullOrEmpty(showOutputS) ? false : Boolean.Parse(showOutputS);

            sw = new Stopwatch();
            sw.Start();
            BruteForce("", maxLength);

            Console.ReadLine();
        }

        static void BruteForce(string fixedPart, int maxDepth)
        {
            if ((fixedPart.Length + 1) > maxDepth)
                return;

            //for (int i = 0; i < attackVector.Length; i++)
            Parallel.For(0,attackVector.Length, i => 
            {
                if ((fixedPart + attackVector[i]).Equals(password))
                {
                    sw.Stop();
                    Console.WriteLine();
                    Console.WriteLine("{1}: PASSWORD FOUND! => {0}", (fixedPart + attackVector[i]), sw.ElapsedMilliseconds);
                    Console.ReadLine();
                    Environment.Exit(0);
                }
                else
                {
                    if (showOutput)
                    {
                        Console.WriteLine("{1}: Tried '{0}' ", (fixedPart + attackVector[i]), sw.ElapsedMilliseconds);
                    }

                    BruteForce((fixedPart + attackVector[i]), maxDepth);

                }
            }
            );
        }
    }
}
