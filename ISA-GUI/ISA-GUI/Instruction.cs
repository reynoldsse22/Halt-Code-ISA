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
		public int programCounterValue, opcode, r1, r2, r3, address, destinationReg, instrFlag, stage, cycleControl, intResult, ID;
		public int stage1Start, stage1End, stage2Start, stage2End, stage3Start, stage3End, stage4Start, stage4End, stage5Start, stage5End, cycle, functionalUnitID;
		public int stage1Cycle, stage2CycleStart, stage2CycleEnd, stage3CycleStart, stage3CycleEnd, stage4Cycle, stage5Cycle;
		public float floatResult;
		public string instrType;
		public string iOp1, iOp2, iOp3;
		public string fOp1, fOp2, fOp3;
		public int iOperand1, iOperand2, iOperand3, ASPR, dependantOpID1, dependantOpID2;
		public float fOperand1, fOperand2, fOperand3;
		public bool isFloat, executionInProgress, doneExecuting;
		public string assembly1, result;
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
			iOp1 = "";
			iOp2 = "";
			iOp3 = "";
			fOp1 = "";
			fOp2 = "";
			fOp3 = "";
			dependantOpID1 = -1;
			dependantOpiD2 = -1;
		}
	}
}
