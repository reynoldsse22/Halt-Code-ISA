﻿// ---------------------------------------------------------------------------
// File name: Pep16.cs
// Project name: ISA-GUI
// ---------------------------------------------------------------------------
// Creators: Samuel Reynolds, Nick Farmer, Carlos Ortiz, & Brandon Beaudry						
// Course-Section: CSCI 4717-201
// Creation Date: 2/6/22		
// ---------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ISA_GUI
{
    public partial class Pep16 : Form
    {
        static string[] instructions = {"HALT",
                                "BR",
                                "BRNE",
                                "BREQ",
                                "LDWM",
                                "STWM",
                                "MOV",
                                "LDWI",
                                "ASL",
                                "ASR",
                                "ADD",
                                "SUB",
                                "AND",
                                "OR",
                                "XOR",
                                "NOT"};

        static int controlInstructionCount = 0;
        static int ArithInstructionCount = 0;
        static int memoryInstructionCount = 0;
        static int memoryOffset = 0;
        static ushort[] registers = new ushort[16];
        static byte[] MainMemory = new byte[1048576];
        static string decodedOutput = "";
        static string decodedAssembly = "";
        bool doneExecuting = false;

        public Pep16()
        {
            InitializeComponent();
            this.Focus();
        }

        /// <summary> Initializes all values and output fields on load of windows form</summary>
        private void Pep16_Load(object sender, EventArgs e)
        {
            setMemoryBox();
            clearRegandMem();
            setRegisters();
        }

        /// <summary> On RUN once the start button is clicked</summary>
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            clearProgram();
            doneExecuting = false;
            startDecoding();
        }

        /// <summary> Prints out the contents of main memory to the GUI</summary>
        private void setMemoryBox() 
        {
            int offset = 0;
            string line = "ADDRESS |   0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F  |\n";
            line += "-------------------------------------------------------------\n";
            int index = 0;
            for (int i = 0; i < (500); i++)
            {
                line += "0x" + offset.ToString("x").PadLeft(4, '0').ToUpper() + "  ";
                line += "|  ";
                for (int j = 0; j < 16; j++)
                {
                    line += MainMemory[index].ToString("x").PadLeft(2, '0').ToUpper();
                    line += " ";
                    index++;
                }
                offset += 16;
                line += " |\n";

            }

            MemoryText.Text = line;
        }

        private void clearRegandMem()
        {
            //Clear registers
            for (int i = 0; i < 16; i++)
            {
                registers[i] = 0;
            }

            //clear main memory
            for (int i = 0; i < 1048576; i++)
            {
                MainMemory[i] = 0;
            }
        }

        /// <summary> Updates the register window values</summary>
        private void setRegisters()
        {
            int clear = 0;
            int zero = 0;
            //Initialize the hexidecimal text field to 0
            //Pad left ensures that the value will be 4 digits.
            r0Hex.Text = "0x" + registers[0].ToString("x").PadLeft(4, '0');
            r1Hex.Text = "0x" + registers[1].ToString("x").PadLeft(4, '0');
            r2Hex.Text = "0x" + registers[2].ToString("x").PadLeft(4, '0');
            r3Hex.Text = "0x" + registers[3].ToString("x").PadLeft(4, '0');
            r4Hex.Text = "0x" + registers[4].ToString("x").PadLeft(4, '0');
            r5Hex.Text = "0x" + registers[5].ToString("x").PadLeft(4, '0');
            r6Hex.Text = "0x" + registers[6].ToString("x").PadLeft(4, '0');
            r7Hex.Text = "0x" + registers[7].ToString("x").PadLeft(4, '0');
            r8Hex.Text = "0x" + registers[8].ToString("x").PadLeft(4, '0');
            r9Hex.Text = "0x" + registers[9].ToString("x").PadLeft(4, '0');
            r10Hex.Text = "0x" + registers[10].ToString("x").PadLeft(4, '0');
            r11Hex.Text = "0x" + registers[11].ToString("x").PadLeft(4, '0');
            r12Hex.Text = "0x" + registers[12].ToString("x").PadLeft(4, '0');
            asprHex.Text = "0x" + registers[13].ToString("x").PadLeft(4, '0');
            ipHex.Text = "0x" + registers[14].ToString("x").PadLeft(4, '0');
            pcHex.Text = "0x" + registers[15].ToString("x").PadLeft(4, '0');

            //Initialize the decimal register text fields to 0
            r0Dec.Text = registers[0].ToString();
            r1Dec.Text = registers[1].ToString();
            r2Dec.Text = registers[2].ToString();
            r3Dec.Text = registers[3].ToString();
            r4Dec.Text = registers[4].ToString();
            r5Dec.Text = registers[5].ToString();
            r6Dec.Text = registers[6].ToString();
            r7Dec.Text = registers[7].ToString();
            r8Dec.Text = registers[8].ToString();
            r9Dec.Text = registers[9].ToString();
            r10Dec.Text = registers[10].ToString();
            r11Dec.Text = registers[11].ToString();
            r12Dec.Text = registers[12].ToString();
            asprDec.Text = registers[13].ToString();
            ipDec.Text = registers[14].ToString();
            pcDec.Text = registers[15].ToString();

            if((registers[13] & 2) == 2)
                ZFlagBox.Text = 1.ToString();
            else
                ZFlagBox.Text = 0.ToString();

            if ((registers[13] & 1) == 1)
                cFlagBox.Text = 1.ToString();
            else
                cFlagBox.Text = 0.ToString();
        }

        /// <summary>Separates every 16-bit instruction into two bytes 
        /// and stores it in memory. Also calls the decodeInstruction function</summary>
        /// <param name="program">Gets the input </param>
        private void getInstructionDetails(string[] program)
        {
            int byte1, byte2, totalInst;

            while(!doneExecuting)
            {
                byte1 = int.Parse(program[registers[14]], System.Globalization.NumberStyles.HexNumber);
                byte2 = int.Parse(program[registers[14] + 1], System.Globalization.NumberStyles.HexNumber);
                decodeInstruction(byte1, byte2);
                MainMemory[memoryOffset] = (byte)byte1;
                memoryOffset++;
                MainMemory[memoryOffset] = (byte)byte2;
                memoryOffset++;
            }

            totalInst = registers[15] / 2;
        }

        /// <summary>Kicks off the decoding and gets inputted machine code from the user</summary>
        private void startDecoding()
        {
            string pr = "";
            string[] program;
            decodedOutput += "   Team: Beaudry, Farmer, Ortiz, Reynolds\n";
            decodedOutput += "Project: ISA Design & Implementation\n";
            decodedOutput += "----------------------------------------------------------------\n\n";
            decodedOutput += "Program Inst Instruct                           Address/\n";
            decodedOutput += "Counter Spec Mnemonic      Type FReg SReg DReg Immediate\n";
            decodedOutput += "------- ---- -------- --------- ---- ---- ---- ---------";
            
            AssemblerListingTextBox.Text = decodedOutput;

            decodedAssembly = "----------------------------------------------------------------\n";
            try
            {
                pr = InputBox.Text;
                program = pr.Split(' ');        //Splits the given program into different instructions
                getInstructionDetails(program);                //The splited program is given to this method to be disassembled
                setStatistics();
            }
            catch (Exception)
            {
                throwException("Error! Invalid Instruction.");
            }

        }

        /// <summary> Decodes the instruction into its assembly listing</summary>
        /// <param name="byte1">Holds the first byte of the instruction </param>
        /// <param name="byte2">Holds the second byte of the instruction </param>
        private void decodeInstruction(int byte1, int byte2)
        {
            int opcode, nibble1, temp;
            int address = -1;
            int r1 = -1;
            int r2 = -1;
            int rdest = -1;

            string instrType = "";
            opcode = byte1 >> 4;            //Gets the opcode from the first byte

            if (opcode == 0)
            {
                instrType = "Control";

                printDecodedInstruction(opcode, instrType, r1.ToString("X"), r2.ToString("X"), rdest.ToString("X"), address.ToString("X"));
                printAssembly("STOP", "", "", "");
                controlInstructionCount++;
                registers[15] = (ushort)(registers[15] + 2);  //Updates program counter
                registers[14] += 2;                         //Updates Instruction Pointer
                setMemoryBox();                             //Updates the main memory display box
                setRegisters();
                doneExecuting = true;                       //Done executing
                return;
            }

            else if (opcode >> 2 == 0)            //Runs if the instruction is a Control type instruction
            {
                instrType = "Control";


                nibble1 = byte1 & 15;       //Gets the second nibble from the first byte and combines it with the second byte 
                nibble1 = nibble1 << 8;
                address = nibble1 + byte2;
                controlInstructionCount++;

                decodeCType(opcode, byte1, byte2);

                printDecodedInstruction(opcode, instrType, r1.ToString("X"), r2.ToString("X"), rdest.ToString("X"), ("0x" + address.ToString("X").PadLeft(4, '0')));
                registers[15] = (ushort)(registers[15] + 2); //Updates program counter
            }
            else if (opcode >> 2 == 1)  //Runs if the instruction is a Load/Store type of instruction
            {
                //temp = opcode & 3;  //Finds out what instruction it is in the I-type category
                r1 = byte1 & 15;    //Gets first register from the first byte
                address = (r1 << 8)| byte2;    //Finds Address of the Load/Store instructions
                memoryInstructionCount++;
                rdest = byte2 & 15;

                instrType = "Memory";


                switch (opcode)
                {
                    case 4:
                        registers[0] = MainMemory[address];
                        r1 = -1;
                        r2 = -1;
                        rdest = 0;
                        printDecodedInstruction(opcode, instrType, r1.ToString("X"), r2.ToString("X"), rdest.ToString("X"), ("0x" + address.ToString("X").PadLeft(4, '0')));
                        printAssembly("LDWM", "r" + rdest.ToString(), "0x" + address.ToString("X").PadLeft(4, '0'), "");
                        break;
                    case 5:
                        MainMemory[address] = (byte)registers[0];
                        r1 = -1;
                        r2 = 0;
                        rdest = -1;
                        printDecodedInstruction(opcode, instrType, r1.ToString("X"), r2.ToString("X"), rdest.ToString("X"), ("0x" + address.ToString("X").PadLeft(4, '0')));
                        printAssembly("STWM", "r" + r2.ToString(), "0x" + address.ToString("X").PadLeft(4, '0'), "");
                        break;
                    case 6:
                        r1 = rdest;
                        rdest = byte2 >> 4;    //MOV is a special instruction that has a destination register instead of an address
                        registers[r1] = registers[rdest];
                        r2 = rdest;
                        rdest = r1;
                        r1 = -1;
                        address = -1;
                        printDecodedInstruction(opcode, instrType, r1.ToString("X"), r2.ToString("X"), rdest.ToString("X"), address.ToString("X"));
                        printAssembly("MOV", "r" + rdest.ToString(), "r" + r2.ToString(), "");
                        break;
                    case 7:
                        registers[0] = (ushort)address;
                        r1 = -1;
                        r2 = -1;
                        rdest = 0;
                        printDecodedInstruction(opcode, instrType, r1.ToString("X"), r2.ToString("X"), rdest.ToString("X"), ("0x" + address.ToString("X").PadLeft(4, '0')));
                        printAssembly("LDWI", "r" + rdest.ToString(), "#" + address.ToString(), "");
                        break;
                }
                

                registers[15] = (ushort)(registers[15] + 2); //Updates program counter
                registers[14] += 2;                         //Updates Instruction Pointer
            }
            else if(opcode >> 3 == 1) //Runs for arithmetic instructions or R-type instructions
            {
                instrType = "ALU";


                temp = opcode & 15; //Gets the first 3 bits of the opcode... Not used
                r1 = byte1 & 15;    //Gets the first, second, and destination registers for the R-Type instruction
                r2 = byte2 >> 4;
                rdest = byte2 & 15;

                printDecodedInstruction(opcode, instrType, r1.ToString("X"), r2.ToString("X"), rdest.ToString("X"), address.ToString("X"));

                decodeRType(temp, r1, r2, rdest);

                ArithInstructionCount++;

                registers[15] = (ushort)(registers[15] + 2); //Updates program counter
                registers[14] += 2;                         //Updates Instruction Pointer

            }
            else
            {
                throwException("Invalid Instruction!");
            }
            setRegisters();
        }


        /// <summary>Decodes the C-Type Instructions</summary>
        private void decodeCType(int opcode, int byte1, int byte2)
        {
            int r1 = byte1 & 15;
            int address = (r1 << 8) | byte2;
            
            switch(opcode)
            {
                case 1: //unconditional branch
                    registers[14] = (ushort)address;
                    printAssembly("BR", "0x" + address.ToString("X").PadLeft(4, '0'), "", "");
                    break;
                case 2: //branch not equal
                    if((registers[13] & 2) == 0)
                        registers[14] = (ushort)address;
                    else
                        registers[14] += 2;                         //Updates Instruction Pointer
                    printAssembly("BRNE", "0x" + address.ToString("X").PadLeft(4, '0'), "", "");
                    break;
                case 3: //branch if equal
                    if ((registers[13] & 2) == 1)
                        registers[14] = (ushort)address;
                    else
                        registers[14] += 2;                         //Updates Instruction Pointer
                    printAssembly("BREQ", "0x" + address.ToString("X").PadLeft(4, '0'), "", "");
                    break;
            }
           
            
        }

        /// <summary>Decodes the R-Type Instructions</summary>
        private void decodeRType(int opcode, int firstReg, int secondReg, int rd)
        {
            int statusFlag = 0;
            bool zero = false;
            bool carry = false;
            switch(opcode)
            {
                case 8:
                    //shift left
                    registers[rd] = (ushort)(registers[firstReg] << registers[secondReg]);
                    if(registers[rd] == 0)
                        zero = true;
                    printAssembly("ASL", "r" + firstReg, "#" + secondReg.ToString(), "r" + rd);
                    break;
                case 9:
                    //shift right
                    registers[rd] = (ushort)(registers[firstReg] >> registers[secondReg]);
                    if (registers[rd] == 0)
                        zero = true;
                    printAssembly("ASR", "r" + firstReg, "#" + secondReg.ToString(), "r" + rd);
                    break;
                case 10:
                    //Add
                    registers[rd] = (ushort)(registers[firstReg] + registers[secondReg]);
                    if (registers[rd] < registers[firstReg])
                        carry = true;
                    if (registers[rd] == 0)
                        zero = true;
                    printAssembly("ADD", "r" + firstReg, "r" + secondReg.ToString(), "r" + rd);
                    break;
                case 11:
                    //Sub
                    registers[rd] = (ushort)(registers[firstReg] - registers[secondReg]);
                    if (registers[rd] > registers[firstReg])
                        carry = true;
                    if (registers[rd] == 0)
                        zero = true;
                    printAssembly("SUB", "r" + firstReg, "r" + secondReg.ToString(), "r" + rd);
                    break;
                case 12:
                    //And
                    registers[rd] = (ushort)(registers[firstReg] & registers[secondReg]);
                    if (registers[rd] == 0)
                        zero = true;
                    printAssembly("AND", "r" + firstReg, "r" + secondReg.ToString(), "r" + rd);
                    break;
                case 13:
                    //Or
                    registers[rd] = (ushort)(registers[firstReg] | registers[secondReg]);
                    if (registers[rd] == 0)
                        zero = true;
                    printAssembly("OR", "r" + firstReg, "r" + secondReg.ToString(), "r" + rd);
                    break;
                case 14:
                    //Xor
                    registers[rd] = (ushort)(registers[firstReg] ^ registers[secondReg]);
                    if (registers[rd] == 0)
                        zero = true;
                    printAssembly("XOR", "r" + firstReg, "r" + secondReg.ToString(), "r" + rd);
                    break;
                case 15:
                    //Not
                    registers[rd] = (ushort)(~registers[firstReg]);
                    if (registers[rd] == 0)
                        zero = true;
                    printAssembly("NOT", "r" + firstReg, "r" + rd.ToString(),"");
                    break;
            }

            if (zero)
                statusFlag += 2;
            if (carry)
                statusFlag += 1;

            registers[13] = (ushort)statusFlag;
            setRegisters();
        }

        /// Sets the output field with the statistics of the program </summary>
        private void setStatistics()
        {
            setMemoryBox();
            int totalInst = registers[15] / 2;
            string statistics = "";
            statistics += "Summary Statistics\n";
            statistics += "------------------\n";
            statistics += String.Format("Total instructions:              {0}\n", totalInst);                       //since we always go by 2's total instructions was pretty simple
            statistics += String.Format("Control instructions:            {0}, {1}%\n", controlInstructionCount, Math.Round((double)controlInstructionCount / totalInst * 100, 2));
            statistics += String.Format("Arithmetic & logic instructions: {0}, {1}%\n", ArithInstructionCount, Math.Round((double)ArithInstructionCount / totalInst * 100, 2));
            statistics += String.Format("Memory instructions:             {0}, {1}%\n", memoryInstructionCount, Math.Round((double)memoryInstructionCount / totalInst * 100, 2));

            StatsTextBox.Text = statistics;
        }


        /// <summary>Prints the details of the given instruction.</summary>
        /// <param name="opcode">The opcode</param>
        /// <param name="instrType">Instruction type</param>
        /// <param name="r1">First register</param>
        /// <param name="r2">Second register</param>
        /// <param name="rdest">Register destination</param>
        /// <param name="address">Address of the instruction</param>
        private void printDecodedInstruction(int opcode, string instrType, string r1, string r2, string rdest, string address)
        {
            if (r1 == "FFFFFFFF") //If any of these variables is a -1 in hex then they are not used in the current intruction and will be printed as N/A
                r1 = "N/A";
            if (r2 == "FFFFFFFF")
                r2 = "N/A";
            if (rdest == "FFFFFFFF")
                rdest = "N/A";
            if (address == "FFFFFFFF")
                address = "N/A";

            decodedOutput += (string.Format("\n{0, 7} {1, 4} {2, 8} {3, 9} {4, 4} {5, 4} {6, 4} {7, 8}",
                            registers[15].ToString("X").PadLeft(4, '0'), opcode.ToString("X"), instructions[opcode], instrType,
                            r1, r2, rdest, address));

            AssemblerListingTextBox.Text = decodedOutput;
        }

        private void printAssembly(string instruction, string first, string second, string third)
        {
            if (second != "")
                first += ",";
            if (third != "")
                second += ",";
            decodedAssembly += (instruction.ToUpper() + "\t" + first  + second +  third + "\n");

            AssemblyTextBox.Text = decodedAssembly;
        }


        /// <summary>Clears registers, memory, and any output fields for new run of program.</summary>
        private void clearProgram()
        {
            memoryOffset = 0;
            controlInstructionCount = 0;
            ArithInstructionCount = 0;
            memoryInstructionCount = 0;
            clearRegandMem();
            setRegisters();
            //setMemoryBox();
            AssemblerListingTextBox.Text = "";
            StatsTextBox.Text = "";
            decodedOutput = "";
        }

        /// <summary>Used to kill the program in case of any error in decoding data arises</summary>
        private void throwException(string message)
        {
            AssemblerListingTextBox.Text = message;
            StatsTextBox.Text = message;
            doneExecuting = true;
        }

        

    }

}
