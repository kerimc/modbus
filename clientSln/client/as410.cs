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
        public short R_02; //stan Do
        public short R_03; //ster Do

        public void updateState()
        {
            R_01 = holdingRegisters[0];
            R_02 = holdingRegisters[1];
            R_03 = holdingRegisters[2];
        }
    }
}
