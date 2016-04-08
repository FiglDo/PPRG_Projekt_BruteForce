using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Bruteforce
{
    //Code from https://code.msdn.microsoft.com/windowsdesktop/Brute-Force-9061ef3a
    class Program
    {
        //define likely password characters
        static char[] attackVector = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r',
            's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R',
            'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ' ','!', '"','#','$','%','&','\'',')','(','*','+',
            '´','-','.','/',':',';','<','=','>','?','@','[','\\',']','^','_','`','{','|','}','~'};

        //your password
        const string password = "!eT@";
        static int Combi;
        static string space;
        static bool showOutput = true;
        static Stopwatch sw;

        static void Main(string[] args)
        {
            sw = new Stopwatch();

            space = " ";
            int Count;
            Console.WriteLine("Welcome to BRUTE FORCE with RA-BI");
            Console.WriteLine(space);
            Console.Write("Show Steps (true/[false]: ");
            var showOutputS = Console.ReadLine();
            showOutput = String.IsNullOrEmpty(showOutputS) ? false : Boolean.Parse(showOutputS);


            DateTime today = DateTime.Now;
            today.ToString("yyyy-MM-dd_HH:mm:ss");
            Console.WriteLine(space);
            Console.WriteLine("START:\t{0}", today);

            sw.Start();
            for (Count = 0; Count <= 15; Count++)
            {
                Recurse(Count, 0, "");
            }
        }

        static void Recurse(int Lenght, int Position, string BaseString)
        {
            int Count = 0;

            for (Count = 0; Count < attackVector.Length; Count++)
            {
                Combi++;
                if (Position < Lenght - 1)
                {
                    Recurse(Lenght, Position + 1, BaseString + attackVector[Count]);
                }
                if (showOutput)
                {
                    Console.WriteLine("{1}: Tried '{0}'",BaseString + attackVector[Count],sw.ElapsedMilliseconds);
                }

                if (BaseString + attackVector[Count] == password)
                {
                    sw.Stop();
                    Console.WriteLine();
                    Console.WriteLine("{1}: PASSWORD FOUND! => {0}", BaseString + attackVector[Count], sw.ElapsedMilliseconds);
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
        }
    }
}
