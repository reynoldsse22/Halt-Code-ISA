﻿// ---------------------------------------------------------------------------
// File name: BranchFU.cs
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
	* Class Name: BranchFU <br>
	* Class Purpose: 
	* 
	* <hr>
	* Date created: 4/2/22 <br>
	* @author Samuel Reynolds, Nick Farmer, Carlos Ortiz, & Brandon Beaudry	
	*/
	internal class BranchFU
	{
		public Instruction instruction;
		public bool oneCycleFU;
		public string name;
		public int indexer;
		/**
	    * Method Name: BranchFU <br>
	    * Method Purpose: Class constructor
	    * 
	    * <hr>
	    * Date created: 4/2/22 <br>
	    * @author Samuel Reynolds, Nick Farmer, Carlos Ortiz, & Brandon Beaudry	
	    */
		public BranchFU(string name)
		{
			this.name = name;
		}
	}
}
