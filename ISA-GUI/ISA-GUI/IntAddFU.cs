// ---------------------------------------------------------------------------
// File name: IntAddFU.cs
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
	* Class Name: IntAddFU <br>
	* Class Purpose: 
	* 
	* <hr>
	* Date created: 4/1/22 <br>
	* @author Samuel Reynolds, Nick Farmer, Carlos Ortiz, & Brandon Beaudry	
	*/
	internal class IntAddFU
	{
		public Instruction instruction;
		public bool oneCycleFU;
		public string name;
		public int indexer;
		/**
	    * Method Name: IntAddFU <br>
	    * Method Purpose: Class constructor
	    * 
	    * <hr>
	    * Date created: 4/1/22 <br>
	    * @author Samuel Reynolds, Nick Farmer, Carlos Ortiz, & Brandon Beaudry	
	    */
		public IntAddFU(string name)
		{
			this.name = name;
		}
	}
}

