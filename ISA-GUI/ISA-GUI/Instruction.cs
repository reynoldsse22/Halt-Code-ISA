// ---------------------------------------------------------------------------
// File name: Instruction.cs
// Project name: ISA-GUI
// ---------------------------------------------------------------------------
// Creators: Samuel Reynolds, Nick Farmer, Carlos Ortiz, & Brandon Beaudry						
// Course-Section: CSCI 4717-201
// Creation Date: 2/27/22		
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
	* Date created: 2/27/21 <br>
	* @author Samuel Reynolds
	*/
	internal class Instruction
	{
		public byte[] binInstruction = new byte[3];
		public int programCounterValue, opcode, r1, r2, r3, address, destinationReg, instrFlag, stage, cycleControl, intResult;
		public int stage1Start, stage1End, stage2Start, stage2End, stage3Start, stage3End, stage4Start, stage4End, stage5Start, stage5End;
		public float floatResult;
		public string instrType;
		public bool isFloat;
		public string assembly1;
		public string assembly2;
		/**
	    * Method Name: Instruction <br>
	    * Method Purpose: Class constructor
	    * 
	    * <hr>
	    * Date created: 2/27/21 <br>
	    * @author Samuel Reynolds
	    */
		public Instruction()
		{
			opcode = 1;
			cycleControl = 0;
			assembly1 = "NOP";
			assembly2 = "";
		}
	}
}
