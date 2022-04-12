﻿// ---------------------------------------------------------------------------
// File name: MemoryUnitFU.cs
// Project name: ISA-GUI
// ---------------------------------------------------------------------------
// Creators: Samuel Reynolds, Nick Farmer, Carlos Ortiz, & Brandon Beaudry						
// Course-Section: CSCI 4717-201
// Creation Date: 4/2/22		
// ---------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISA_GUI
{
	/**
	* Class Name: MemoryUnitFu <br>
	* Class Purpose: 
	* 
	* <hr>
	* Date created: 4/2/22 <br>
	* @author Samuel Reynolds, Nick Farmer, Carlos Ortiz, & Brandon Beaudry	
	*/
	internal class MemoryUnit
	{
		public Instruction instruction;
		public bool oneCycleFU;
		public string name;
		public int indexer;
		/**
	    * Method Name: MemoryUnitFu <br>
	    * Method Purpose: Class constructor
	    * 
	    * <hr>
	    * Date created: 4/2/22 <br>
	    * @author Samuel Reynolds, Nick Farmer, Carlos Ortiz, & Brandon Beaudry	
	    */
		public MemoryUnit(string name)
		{
			this.name = name;
		}
	}
}
