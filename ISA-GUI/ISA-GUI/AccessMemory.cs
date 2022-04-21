﻿// ---------------------------------------------------------------------------
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
		public int tag, index, offset;

		//Allows for configuration
        public int offsetBitAmount { get; set; }
        public int indexBitAmount { get; set; }
		//Number of words allowed in the cache
        public int numberOfWords { get; set; }
        public int offsetMask;
		public int indexMask;
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
			tag = 0;
			index = 0;
			offset = 0;
			startingAddress = 0;

			offsetBitAmount = 5;
			indexBitAmount = 4;

			numberOfWords = 10;
			//This should be configurable in the future to allow 2/4 way association
			offsetMask = (int)Math.Pow(2, offsetBitAmount + 1) - 1;
			indexMask = (int)Math.Pow(2, indexBitAmount + 1) - 1;
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
			load_buffer.instruction.cycleControl--;
			switch (instruction.opcode)
			{
				case 9:
					//Insert caching logic here
					//Call findCacheVariables to get the offset, index and tag to reference the cache
					findCacheVariables(instruction, offset, index, tag);

					//Find whether the index and tag exist within the cache

					if(DC.tagIndexCache[index] == tag && DC.l1Cache[index] != null)
                    {
						//If there's a hit then get the memory from the offset plus the two bytes after
						if (!instruction.isFloat)
						{
							load_buffer.instruction.executionInProgress = true;
							load_buffer.instruction.intResult =  DC.l1Cache[index][offset] << 16;            //Loads the MSB value from the address in memory to r0
							load_buffer.instruction.intResult += DC.l1Cache[index][offset + 1] << 8;      //Loads the TSB value from the address in memory to r0
							load_buffer.instruction.intResult += DC.l1Cache[index][offset + 2];           //Loads the LSB value from the address in memory to r0
							result = load_buffer.instruction.intResult.ToString();
							if (int.Parse(result) == 0 && ASPR == 1)
								load_buffer.instruction.ASPR = 1;
						}
						else
						{
							load_buffer.instruction.executionInProgress = true;
							byte[] memoryFloat = new byte[4];
							memoryFloat[3] = DC.l1Cache[index][offset];                           //Loads the MSB value from the address in memory to f0
							memoryFloat[2] = DC.l1Cache[index][offset + 1];                       //Loads the TSB value from the address in memory to f0
							memoryFloat[1] = DC.l1Cache[index][offset + 2];                       //Loads the LSB value from the address in memory to f0
							load_buffer.instruction.floatResult = System.BitConverter.ToSingle(memoryFloat, 0);
							result = load_buffer.instruction.floatResult.ToString();
							if (float.Parse(result) == 0f && ASPR == 1)
								instruction.ASPR = 1;
						}
					}
					else
                    {
						//Update cache with the memory from main memory (Add index and tag to the cache)
						startingAddress = instruction.address & ~offsetMask;
						updateCache(startingAddress, ref memory, ref DC);

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
					}
					if (load_buffer.instruction.cycleControl == 0)
					{
						load_buffer.instruction.executionInProgress = false;
						load_buffer.instruction.doneExecuting = true;
					}
					break;
				case 10:
					//Insert caching logic here
					//Call findCacheVariables to get the offset, index and tag to reference the cache
					registers.ASPR = instruction.ASPR;
					findCacheVariables(instruction, offset, index, tag);
					//Find whether the index and tag exist within the cache
					if (DC.tagIndexCache[index] == tag && DC.l1Cache[index] != null)
					{
						//If there's a hit then store the new memory into the cache
						//Update main memory as well
					}
                    else
                    {
						//If there's a miss then do NOT update the cache
						//Update main memory by falling through
                    }




					if (!instruction.isFloat)
					{
						load_buffer.instruction.executionInProgress = true;
						result = load_buffer.instruction.iOperand1.ToString();
						if (load_buffer.instruction.iOperand1 == 0 && ASPR == 1)
							load_buffer.instruction.ASPR = 1;
						if (load_buffer.instruction.cycleControl == 0)
						{
							load_buffer.instruction.executionInProgress = false;
							load_buffer.instruction.doneExecuting = true;
						}
					}
					else
					{
						load_buffer.instruction.executionInProgress = true;
						result = load_buffer.instruction.fOperand1.ToString();
						if (load_buffer.instruction.fOperand1 == 0f && ASPR == 1)
							load_buffer.instruction.ASPR = 1;
						if (load_buffer.instruction.cycleControl == 0)
						{
							load_buffer.instruction.executionInProgress = false;
							load_buffer.instruction.doneExecuting = true;
						}
					}
					break;
				case 11:
					if (!instruction.isFloat)
                    {
						load_buffer.instruction.executionInProgress = true;
						result = load_buffer.instruction.address.ToString();
						if (int.Parse(result) == 0 && ASPR == 1)
							load_buffer.instruction.ASPR = 1;
						if (load_buffer.instruction.cycleControl == 0)
						{
							load_buffer.instruction.executionInProgress = false;
							load_buffer.instruction.doneExecuting = true;
						}
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
						if (load_buffer.instruction.cycleControl == 0)
						{
							load_buffer.instruction.executionInProgress = false;
							load_buffer.instruction.doneExecuting = true;
						}
					}
					break;
				case 12:
					if (!instruction.isFloat)
                    {
						load_buffer.instruction.executionInProgress = true;
						result = (load_buffer.instruction.iOperand1 + (load_buffer.instruction.address << 12)).ToString();
						if (int.Parse(result) == 0 && ASPR == 1)
							load_buffer.instruction.ASPR = 1;
						if (load_buffer.instruction.cycleControl == 0)
						{
							load_buffer.instruction.executionInProgress = false;
							load_buffer.instruction.doneExecuting = true;
						}
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
						if (load_buffer.instruction.cycleControl == 0)
						{
							load_buffer.instruction.executionInProgress = false;
							load_buffer.instruction.doneExecuting = true;
						}
					}
					break;
			}
		}

        /// <summary>Updates the cache.</summary>
        /// <param name="index">The index.</param>
        /// <param name="address">The address.</param>
        /// <param name="memory">The main memory.</param>
        /// <param name="dC">
        ///   <para>
        /// The cache.
        /// </para>
        /// </param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void updateCache(int address, ref DataMemory memory, ref DataCache DC)
        {
			//Adds new byte of memory into the cache
			byte[] mem = new byte[numberOfWords];

			for(int x = 0; x < numberOfWords; x++)
            {
				mem[x] = (byte)memory.MainMemory[address + x];
            }

			DC.l1Cache[index] = mem;
			DC.tagIndexCache[index] = tag;
        }

        /// <summary>
        ///   <para>
        /// Finds the cache variables in order to check if the address is in the cache.
        /// </para>
        /// </summary>
        /// <param name="inst">The instruction.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="index">The index.</param>
        /// <param name="tag">The tag.</param>
        public void findCacheVariables(Instruction inst, int offset, int index, int tag)
        {
			int address = inst.address;
			offset = address & offsetMask;
			address = address >> offsetBitAmount;
			index = address & indexMask;
			address = address >> indexBitAmount;
			tag = address;
		}

	}
}
