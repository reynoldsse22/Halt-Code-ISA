// ---------------------------------------------------------------------------
// File name: CentralProcessingUnit.cs
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
	* Class Name: CentralProcessingUnit <br>
	* Class Purpose: To simulate a CPU in a 16-bit system. Is responsible for delegating fetching, decoding, and executing tasks to their respective functional units
	* 
	* <hr>
	* Date created: 2/19/21 <br>
	* @author Samuel Reynolds
	*/
    internal class CentralProcessingUnit
    {
        public RegisterFile registers;
        public ALU alu;
        public DataMemory dataMemory;
        public Fetch fetch;
        public ControlUnit CU;
        public ExecutionUnit EU;
        public InstructionMemory IM;
        static string[] instructions = {"HALT",
                                "NOP",
                                "BR",
                                "BRNE",
                                "BREQ",
                                "BRLT",
                                "BRLE",
                                "BRGT",
                                "BRGE",
                                "LDWM",
                                "STWM",
                                "MOV",
                                "LDWI",
                                "CMPI",
                                "CMPR",
                                "N/A",
                                "ASL",
                                "ASR",
                                "LSL",
                                "LSR",
                                "ADD",
                                "SUB",
                                "MULT",
                                "DIV",
                                "AND",
                                "OR",
                                "XOR",
                                "NOT"};

        /**
	    * Method Name: CentralProcessingUnit <br>
	    * Method Purpose: Class constructor
	    * 
	    * <hr>
	    * Date created: 2/19/21 <br>
	    * @author Samuel Reynolds
	    */
        public CentralProcessingUnit()
        {
            registers = new RegisterFile();
            alu = new ALU();
            dataMemory = new DataMemory();
            fetch = new Fetch();
            CU = new ControlUnit();
            EU = new ExecutionUnit();
            IM = new InstructionMemory();
        }

        /**
		 * Method Name: runProgram <br>
		 * Method Purpose: Starts and runs the whole program, sending instructions to different functional units for processing and execution
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 *   @param  List<string> input
		 *   @param  bool debug
		 *   @param  StringBuilder assemblyString
		 *   @param  ref StringBuilder decodedString
		 *   @param  ref bool halted
		 */
        public bool runProgram(List<string> input, bool debug, ref StringBuilder assemblyString, ref StringBuilder decodedString, ref bool halted)
        {
            int opcode, r1, r2, r3, address, addrMode;            //parameters for the instructions
            string instrType;
            byte[] instruct = new byte[3];

            if (debug && IM.instructions.Count == 0)    //if in debug mode and the program hasn't been started
            {
                IM.setInstructionSize(input.Count);     //Set the instruction size based on the amount of instructions given
                storeProgramInMemory(input);            //Store the program in main memory and the instruction memory unit
                instruct = fetch.getNextInstruction(ref registers, ref IM);        //FETCH - get the next instruction
                CU.decode(ref IM, instruct, out opcode, out r1, out r2, out r3, out address, out instrType, out addrMode);      //DECODE - Decode the instruction
                halted = EU.execute(ref registers, ref dataMemory, ref alu, ref IM, in opcode, in r1, in r2, in r3, in address);        //EXECUTE - Execute the instruction
                buildAssemblyString(ref assemblyString, opcode, r1, r2, r3, address, instrType);    //Build the associated assembly syntax string for the instruction
                buildDecodedString(ref decodedString, opcode, r1, r2, r3, address, instrType, addrMode);      //Build the decoded instruction string
            }
            else if(debug && IM.instructions.Count > 0) //if in debug mode and the program has been started
            {
                instruct = fetch.getNextInstruction(ref registers, ref IM);            //FETCH - get the next instruction
                CU.decode(ref IM, instruct, out opcode, out r1, out r2, out r3, out address, out instrType, out addrMode);          //DECODE - Decode the instruction
                halted = EU.execute(ref registers, ref dataMemory, ref alu, ref IM, in opcode, in r1, in r2, in r3, in address);        //EXECUTE - Execute the instruction
                buildAssemblyString(ref assemblyString, opcode, r1, r2, r3, address, instrType);    //Build the associated assembly syntax string for the instruction
                buildDecodedString(ref decodedString, opcode, r1, r2, r3, address, instrType, addrMode);      //Build the decoded instruction string
            }
            else   //If not in debug mode (run button was pressed)
            {
                IM.setInstructionSize(input.Count);     //Set the instruction size based on the amount of instructions given
                storeProgramInMemory(input);            //Store the program in main memory and the instruction memory unit
                while (!halted) //while the halt instruction hasn't been discovered
                {
                    instruct = fetch.getNextInstruction(ref registers, ref IM);            //FETCH - get the next instruction
                    CU.decode(ref IM, instruct, out opcode, out r1, out r2, out r3, out address, out instrType, out addrMode);      //DECODE - Decode the instruction
                    halted = EU.execute(ref registers, ref dataMemory, ref alu, ref IM, in opcode, in r1, in r2, in r3, in address);       //EXECUTE - Execute the instruction
                    buildAssemblyString(ref assemblyString, opcode, r1, r2, r3, address, instrType);        //Build the associated assembly syntax string for the instruction
                    buildDecodedString(ref decodedString, opcode, r1, r2, r3, address, instrType, addrMode);          //Build the decoded instruction string
                }
            }
            return halted;  //return program status (if halt instruction was found or not)
        }


        /**
		 * Method Name: storeProgramInMemory <br>
		 * Method Purpose: Stores teh instruction in main memory as well as the instruction memory functional unit
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 *   @param  List<string> input
		 */
        public void storeProgramInMemory(List<string> program)
        {
            int memoryOffset = 0;       //used to offset memory addresses (indexes)
            foreach(string b in program)
            {
                dataMemory.MainMemory[memoryOffset] = byte.Parse(program[memoryOffset], System.Globalization.NumberStyles.HexNumber); 
                IM.instructions[memoryOffset] = byte.Parse(program[memoryOffset++], System.Globalization.NumberStyles.HexNumber);
            }
        }

        /**
		 * Method Name: buildAssemblyString <br>
		 * Method Purpose: Builds the assembly string to include the current instruction assembly syntax
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 *   @param  StringBuilder assemblyString
		 *   @param  int opcode
		 *   @param  int r1
		 *   @param  int r2
		 *   @param  int r3
		 *   @param  int address
		 *   @param  string instrType
		 */
        public void buildAssemblyString(ref StringBuilder assemblyString, int opcode, int r1, int r2, int r3, int address, string instrType)
        {
            switch(opcode) //switch on the opcode value
            {
                case 0:
                    appendAssemblyString(ref assemblyString, "STOP", "", "", ""); //HALT
                    break;
                case 1:
                case 2:
                case 3:
                    appendAssemblyString(ref assemblyString, instructions[opcode], "0x" + address.ToString("X").PadLeft(4, '0'), "", "");
                    break;
                case 4:
                case 5:
                    appendAssemblyString(ref assemblyString, instructions[opcode], "r" + r3.ToString(), "0x" + address.ToString("X").PadLeft(4, '0'), "");
                    break;
                case 6:
                    appendAssemblyString(ref assemblyString, instructions[opcode], "r" + r3.ToString(), "r" + r2.ToString(), "");
                    break;
                case 7:
                    appendAssemblyString(ref assemblyString, instructions[opcode], "r" + r3.ToString(), "#" + address.ToString(), "");
                    break;
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                    appendAssemblyString(ref assemblyString, instructions[opcode], "r" + r1.ToString(), "r" + r2.ToString(), "r" + r3.ToString());
                    break;
                case 15:
                    appendAssemblyString(ref assemblyString, instructions[opcode], "r" + r1.ToString(), "r" + r3.ToString(), "");
                    break;
            }
        }

        /**
		 * Method Name: appendAssemblyString <br>
		 * Method Purpose: Appends the current instruction assembly syntax to the assembly string
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 *   @param  StringBuilder assemblyString
		 *   @param  string instruction
		 *   @param  string first
		 *   @param  string second
		 *   @param  string third
		 *   @param  int address
		 *   @param  string instrType
		 */
        public void appendAssemblyString(ref StringBuilder assemblyString, string instruction, string first, string second, string third)
        {
            if ((IM.ProgramCounter / 2) < CU.instructionsProcessed) //if the program counter is referencing an instruction we have already processed - Don't need the assembly syntax
                return;

            if (second != "")
                first += ",";
            if (third != "")
                second += ",";

            assemblyString.Append(instruction.ToUpper() + "\t" + first + second + third + "\n");
        }

        /**
        * Method Name: buildDecodedString <br>
        * Method Purpose: Appends to the decoded instruction string to include the current instruction
        * 
        * <br>
        * Date created: 2/19/22 <br>
        * <hr>
        *   @param  StringBuilder assemblyString
        *   @param  int opcode
        *   @param  int r1
        *   @param  int r2
        *   @param  int r3
        *   @param  int address
        *   @param  string instrType
        */
        public void buildDecodedString(ref StringBuilder decodedString, int opcode, int r1, int r2, int r3, int address, string instrType, int addrMode)
        {
            //If any of these variables is negative then they are not used in the current intruction and will be printed as N/A
            string r1Str, r2Str, r3Str, addressStr, addressingMode;
            if (r1 == -1) 
                r1Str = "N/A";
            else
                r1Str = r1.ToString("X");
            if (r2 == -1)
                r2Str = "N/A";
            else
                r2Str = r2.ToString("X");
            if (r3 == -1)
                r3Str = "N/A";
            else
                r3Str = r3.ToString("X");
            if (address == -1)
                addressStr = "N/A";
            else
                addressStr = address.ToString("X");
            switch(addrMode)
            { 
                case 1:
                    addressingMode = "FLT";
                    break;
                case 2:
                    addressingMode = "STAT";
                    break;
                default:
                    addressingMode = "N/A";
                    break;
            }


            if (opcode == 0 || opcode == 1 || opcode == 11 || (opcode >= 17 && opcode <=27))
            {
                decodedString.Append((string.Format("\n{0, 7} {1, 4} {2, 4} {3, 8} {4, 9} {5, 4} {6, 4} {7, 4} {8, 8}",
                            (IM.ProgramCounter - 2).ToString("X").PadLeft(4, '0'), addressingMode, opcode.ToString("X"), instructions[opcode], instrType,
                            r1Str, r2Str, r3Str, addressStr)));
            }
            else
            {
                decodedString.Append((string.Format("\n{0, 7} {1, 4} {2, 4} {3, 8} {4, 9} {5, 4} {6, 4} {7, 4} {8, 8}",
                            (IM.ProgramCounter - 2).ToString("X").PadLeft(4, '0'), addressingMode, opcode.ToString("X"), instructions[opcode], instrType,
                            r1Str, r2Str, r3Str, ("0x" + addressStr.PadLeft(4, '0')))));

            }
        }
    }
}
