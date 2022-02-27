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
        public bool execute(ref RegisterFile registers, ref DataMemory memory, ref ALU alu, ref InstructionMemory IM, in int opcode, in int r1, in int r2, in int r3, in int address, in int instrFlag)
        {
            int floatingPoint = instrFlag & 2;          //gets the floating point bit from the first four bits 0X00
            int ASPR = instrFlag & 1;                   //gets the ASPR bit from the first four bits 00X0

            switch (opcode)
            { 
                //CONTROL INSTRUCTIONS
                case 0:
                    return true;                                                    //HALT
                case 1:
                    return false;                                                   //No Operation
                case 2:
                    IM.ProgramCounter = address;                            //Move the branching address into the program counter/instruction pointer
                    break;
                case 3:
                    if ((registers.ASPR & 2) == 0)
                        IM.ProgramCounter = address;                        //Move the BNE address into the program counter/instruction pointer
                    break;
                case 4:
                    if ((registers.ASPR & 2) == 1)
                        IM.ProgramCounter = address;                        //Move the BEQ address into the program counter/instruction pointer
                    break;
                case 5:
                    if((registers.ASPR & 1) == 1 && (registers.ASPR & 1) == 0)
                        IM.ProgramCounter = address;                        //Move the BLT address into the program counter/instruction pointer
                    break;
                case 6:
                    if ((registers.ASPR & 2) == 1 || (registers.ASPR & 1) == 1)
                        IM.ProgramCounter = address;                        //Move the BLE address into the program counter/instruction pointer
                    break;
                case 7:
                    if ((registers.ASPR & 2) == 0 && (registers.ASPR & 1) == 0)
                        IM.ProgramCounter = address;                        //Move the BGT address into the program counter/instruction pointer
                    break;
                case 8:
                    if (((registers.ASPR & 2) == 0 || (registers.ASPR & 1) == 0) || ((registers.ASPR & 2) == 1 || (registers.ASPR & 1) == 0))
                        IM.ProgramCounter = address;                        //Move the BGE address into the program counter/instruction pointer
                    break;

                //MEMORY INSTRUCTIONS
                case 9:
                    if(floatingPoint != 1)
                    {
                        registers.intRegisters[0] = memory.MainMemory[address] << 16;            //Loads the MSB value from the address in memory to r0
                        registers.intRegisters[0] += (memory.MainMemory[address + 1] << 8);      //Loads the TSB value from the address in memory to r0
                        registers.intRegisters[0] += (memory.MainMemory[address + 2]);           //Loads the LSB value from the address in memory to r0
                    }
                    else
                    {
                        registers.floatRegisters[0] = memory.MainMemory[address] << 16;            //Loads the MSB value from the address in memory to f0
                        registers.floatRegisters[0] += (memory.MainMemory[address + 1] << 8);      //Loads the TSB value from the address in memory to f0
                        registers.floatRegisters[0] += (memory.MainMemory[address + 2]);           //Loads the LSB value from the address in memory to f0
                    }
                    break;
                case 10:
                    if(floatingPoint != 1)
                    {
                        memory.MainMemory[address] = (byte)(registers.intRegisters[r1] & 16711680);      //Stores the MSB value of r0 at the address in memory
                        memory.MainMemory[address + 1] = (byte)(registers.intRegisters[r1] & 65280);     //Stores the TSB value of r0 at the address in memory
                        memory.MainMemory[address + 2] = (byte)(registers.intRegisters[r1] & 255);       //Stores the LSB value of r0 at the address in memory
                    }
                    else
                    {
                        //memory.MainMemory[address] = (byte)(registers.floatRegisters[0] & (float)16711680);      //Stores the MSB value of f0 at the address in memory
                        //memory.MainMemory[address + 1] = (byte)(registers.floatRegisters[0] & (float)65280);     //Stores the TSB value of f0 at the address in memory
                        //memory.MainMemory[address + 2] = (byte)(registers.floatRegisters[0] & (float)255);       //Stores the LSB value of f0 at the address in memory
                    }
                    break;
                case 11:
                    if (floatingPoint != 1)
                        registers.intRegisters[r1] = address;
                    else
                        registers.floatRegisters[r1] = address;
                    break;
                case 12:
                    if(floatingPoint != 1)
                        registers.intRegisters[r1] += (address << 12);
                    else
                        registers.floatRegisters[r1] += (address << 12);
                    break;
                case 13:
                case 14:
                    alu.execute(ref registers, ref memory, ref alu, ref IM, in opcode, in r1, in r2, in r3, in address, in instrFlag);
                    break;
                case 15:
                    if(floatingPoint != 1)
                        registers.intRegisters[r3] = registers.intRegisters[r2];              //Moves a value from one register to another
                    else
                        registers.floatRegisters[r3] = registers.floatRegisters[r2];
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
                    alu.execute(ref registers, ref memory, ref alu, ref IM, in opcode, in r1, in r2, in r3, in address, in instrFlag);    //Transfer to the ALU
                    break;
            }
            return false;
        }
    }


}
