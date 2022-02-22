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
        public void decode(ref InstructionMemory IM, out int opcode, out int r1, out int r2, out int r3, out int address, out string instrType)
        {
            //value is -1 if not used in the current instruction
            int nibble1, temp;
            address = -1;
            r1 = -1;
            r2 = -1;
            r3 = -1;
            instrType = "";

            byte MSB = (byte)(IM.CurrentInstruction >> 8);
            byte LSB = (byte)(IM.CurrentInstruction & 255);

            opcode = MSB >> 4;

            if(opcode == 0) //Halt instruction
            {
                instrType = "Control";
                controlInstructionCount++;
                if ((IM.ProgramCounter/2) > instructionsProcessed)
                    instructionsProcessed++;
                totalInstructions++;
            }
            else if(opcode >> 2 == 0) //C-type instructions
            {
                instrType = "Control";
                nibble1 = MSB & 15;       //Gets the second nibble from the first byte and combines it with the second byte 
                address = (nibble1 << 8) | LSB;
                controlInstructionCount++;
                if ((IM.ProgramCounter / 2) > instructionsProcessed)
                    instructionsProcessed++;
                totalInstructions++;
            }
            else if(opcode >> 2 == 1)  //I-type instructions
            {
                instrType = "Memory";
                nibble1 = MSB & 15;    //Gets first register from the first byte
                address = (nibble1 << 8) | LSB;    //Finds Address of the Load/Store instructions
                r3 = LSB & 15;

                switch (opcode)
                {
                    case 4:
                    case 5:
                        r3 = 0;
                        break;
                    case 6:
                        r1 = 0;
                        nibble1 = r3;
                        r2 = LSB >> 4;    //MOV is a special instruction that has a destination register instead of an address
                        r3 = nibble1;
                        address = -1;
                        break;
                    case 7:
                        r1 = -1;
                        r2 = -1;
                        r3 = 0;
                        break;
                }
                memoryInstructionCount++;
                if ((IM.ProgramCounter / 2) > instructionsProcessed)
                    instructionsProcessed++;
                totalInstructions++;
            }
            else if(opcode >> 3 == 1) //R-type instructions
            {
                instrType = "ALU";
                temp = opcode & 15; //Gets the first 3 bits of the opcode... Not used
                r1 = MSB & 15;    //Gets the first, second, and destination registers for the R-Type instruction
                r2 = LSB >> 4;
                r3 = LSB & 15;
                ALUInstructionCount++;
                if ((IM.ProgramCounter / 2) >= instructionsProcessed)
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
