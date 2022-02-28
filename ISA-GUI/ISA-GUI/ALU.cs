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
        public void execute(ref RegisterFile registers, ref DataMemory memory, ref ALU alu, ref InstructionMemory IM, in int opcode, in int r1, in int r2, in int r3, in int address, in int instrFlag)
        {
            int statusFlag = 0;
            int floatingPoint = (instrFlag & 2) >> 1;      //gets the floating point bit from the first four bits 0X00
            int ASPR = instrFlag & 1;               //gets the ASPR bit from the first four bits 00X0
            bool zero = false;
            bool carry = false;
            switch (opcode)
            {
                case 13:
                    if (floatingPoint != 1)
                    {
                        int compare = (address - registers.intRegisters[r1]);
                        if (compare > address && ASPR == 1)
                            carry = true;
                        if (compare == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                    {
                        float compare = (address - registers.floatRegisters[r1]);
                        if (compare > address && ASPR == 1)
                            carry = true;
                        if (compare == 0 && ASPR == 1)
                            zero = true;
                    }
                    break;
                case 14:
                    if (floatingPoint != 1)
                    {
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
                    if (floatingPoint!= 1)
                    {
                        registers.intRegisters[r3] = (registers.intRegisters[r1] << registers.intRegisters[r2]);
                        if (registers.intRegisters[r3] == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                        throw new Exception("Invalid Instruction!");
                    break;
                case 17:
                    //arithmetic shift right
                    if (floatingPoint != 1)
                    {
                        registers.intRegisters[r3] = (registers.intRegisters[r1] >> registers.intRegisters[r2]);
                        if (registers.intRegisters[r3] == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                        throw new Exception("Invalid Instruction!");
                    break;
                case 18:
                    //logical shift left
                    if (floatingPoint != 1)
                    {
                        int rotateBit;
                        rotateBit = (registers.intRegisters[r1] & 8388608) >> 23;
                        registers.intRegisters[r3] = registers.intRegisters[r1] << registers.intRegisters[r2];
                        registers.intRegisters[r3] = registers.intRegisters[r3] & 16777215;
                        registers.intRegisters[r3] = registers.intRegisters[r3] | rotateBit;
                        if (registers.intRegisters[r3] == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                        throw new Exception("Invalid Instruction!");
                    break;
                case 19:
                    //logical shift right
                    if (floatingPoint != 1)
                    {
                        int rotateBit;
                        rotateBit = (registers.intRegisters[r1] & 1) << 23;
                        registers.intRegisters[r3] = registers.intRegisters[r1] >> registers.intRegisters[r2];
                        registers.intRegisters[r3] = registers.intRegisters[r3] & 16777215;
                        registers.intRegisters[r3] = registers.intRegisters[r3] | rotateBit;
                        if (registers.intRegisters[r3] == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                        throw new Exception("Invalid Instruction!");
                    break;
                case 20:
                    //Add
                    if (floatingPoint != 1)
                    {
                        registers.intRegisters[r3] = (registers.intRegisters[r1] + registers.intRegisters[r2]);
                        if (registers.intRegisters[r3] < registers.intRegisters[r1] && ASPR == 1)
                            carry = true;
                        if (registers.intRegisters[r3] == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                    {
                        registers.floatRegisters[r3] = (registers.floatRegisters[r1] + registers.floatRegisters[r2]);
                        if (registers.floatRegisters[r3] < registers.floatRegisters[r1] && ASPR == 1)
                            carry = true;
                        if (registers.floatRegisters[r3] == 0 && ASPR == 1)
                            zero = true;
                    }
                    break;
                case 21:
                    //Sub
                    if (floatingPoint != 1)
                    {
                        registers.intRegisters[r3] = (registers.intRegisters[r1] - registers.intRegisters[r2]);
                        if (registers.intRegisters[r3] > registers.intRegisters[r1] && ASPR == 1)
                            carry = true;
                        if (registers.intRegisters[r3] == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                    {
                        registers.floatRegisters[r3] = (registers.floatRegisters[r1] - registers.floatRegisters[r2]);
                        if (registers.floatRegisters[r3] > registers.floatRegisters[r1] && ASPR == 1)
                            carry = true;
                        if (registers.floatRegisters[r3] == 0 && ASPR == 1)
                            zero = true;
                    }
                    break;
                case 22:
                    //Multiply
                    if (floatingPoint != 1)
                    {
                        registers.intRegisters[r3] = registers.intRegisters[r1] * registers.intRegisters[r2];
                    }
                    else
                    {
                        registers.floatRegisters[r3] = registers.floatRegisters[r1] * registers.floatRegisters[r2];
                    }
                    break;
                case 23:
                    //Divide
                    if (floatingPoint != 1)
                    {
                        registers.intRegisters[r3] = (int)registers.intRegisters[r1] / (int)registers.intRegisters[r2];
                    }
                    else
                    {
                        registers.floatRegisters[r3] = (int)registers.floatRegisters[r1] / (int)registers.floatRegisters[r2];
                    }
                    break;
                case 24:
                    //AND
                    if (floatingPoint != 1)
                    {
                        registers.intRegisters[r3] = (registers.intRegisters[r1] & registers.intRegisters[r2]);
                        if (registers.intRegisters[r3] == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                        throw new Exception("Invalid Instruction!");
                    break;
                case 25:
                    //OR
                    if (floatingPoint != 1)
                    {
                        registers.intRegisters[r3] = (registers.intRegisters[r1] | registers.intRegisters[r2]);
                        if (registers.intRegisters[r3] == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                        throw new Exception("Invalid Instruction!");
                    break;
                case 26:
                    //XOR
                    if (floatingPoint != 1)
                    {
                        registers.intRegisters[r3] = (registers.intRegisters[r1] ^ registers.intRegisters[r2]);
                        if (registers.intRegisters[r3] == 0 && ASPR == 1)
                            zero = true;
                    }
                    else
                        throw new Exception("Invalid Instruction!");
                    break;
                case 27:
                    //NOT
                    if (floatingPoint != 1)
                    {
                        registers.intRegisters[r3] = (~registers.intRegisters[r1]);
                        if (registers.intRegisters[r3] == 0 && ASPR == 1)
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
