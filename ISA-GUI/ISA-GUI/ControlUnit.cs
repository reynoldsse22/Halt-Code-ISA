// ---------------------------------------------------------------------------
// File name: ControlUnit.cs
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
	* Class Name: BUC10 <br>
	* Class Purpose: Used to decode an instruction
	* 
	* <hr>
	* Date created: 2/19/21 <br>
	* @author Samuel Reynolds
	*/
    internal class ControlUnit
    {
        public  int controlInstructionCount;
        public  int ALUInstructionCount;
        public  int memoryInstructionCount;
        public  int totalInstructions;
        public  int instructionsProcessed;
        public  bool occupied;
        public bool success;
        public bool inProgress;

        /**
	    * Method Name: ControlUnit <br>
	    * Method Purpose: Class constructor
	    * 
	    * <hr>
	    * Date created: 2/19/21 <br>
	    * @author Samuel Reynolds
	    */
        public ControlUnit()
        {
            controlInstructionCount = 0;
            ALUInstructionCount = 0;
            memoryInstructionCount = 0;
            totalInstructions = 0; 
            occupied = false;
            success = false;
            inProgress = false;
        }

        /**
		 * Method Name: decode <br>
		 * Method Purpose: Decodes the current instruction and splits up its opcode/operands/address/values
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 *   @param  InstructionMemory IM
		 *   @param  Instruction instruction
		 *   @param  ConfigCycle config
		 */
        public void decode(ref InstructionMemory IM, ref Instruction instruction, ref ConfigCycle config)
        {
            inProgress = true;
            occupied = true;
            instruction.cycleControl = config.regAccess;
            int opcode = instruction.opcode;
            int r1 = instruction.r1;
            int r2 = instruction.r2;
            int r3 = instruction.r3;
            int address = instruction.address;
            int instrFlag = instruction.instrFlag;
            string instrType = instruction.instrType;
            //value is -1 if not used in the current instruction
            int nibble1, temp;
            address = -1;
            r1 = -1;
            r2 = -1;
            r3 = -1;
            instrType = "";

            byte MSB = instruction.binInstruction[0];
            byte TSB = instruction.binInstruction[1];
            byte LSB = instruction.binInstruction[2];

            instrFlag = MSB >> 5;
            opcode = MSB & 31;

            if (((instrFlag & 2) >> 1) == 1)
                instruction.isFloat = true;
            else
                instruction.isFloat = false;

            if(opcode == 0) //Halt instruction
            {
                instrType = "Control";
                controlInstructionCount++;
                if ((IM.ProgramCounter/3) > instructionsProcessed)
                    instructionsProcessed++;
                totalInstructions++;
            }
            else if(opcode == 1)
            {
                //No Op Instruction
            }
            else if(opcode >> 3 == 0 || (opcode & 31) == 8) //C-type instructions
            {
                instrType = "Control";
                nibble1 = MSB & 15;       //Gets the second nibble from the first byte and combines it with the second byte 
                address = (TSB << 8) | LSB;

                //if register above 6 is discovered, decrease by 7 to target floating point register
                if (r1 > 6)
                    r1 -= 7;
                if (r2 > 6)
                    r2 -= 7;
                if (r3 > 6)
                    r3 -= 7;

                controlInstructionCount++;
                if ((IM.ProgramCounter / 3) > instructionsProcessed)
                    instructionsProcessed++;
                totalInstructions++;
            }
            else if(opcode >> 4 == 1) //R-type instructions
            {
                instrType = "ALU";
                temp = opcode & 15; //Gets the first 3 bits of the opcode... Not used
                r1 = TSB;    //Gets the first, second, and destination registers for the R-Type instruction
                r2 = (LSB & 240) >> 4;
                r3 = LSB & 15;

                //if register above 6 is discovered, decrease by 7 to target floating point register
                if (r1 > 6)
                    r1 -= 7;
                if (r2 > 6)
                    r2 -= 7;
                if (r3 > 6)
                    r3 -= 7;

                ALUInstructionCount++;
                if ((IM.ProgramCounter / 3) >= instructionsProcessed)
                    instructionsProcessed++;
                totalInstructions++;
            }
            else if(opcode >> 3 == 1)
            {
                instrType = "Memory";
                //nibble1 = MSB & 15;    //Gets first register from the first byte
                address = (TSB << 8) | LSB;    //Finds Address of the Load/Store instructions
                r3 = LSB >> 4;

                switch (opcode)
                {
                    case 9:
                    case 10:
                        r3 = 0;
                        break;
                    case 11:
                    case 12:
                    case 13:
                        r1 = -1;
                        r2 = -1;
                        r3 = (TSB & 240) >> 4;
                        address = address & 4095;
                        break;
                    case 14:
                        r1 = r3;
                        r2 = LSB & 15;
                        r3 = -1;
                        address = -1;
                        break;
                    case 15:
                        r1 = -1;
                        r2 = r3;
                        r3 = LSB & 15;
                        address = -1;
                        break;
                    
                }
                //if register above 6 is discovered, decrease by 7 to target floating point register
                if (r1 > 6)
                    r1 -= 7;
                if (r2 > 6)
                    r2 -= 7;
                if (r3 > 6)
                    r3 -= 7;

                memoryInstructionCount++;
                if ((IM.ProgramCounter / 3) > instructionsProcessed)
                    instructionsProcessed++;
                totalInstructions++;
            }
            else
            {
                throw new Exception("Invalid Instruction!");
            }

            instruction.opcode = opcode; 
            instruction.r1 = r1;
            instruction.r2 = r2;
            instruction.r3 = r3;
            instruction.instrFlag = instrFlag;
            instruction.address = address;
            instruction.instrType = instrType;
            instruction.cycleControl--;
            success = true;
            return;
        }
    }
}
