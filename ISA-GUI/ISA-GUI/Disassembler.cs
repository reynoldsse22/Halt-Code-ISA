using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISA_GUI
{
    internal class Disassembler
    {
        List<byte> instructions = new List<byte>();
        string assemblyString = "";
        static string[] instructionList = {"HALT",
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
        public Disassembler()
        {

        }

        public string Disassemble(List<string> program)
        {
            Instruction instruction = new Instruction();
            int offset = 0;
            for (int i = 0; i < program.Count; i++)
            {
                instructions.Add(0);
            }

            foreach (string b in program)
            {
                instructions[offset] = byte.Parse(program[offset++], System.Globalization.NumberStyles.HexNumber);
            }
            offset = 0;
            while(offset < instructions.Count)
            {
                instruction.binInstruction[0] = instructions[offset++];
                instruction.binInstruction[1] = instructions[offset++];
                instruction.binInstruction[2] = instructions[offset++];
                
                instruction = Decode(instruction);
                buildAssemblyString(instruction);
            }

            return assemblyString;
        }

        private Instruction Decode(Instruction instruction)
        {
            int opcode = instruction.opcode;
            int r1 = instruction.r1;
            int r2 = instruction.r2;
            int r3 = instruction.r3;
            int address = instruction.address;
            int instrFlag = instruction.instrFlag;
            string instrType = instruction.instrType;
            //value is -1 if not used in the current instruction
            int nibble1, temp;
            address = -1;
            r1 = -1;
            r2 = -1;
            r3 = -1;
            instrType = "";

            byte MSB = instruction.binInstruction[0];
            byte TSB = instruction.binInstruction[1];
            byte LSB = instruction.binInstruction[2];

            instrFlag = MSB >> 5;
            opcode = MSB & 31;

            if (((instrFlag & 2) >> 1) == 1)
                instruction.isFloat = true;
            else
                instruction.isFloat = false;

            if (opcode == 0) //Halt instruction
            {
                instrType = "Control";
            }
            else if (opcode == 1)
            {
                //No Op Instruction
            }
            else if (opcode >> 3 == 0 || (opcode & 31) == 8) //C-type instructions
            {
                instrType = "Control";
                nibble1 = MSB & 15;       //Gets the second nibble from the first byte and combines it with the second byte 
                address = (TSB << 8) | LSB;

                //if register above 6 is discovered, decrease by 7 to target floating point register
                if (r1 > 6)
                    r1 -= 7;
                if (r2 > 6)
                    r2 -= 7;
                if (r3 > 6)
                    r3 -= 7;
            }
            else if (opcode >> 4 == 1) //R-type instructions
            {
                instrType = "ALU";
                temp = opcode & 15; //Gets the first 3 bits of the opcode... Not used
                r1 = TSB;    //Gets the first, second, and destination registers for the R-Type instruction
                r2 = (LSB & 240) >> 4;
                r3 = LSB & 15;

                //if register above 6 is discovered, decrease by 7 to target floating point register
                if (r1 > 6)
                    r1 -= 7;
                if (r2 > 6)
                    r2 -= 7;
                if (r3 > 6)
                    r3 -= 7;

            }
            else if (opcode >> 3 == 1)
            {
                instrType = "Memory";
                //nibble1 = MSB & 15;    //Gets first register from the first byte
                address = (TSB << 8) | LSB;    //Finds Address of the Load/Store instructions
                r3 = LSB >> 4;

                switch (opcode)
                {
                    case 9:
                    case 10:
                        r3 = 0;
                        break;
                    case 11:
                    case 12:
                    case 13:
                        r1 = -1;
                        r2 = -1;
                        r3 = (TSB & 240) >> 4;
                        address = address & 4095;
                        break;
                    case 14:
                        r1 = r3;
                        r2 = LSB & 15;
                        r3 = -1;
                        address = -1;
                        break;
                    case 15:
                        r1 = -1;
                        r2 = r3;
                        r3 = LSB & 15;
                        address = -1;
                        break;

                }
                //if register above 6 is discovered, decrease by 7 to target floating point register
                if (r1 > 6)
                    r1 -= 7;
                if (r2 > 6)
                    r2 -= 7;
                if (r3 > 6)
                    r3 -= 7;

            }
            else
            {
                throw new Exception("Invalid Instruction!");
            }

            instruction.opcode = opcode;
            instruction.r1 = r1;
            instruction.r2 = r2;
            instruction.r3 = r3;
            instruction.instrFlag = instrFlag;
            instruction.address = address;
            instruction.instrType = instrType;
            return instruction;
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
        public void buildAssemblyString(Instruction instruction)
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
                    appendAssemblyString("HALT", "", "", "", ref instruction); //HALT
                    break;
                case 1:
                    appendAssemblyString("NOP", "", "", "", ref instruction); //NOP
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
                    appendAssemblyString(instructionList[opcode], "0x" + address.ToString("X").PadLeft(4, '0'), "", "", ref instruction);
                    break;
                case 11:
                case 12:
                case 13:
                    appendAssemblyString(instructionList[opcode], registerString.Trim() + r3.ToString().Trim(), "#" + address.ToString().Trim(), "", ref instruction);
                    break;
                case 14:
                    appendAssemblyString(instructionList[opcode], registerString.Trim() + r1.ToString(), registerString.Trim() + r2.ToString().Trim(), "", ref instruction);
                    break;
                case 15:
                    appendAssemblyString(instructionList[opcode], registerString.Trim() + r2.ToString(), registerString.Trim() + r3.ToString().Trim(), "", ref instruction);
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
                    appendAssemblyString(instructionList[opcode], registerString.Trim() + r1.ToString().Trim(), registerString.Trim() + r2.ToString().Trim(),
                        registerString.Trim() + r3.ToString().Trim(), ref instruction);
                    break;
                case 27:
                    appendAssemblyString(instructionList[opcode], registerString.Trim() + r1.ToString().Trim(), registerString.Trim() + r3.ToString().Trim(), "", ref instruction);
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
         private void appendAssemblyString(string instOp, string first, string second, string third, ref Instruction instruct)
         {
             string updatedAssembly = instOp.ToUpper();

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
             assemblyString += (updatedAssembly + " " + first + second + third + "\n");
             instruct.assembly2 = first + second + third;
            }
    }
}
