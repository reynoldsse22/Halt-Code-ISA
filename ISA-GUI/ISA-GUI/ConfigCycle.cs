// ---------------------------------------------------------------------------
// File name: ConfigCycle.cs
// Project name: ISA-GUI
// ---------------------------------------------------------------------------
// Creators: Samuel Reynolds, Nick Farmer, Carlos Ortiz, & Brandon Beaudry						
// Course-Section: CSCI 4717-201
// Creation Date: 2/28/22		
// ---------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISA_GUI
{
	/**
	* Class Name: WriteResult <br>
	* Class Purpose: The configuration class for the length of cycles certain instructions take
	* 
	* <hr>
	* Date created: 2/28/21 <br>
	* @author Samuel Reynolds
	*/
	public class ConfigCycle
    {
		public int fetch, memAccess, regAccess, load_store, intALU, flAddSub, flMult, flDiv, calcAddress;
		public bool predictionSet;
		public bool forwardingSet;

		/**
	    * Method Name: ConfigCycle <br>
	    * Method Purpose: Class constructor
	    * 
	    * <hr>
	    * Date created: 2/28/21 <br>
	    * @author Samuel Reynolds
	    */
		public ConfigCycle()
        {
			fetch = 1;
			memAccess = 3;
			regAccess = 1;
			load_store = 1;
			intALU = 1;
			flAddSub = 2;
			flMult = 5;
			flDiv = 10;
			calcAddress = 1;
			predictionSet = false;
			forwardingSet = false;
        }
    }
}
