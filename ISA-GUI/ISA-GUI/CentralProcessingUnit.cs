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
        public WriteResult WR;
        public AccessMemory AM;
        public int cycleCount;
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
                                "LDWM",
                                "LDHH",
                                "CMPI",
                                "CMPR",
                                "MOV",
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
            AM = new AccessMemory();
            WR = new WriteResult();
            cycleCount = 0;
        }

        public void runCycle(List<string> input, bool stepThrough, ref StringBuilder assemblyString, ref StringBuilder decodedString,
                ref StringBuilder pipelineString, ref bool halted, ref ConfigCycle config, ref Instruction[] stages)
        {
            byte[] instruct = new byte[3];
            if (IM.instructions.Count == 0)    //if the program hasn't been started
            {
                IM.setInstructionSize(input.Count);     //Set the instruction size based on the amount of instructions given
                storeProgramInMemory(input);            //Store the program in main memory and the instruction memory unit
            }
            if (halted)
                return;
            do 
            {
                cycleCount++;
                if (stages[4] != null && cycleCount > 5)
                {
                    if (WR.success == false)
                    {
                        stages[4].start = cycleCount;
                        WR.writeToReg(registers, ref stages[4], ref config);
                        stages[4].stage = 4;
                    }
                    else
                        stages[4].cycleControl--;

                    if (stages[4].cycleControl == 0)
                    {
                        buildAssemblyString(ref assemblyString, stages[4]);    //Build the associated assembly syntax string for the instruction
                        buildDecodedString(ref decodedString, stages[4]);      //Build the decoded instruction string
                        WR.success = false;
                        stages[4].end = cycleCount;
                        if (stages[4].opcode == 0)
                            halted = true;

                        stages[4] = null;
                        WR.occupied = false;
                    }
                }
                if (stages[3] != null && cycleCount > 4)
                {
                    if (AM.success == false)
                    {
                        stages[3].start = cycleCount;
                        AM.accessMemory(ref dataMemory, ref registers, ref stages[3], ref config);
                        stages[3].stage = 4;
                    }
                    else
                        stages[3].cycleControl--;

                    if (stages[3].cycleControl == 0)
                    {
                        if (!WR.occupied)
                        {
                            AM.success = false;
                            stages[3].end = cycleCount;
                            stages[4] = stages[3];
                            stages[3] = null;
                            AM.occupied = false;
                        }
                    }
                }
                if (stages[2] != null && cycleCount > 3)
                {
                    if (EU.success == false)
                    {
                        stages[2].start = cycleCount;
                        EU.execute(ref registers, ref dataMemory, ref alu, ref IM, ref stages[2], ref config);        //EXECUTE - Execute the instruction
                        stages[2].stage = 3;
                    }
                    else
                        stages[2].cycleControl--;

                    if (stages[2].cycleControl == 0)
                    {
                        if (!AM.occupied)
                        {
                            EU.success = false;
                            stages[2].end = cycleCount;
                            stages[3] = stages[2];
                            stages[2] = null;
                            EU.occupied = false;
                        }

                    }
                }
                if (stages[1] != null && cycleCount > 2)
                {

                    if (CU.success == false)
                    {
                        stages[1].start = cycleCount;
                        CU.decode(ref IM, ref stages[1], ref config);      //DECODE - Decode the instruction
                        stages[1].stage = 2;
                    }
                    else
                        stages[1].cycleControl--;

                    if (stages[1].cycleControl == 0)
                    {
                        if (!EU.occupied)
                        {
                            CU.success = false;
                            stages[1].end = cycleCount;
                            stages[2] = stages[1];
                            stages[1] = null;
                            CU.occupied = false;
                        }
                    }
                }
                

                if (stages[0] != null)
                {
                    if (!CU.occupied && cycleCount > 1)
                    {
                        fetch.success = false;
                        stages[0].end = cycleCount;
                        stages[1] = stages[0];
                        stages[0] = null;
                        fetch.occupied = false;
                    }

                }

                if (fetch.success == false)
                {
                    stages[0] = fetch.getNextInstruction(ref registers, ref IM, ref config);        //FETCH - get the next instruction
                    stages[0].stage = 1;
                    stages[0].start = cycleCount;
                }
                

            } while (!halted && !stepThrough);

            return;                         
        }


        /**
		 * Method Name: storeProgramInMemory <br>
		 * Method Purpose: Stores teh instruction in main memory as well as the instruction memory functional unit
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 *   @param  List<string> program
		 */
        public void storeProgramInMemory(List<string> program)
        {
            int memoryOffset = 0;       //used to offset memory addresses (indexes)
            foreach (string b in program)
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
		 *   @param  Instruction instruction
		 */
        public void buildAssemblyString(ref StringBuilder assemblyString, Instruction instruction)
        {
            int opcode = instruction.opcode;
            int r1 = instruction.r1;
            int r2 = instruction.r2;
            int r3 = instruction.r3;
            int address = instruction.address;
            int instrFlag = instruction.instrFlag;
            string instrType = instruction.instrType;
            string registerString = "r";
            if (instrFlag == 1)
            {
                registerString = "f";
            }
            switch (opcode) //switch on the opcode value
            {
                case 0:
                    appendAssemblyString(ref assemblyString, "STOP", "", "", ""); //HALT
                    break;
                case 1:
                    appendAssemblyString(ref assemblyString, "NOP", "", "", ""); //NOP
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                    appendAssemblyString(ref assemblyString, instructions[opcode], "0x" + address.ToString("X").PadLeft(4, '0'), "", "");
                    break;
                case 11:
                case 12:
                case 13:
                    appendAssemblyString(ref assemblyString, instructions[opcode], registerString + r3.ToString(), "#" + address.ToString(), "");
                    break;
                case 14:
                    appendAssemblyString(ref assemblyString, instructions[opcode], registerString + r1.ToString(), registerString + r2.ToString(), "");
                    break;
                case 15:
                    appendAssemblyString(ref assemblyString, instructions[opcode], registerString + r2.ToString(), registerString + r3.ToString(), "");
                    break;
                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                    appendAssemblyString(ref assemblyString, instructions[opcode], registerString + r1.ToString(), registerString + r2.ToString(), registerString + r3.ToString());
                    break;
                case 27:
                    appendAssemblyString(ref assemblyString, instructions[opcode], registerString + r1.ToString(), registerString + r3.ToString(), "");
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
		 */
        public void appendAssemblyString(ref StringBuilder assemblyString, string instruction, string first, string second, string third)
        {
            if ((IM.ProgramCounter / 3) < CU.instructionsProcessed) //if the program counter is referencing an instruction we have already processed - Don't need the assembly syntax
                return;

            if (second != "")
                first += ",";
            if (third != "")
                second += ",";

            assemblyString.Append(instruction.ToUpper() + "\t" + first + second + third + "\n");
        }

        //NEEDS TO BE UPDATED//
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
        public void buildDecodedString(ref StringBuilder decodedString, Instruction instruction)
        {
            int opcode = instruction.opcode;
            int r1 = instruction.r1;
            int r2 = instruction.r2;
            int r3 = instruction.r3;
            int address = instruction.address;
            int instrFlag = instruction.instrFlag;
            string instrType = instruction.instrType;
            //If any of these variables is negative then they are not used in the current intruction and will be printed as N/A
            string r1Str, r2Str, r3Str, addressStr, instrFlagStr;
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
            switch (instrFlag)
            {
                case 1:
                    instrFlagStr = "STAT";
                    break;
                case 2:
                    instrFlagStr = "FLT";
                    break;
                case 3:
                    instrFlagStr = "FLST";
                    break;
                default:
                    instrFlagStr = "N/A";
                    break;
            }


            if (opcode == 0 || opcode == 1 || (opcode >= 11 && opcode <= 27))
            {
                decodedString.Append((string.Format("\n{0, 8} {1, 4} {2, 4} {3, 8} {4, 9} {5, 4} {6, 4} {7, 4} {8, 10}",
                            "0x" + instruction.programCounterValue.ToString("X").PadLeft(6, '0'), instrFlagStr, opcode.ToString("X"), instructions[opcode], instrType,
                            r1Str, r2Str, r3Str, addressStr)));
            }
            else
            {
                decodedString.Append((string.Format("\n{0, 8} {1, 4} {2, 4} {3, 8} {4, 9} {5, 4} {6, 4} {7, 4} {8, 10}",
                            "0x" + instruction.programCounterValue.ToString("X").PadLeft(6, '0'), instrFlagStr, opcode.ToString("X"), instructions[opcode], instrType,
                            r1Str, r2Str, r3Str, ("0x" + addressStr.PadLeft(6, '0')))));

            }
        }


        public void buildPipelineString(ref StringBuilder pipelineString, Instruction instruction)
        {

        }
    }
}
