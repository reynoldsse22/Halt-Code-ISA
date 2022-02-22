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
		 */
        public bool execute(ref RegisterFile registers, ref DataMemory memory, ref ALU alu, ref InstructionMemory IM, in int opcode, in int r1, in int r2, in int r3, in int address)
        {
            switch(opcode)
            {
                case 0:
                    return true;                                                    //HALT
                case 1:
                    IM.ProgramCounter = (ushort)address;                            //Move the branching address into the program counter/instruction pointer
                    break;
                case 2:
                    if ((registers.registers[13] & 2) == 0)
                        IM.ProgramCounter = (ushort)address;                        //Move the BNE address into the program counter/instruction pointer
                    break;
                case 3:
                    if ((registers.registers[13] & 2) == 1)
                        IM.ProgramCounter = (ushort)address;                        //Move the BEQ address into the program counter/instruction pointer
                    break;
                case 4:
                    registers.registers[0] = memory.MainMemory[address];            //Loads the value from the address in memory to r0
                    break;
                case 5:
                    memory.MainMemory[address] = (byte)registers.registers[0];      //Stores the value of r0 at the address in memory
                    break;
                case 6:
                    registers.registers[r3] = registers.registers[r1];              //Moves a value from one register to another
                    break;
                case 7:
                    registers.registers[0] = (ushort)address;                       //loads an immediate into r0
                    break;
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    alu.execute(ref registers, ref memory, ref alu, ref IM, in opcode, in r1, in r2, in r3, in address);    //Transfer to the ALU
                    break;
            }
            return false;
        }
    }


}
