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
        public Instruction stall = new Instruction();
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

                if (stages[4] != null && !WR.inProgress)
                {
                    if (stages[4].cycleControl == 0)
                    {
                        stages[4].stage5End = cycleCount - 1;
                        buildAssemblyString(ref assemblyString, ref stages[4]);    //Build the associated assembly syntax string for the instruction
                        buildDecodedString(ref decodedString, stages[4]);      //Build the decoded instruction string
                        buildPipelineString(ref pipelineString, ref stages[4]);   //Build the pipeline string
                        WR.success = false;
                        if (stages[4].opcode == 0)
                            halted = true;

                        stages[4] = null;
                        WR.occupied = false;
                    }
                }

            stage5:
                if (stages[4] != null)
                {
                    if (WR.success == false)
                    {
                        stages[4].stage5Start = cycleCount;
                        WR.writeToReg(registers, ref stages[4], ref config);
                        stages[4].stage = 4;
                    }
                    else
                        stages[4].cycleControl--;

                    if (stages[4].cycleControl == 0)
                        WR.inProgress = false;

                }
                if (stages[3] != null && !AM.inProgress)
                {
                    if (stages[3].cycleControl == 0)
                    {
                        if (!WR.occupied)
                        {
                            AM.success = false;
                            stages[3].stage4End = cycleCount - 1;
                            stages[4] = stages[3];
                            stages[3] = null;
                            AM.occupied = false;
                            WR.occupied = true;
                            goto stage5;
                        }
                    }
                }

            stage4:
                if (stages[3] != null)
                {
                    if (AM.success == false)
                    {
                        stages[3].stage4Start = cycleCount;
                        AM.accessMemory(ref dataMemory, ref registers, ref stages[3], ref config);
                        stages[3].stage = 4;
                    }
                    else
                        stages[3].cycleControl--;

                    if (stages[3].cycleControl == 0)
                        AM.inProgress = false;
                }

                if (stages[2] != null && !EU.inProgress)
                {
                    if (stages[2].cycleControl == 0)
                    {
                        if (!AM.occupied)
                        {
                            EU.success = false;
                            stages[2].stage3End = cycleCount - 1;
                            stages[3] = stages[2];
                            stages[2] = null;
                            EU.occupied = false;
                            AM.occupied = true;
                            goto stage4;
                        }
                    }
                }

            stage3:
                if (stages[2] != null)
                {
                    if (EU.success == false)
                    {
                        stages[2].stage3Start = cycleCount;
                        EU.execute(ref registers, ref dataMemory, ref alu, ref IM, ref stages[2], ref config);        //EXECUTE - Execute the instruction
                        stages[2].stage = 3;
                    }
                    else
                        stages[2].cycleControl--;

                    if (stages[2].cycleControl == 0)
                        EU.inProgress = false;

                }

                if(stages[1] != null && !CU.inProgress)
                {
                    if (stages[1].cycleControl == 0)
                    {
                        if (!EU.occupied)
                        {
                            CU.success = false;
                            stages[1].stage2End = cycleCount - 1;
                            stages[2] = stages[1];
                            stages[1] = null;
                            CU.occupied = false;
                            EU.occupied = true;
                            goto stage3;
                        }
                    }
                }
                //end stage 3
                

                stage2:
                if (stages[1] != null && cycleCount > 1)
                {

                    if (CU.success == false)
                    {
                        stages[1].stage2Start = cycleCount;
                        CU.decode(ref IM, ref stages[1], ref config);      //DECODE - Decode the instruction
                        stages[1].stage = 2;
                    }
                    else
                        stages[1].cycleControl--;

                    if (stages[1].cycleControl == 0)
                        CU.inProgress = false;
                }
                if(stages[0] != null && !fetch.inProgress)
                {
                    if (stages[0].cycleControl == 0)
                    {
                        fetch.inProgress = false;
                        if (!CU.occupied)
                        {
                            stages[0].stage1End = cycleCount - 1;
                            fetch.success = false;
                            stages[1] = stages[0];
                            stages[0] = null;
                            fetch.occupied = false;
                            CU.occupied = true;
                            goto stage2;
                        }
                    }
                }

                //end stage 2
                
                if (stages[0] == null)
                {
                    stages[0] = fetch.getNextInstruction(ref registers, ref IM, ref config);        //FETCH - get the next instruction
                    stages[0].stage1Start = cycleCount;
                    if (stages[0].cycleControl == 0)
                        fetch.inProgress = false;
                    continue;
                }

                //end stage 1


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
        public void buildAssemblyString(ref StringBuilder assemblyString, ref Instruction instruction)
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
                    appendAssemblyString(ref assemblyString, "STOP", "", "", "", ref instruction); //HALT
                    break;
                case 1:
                    appendAssemblyString(ref assemblyString, "NOP", "", "", "", ref instruction); //NOP
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
                    appendAssemblyString(ref assemblyString, instructions[opcode], "0x" + address.ToString("X").PadLeft(4, '0'), "", "", ref instruction);
                    break;
                case 11:
                case 12:
                case 13:
                    appendAssemblyString(ref assemblyString, instructions[opcode], registerString.Trim() + r3.ToString().Trim(), "#" + address.ToString().Trim(), "", ref instruction);
                    break;
                case 14:
                    appendAssemblyString(ref assemblyString, instructions[opcode], registerString.Trim() + r1.ToString(), registerString.Trim() + r2.ToString().Trim(), "", ref instruction);
                    break;
                case 15:
                    appendAssemblyString(ref assemblyString, instructions[opcode], registerString.Trim() + r2.ToString(), registerString.Trim() + r3.ToString().Trim(), "", ref instruction);
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
                    appendAssemblyString(ref assemblyString, instructions[opcode], registerString.Trim() + r1.ToString().Trim(), registerString.Trim() + r2.ToString().Trim(), 
                        registerString.Trim() + r3.ToString().Trim(), ref instruction);
                    break;
                case 27:
                    appendAssemblyString(ref assemblyString, instructions[opcode], registerString.Trim() + r1.ToString().Trim(), registerString.Trim() + r3.ToString().Trim(), "", ref instruction);
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
        public void appendAssemblyString(ref StringBuilder assemblyString, string instruction, string first, string second, string third, ref Instruction instruct)
        {
            if ((IM.ProgramCounter / 3) < CU.instructionsProcessed) //if the program counter is referencing an instruction we have already processed - Don't need the assembly syntax
                return;

            if (second != "")
                first += ",";
            if (third != "")
                second += ",";

            assemblyString.Append(instruction.ToUpper() + "\t" + first + second + third + "\n");
            instruct.assembly1 = instruction.ToUpper();
            instruct.assembly2 = first + second + third;

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


        public void buildPipelineString(ref StringBuilder pipelineString, ref Instruction instruction)
        {
            int opcode = instruction.opcode;
            int r1 = instruction.r1;
            int r2 = instruction.r2;
            int r3 = instruction.r3;
            int address = instruction.address;
            int instrFlag = instruction.instrFlag;
            string instrType = instruction.instrType;
            //If any of these variables is negative then they are not used in the current intruction and will be printed as N/A
            string r1Str, r2Str, r3Str, addressStr;
            //checks to see if the register is  a float or not
            string ifFloat1, ifFloat2, ifFloat3;
            string stage1, stage2, stage3, stage4, stage5;
            if (r1 >= 7)
                ifFloat1 = "f";
            else
                ifFloat1 = "r";

            if (r2 >= 7)
                ifFloat2 = "f";
            else
                ifFloat2 = "r";

            if (r3 >= 7)
                ifFloat3 = "f";
            else
                ifFloat3 = "r";
            //end of check for float reg's
            if (r1 == -1)
            {
                r1Str = "";
                ifFloat1 = "";
            }   
            else
                r1Str = r1.ToString("X");

            if (r2 == -1)
            {
                r2Str = "";
                ifFloat2 = "";
            }
            else
                r2Str = r2.ToString("X");
            if (r3 == -1)
            {
                r3Str = "";
                ifFloat3 = "";
            }
                
            else
                r3Str = r3.ToString("X");
            if (address == -1)
                addressStr = "";
            else
                addressStr = address.ToString("X");

            if (instruction.stage1Start == instruction.stage1End)
                stage1 = instruction.stage1Start.ToString();
            else
                stage1 = instruction.stage1Start.ToString() + " - " + instruction.stage1End.ToString();

            if (instruction.stage2Start == instruction.stage2End)
                stage2 = instruction.stage2Start.ToString();
            else
                stage2 = instruction.stage2Start.ToString() + " - " + instruction.stage2End.ToString();

            if (instruction.stage3Start == instruction.stage3End)
                stage3 = instruction.stage3Start.ToString();
            else
                stage3 = instruction.stage3Start.ToString() + " - " + instruction.stage3End.ToString();

            if (instruction.stage4Start == instruction.stage4End)
                stage4 = instruction.stage4Start.ToString();
            else
                stage4 = instruction.stage4Start.ToString() + " - " + instruction.stage4End.ToString();

            if (instruction.stage5Start == instruction.stage5End)
                stage5 = instruction.stage5Start.ToString();
            else
                stage5 = instruction.stage5Start.ToString() + " - " + instruction.stage5End.ToString();

            string output = (string.Format("\n{0, 6} {1,14} {2, 6} {3, 8} {4, 8} {5, 7} {6, 9}",
                            instruction.assembly1.PadRight(6),instruction.assembly2.PadRight(14), stage1.PadLeft(6), stage2.PadLeft(8), stage3.PadLeft(8), stage4.PadLeft(7), stage5.PadLeft(9)));

            pipelineString.Append(output);

          //  pipelineString.Append(instructions[opcode] +" "+ ifFloat1+r1Str + ", " +ifFloat2+r2Str + ", " + ifFloat3 +r3Str + "  " + instruction.stage1Start +" - " + instruction.stage1End +
          //      "     " + instruction.stage2Start + " - " + instruction.stage2End + "      " + instruction.stage3Start + " - " + instruction.stage3End + "     "+ instruction.stage4Start + " - " + 
          //      instruction.stage4End + "    " + instruction.stage5Start + " - " + instruction.stage5End +
          //       "\n");
            
        }
    }
}
