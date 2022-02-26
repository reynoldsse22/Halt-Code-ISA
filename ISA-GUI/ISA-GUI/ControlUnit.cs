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
        public int instructionsProcessed;

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
        }

        /**
		 * Method Name: decode <br>
		 * Method Purpose: Decodes the current instruction and splits up its opcode/operands/address/values
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 *   @param  InstructionMemory
		 *   @param  int opcode
		 *   @param  int r1
		 *   @param  int r2
		 *   @param  int r3
		 *   @param  int address
		 *   @param  string instrType
		 */
        public void decode(ref InstructionMemory IM, byte[] instruct, out int opcode, out int r1, out int r2, out int r3, out int address, out string instrType, out int instrFlag)
        {
            //value is -1 if not used in the current instruction
            int nibble1, temp;
            address = -1;
            r1 = -1;
            r2 = -1;
            r3 = -1;
            instrType = "";

            byte MSB = instruct[0];
            byte TSB = instruct[1];
            byte LSB = instruct[2];

            instrFlag = MSB >> 5;
            opcode = MSB & 31;

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
                    case 15:
                        r1 = -1;
                        r2 = r3;
                        r3 = LSB & 15;
                        address = -1;
                        break;
                    
                }
                memoryInstructionCount++;
                if ((IM.ProgramCounter / 3) > instructionsProcessed)
                    instructionsProcessed++;
                totalInstructions++;
            }
            else
            {
                throw new Exception("Invalid Instruction!");
            }

            
        }



    }
}
