// ---------------------------------------------------------------------------
// File name: DataMemory.cs
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
	* Class Purpose: Holds Main Memory
	* 
	* <hr>
	* Date created: 2/19/21 <br>
	* @author Samuel Reynolds
	*/
    internal class DataMemory
    {
        public byte[] MainMemory = new byte[1048576];       //A full mebibyte of main memory

		/**
	    * Method Name: DataMemory <br>
	    * Method Purpose: Class constructor
	    * 
	    * <hr>
	    * Date created: 2/19/21 <br>
	    * @author Samuel Reynolds
	    */
		public DataMemory()
        {
            //clear main memory
            Array.Clear(MainMemory, 0, MainMemory.Length);
        }
    }
}
