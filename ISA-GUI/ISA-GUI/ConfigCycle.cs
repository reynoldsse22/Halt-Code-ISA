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
		public int fetch, memAccess, regAccess, load_store, intALU, flAddSub, flMult, flDiv, calcAddress, programSpeed;
		public int load, store, intAdd, intSub, intMult, intDiv, flAdd, flSub, bitwise, shift, effAddress, reorderbuffersize;
		public bool predictionSet;
		public bool forwardingSet;
		public bool dynamicPipelineSet;

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
			programSpeed = 0;
			fetch = 1;
			memAccess = 3;
			regAccess = 1;
			load_store = 1;
			load = 1;
			store = 1;
			intALU = 1;
			intAdd = 1;
			intSub = 1;
			intMult = 1;
			intDiv = 1;
			flAdd = 2;
			flSub = 2;
			bitwise = 1;
			shift = 1;
			effAddress = 1;
			flAddSub = 2;
			flMult = 5;
			flDiv = 10;
			calcAddress = 1;
			reorderbuffersize = 10;
			predictionSet = false;
			forwardingSet = false;
			dynamicPipelineSet = true;
        }
    }
}
