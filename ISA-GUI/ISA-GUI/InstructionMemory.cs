// ---------------------------------------------------------------------------
// File name: InstructionMemory.cs
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
   * Class Name: InstructionMemory <br>
   * Class Purpose: Holds the program instructions and is responsible for keeping track of the program counter
   * 
   * <hr>
   * Date created: 2/19/21 <br>
   * @author Samuel Reynolds
   */
    internal class InstructionMemory
    {
        public List<byte> instructions;     //the program instructions
        public ushort ProgramCounter;       //The program counter
        public ushort CurrentInstruction;   //The current instruction register CIR

        /**
	    * Method Name: InstructionMemory <br>
	    * Method Purpose: Class constructor
	    * 
	    * <hr>
	    * Date created: 2/19/21 <br>
	    * @author Samuel Reynolds
	    */
        public InstructionMemory()
        {
            instructions = new List<byte>();
            ProgramCounter = 0;
            CurrentInstruction = 0;
        }

        /**
		 * Method Name: setInstructionSize <br>
		 * Method Purpose: Sets the size of the instruction list and initiates it to 0
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 *   @param  int size
		 */
        public void setInstructionSize(int size)
        {
            for(int i = 0; i < size; i++)
            {
                instructions.Add(0);
            }
        }
    }
}
