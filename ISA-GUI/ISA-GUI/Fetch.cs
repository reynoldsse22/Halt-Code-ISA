// ---------------------------------------------------------------------------
// File name: Fetch.cs
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
   * Class Name: Fetch <br>
   * Class Purpose: Used to grab the next instruction
   * 
   * <hr>
   * Date created: 2/19/21 <br>
   * @author Samuel Reynolds
   */
    internal class Fetch
    {
        public bool occupied;
        public bool success;
        public bool inProgress;
        public bool hazardDetected;
        /**
	    * Method Name: Fetch <br>
	    * Method Purpose: Class constructor
	    * 
	    * <hr>
	    * Date created: 2/19/21 <br>
	    * @author Samuel Reynolds
	    */
        public Fetch()
        {
            occupied = false;
            success = false;
            inProgress = false;
            hazardDetected = false;
        }

        /**
		 * Method Name: getNextInstruction <br>
		 * Method Purpose: Goes to the instruction memory unit and fetches the next instruction (2 bytes)
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 *   @param  RegisterFile register
		 *   @param  InstructionMemory IM
		 *   @param  ConfigCycle config
		 */
        public Instruction getNextInstruction(ref RegisterFile registers, ref InstructionMemory IM, ref ConfigCycle config, ref Instruction[] stages, bool branchTaken, bool predictionSet)
        {
            occupied = true;
            inProgress = true;
            hazardDetected = false;
            Instruction instruction = new Instruction();
            if ((IM.ProgramCounter + 3) > IM.instructions.Count)
                return null;

            // BRANCH PREDICTION
            if(predictionSet && stages[1] != null && stages[2] != null)
            {
                if((stages[1].opcode >= 2 && stages[1].opcode <= 9) || (stages[2].opcode >= 2 && stages[2].opcode <= 9))
                {
                    if(branchTaken)
                    {
                        if (stages[2].address == IM.ProgramCounter) //if next instruction already in pipeline, increment the program counter by three to get next instruction
                        {
                            IM.ProgramCounter += 3;
                            goto getNext;
                        } 
                        instruction.programCounterValue = stages[1].address;
                        instruction.binInstruction[0] = IM.instructions[stages[1].address];
                        instruction.binInstruction[1] = IM.instructions[stages[1].address + 1];
                        instruction.binInstruction[2] = IM.instructions[stages[1].address + 2];
                        goto finishMethod;
                    }
                }
            }

            getNext:
            instruction.programCounterValue = IM.ProgramCounter;
            instruction.binInstruction[0] = IM.instructions[IM.ProgramCounter++];
            instruction.binInstruction[1] = IM.instructions[IM.ProgramCounter++];
            instruction.binInstruction[2] = IM.instructions[IM.ProgramCounter++];

            finishMethod:
            instruction.cycleControl = config.fetch;
            IM.CurrentInstruction = (instruction.binInstruction[2] + (instruction.binInstruction[1] << 8)+ (instruction.binInstruction[0] << 16));
            success = true;
            return instruction;
        }



        /**
		 * Method Name: getNextInstruction <br>
		 * Method Purpose: Goes to the instruction memory unit and fetches the next instruction (2 bytes)
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 *   @param  RegisterFile register
		 *   @param  InstructionMemory IM
		 *   @param  ConfigCycle config
		 
        public Instruction getNextInstructionDynamic(ref RegisterFile registers, ref InstructionMemory IM, ref ConfigCycle config, bool branchTaken)
        {
            occupied = true;
            inProgress = true;
            hazardDetected = false;
            Instruction instruction = new Instruction();
            if ((IM.ProgramCounter + 3) > IM.instructions.Count)
                return null;

            // BRANCH PREDICTION
            if (config.predictionSet)
            {
                if (branchTaken)
                {
                    instruction.programCounterValue = stages[1].address;
                    instruction.binInstruction[0] = IM.instructions[stages[1].address];
                    instruction.binInstruction[1] = IM.instructions[stages[1].address + 1];
                    instruction.binInstruction[2] = IM.instructions[stages[1].address + 2];
                }
                else
                {
                    instruction.programCounterValue = IM.ProgramCounter;
                    instruction.binInstruction[0] = IM.instructions[IM.ProgramCounter++];
                    instruction.binInstruction[1] = IM.instructions[IM.ProgramCounter++];
                    instruction.binInstruction[2] = IM.instructions[IM.ProgramCounter++];
                }
            }
            else
            {
                instruction.programCounterValue = IM.ProgramCounter;
                instruction.binInstruction[0] = IM.instructions[IM.ProgramCounter++];
                instruction.binInstruction[1] = IM.instructions[IM.ProgramCounter++];
                instruction.binInstruction[2] = IM.instructions[IM.ProgramCounter++];
            }

            instruction.cycleControl = config.fetch;
            IM.CurrentInstruction = (instruction.binInstruction[2] + (instruction.binInstruction[1] << 8) + (instruction.binInstruction[0] << 16));
            success = true;
            return instruction;
        }

        */


        /**
		 * Method Name: getNextInstruction <br>
		 * Method Purpose: Goes to the instruction memory unit and fetches the next instruction (2 bytes), solely used to get the next instruction
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 *   @param  RegisterFile register
		 *   @param  InstructionMemory IM
		 *   @param  ConfigCycle config
		 */
        public Instruction getNextInstruction(ref InstructionMemory IM)
        {
            occupied = true;
            inProgress = true;
            hazardDetected = false;
            Instruction instruction = new Instruction();
            if ((IM.ProgramCounter + 3) > IM.instructions.Count)
                return null;
            instruction.programCounterValue = IM.ProgramCounter;
            instruction.binInstruction[0] = IM.instructions[IM.ProgramCounter++];
            instruction.binInstruction[1] = IM.instructions[IM.ProgramCounter++];
            instruction.binInstruction[2] = IM.instructions[IM.ProgramCounter++];

            return instruction;
        }
    }
}
