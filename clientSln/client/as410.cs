using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace client
{
   
    class as410
    {

        public short[] holdingRegisters = new short[50];
        public short R_01; //Stan Di
        public int[] digitalInput = new int[7]; 
        public short R_02; //stan Do
        public int[] digitalOutput = new int[7]; // 
        public short R_03; //ster Do

        public void updateState()
        {
            R_01 = holdingRegisters[0];

            string stringDI = Convert.ToString(R_01, 2); //Convert to binary in a string

            int[] DiBits = stringDI.PadLeft(8, '0') // Add 0's from left
                         .Select(c => int.Parse(c.ToString())) // convert each char to int
                         .ToArray(); // Convert IEnumerable from select to Array
            digitalInput = DiBits;

            R_02 = holdingRegisters[1];

            string stringDO = Convert.ToString(R_02, 2); //Convert to binary in a string
            int[] DoBits = stringDO.PadLeft(8, '0') // Add 0's from left
                        .Select(c => int.Parse(c.ToString())) // convert each char to int
                        .ToArray(); // Convert IEnumerable from select to Array
            digitalOutput = DoBits;


           
            R_03 = holdingRegisters[2];
        }
    }
}
