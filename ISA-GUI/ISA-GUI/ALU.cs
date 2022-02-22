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
        public void execute(ref RegisterFile registers, ref DataMemory memory, ref ALU alu, ref InstructionMemory IM, in int opcode, in int r1, in int r2, in int r3, in int address)
        {
            int statusFlag = 0;
            bool zero = false;
            bool carry = false;
            switch (opcode)
            {
                case 8:
                    //shift left
                    registers.registers[r3] = (ushort)(registers.registers[r1] << registers.registers[r2]);
                    if (registers.registers[r3] == 0)
                        zero = true;
                    break;
                case 9:
                    //shift right
                    registers.registers[r3] = (ushort)(registers.registers[r1] >> registers.registers[r2]);
                    if (registers.registers[r3] == 0)
                        zero = true;
                    break;
                case 10:
                    //Add
                    registers.registers[r3] = (ushort)(registers.registers[r1] + registers.registers[r2]);
                    if (registers.registers[r3] < registers.registers[r1])
                        carry = true;
                    if (registers.registers[r3] == 0)
                        zero = true;
                    break;
                case 11:
                    //Sub
                    registers.registers[r3] = (ushort)(registers.registers[r1] - registers.registers[r2]);
                    if (registers.registers[r3] > registers.registers[r1])
                        carry = true;
                    if (registers.registers[r3] == 0)
                        zero = true;
                    break;
                case 12:
                    //And
                    registers.registers[r3] = (ushort)(registers.registers[r1] & registers.registers[r2]);
                    if (registers.registers[r3] == 0)
                        zero = true;
                    break;
                case 13:
                    //Or
                    registers.registers[r3] = (ushort)(registers.registers[r1] | registers.registers[r2]);
                    if (registers.registers[r3] == 0)
                        zero = true;
                    break;
                case 14:
                    //Xor
                    registers.registers[r3] = (ushort)(registers.registers[r1] ^ registers.registers[r2]);
                    if (registers.registers[r3] == 0)
                        zero = true;
                    break;
                case 15:
                    //Not
                    registers.registers[r3] = (ushort)(~registers.registers[r1]);
                    if (registers.registers[r3] == 0)
                        zero = true;
                    break;
            }

           // Determine the bit map for ASPR register

            if (zero)
                statusFlag += 2;
            if (carry)
                statusFlag += 1;

            registers.registers[13] = (ushort)statusFlag;       //assign the ASPR register the bit map value
        }
    }
}
