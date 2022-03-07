// ---------------------------------------------------------------------------
// File name: AccessMemory.cs
// Project name: ISA-GUI
// ---------------------------------------------------------------------------
// Creators: Samuel Reynolds, Nick Farmer, Carlos Ortiz, & Brandon Beaudry						
// Course-Section: CSCI 4717-201
// Creation Date: 2/27/22		
// ---------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISA_GUI
{
	/**
	* Class Name: RegisterFile <br>
	* Class Purpose: Holds Main Memory
	* 
	* <hr>
	* Date created: 2/27/21 <br>
	* @author Samuel Reynolds
	*/
	internal class AccessMemory
    {
		public bool occupied;
		public bool success;
		public bool inProgress;
		public bool hazardDetected;

		/**
	    * Method Name: AccessMemory <br>
	    * Method Purpose: Class constructor
	    * 
	    * <hr>
	    * Date created: 2/27/21 <br>
	    * @author Samuel Reynolds
	    */
		public AccessMemory()
		{
			occupied = false;
			success = false;
			inProgress = false;
			hazardDetected = false;
		}

		/**
		 * Method Name: accessMemory <br>
		 * Method Purpose: Sets the stage for accessing and writing to main memory
		 * 
		 * <br>
		 * Date created: 3/2/22 <br>
		 * <hr>
		 *   @param  DataMemory memory
		 *   @param  RegisterFile registers
		 *   @param Instruction instruction
		 *   @param	ConfigCycle config
		 */
		public void accessMemory(ref DataMemory memory, ref RegisterFile registers, ref Instruction instruction, ref ConfigCycle config)
        {
			inProgress = true;
			occupied = true;
			hazardDetected = false;
			switch (instruction.opcode)
            {
				case 9:
					instruction.cycleControl = config.memAccess;
					if (!instruction.isFloat)
                    {
						instruction.intResult = memory.MainMemory[instruction.address] << 16;            //Loads the MSB value from the address in memory to r0
						instruction.intResult += (memory.MainMemory[instruction.address + 1] << 8);      //Loads the TSB value from the address in memory to r0
						instruction.intResult += (memory.MainMemory[instruction.address + 2]);           //Loads the LSB value from the address in memory to r0
						registers.intRegisters[0] = instruction.intResult;
					}
                    else
                    {
						byte[] memoryFloat = new byte[4];
						memoryFloat[3] = (byte)(memory.MainMemory[instruction.address]);                           //Loads the MSB value from the address in memory to f0
						memoryFloat[2] = (byte)(memory.MainMemory[instruction.address + 1]);                       //Loads the TSB value from the address in memory to f0
						memoryFloat[1] = (byte)(memory.MainMemory[instruction.address + 2]);                       //Loads the LSB value from the address in memory to f0
						instruction.floatResult = System.BitConverter.ToSingle(memoryFloat, 0);
						registers.floatRegisters[0] = instruction.floatResult;
					}
					break;
				case 10:
					instruction.cycleControl = config.memAccess;
					if (!instruction.isFloat)
                    {
						memory.MainMemory[instruction.address] = (byte)(registers.intRegisters[0] & 16711680);      //Stores the MSB value of r0 at the address in memory
						memory.MainMemory[instruction.address + 1] = (byte)(registers.intRegisters[0] & 65280);     //Stores the TSB value of r0 at the address in memory
						memory.MainMemory[instruction.address + 2] = (byte)(registers.intRegisters[0] & 255);       //Stores the LSB value of r0 at the address in memory
					}
                    else
                    {
						byte[] currentFloat = System.BitConverter.GetBytes(registers.floatRegisters[0]);        //Float to be stored
						memory.MainMemory[instruction.address] = currentFloat[3];                                           //Stores the MSB value of f0 at the address in memory
						memory.MainMemory[instruction.address + 1] = currentFloat[2];                                       //Stores the TSB value of f0 at the address in memory
						memory.MainMemory[instruction.address + 2] = currentFloat[1];                                       //Stores the LSB value of f0 at the address in memory
					}
					break;
				default:
					instruction.cycleControl = config.calcAddress;
					break;
			}
			success = true;
        }

	}
}
