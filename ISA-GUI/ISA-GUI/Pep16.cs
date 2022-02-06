// ---------------------------------------------------------------------------
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
                                "LDRW",
                                "STRW",
                                "MOV",
                                "LDRI",
                                "ASL",
                                "ASR",
                                "ADD",
                                "SUB",
                                "AND",
                                "OR",
                                "XOR",
                                "NOT"};

        static int controlInstrunctionCount = 0;
        static int ArithInstrunctionCount = 0;
        static int memoryInstrunctionCount = 0;
        static int memoryOffset = 0;
        static short[] registers = new short[16];

        static byte[] MainMemory = new byte[1048576];

        static string decodedOutput = "";

        public Pep16()
        {
            InitializeComponent();
            this.Focus();
        }

        /// <summary> Initializes all values and output fields on load of windows form</summary>
        private void Pep16_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < 16; i++)
            {
                registers[i] = 0;
            }

            for (int i = 0; i < 1048576; i++)
            {
                MainMemory[i] = 0;
            }
            setMemoryBox();
            setRegisters();
        }

        /// <summary> On RUN once the start button is clicked</summary>
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            clearProgram();
            startDecoding();
        }

        /// <summary> Prints out the contents of main memory to the GUI</summary>
        private void setMemoryBox()
        {

            int offset = 0;
            string line = "";
            int index = 0;
            for (int i = 0; i < (100); i++)
            {
                line += "0x" + offset.ToString("x").PadLeft(4, '0') + "  ";
                line += "\t";
                for (int j = 0; j < 16; j++)
                {
                    line += MainMemory[index].ToString("x").PadLeft(2, '0');
                    line += " ";
                    index++;
                }
                offset += 16;
                line += "\n";

            }

            MemoryText.Text = line;
        }

        /// <summary> Initializes and clears registers/main memory</summary>
        private void setRegisters()
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
        }

        /// <summary>Separates every 16-bit instruction into two bytes 
        /// and stores it in memory. Also calls the decodeInstruction function</summary>
        /// <param name="program">Gets the input </param>
        private void getInstructionDetails(string[] program)
        {
            int byte1, byte2, totalInst;
            for (int x = 0; x < program.Length; x = x + 2)      //Gets two bytes from the instructions and finds out what that instruction does.
            {
                byte1 = int.Parse(program[x], System.Globalization.NumberStyles.HexNumber);
                byte2 = int.Parse(program[x + 1], System.Globalization.NumberStyles.HexNumber);
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
            decodedOutput += "Project: ISA Design & Implementation\n\n";
            decodedOutput += "Program Inst Instruct\n";
            decodedOutput += "Counter Spec Mnemonic      Type FReg SReg DReg  Address\n";
            decodedOutput += "------- ---- -------- --------- ---- ---- ---- --------";

            AssemblerListingTextBox.Text = decodedOutput;

            try
            {
                pr = InputBox.Text;
                program = pr.Split(' ');        //Splits the given program into different instructions

                getInstructionDetails(program);                //The splited program is given to this method to be disassembled
            }
            catch (Exception)
            {
                decodedOutput = "Error! Invalid Instruction.";
            }

            setStatistics();
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

                controlInstrunctionCount++;
                registers[15] = (short)(registers[15] + 2);  //Updates program counter
                decodeCType(byte1, byte2);
                return;
            }

            else if (opcode >> 2 == 0)            //Runs if the instruction is a Control type instruction
            {
                instrType = "Control";

                printDecodedInstruction(opcode, instrType, r1.ToString("X"), r2.ToString("X"), rdest.ToString("X"), address.ToString("X"));

                nibble1 = byte1 & 15;       //Gets the second nibble from the first byte and combines it with the second byte 
                nibble1 = nibble1 << 8;
                address = nibble1 + byte2;
                controlInstrunctionCount++;

                registers[15] = (short)(registers[15] + 2); //Updates program counter
            }
            else if (opcode >> 2 == 1)  //Runs if the instruction is a Load/Store type of instruction
            {
                temp = opcode & 3;  //Finds out what instruction it is in the I-type category
                r1 = byte1 & 15;    //Gets first register from the first byte
                address = byte2;    //Finds Address of the Load/Store instructions
                memoryInstrunctionCount++;

                instrType = "Direct";

                printDecodedInstruction(opcode, instrType, r1.ToString("X"), r2.ToString("X"), rdest.ToString("X"), address.ToString("X"));

                switch (temp)
                {
                    case 0:

                        break;
                    case 1:


                        break;
                    case 2:
                        rdest = byte2 & 15;    //MOV is a special instruction that has a destination register instead of an address
                        address = -1;
                        instrType = "Immediate";

                        break;
                    case 3:

                        break;



                }
                registers[15] = (short)(registers[15] + 2); //Updates program counter
            }
            else //Runs for arithmetic instructions or R-type instructions
            {
                instrType = "ALU";

                printDecodedInstruction(opcode, instrType, r1.ToString("X"), r2.ToString("X"), rdest.ToString("X"), address.ToString("X"));

                temp = opcode & 15; //Gets the first 3 bits of the opcode... Not used
                r1 = byte1 & 15;    //Gets the first, second, and destination registers for the R-Type instruction
                r2 = byte2 >> 4;
                rdest = byte2 & 15;
                ArithInstrunctionCount++;

                registers[15] = (short)(registers[15] + 2); //Updates program counter
            }
        }

        /// <summary>Decodes the I-Type Instructions</summary>
        private void decodeIType(int byte1, int byte2)
        {

        }

        /// <summary>Decodes the C-Type Instructions</summary>
        private void decodeCType(int byte1, int byte2)
        {

        }

        /// <summary>Decodes the R-Type Instructions</summary>
        private void decodeRType(int byte1, int byte2)
        {

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
            statistics += String.Format("Control instructions:            {0}, {1}%\n", controlInstrunctionCount, Math.Round((double)controlInstrunctionCount / totalInst * 100, 2));
            statistics += String.Format("Arithmetic & logic instructions: {0}, {1}%\n", ArithInstrunctionCount, Math.Round((double)ArithInstrunctionCount / totalInst * 100, 2));
            statistics += String.Format("Memory instructions:             {0}, {1}%\n", memoryInstrunctionCount, Math.Round((double)memoryInstrunctionCount / totalInst * 100, 2));

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

        /// <summary>Clears registers, memory, and any output fields for new run of program.</summary>
        private void clearProgram()
        {
            memoryOffset = 0;
            controlInstrunctionCount = 0;
            ArithInstrunctionCount = 0;
            memoryInstrunctionCount = 0;
            setRegisters();
            //setMemoryBox();
            AssemblerListingTextBox.Text = "";
            StatsTextBox.Text = "";
            decodedOutput = "";
        }

    }

}
