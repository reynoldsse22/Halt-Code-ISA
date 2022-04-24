// ---------------------------------------------------------------------------
// File name: WriteResult.cs
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
	* Class Name: WriteResult <br>
	* Class Purpose: Stage 5 of the pipeline where we write the result to the register file.
	* 
	* <hr>
	* Date created: 2/27/21 <br>
	* @author Samuel Reynolds
	*/
	internal class WriteResult
    {
		public bool occupied;
		public bool success;
		public bool inProgress;
		public bool hazardDetected;
		/**
	    * Method Name: WriteResult <br>
	    * Method Purpose: Class constructor
	    * 
	    * <hr>
	    * Date created: 2/27/21 <br>
	    * @author Samuel Reynolds
	    */
		public WriteResult()
        {
			occupied = false;
			success = false;
			inProgress = false;
			hazardDetected = false;
		}

		/**
		 * Method Name: writeToReg <br>
		 * Method Purpose: Passes the instruction and the configuration file along with the registers in order to write the previously calculated result to
		 * the appropriate register
		 * 
		 * <br>
		 * Date created: 3/2/22 <br>
		 * <hr>
		 *   @param  RegisterFil registers
		 *   @param  Instruction instruction
		 *   @param ConfigCycle config
		 */
		public void writeToReg(RegisterFile registers, ref Instruction instruction, ref ConfigCycle config)
        {
			inProgress = true;
			occupied = true;
			hazardDetected = false;
			instruction.cycleControl = config.regAccess;
			switch (instruction.opcode)
            {
				case 11:
					if (!instruction.isFloat)
						registers.intRegisters[instruction.r3] = instruction.address;
					else
					{
						byte[] floatArray = new byte[4];                //create a new array of 4 bytes to convert the low address to float
																		//Must be read in back to front because BitConverter reads the first element of array as LSB
						floatArray[0] = 0x00;                           //last byte is 0 because we don't use it
						floatArray[1] = (byte)(instruction.address & 255);          //1 byte
						floatArray[2] = (byte)((instruction.address >> 8) & 15);    //get first 4 bits by shifting right 8 times and ANDing with 0xF to get only those 4
						floatArray[3] = 0x00;                           //first byte is 0 because we don't use them 
						registers.floatRegisters[instruction.r3] = System.BitConverter.ToSingle(floatArray, 0);
					}
					break;
				case 12:
					if (!instruction.isFloat)
						registers.intRegisters[instruction.r3] += (instruction.address << 12);
					else
					{
						byte[] backArray = System.BitConverter.GetBytes(registers.floatRegisters[instruction.r3]);      //old 12 bytes of array
						byte[] floatArray = new byte[4];                //create a new array of 4 bytes to convert the low address to float
																		//Must be read in back to front because BitConverter reads the first element of array as LSB
						floatArray[0] = 0x00;                           //last byte is 0 because we don't use it
						floatArray[1] = backArray[1];                   //the old elements are returned to their former spot
						floatArray[2] = (byte)(((instruction.address & 15) << 4) | (int)backArray[2]);    //get first of 4 bits by ANDing with the address by 0xF
																							  //and shifting left 4 (should be in form 0x_0)
																							  //the second 4 are found by ORing with the old array
						floatArray[3] = (byte)((instruction.address >> 4) & 255);   //get first byte by shifting right 4 and ANDing with 0xFF
						registers.floatRegisters[instruction.r3] = System.BitConverter.ToSingle(floatArray, 0);
					}
					break;
				case 13:
					break;
				case 14:
					break;
				case 15:
					//MOV
					if (!instruction.isFloat)
						registers.intRegisters[instruction.destinationReg] = instruction.intResult;
					else
						registers.floatRegisters[instruction.destinationReg] = instruction.floatResult;
					break;
				case 16:
				case 17:
				case 18:
				case 19:
					//Shifting
					registers.intRegisters[instruction.destinationReg] = instruction.intResult;
					break;
				case 20:
				case 21:
				case 22:
				case 23:
				case 24:
				case 25:
				case 26:
				case 27:
					//ALU instructions
					if (!instruction.isFloat)
						registers.intRegisters[instruction.destinationReg] = instruction.intResult;
					else
						registers.floatRegisters[instruction.destinationReg] = instruction.floatResult;
					break;
			}
			success = true;
        }


		/**
		 * Method Name: writeToReg <br>
		 * Method Purpose: Passes the instruction and the configuration file along with the registers in order to write the previously calculated result to
		 * the appropriate register
		 * 
		 * <br>
		 * Date created: 3/2/22 <br>
		 * <hr>
		 *   @param  RegisterFil registers
		 *   @param  Instruction instruction
		 *   @param ConfigCycle config
		 */
		public bool writeToCDB(Instruction instruction, ref CommonDataBus CDB, in string result)
		{
			switch (instruction.functionalUnitID)
			{
				case 1:
					if(!CDB.CDB.ContainsKey("intAddFu" + instruction.functionalUnitIndex))
                    {
						CDB.index.Add(instruction.ID, CDB.CDB.Count);
						CDB.IDIndex.Add(CDB.CDB.Count, instruction.ID);
						CDB.CDB.Add("intAddFu" + instruction.functionalUnitIndex, result);
						return true;
					}
					break;
				case 2:
					if (!CDB.CDB.ContainsKey("intSubFu" + instruction.functionalUnitIndex))
					{
						CDB.index.Add(instruction.ID, CDB.CDB.Count);
						CDB.IDIndex.Add(CDB.CDB.Count, instruction.ID);
						CDB.CDB.Add("intSubFu" + instruction.functionalUnitIndex, result);
						return true;
					}
					break;
				case 3:
					if (!CDB.CDB.ContainsKey("intMulFu" + instruction.functionalUnitIndex))
					{
						CDB.index.Add(instruction.ID, CDB.CDB.Count);
						CDB.IDIndex.Add(CDB.CDB.Count, instruction.ID);
						CDB.CDB.Add("intMulFu" + instruction.functionalUnitIndex, result);
						return true;
					}
					break;
				case 4:
					if (!CDB.CDB.ContainsKey("intDivFu" + instruction.functionalUnitIndex))
					{
						CDB.index.Add(instruction.ID, CDB.CDB.Count);
						CDB.IDIndex.Add(CDB.CDB.Count, instruction.ID);
						CDB.CDB.Add("intDivFu" + instruction.functionalUnitIndex, result);
						return true;
					}
					break;
				case 5:
					if (!CDB.CDB.ContainsKey("flAddFu" + instruction.functionalUnitIndex))
					{
						CDB.index.Add(instruction.ID, CDB.CDB.Count);
						CDB.IDIndex.Add(CDB.CDB.Count, instruction.ID);
						CDB.CDB.Add("flAddFu" + instruction.functionalUnitIndex, result);
						return true;
					}
					break;
				case 6:
					if (!CDB.CDB.ContainsKey("flSubFu" + instruction.functionalUnitIndex))
					{
						CDB.index.Add(instruction.ID, CDB.CDB.Count);
						CDB.IDIndex.Add(CDB.CDB.Count, instruction.ID);
						CDB.CDB.Add("flSubFu" + instruction.functionalUnitIndex, result);
						return true;
					}
					break;
				case 7:
					if (!CDB.CDB.ContainsKey("flMultFu" + instruction.functionalUnitIndex))
					{
						CDB.index.Add(instruction.ID, CDB.CDB.Count);
						CDB.IDIndex.Add(CDB.CDB.Count, instruction.ID);
						CDB.CDB.Add("flMultFu" + instruction.functionalUnitIndex, result);
						return true;
					}
					break;
				case 8:
					if (!CDB.CDB.ContainsKey("flDivFu" + instruction.functionalUnitIndex))
					{
						CDB.index.Add(instruction.ID, CDB.CDB.Count);
						CDB.IDIndex.Add(CDB.CDB.Count, instruction.ID);
						CDB.CDB.Add("flDivFu" + instruction.functionalUnitIndex, result);
						return true;
					}
					break;
				case 9:
					if (!CDB.CDB.ContainsKey("bitFu" + instruction.functionalUnitIndex))
					{
						CDB.index.Add(instruction.ID, CDB.CDB.Count);
						CDB.IDIndex.Add(CDB.CDB.Count, instruction.ID);
						CDB.CDB.Add("bitFu" + instruction.functionalUnitIndex, result);
						return true;
					}
					break;
				case 10:
					if (!CDB.CDB.ContainsKey("memoryFu" + instruction.functionalUnitIndex))
					{
						CDB.index.Add(instruction.ID, CDB.CDB.Count);
						CDB.IDIndex.Add(CDB.CDB.Count, instruction.ID);
						CDB.CDB.Add("memoryFu" + instruction.functionalUnitIndex, result);
						return true;
					}
					break;
				case 11:
					if (!CDB.CDB.ContainsKey("branchFu" + instruction.functionalUnitIndex))
					{
						CDB.index.Add(instruction.ID, CDB.CDB.Count);
						CDB.IDIndex.Add(CDB.CDB.Count, instruction.ID);
						CDB.CDB.Add("branchFu" + instruction.functionalUnitIndex, result);
						return true;
					}
					break;
				case 12:
					if (!CDB.CDB.ContainsKey("shiftFu" + instruction.functionalUnitIndex))
					{
						CDB.index.Add(instruction.ID, CDB.CDB.Count);
						CDB.IDIndex.Add(CDB.CDB.Count, instruction.ID);
						CDB.CDB.Add("shiftFu" + instruction.functionalUnitIndex, result);
						return true;
					}
					break;
			}
			return false;
		}

		/**
		 * Method Name: writeToReg <br>
		 * Method Purpose: Passes the instruction and the configuration file along with the registers in order to write the previously calculated result to
		 * the appropriate register
		 * 
		 * <br>
		 * Date created: 3/2/22 <br>
		 * <hr>
		 *   @param  RegisterFil registers
		 *   @param  Instruction instruction
		 *   @param ConfigCycle config
		 */
		public void commit(ref RegisterFile registers, ref Instruction instruction, ref DataMemory memory, ref bool halted, ref InstructionMemory IM, ref bool branchTaken, 
			ref DataCache DC, ref StringBuilder cacheString, ref MemoryUnit[] memFU, ref ConfigCycle config)
		{
			halted = false;
			switch (instruction.opcode)
			{
				case 0:
					halted = true;
					break;
				case 1:
					break;
				case 2:
					IM.ProgramCounter = instruction.address;                            //Move the branching address into the program counter/instruction pointer
					branchTaken = true;
					break;
				case 3:
					if ((registers.ASPR & 2) == 0)
					{
						IM.ProgramCounter = instruction.address;                        //Move the BNE address into the program counter/instruction pointer
						branchTaken = true;
					}
					else
						branchTaken = false;
					break;
				case 4:
					if ((registers.ASPR & 2) == 1)
					{
						IM.ProgramCounter = instruction.address;                        //Move the BEQ address into the program counter/instruction pointer
						branchTaken = true;
					}
					else
						branchTaken = false;
					break;
				case 5:
					if ((registers.ASPR & 1) == 1 && (registers.ASPR & 1) == 0)
					{
						IM.ProgramCounter = instruction.address;                        //Move the BLT address into the program counter/instruction pointer
						branchTaken = true;
					}
					else
						branchTaken = false;
					break;
				case 6:
					if ((registers.ASPR & 2) == 1 || (registers.ASPR & 1) == 1)
					{
						IM.ProgramCounter = instruction.address;                        //Move the BLE address into the program counter/instruction pointer
						branchTaken = true;
					}
					else
						branchTaken = false;
					break;
				case 7:
					if ((registers.ASPR & 2) == 0 && (registers.ASPR & 1) == 0)
					{
						IM.ProgramCounter = instruction.address;                        //Move the BGT address into the program counter/instruction pointer
						branchTaken = true;
					}
					else
						branchTaken = false;
					break;
				case 8:
					if (((registers.ASPR & 2) == 0 || (registers.ASPR & 1) == 0) || ((registers.ASPR & 2) == 1 || (registers.ASPR & 1) == 0))
					{
						IM.ProgramCounter = instruction.address;                        //Move the BGE address into the program counter/instruction pointer
						branchTaken = true;
					}
					else
						branchTaken = false;
					break;

				case 9:
					registers.ASPR = instruction.ASPR;
					if (instruction.isFloat)
						registers.floatRegisters[0] = float.Parse(instruction.result);
					else
						registers.intRegisters[0] = int.Parse(instruction.result);
					break;

				case 10:
					if(instruction.ID != memFU[instruction.functionalUnitIndex].instruction.ID)
                    {
						throw new Exception();
                    }

					//Insert caching logic here
					registers.ASPR = instruction.ASPR;


					if (config.cachingSet)
                    {
						if (!memFU[instruction.functionalUnitIndex].instruction.executionInProgress && !memFU[instruction.functionalUnitIndex].instruction.doneExecuting)
						{
							//Insert caching logic here
							//Call findCacheVariables to get the offset, index and tag to reference the cache
							DC.findCacheVariables(instruction);

							//Find whether the index and tag exist within the cache
							DC.findInstructionInCache(ref instruction);
						}
						else
						{
							memFU[instruction.functionalUnitIndex].instruction.cycleControl--;
							goto endCycleCheck;
						}

						switch (instruction.hitOrMiss)
						{
							case Instruction.cacheHit.HIT:
								if (!memFU[instruction.functionalUnitIndex].instruction.executionInProgress && !memFU[instruction.functionalUnitIndex].instruction.doneExecuting)
								{
									memFU[instruction.functionalUnitIndex].instruction.cycleControl = config.cacheHit - 1;
								}
								//If there's a hit then store the new memory into the cache
								//Update main memory as well
								if (!instruction.isFloat)
								{
									memFU[instruction.functionalUnitIndex].instruction.executionInProgress = true;
									DC.updateWriteCache(int.Parse(instruction.result));
									writeIntMemory(instruction, ref memory);
								}
								else
								{
									memFU[instruction.functionalUnitIndex].instruction.executionInProgress = true;
									float fResult = float.Parse(instruction.result);                    //CAN BE IMPLEMENTED BETTER BECAUSE THERE'S CODE DUPLICATION
									byte[] currentFloat = System.BitConverter.GetBytes(fResult);                          //Float to be stored
									DC.updateWriteCache(currentFloat);
									writeFloatMemory(instruction, ref memory);

								}

								break;
							case Instruction.cacheHit.CONFLICTED:
							case Instruction.cacheHit.MISS:
								if (!memFU[instruction.functionalUnitIndex].instruction.executionInProgress && !memFU[instruction.functionalUnitIndex].instruction.doneExecuting)
								{
									memFU[instruction.functionalUnitIndex].instruction.cycleControl = config.cacheMiss - 1;
								}
								//If there's a miss then do NOT update the cache
								//Update main memory by falling through
								if (!instruction.isFloat)
								{
									memFU[instruction.functionalUnitIndex].instruction.executionInProgress = true;
									writeIntMemory(instruction, ref memory);
								}
								else
								{
									memFU[instruction.functionalUnitIndex].instruction.executionInProgress = true;
									writeFloatMemory(instruction, ref memory);
								}
								break;
						}

					endCycleCheck:
						if (memFU[instruction.functionalUnitIndex].instruction.cycleControl == 0)
						{
							memFU[instruction.functionalUnitIndex].instruction.executionInProgress = false;
							memFU[instruction.functionalUnitIndex].instruction.doneExecuting = true;
						}
					}
                    else
                    {
						if (!instruction.isFloat)
						{
							writeIntMemory(instruction, ref memory);
						}
						else
						{
							writeFloatMemory(instruction, ref memory);
						}
						memFU[instruction.functionalUnitIndex].instruction.executionInProgress = false;
						memFU[instruction.functionalUnitIndex].instruction.doneExecuting = true;
					}
					break;
				case 11:
					if (!instruction.isFloat)
						registers.intRegisters[instruction.r3] = int.Parse(instruction.result);
					else
						registers.floatRegisters[instruction.r3] = float.Parse(instruction.result);
					break;
				case 12:
					registers.ASPR = instruction.ASPR;
					if (!instruction.isFloat)
						registers.intRegisters[instruction.r3] = int.Parse(instruction.result);
					else
						registers.floatRegisters[instruction.r3] = float.Parse(instruction.result);
					break;
				case 13:
					registers.ASPR = instruction.ASPR;
					break;
				case 14:
					registers.ASPR = instruction.ASPR;
					break;
				case 15:
					//MOV
					registers.ASPR = instruction.ASPR;
					if (!instruction.isFloat)
						registers.intRegisters[instruction.destinationReg] = int.Parse(instruction.result);
					else
						registers.floatRegisters[instruction.destinationReg] = float.Parse(instruction.result);
					break;
				case 16:
				case 17:
				case 18:
				case 19:
					//Shifting
					registers.ASPR = instruction.ASPR;
					registers.intRegisters[instruction.destinationReg] = int.Parse(instruction.result);
					break;
				case 20:
				case 21:
				case 22:
				case 23:
				case 24:
				case 25:
				case 26:
				case 27:
					registers.ASPR = instruction.ASPR;
					//ALU instructions
					if (!instruction.isFloat)
						registers.intRegisters[instruction.destinationReg] = int.Parse(instruction.result);
					else
						registers.floatRegisters[instruction.destinationReg] = float.Parse(instruction.result);
					break;
			}

		}

        /// <summary>Writes the int result to memory.</summary>
        /// <param name="instruction">The instruction.</param>
        /// <param name="memory">The memory.</param>
        public void writeIntMemory(Instruction instruction, ref DataMemory memory)
        {
			int iResult = int.Parse(instruction.result);
			memory.MainMemory[instruction.address] = (byte)((iResult & 16711680) >> 16);      //Stores the MSB value of r0 at the address in memory
			memory.MainMemory[instruction.address + 1] = (byte)((iResult & 65280) >> 8);     //Stores the TSB value of r0 at the address in memory
			memory.MainMemory[instruction.address + 2] = (byte)(iResult & 255);       //Stores the LSB value of r0 at the address in memory
		}

        /// <summary>Writes the float result to memory.</summary>
        /// <param name="instruction">The instruction.</param>
        /// <param name="memory">The memory.</param>
        public void writeFloatMemory(Instruction instruction, ref DataMemory memory)
        {
			float fResult = float.Parse(instruction.result);
			byte[] currentFloat = System.BitConverter.GetBytes(fResult);                          //Float to be stored
			memory.MainMemory[instruction.address] = currentFloat[3];                                           //Stores the MSB value of f0 at the address in memory
			memory.MainMemory[instruction.address + 1] = currentFloat[2];                                       //Stores the TSB value of f0 at the address in memory
			memory.MainMemory[instruction.address + 2] = currentFloat[1];                                       //Stores the LSB value of f0 at the address in memory
		}
	}
}
