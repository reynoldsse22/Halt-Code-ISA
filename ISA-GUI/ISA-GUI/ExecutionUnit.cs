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
            int floatingPoint = (instrFlag & 2) >> 1;          //gets the floating point bit from the first four bits 0X00
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
                        byte[] memoryFloat = new byte[4];
                        memoryFloat[3] = (byte)(memory.MainMemory[address]);                           //Loads the MSB value from the address in memory to f0
                        memoryFloat[2] = (byte)(memory.MainMemory[address + 1]);                       //Loads the TSB value from the address in memory to f0
                        memoryFloat[1] = (byte)(memory.MainMemory[address + 2]);                       //Loads the LSB value from the address in memory to f0
                        registers.floatRegisters[0] = System.BitConverter.ToSingle(memoryFloat, 0);
                    }
                    break;
                case 10:
                    if(floatingPoint != 1)
                    {
                        memory.MainMemory[address] = (byte)(registers.intRegisters[0] & 16711680);      //Stores the MSB value of r0 at the address in memory
                        memory.MainMemory[address + 1] = (byte)(registers.intRegisters[0] & 65280);     //Stores the TSB value of r0 at the address in memory
                        memory.MainMemory[address + 2] = (byte)(registers.intRegisters[0] & 255);       //Stores the LSB value of r0 at the address in memory
                    }
                    else
                    {
                        byte[] currentFloat = System.BitConverter.GetBytes(registers.floatRegisters[0]);        //Float to be stored
                        memory.MainMemory[address] = currentFloat[3];                                           //Stores the MSB value of f0 at the address in memory
                        memory.MainMemory[address + 1] = currentFloat[2];                                       //Stores the TSB value of f0 at the address in memory
                        memory.MainMemory[address + 2] = currentFloat[1];                                       //Stores the LSB value of f0 at the address in memory
                    }
                    break;
                case 11:
                    if (floatingPoint != 1)
                        registers.intRegisters[r1] = address;
                    else
                    {
                        byte[] floatArray = new byte[4];                //create a new array of 4 bytes to convert the low address to float
                                                                        //Must be read in back to front because BitConverter reads the first element of array as LSB
                        floatArray[0] = 0x00;                           //last byte is 0 because we don't use it
                        floatArray[1] = (byte)(address & 255);          //1 byte
                        floatArray[2] = (byte)((address >> 8) & 15);    //get first 4 bits by shifting right 8 times and ANDing with 0xF to get only those 4
                        floatArray[3] = 0x00;                           //first byte is 0 because we don't use them 
                        registers.floatRegisters[r1] = System.BitConverter.ToSingle(floatArray, 0);
                    }
                    break;
                case 12:
                    if(floatingPoint != 1)
                        registers.intRegisters[r1] += (address << 12);
                    else
                    {
                        byte[] backArray = System.BitConverter.GetBytes(registers.floatRegisters[r1]);      //old 12 bytes of array
                        byte[] floatArray = new byte[4];                //create a new array of 4 bytes to convert the low address to float
                                                                        //Must be read in back to front because BitConverter reads the first element of array as LSB
                        floatArray[0] = 0x00;                           //last byte is 0 because we don't use it
                        floatArray[1] = backArray[1];                   //the old elements are returned to their former spot
                        floatArray[2] = (byte)(((address & 15) << 4) | (int)backArray[2]);    //get first of 4 bits by ANDing with the address by 0xF
                                                                                              //and shifting left 4 (should be in form 0x_0)
                                                                                              //the second 4 are found by ORing with the old array
                        floatArray[3] = (byte)((address >> 4) & 255);   //get first byte by shifting right 4 and ANDing with 0xFF
                        registers.floatRegisters[r1] = System.BitConverter.ToSingle(floatArray, 0);
                    }
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
