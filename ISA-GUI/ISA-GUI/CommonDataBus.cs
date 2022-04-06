// ---------------------------------------------------------------------------
// File name: CommonDataBus.cs
// Project name: ISA-GUI
// ---------------------------------------------------------------------------
// Creators: Samuel Reynolds, Nick Farmer, Carlos Ortiz, & Brandon Beaudry						
// Course-Section: CSCI 4717-201
// Creation Date: 4/1/22		
// ---------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISA_GUI
{
	/**
	* Class Name: Instruction <br>
	* Class Purpose: Dictates the format of an instruction
	* 
	* <hr>
	* Date created: 4/1/22 <br>
	* @author Samuel Reynolds, Nick Farmer, Carlos Ortiz, & Brandon Beaudry	
	*/
	internal class CommonDataBus
	{
		public Dictionary<string, string> CDB;
		public Dictionary<int, int> index;
		public Dictionary<int, int> IDIndex;
		/**
	    * Method Name: CommonDataBus <br>
	    * Method Purpose: Class constructor
	    * 
	    * <hr>
	    * Date created: 4/1/22 <br>
	    * @author Samuel Reynolds, Nick Farmer, Carlos Ortiz, & Brandon Beaudry	
	    */
		public CommonDataBus()
		{
			CDB = new Dictionary<string, string>();
			index = new Dictionary<int, int>();   //Instruction ID is key, CDB index is value
			IDIndex = new Dictionary<int, int>(); //CDB index is key, instruction ID is value
		}
	}
}

