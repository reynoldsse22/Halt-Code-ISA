// ---------------------------------------------------------------------------
// File name: ALU.cs
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
	* Class Name: ALU <br>
	* Class Purpose: To execute the Arithmetic and Logic instructions
	* 
	* <hr>
	* Date created: 2/19/21 <br>
	* @author Samuel Reynolds
	*/
    internal class ALU
    {
        /**
	    * Method Name: ALU <br>
	    * Method Purpose: Class constructor
	    * 
	    * <hr>
	    * Date created: 2/19/21 <br>
	    * @author Samuel Reynolds
	    */
        public ALU()
        {
        }

        /**
		 * Method Name: execute <br>
		 * Method Purpose: Executes a given R-Type instruction 
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
		 */
        public void execute(ref RegisterFile registers, ref DataMemory memory, ref ALU alu, ref InstructionMemory IM, ref Instruction instruction, ref ConfigCycle config)
        {
            int opcode = instruction.opcode;
            int r1 = instruction.r1;
            int r2 = instruction.r2;
            int r3 = instruction.r3;
            int address = instruction.address;
            int instrFlag = instruction.instrFlag;
            string instrType = instruction.instrType;
            int statusFlag = 0;
            int ASPR = instrFlag & 1;               //gets the ASPR bit from the first four bits 00X0
            bool zero = false;
            bool carry = false;
            switch (opcode)
            {
                case 13:
                    //Compare Immediate to Register
                    if (!instruction.isFloat)
                    {
                        instruction.cycleControl = config.intALU;
                        int compare = (address - registers.intRegisters[r1]);
                        if (compare > address && ASPR == 1)
                            carry = true;
                        if (compare == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                    {
                        instruction.cycleControl = config.flAddSub;
                        float compare = (address - registers.floatRegisters[r1]);
                        if (compare > address && ASPR == 1)
                            carry = true;
                        if (compare == 0 && ASPR == 1)
                            zero = true;
                    }
                    break;
                case 14:
                    //Compare Registers
                    if (!instruction.isFloat)
                    {
                        instruction.cycleControl = config.calcAddress;
                        int compare = (registers.intRegisters[r1] - registers.intRegisters[r2]);
                        if (compare > address && ASPR == 1)
                            carry = true;
                        if (compare == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                    {
                        float compare = (registers.floatRegisters[r1] - registers.intRegisters[r2]);
                        if (compare > address && ASPR == 1)
                            carry = true;
                        if (compare == 0 && ASPR == 1)
                            zero = true;
                    }
                    break;
                case 16:
                    //arithmetic shift left
                    if (!instruction.isFloat)
                    {
                        instruction.cycleControl = config.intALU;
                        instruction.destinationReg = r3;
                        instruction.intResult = (registers.intRegisters[r1] << registers.intRegisters[r2]);
                        if (instruction.intResult == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                        throw new Exception("Invalid Instruction!");
                    break;
                case 17:
                    //arithmetic shift right
                    if (!instruction.isFloat)
                    {
                        instruction.cycleControl = config.intALU;
                        instruction.destinationReg = r3;
                        instruction.intResult = (registers.intRegisters[r1] >> registers.intRegisters[r2]);
                        if (instruction.intResult == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                        throw new Exception("Invalid Instruction!");
                    break;
                case 18:
                    //logical shift left
                    if (!instruction.isFloat)
                    {
                        instruction.cycleControl = config.intALU;
                        instruction.destinationReg = r3;
                        int rotateBit;
                        rotateBit = (registers.intRegisters[r1] & 8388608) >> 23;
                        instruction.intResult = registers.intRegisters[r1] << registers.intRegisters[r2];
                        instruction.intResult = registers.intRegisters[r3] & 16777215;
                        instruction.intResult = registers.intRegisters[r3] | rotateBit;
                        if (instruction.intResult == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                        throw new Exception("Invalid Instruction!");
                    break;
                case 19:
                    //logical shift right
                    if (!instruction.isFloat)
                    {
                        instruction.cycleControl = config.intALU;
                        instruction.destinationReg = r3;
                        int rotateBit;
                        rotateBit = (registers.intRegisters[r1] & 1) << 23;
                        instruction.intResult = registers.intRegisters[r1] >> registers.intRegisters[r2];
                        instruction.intResult = registers.intRegisters[r3] & 16777215;
                        instruction.intResult = registers.intRegisters[r3] | rotateBit;
                        if (instruction.intResult == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                        throw new Exception("Invalid Instruction!");
                    break;
                case 20:
                    //Add
                    instruction.destinationReg = r3;
                    if (!instruction.isFloat)
                    {
                        instruction.cycleControl = config.intALU;
                        instruction.intResult = (registers.intRegisters[r1] + registers.intRegisters[r2]);
                        if (instruction.intResult < registers.intRegisters[r1] && ASPR == 1)
                            carry = true;
                        if (instruction.intResult == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                    {
                        instruction.cycleControl = config.flAddSub;
                        instruction.floatResult = (registers.floatRegisters[r1] + registers.floatRegisters[r2]);
                        if (instruction.floatResult < registers.floatRegisters[r1] && ASPR == 1)
                            carry = true;
                        if (instruction.floatResult == 0 && ASPR == 1)
                            zero = true;
                    }
                    break;
                case 21:
                    //Sub
                    instruction.destinationReg = r3;
                    if (!instruction.isFloat)
                    {
                        instruction.cycleControl = config.intALU;
                        instruction.intResult = (registers.intRegisters[r1] - registers.intRegisters[r2]);
                        if (instruction.intResult > registers.intRegisters[r1] && ASPR == 1)
                            carry = true;
                        if (instruction.intResult == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                    {
                        instruction.cycleControl = config.flAddSub;
                        instruction.floatResult = (registers.floatRegisters[r1] - registers.floatRegisters[r2]);
                        if (instruction.floatResult > registers.floatRegisters[r1] && ASPR == 1)
                            carry = true;
                        if (instruction.floatResult == 0 && ASPR == 1)
                            zero = true;
                    }
                    break;
                case 22:
                    //Multiply
                    instruction.destinationReg = r3;
                    if (!instruction.isFloat)
                    {
                        instruction.cycleControl = config.intALU;
                        instruction.intResult = registers.intRegisters[r1] * registers.intRegisters[r2];
                        if (instruction.intResult < registers.intRegisters[r1] && ASPR == 1)
                            carry = true;
                        if (instruction.intResult == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                    {
                        instruction.cycleControl = config.flMult;
                        instruction.floatResult = registers.floatRegisters[r1] * registers.floatRegisters[r2];
                        if (instruction.floatResult > registers.floatRegisters[r1] && ASPR == 1)
                            carry = true;
                        if (instruction.floatResult == 0 && ASPR == 1)
                            zero = true;
                    }
                    break;
                case 23:
                    //Divide
                    instruction.destinationReg = r3;
                    if (!instruction.isFloat)
                    {
                        instruction.cycleControl = config.intALU;
                        instruction.intResult = (int)registers.intRegisters[r1] / (int)registers.intRegisters[r2];
                        if (instruction.intResult > registers.intRegisters[r1] && ASPR == 1)
                            carry = true;
                        if (instruction.intResult == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                    {
                        instruction.cycleControl = config.flDiv;
                        instruction.floatResult = registers.floatRegisters[r1] / registers.floatRegisters[r2];
                        if (instruction.floatResult > registers.floatRegisters[r1] && ASPR == 1)
                            carry = true;
                        if (instruction.floatResult == 0 && ASPR == 1)
                            zero = true;
                    }
                    break;
                case 24:
                    //AND
                    if (!instruction.isFloat)
                    {
                        instruction.cycleControl = config.intALU;
                        instruction.destinationReg = r3;
                        instruction.intResult = (registers.intRegisters[r1] & registers.intRegisters[r2]);
                        if (instruction.intResult == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                        throw new Exception("Invalid Instruction!");
                    break;
                case 25:
                    //OR
                    if (!instruction.isFloat)
                    {
                        instruction.cycleControl = config.intALU;
                        instruction.destinationReg = r3;
                        instruction.intResult = (registers.intRegisters[r1] | registers.intRegisters[r2]);
                        if (instruction.intResult == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                        throw new Exception("Invalid Instruction!");
                    break;
                case 26:
                    //XOR
                    if (!instruction.isFloat)
                    {
                        instruction.cycleControl = config.intALU;
                        instruction.destinationReg = r3;
                        instruction.intResult = (registers.intRegisters[r1] ^ registers.intRegisters[r2]);
                        if (instruction.intResult == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                        throw new Exception("Invalid Instruction!");
                    break;
                case 27:
                    //NOT
                    if (!instruction.isFloat)
                    {
                        instruction.cycleControl = config.intALU;
                        instruction.destinationReg = r3;
                        instruction.intResult = (~registers.intRegisters[r1]);
                        if (instruction.intResult == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                        throw new Exception("Invalid Instruction!");
                    break;
            }

           // Determine the bit map for ASPR register
           if(ASPR == 1)
            {
                if (zero)
                    statusFlag += 2;
                if (carry)
                    statusFlag += 1;

                registers.ASPR = statusFlag;       //assign the ASPR register the bit map value
            }

        }
    }
}
