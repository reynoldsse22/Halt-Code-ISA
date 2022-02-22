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
        public ushort[] registers = new ushort[16];     //16 16-bit registers

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
            //Initialize registers
            Array.Clear(registers, 0, registers.Length);
        }
    }
}
