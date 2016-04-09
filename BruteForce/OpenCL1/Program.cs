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
            string vecSum = @"
                __kernel void
                floatVectorSum(__global       float * v1,
                __global       float * v2)
                {
                // Vector element index
                int i = get_global_id(0);
                v1[i] = v1[i] + v2[i];
                }";

            //Initializes OpenCL Platforms and Devices and sets everything up
            CLCalc.InitCL();

            //Compiles the source codes. The source is a string array because the user may want
            //to split the source into many strings.
            CLCalc.Program.Compile(new string[] { vecSum });

            //Gets host access to the OpenCL floatVectorSum kernel
            CLCalc.Program.Kernel VectorSum = new OpenCLTemplate.CLCalc.Program.Kernel("floatVectorSum");


            //We want to sum 2000 numbers
            int n = 2000;

            //Create vectors with 2000 numbers
            float[] v1 = new float[n], v2 = new float[n];

            //Creates population for v1 and v2
            for (int i = 0; i < n; i++)
            {
                v1[i] = (float)i / 10;
                v2[i] = -(float)i / 9;
            }


            //Creates vectors v1 and v2 in the device memory
            OpenCLTemplate.CLCalc.Program.Variable varV1 = new OpenCLTemplate.CLCalc.Program.Variable(v1);
            OpenCLTemplate.CLCalc.Program.Variable varV2 = new OpenCLTemplate.CLCalc.Program.Variable(v2);
            
            //Arguments of VectorSum kernel
            OpenCLTemplate.CLCalc.Program.Variable[] argsCL = new OpenCLTemplate.CLCalc.Program.Variable[] { varV1, varV2 };

            int[] workers = new int[1] { n };

            //Execute the kernel
            VectorSum.Execute(argsCL, workers);

            //Read device memory varV1 to host memory v1
            varV1.ReadFromDeviceTo(v1);

        }
    }
}
