// ---------------------------------------------------------------------------
// File name: Fetch.cs
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
   * Class Name: Fetch <br>
   * Class Purpose: Used to grab the next instruction
   * 
   * <hr>
   * Date created: 2/19/21 <br>
   * @author Samuel Reynolds
   */
    internal class Fetch
    {
        /**
	    * Method Name: Fetch <br>
	    * Method Purpose: Class constructor
	    * 
	    * <hr>
	    * Date created: 2/19/21 <br>
	    * @author Samuel Reynolds
	    */
        public Fetch()
        {
        }

        /**
		 * Method Name: getNextInstruction <br>
		 * Method Purpose: Goes to the instruction memory unit and fetches the next instruction (2 bytes)
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 *   @param  object sender
		 *   @param  EventArgs e
		 */
        public byte[] getNextInstruction(ref RegisterFile registers, ref InstructionMemory IM)
        {
            byte[] instruction = new byte[3];
            instruction[0] = IM.instructions[IM.ProgramCounter++];
            instruction[1] = IM.instructions[IM.ProgramCounter++];
            instruction[2] = IM.instructions[IM.ProgramCounter++];
            IM.CurrentInstruction = (ushort)(instruction[2] + (instruction[1] << 8)+ (instruction[0] << 16));
            return instruction;
        }
    }
}
