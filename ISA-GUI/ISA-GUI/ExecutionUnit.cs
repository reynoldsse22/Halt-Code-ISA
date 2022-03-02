// ---------------------------------------------------------------------------
// File name: ExecutionUnit.cs
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
   * Class Name: ExecutionUnit <br>
   * Class Purpose: To kickoff the pipeline process and update GUI elements
   * 
   * <hr>
   * Date created: 2/19/21 <br>
   * @author Samuel Reynolds
   */
    internal class ExecutionUnit
    {
        public bool occupied;
        public bool success;
        public bool inProgress;
        /**
	    * Method Name: ExecutionUnit <br>
	    * Method Purpose: Class constructor
	    * 
	    * <hr>
	    * Date created: 2/19/21 <br>
	    * @author Samuel Reynolds
	    */
        public ExecutionUnit()
        {
            occupied = false;
            success = false;
            inProgress = false;

        }

        /**
		 * Method Name: execute <br>
		 * Method Purpose: Executes a given instruction or passes along to ALU if needed
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 *   @param  RegisterFile registers
		 *   @param  DataMemory memory
		 *   @param  ALU alu
		 *   @param  InstructionMemory IM
		 *   @param  int opcode
		 *   @param  int r1
		 *   @param  int r2
		 *   @param  int r3
		 *   @param  int address
		 *   @param  int instrFlag
		 */
        public void execute(ref RegisterFile registers, ref DataMemory memory, ref ALU alu, ref InstructionMemory IM, 
                ref Instruction instruction, ref ConfigCycle config)
        {
            inProgress = true;
            occupied = true;
            int opcode = instruction.opcode;
            int r1 = instruction.r1;
            int r2 = instruction.r2;
            int r3 = instruction.r3;
            int address = instruction.address;
            int instrFlag = instruction.instrFlag;
            string instrType = instruction.instrType;
            int ASPR = instrFlag & 1;                           //gets the ASPR bit from the first four bits 00X0

            switch (opcode)
            { 
                //CONTROL INSTRUCTIONS
                case 0:
                    instruction.cycleControl = config.calcAddress;
                    instruction.cycleControl--;
                    return;                                     //HALT
                case 1:
                    instruction.cycleControl = 0;
                    return;                                     //No Operation
                case 2:
                    instruction.cycleControl = config.calcAddress;
                    IM.ProgramCounter = address;                            //Move the branching address into the program counter/instruction pointer
                    break;
                case 3:
                    instruction.cycleControl = config.calcAddress;
                    if ((registers.ASPR & 2) == 0)
                        IM.ProgramCounter = address;                        //Move the BNE address into the program counter/instruction pointer
                    break;
                case 4:
                    instruction.cycleControl = config.calcAddress;
                    if ((registers.ASPR & 2) == 1)
                        IM.ProgramCounter = address;                        //Move the BEQ address into the program counter/instruction pointer
                    break;
                case 5:
                    instruction.cycleControl = config.calcAddress;
                    if ((registers.ASPR & 1) == 1 && (registers.ASPR & 1) == 0)
                        IM.ProgramCounter = address;                        //Move the BLT address into the program counter/instruction pointer
                    break;
                case 6:
                    instruction.cycleControl = config.calcAddress;
                    if ((registers.ASPR & 2) == 1 || (registers.ASPR & 1) == 1)
                        IM.ProgramCounter = address;                        //Move the BLE address into the program counter/instruction pointer
                    break;
                case 7:
                    instruction.cycleControl = config.calcAddress;
                    if ((registers.ASPR & 2) == 0 && (registers.ASPR & 1) == 0)
                        IM.ProgramCounter = address;                        //Move the BGT address into the program counter/instruction pointer
                    break;
                case 8:
                    instruction.cycleControl = config.calcAddress;
                    if (((registers.ASPR & 2) == 0 || (registers.ASPR & 1) == 0) || ((registers.ASPR & 2) == 1 || (registers.ASPR & 1) == 0))
                        IM.ProgramCounter = address;                        //Move the BGE address into the program counter/instruction pointer
                    break;

                //MEMORY INSTRUCTIONS
                case 9:
                    instruction.cycleControl = config.load_store;
                    instruction.destinationReg = 0;
                    break;
                case 10:
                    instruction.cycleControl = config.load_store;
                    break;
                case 11:
                    instruction.cycleControl = config.load_store;
                    instruction.destinationReg = r3;
                    break;
                case 12:
                    instruction.cycleControl = config.load_store;
                    instruction.destinationReg = r3;
                    break;
                case 13:
                case 14:
                    alu.execute(ref registers, ref memory, ref alu, ref IM, ref instruction, ref config);
                    break;
                case 15:
                    instruction.cycleControl = config.calcAddress;
                    instruction.destinationReg = r3;
                    if (!instruction.isFloat)
                        instruction.intResult = registers.intRegisters[r2];
                    else
                        instruction.floatResult = registers.floatRegisters[r2];
                    break;
                

                //ALU INSTRUCTIONS
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                    alu.execute(ref registers, ref memory, ref alu, ref IM, ref instruction, ref config);    //Transfer to the ALU
                    break;
            }
            success = true;
            instruction.cycleControl--;
            return;
        }
    }


}
