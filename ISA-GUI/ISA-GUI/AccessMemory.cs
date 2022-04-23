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

		public int startingAddress;

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

			startingAddress = 0;
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
						memory.MainMemory[instruction.address] = (byte)((registers.intRegisters[0] & 16711680) >> 16);      //Stores the MSB value of r0 at the address in memory
						memory.MainMemory[instruction.address + 1] = (byte)((registers.intRegisters[0] & 65280) >> 8);     //Stores the TSB value of r0 at the address in memory
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
					instruction.cycleControl = 1;
					break;
			}
			success = true;
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
		public void accessMemoryDynamic(ref DataMemory memory, ref RegisterFile registers, Instruction instruction, ref ConfigCycle config, 
			out string result, ref MemoryUnit load_buffer, ref DataCache DC)
		{
			result = "";
			int ASPR = instruction.instrFlag & 1;
			if(!load_buffer.instruction.executionInProgress && !load_buffer.instruction.doneExecuting)
            {
				load_buffer.instruction.cycleControl = config.load;
			}
			else
            {
				load_buffer.instruction.cycleControl--;
				goto endCycleCheck;
			}
			switch (instruction.opcode)
			{
				case 9:
					if (!load_buffer.instruction.executionInProgress && !load_buffer.instruction.doneExecuting)
					{
						//Insert caching logic here
						//Call findCacheVariables to get the offset, index and tag to reference the cache
						DC.findCacheVariables(instruction);

						//Find whether the index and tag exist within the cache
						DC.findInstuctionInCache(ref instruction);
					}

					//Runs case depending on whether the instruction hits, conflicts, or misses
					switch(instruction.hitOrMiss)
                    {
						case Instruction.cacheHit.HIT:
							if (!load_buffer.instruction.executionInProgress && !load_buffer.instruction.doneExecuting)
							{
								load_buffer.instruction.cycleControl = config.cacheHit - 1;
							}
							//If there's a hit then get the memory from the offset plus the two bytes after
							if (!instruction.isFloat)
							{
								load_buffer.instruction.executionInProgress = true;
								load_buffer.instruction.intResult = DC.l1Cache[DC.index][DC.offset] << 16;            //Loads the MSB value from the address in memory to r0
								load_buffer.instruction.intResult += DC.l1Cache[DC.index][DC.offset + 1] << 8;      //Loads the TSB value from the address in memory to r0
								load_buffer.instruction.intResult += DC.l1Cache[DC.index][DC.offset + 2];           //Loads the LSB value from the address in memory to r0
								result = load_buffer.instruction.intResult.ToString();
								if (int.Parse(result) == 0 && ASPR == 1)
									load_buffer.instruction.ASPR = 1;
							}
							else
							{
								load_buffer.instruction.executionInProgress = true;
								byte[] memoryFloat = new byte[4];
								memoryFloat[3] = DC.l1Cache[DC.index][DC.offset];                           //Loads the MSB value from the address in memory to f0
								memoryFloat[2] = DC.l1Cache[DC.index][DC.offset + 1];                       //Loads the TSB value from the address in memory to f0
								memoryFloat[1] = DC.l1Cache[DC.index][DC.offset + 2];                       //Loads the LSB value from the address in memory to f0
								load_buffer.instruction.floatResult = System.BitConverter.ToSingle(memoryFloat, 0);
								result = load_buffer.instruction.floatResult.ToString();
								if (float.Parse(result) == 0f && ASPR == 1)
									instruction.ASPR = 1;
							}
							break;
						case Instruction.cacheHit.CONFLICTED:
						case Instruction.cacheHit.MISS:
							if (!load_buffer.instruction.executionInProgress && !load_buffer.instruction.doneExecuting)
							{
								//Update cache with the memory from main memory (Add index and tag to the cache)
								startingAddress = instruction.address & ~DC.offsetMask;
								DC.updateCache(startingAddress, ref memory);
								load_buffer.instruction.cycleControl = config.cacheMiss - 1;
							}
							
							//Fix cycleControl to stall for the miss
							//If there's a miss then fall to main memory
							if (!instruction.isFloat)
							{
								load_buffer.instruction.executionInProgress = true;
								load_buffer.instruction.intResult = memory.MainMemory[load_buffer.instruction.address] << 16;            //Loads the MSB value from the address in memory to r0
								load_buffer.instruction.intResult += (memory.MainMemory[load_buffer.instruction.address + 1] << 8);      //Loads the TSB value from the address in memory to r0
								load_buffer.instruction.intResult += (memory.MainMemory[load_buffer.instruction.address + 2]);           //Loads the LSB value from the address in memory to r0
								result = load_buffer.instruction.intResult.ToString();
								if (int.Parse(result) == 0 && ASPR == 1)
									load_buffer.instruction.ASPR = 1;
							}
							else
							{
								load_buffer.instruction.executionInProgress = true;
								byte[] memoryFloat = new byte[4];
								memoryFloat[3] = (byte)(memory.MainMemory[load_buffer.instruction.address]);                           //Loads the MSB value from the address in memory to f0
								memoryFloat[2] = (byte)(memory.MainMemory[load_buffer.instruction.address + 1]);                       //Loads the TSB value from the address in memory to f0
								memoryFloat[1] = (byte)(memory.MainMemory[load_buffer.instruction.address + 2]);                       //Loads the LSB value from the address in memory to f0
								load_buffer.instruction.floatResult = System.BitConverter.ToSingle(memoryFloat, 0);
								result = load_buffer.instruction.floatResult.ToString();
								if (float.Parse(result) == 0f && ASPR == 1)
									instruction.ASPR = 1;
							}
							break;
					}
					break;

				case 10:
					if (!instruction.isFloat)
					{
						load_buffer.instruction.executionInProgress = true;
						result = load_buffer.instruction.iOperand1.ToString();
						if (load_buffer.instruction.iOperand1 == 0 && ASPR == 1)
							load_buffer.instruction.ASPR = 1;
					}
					else
					{
						load_buffer.instruction.executionInProgress = true;
						result = load_buffer.instruction.fOperand1.ToString();
						if (load_buffer.instruction.fOperand1 == 0f && ASPR == 1)
							load_buffer.instruction.ASPR = 1;
					}
					break;
				case 11:
					if (!instruction.isFloat)
                    {
						load_buffer.instruction.executionInProgress = true;
						result = load_buffer.instruction.address.ToString();
						if (int.Parse(result) == 0 && ASPR == 1)
							load_buffer.instruction.ASPR = 1;
					}
					else
					{
						load_buffer.instruction.executionInProgress = true;
						byte[] floatArray = new byte[4];                //create a new array of 4 bytes to convert the low address to float
																		//Must be read in back to front because BitConverter reads the first element of array as LSB
						floatArray[0] = 0x00;                           //last byte is 0 because we don't use it
						floatArray[1] = (byte)(load_buffer.instruction.address & 255);          //1 byte
						floatArray[2] = (byte)((load_buffer.instruction.address >> 8) & 15);    //get first 4 bits by shifting right 8 times and ANDing with 0xF to get only those 4
						floatArray[3] = 0x00;                           //first byte is 0 because we don't use them 
						result = System.BitConverter.ToSingle(floatArray, 0).ToString();
						if (float.Parse(result) == 0f && ASPR == 1)
							load_buffer.instruction.ASPR = 1;
					}
					break;
				case 12:
					if (!instruction.isFloat)
                    {
						load_buffer.instruction.executionInProgress = true;
						result = (load_buffer.instruction.iOperand1 + (load_buffer.instruction.address << 12)).ToString();
						if (int.Parse(result) == 0 && ASPR == 1)
							load_buffer.instruction.ASPR = 1;
					}
					else
					{
						load_buffer.instruction.executionInProgress = true;
						byte[] backArray = System.BitConverter.GetBytes(load_buffer.instruction.fOperand1);      //old 12 bytes of array
						byte[] floatArray = new byte[4];                //create a new array of 4 bytes to convert the low address to float
																		//Must be read in back to front because BitConverter reads the first element of array as LSB
						floatArray[0] = 0x00;                           //last byte is 0 because we don't use it
						floatArray[1] = backArray[1];                   //the old elements are returned to their former spot
						floatArray[2] = (byte)(((load_buffer.instruction.address & 15) << 4) | (int)backArray[2]);    //get first of 4 bits by ANDing with the address by 0xF
																										  //and shifting left 4 (should be in form 0x_0)
																										  //the second 4 are found by ORing with the old array
						floatArray[3] = (byte)((load_buffer.instruction.address >> 4) & 255);   //get first byte by shifting right 4 and ANDing with 0xFF
						result = System.BitConverter.ToSingle(floatArray, 0).ToString();
						if (float.Parse(result) == 0f && ASPR == 1)
							load_buffer.instruction.ASPR = 1;
					}
					break;
			}

			endCycleCheck:
			if (load_buffer.instruction.cycleControl == 0)
			{
				load_buffer.instruction.executionInProgress = false;
				load_buffer.instruction.doneExecuting = true;
			}
		}

	}
}
