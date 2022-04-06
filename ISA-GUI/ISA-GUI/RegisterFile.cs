// ---------------------------------------------------------------------------
// File name: RegisterFile.cs
// Project name: ISA-GUI
// ---------------------------------------------------------------------------
// Creators: Samuel Reynolds, Nick Farmer, Carlos Ortiz, & Brandon Beaudry						
// Course-Section: CSCI 4717-201
// Creation Date: 2/19/22		
// ---------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISA_GUI
{
    /**
	* Class Name: RegisterFile <br>
	* Class Purpose: Holds the CPU registers
	* 
	* <hr>
	* Date created: 2/19/21 <br>
	* @author Samuel Reynolds
	*/
    internal class RegisterFile
    {
        public int[] intRegisters = new int[16];     //16 16-bit registers
		public float[] floatRegisters = new float[16];
		public string[] intQi = new string[16];
		public int[] intQiIndex = new int[16];
		public string[] floatQi = new string[16];
		public int[] floatQiIndex = new int[16];
		public int ASPR;

		/**
	    * Method Name: RegisterFile <br>
	    * Method Purpose: Class constructor
	    * 
	    * <hr>
	    * Date created: 2/19/21 <br>
	    * @author Samuel Reynolds
	    */
		public RegisterFile()
        {
			for(int i = 0; i < 16; i++)
            {
				intQi[i] = "0";
				floatQi[i] = "0";
				intQiIndex[i] = -1;
				floatQiIndex[i] = -1;
            }
            //Initialize registers
            Array.Clear(intRegisters, 0, intRegisters.Length);
			Array.Clear(floatRegisters, 0, floatRegisters.Length);
			ASPR = 0;
		}
    }
}
