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
        }

		public void writeToReg(RegisterFile registers, ref Instruction instruction, ref ConfigCycle config)
        {
			occupied = true;
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
					if (!instruction.isFloat)
						registers.intRegisters[instruction.destinationReg] = instruction.intResult;
					else
						registers.floatRegisters[instruction.destinationReg] = instruction.floatResult;
					break;
				case 16:
				case 17:
				case 18:
				case 19:
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
					if (!instruction.isFloat)
						registers.intRegisters[instruction.destinationReg] = instruction.intResult;
					else
						registers.floatRegisters[instruction.destinationReg] = instruction.floatResult;
					break;
			}
			success = true;
			instruction.cycleControl--;
        }

    }
}
