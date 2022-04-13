using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISA_GUI
{
    internal class Printer
    {
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
                                "LDHL",
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
        public Printer()
        {

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
            if (instrFlag == 2 || instrFlag == 3)
            {
                registerString = "f";
            }
            switch (opcode) //switch on the opcode value
            {
                case 0:
                    appendAssemblyString(ref assemblyString, "HALT", "", "", "", ref instruction); //HALT
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
            string updatedAssembly = instruction.ToUpper();

            if (second != "")
                first += ",";
            if (third != "")
                second += ",";

            if (instruct.opcode >= 9)
            {
                if (instruct.instrFlag == 2)
                    updatedAssembly = "f." + updatedAssembly;
                else if (instruct.instrFlag == 1)
                    updatedAssembly += ".s";
                else if (instruct.instrFlag == 3)
                    updatedAssembly = "f." + updatedAssembly + 's';
            }
            instruct.assembly1 = updatedAssembly;
            assemblyString.Append(updatedAssembly + " " + first + second + third + "\n");
            instruct.assembly2 = first + second + third;
        }

        /**
        * Method Name: buildDecodedString <br>
        * Method Purpose: Appends to the decoded instruction string to include the current instruction
        * 
        * <br>
        * Date created: 2/19/22 <br>
        * <hr>
        *   @param  StringBuilder decodedString
        *   @param Instruction instruction
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

        /**
        * Method Name: buildPipelineString <br>
        * Method Purpose: Builds the pipeline output for the instruction across all 5 stages
        * 
        * <br>
        * Date created: 3/02/22 <br>
        * <hr>
        *   @param  StringBuilder pipelineString
        *   @param  Instruction instruction
        */
        public void buildPipelineString(ref StringBuilder pipelineString, ref Instruction instruction)
        {
            string stage1, stage2, stage3, stage4, stage5;

            if (instruction.stage1Start == instruction.stage1End)
                stage1 = instruction.stage1Start.ToString();
            else
                stage1 = instruction.stage1Start.ToString() + "-" + instruction.stage1End.ToString();

            if (instruction.stage2Start == instruction.stage2End)
                stage2 = instruction.stage2Start.ToString();
            else
                stage2 = instruction.stage2Start.ToString() + "-" + instruction.stage2End.ToString();

            if (instruction.stage3Start == instruction.stage3End)
                stage3 = instruction.stage3Start.ToString();
            else
                stage3 = instruction.stage3Start.ToString() + "-" + instruction.stage3End.ToString();

            if (instruction.stage4Start == instruction.stage4End)
                stage4 = instruction.stage4Start.ToString();
            else
                stage4 = instruction.stage4Start.ToString() + "-" + instruction.stage4End.ToString();

            if (instruction.stage5Start == instruction.stage5End)
                stage5 = instruction.stage5Start.ToString();
            else
                stage5 = instruction.stage5Start.ToString() + "-" + instruction.stage5End.ToString();
            if (stage1 == "0")
                stage1 = " ";
            if (stage2 == "0")
                stage2 = " ";
            if (stage3 == "0")
                stage3 = " ";
            if (stage4 == "0")
                stage4 = " ";
            if (stage5 == "0")
                stage5 = " ";

            string output = (string.Format("\n{0, 7} {1,13} {2, 7} {3, 8} {4, 8} {5, 7} {6, 9}",
                            instruction.assembly1.PadRight(7), instruction.assembly2.PadRight(13), stage1.PadLeft(7), stage2.PadLeft(8), stage3.PadLeft(8), stage4.PadLeft(7), stage5.PadLeft(9)));

            pipelineString.Append(output);

        }

        public void buildDynamicPipelineString(ref StringBuilder pipelineString, Instruction instruction)
        {
            string stage1, stage2, stage3, stage4, stage5;

            stage1 = instruction.stage1Cycle.ToString();

            if (instruction.stage2CycleStart == instruction.stage2CycleEnd)
                stage2 = instruction.stage2CycleEnd.ToString();
            else
                stage2 = instruction.stage2CycleStart.ToString() + "-" + instruction.stage2CycleEnd.ToString();

            if (instruction.stage3CycleStart == instruction.stage3CycleEnd)
                stage3 = instruction.stage3CycleEnd.ToString();
            else
                stage3 = instruction.stage3CycleStart.ToString() + "-" + instruction.stage3CycleEnd.ToString();

            stage4 = instruction.stage4Cycle.ToString();

            stage5 = instruction.stage5Cycle.ToString();


            if (stage1 == "0")
                stage1 = " ";
            if (stage2 == "0")
                stage2 = " ";
            if (stage3 == "0")
                stage3 = " ";
            if (stage4 == "0")
                stage4 = " ";
            if (stage5 == "0")
                stage5 = " ";

            string output = (string.Format("\n{0, 7} {1,13} {2, 7} {3, 8} {4, 8} {5, 7} {6, 9}",
                            instruction.assembly1.PadRight(7), instruction.assembly2.PadRight(13), stage1.PadLeft(7), stage2.PadLeft(8), stage3.PadLeft(8), stage4.PadLeft(7), stage5.PadLeft(9)));

            pipelineString.Append(output);
        }
    }
}
