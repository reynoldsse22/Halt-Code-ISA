// ---------------------------------------------------------------------------
// File name: ReorderBuffer.cs
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
using System.Collections;

namespace ISA_GUI
{
	/**
	* Class Name: ReorderBuffer <br>
	* Class Purpose: 
	* 
	* <hr>
	* Date created: 4/1/22 <br>
	* @author Samuel Reynolds, Nick Farmer, Carlos Ortiz, & Brandon Beaudry	
	*/
	internal class ReorderBuffer
	{
		public List<Instruction> reorderBuffer;
		public int reorderIndex;
		public bool oneCommitPerCycle;		//The reorder buffer should only commit one instruction per cycle. Up for change
		/**
	    * Method Name: ReorderBuffer <br>
	    * Method Purpose: Class constructor
	    * 
	    * <hr>
	    * Date created: 4/1/22 <br>
	    * @author Samuel Reynolds, Nick Farmer, Carlos Ortiz, & Brandon Beaudry	
	    */
		public ReorderBuffer()
		{
			reorderBuffer = new List<Instruction>();
			oneCommitPerCycle = true;
			reorderIndex = 1;
		}


		public void addToReorderBuffer(Instruction inst, ref ConfigCycle config)
        {
			if (reorderBuffer.Count == config.reorderbuffersize)
				throw new Exception();
			else
				reorderBuffer.Add(inst);
			return;
        }

		public int checkCommit(Instruction inst, ref WriteResult WR, ref DataMemory memory, ref bool branchTaken, ref InstructionMemory IM, 
			ref RegisterFile registers, ref bool halted, ref CommonDataBus CDB, ref DataCache DC)
        {
			if(reorderIndex == inst.ID && oneCommitPerCycle)
            {
				WR.commit(ref registers, ref inst, ref memory, ref halted, ref IM, ref branchTaken, ref DC);
				reorderIndex++;
				oneCommitPerCycle = false;
				return removeFromReorderBuffer(inst, ref CDB);
            }
			return -1;
        }

        public int removeFromReorderBuffer(Instruction instruction, ref CommonDataBus CDB)
        {
			reorderBuffer.RemoveAt(reorderBuffer.FindIndex(Instruction => Instruction.ID == instruction.ID));
			return instruction.ID;
        }

		public void removeAllInstructionsAfterHazard(int id)
        {
			foreach(Instruction inst in reorderBuffer.ToList())
            {
				if(inst.ID > id)
                {
					reorderBuffer.Remove(inst);
                }
            }
        }
	}
}

