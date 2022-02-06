// ---------------------------------------------------------------------------
// File name: ISA.cs
// Project name: ISA-Disassembler
// ---------------------------------------------------------------------------
// Creators: Samuel Reynolds, Nick Farmer, Carlos Ortiz, & Brandon Beaudry						
// Course-Section: CSCI 4717-201
// Creation Date: 2/4/22		
// ---------------------------------------------------------------------------

//Nick Farmer Testing
//test 2
class ISA
{
    /**
	* Class Name: ISA <br>
	* Class Purpose: 
	* 
	* <hr>
	* Date created: 2/4/22 <br>
	* @author Samuel Reynolds
	*/
    static string[] instructions = {"HALT",
                                "BR",
                                "BRNE",
                                "BREQ",
                                "LDRW",
                                "STRW",
                                "MOV",
                                "LDRI",
                                "SHIFTL",
                                "SHIFTR",
                                "ADD",
                                "SUB",
                                "AND",
                                "OR",
                                "XOR",
                                "NOT"};
    static int programCounter = 0;
    static int controlInstrunctionCount = 0;
    static int ArithInstrunctionCount = 0;
    static int memoryInstrunctionCount = 0;

   


    /// <summary>Defines the entry point of the application.</summary>
    /// <param name="args">The arguments.</param>
    static void Main(string[] args)
	{
        string pr = "";     
        string[] program;
        int[] counter = new int[16];
        Console.WriteLine("Enter the program you want to disassemble in Hex");
        try
        {
            pr = Console.ReadLine();
            program = pr.Split(' ');        //Splits the given program into different instructions

            decode(program);                //The splited program is given to this method to be disassembled
        }
        catch (Exception)
        {
            Console.WriteLine("Enter Valid Input");
        }

	}

    /// <summary>Starts the disassembling process of the given code.</summary>
    /// <param name="program">The program.</param>
    private static void decode(string[] program)
    {
        int byte1, byte2, totalInst;

        Console.WriteLine("   Team: Farmer, Reynolds, Ortiz, Beaudry");
        Console.WriteLine("Project: ISA Design & Implementation\n");
        Console.WriteLine("Program Inst Instruct");
        Console.WriteLine("Counter Spec Mnemonic      Type FReg SReg DReg  Address");
        Console.WriteLine("------- ---- -------- --------- ---- ---- ---- --------");
        for (int x = 0; x < program.Length; x = x + 2)      //Gets two bytes from the instructions and finds out what that instruction does.
        {
            byte1 = int.Parse(program[x], System.Globalization.NumberStyles.HexNumber);
            byte2 = int.Parse(program[x+1], System.Globalization.NumberStyles.HexNumber);
            findDetails(byte1, byte2);
        }

        totalInst = programCounter / 2;
        //start of the Summary Stats
        Console.WriteLine();
        Console.WriteLine("Summary Statistics");
        Console.WriteLine("------------------"); 
        Console.WriteLine("Total instructions:              {0}", totalInst);                       //since we always go by 2's total instructions was pretty simple
        Console.WriteLine("Control instructions:            {0}, {1}%", controlInstrunctionCount, Math.Round((double) controlInstrunctionCount / totalInst * 100, 2));
        Console.WriteLine("Arithmetic & logic instructions: {0}, {1}%", ArithInstrunctionCount, Math.Round((double) ArithInstrunctionCount / totalInst * 100, 2));
        Console.WriteLine("Memory instructions:             {0}, {1}%", memoryInstrunctionCount, Math.Round((double) memoryInstrunctionCount / totalInst * 100, 2));
        Console.WriteLine();
        //end of summary Stats
        Console.WriteLine("The Program is finished");  //The program is finished once there are no more bytes to process.
        Console.ReadLine();
    }

    /// <summary>
    ///   <para>
    /// Finds the details from the opcode to find what instruction the machine code is trying to do</para>
    /// </summary>
    /// <param name="byte1">First byte of a given instruction</param>
    /// <param name="byte2">Second byte of a given instruction</param>
    private static void findDetails(int byte1, int byte2)
    {
        int opcode, nibble1, temp;
        int address = -1;
        int r1 = -1;
        int r2 = -1;
        int rdest = -1;

        string instrType = "";
        opcode = byte1 >> 4;            //Gets the opcode from the first byte

        if(opcode == 0)
        {
            instrType = "Control";

            printDetails(opcode, instrType, r1.ToString("X"), r2.ToString("X"), rdest.ToString("X"), address.ToString("X"));

            controlInstrunctionCount++;
            programCounter = programCounter + 2;

            return;
        }

        else if(opcode >> 2 == 0)            //Runs if the instruction is a Control type instruction
        {
            instrType = "Control";
            nibble1 = byte1 & 15;       //Gets the second nibble from the first byte and combines it with the second byte 
            nibble1 = nibble1 << 8;
            address = nibble1 + byte2;
            controlInstrunctionCount++;


            printDetails(opcode, instrType, r1.ToString("X"), r2.ToString("X"), rdest.ToString("X"), address.ToString("X"));


            programCounter = programCounter + 2; //Updates program counter
        }
        else if(opcode >> 2 == 1)  //Runs if the instruction is a Load/Store type of instruction
        {
            temp = opcode & 3;  //Finds out what instruction it is in the I-type category
            r1 = byte1 & 15;    //Gets first register from the first byte
            address = byte2;    //Finds Address of the Load/Store instructions
            memoryInstrunctionCount++;

            instrType = "Direct";
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
            printDetails(opcode, instrType, r1.ToString("X"), r2.ToString("X"), rdest.ToString("X"), address.ToString("X"));
            programCounter = programCounter + 2;
        }
        else //Runs for arithmetic instructions or R-type instructions
        {
            instrType = "ALU";
            temp = opcode & 15; //Gets the first 3 bits of the opcode... Not used
            r1 = byte1 & 15;    //Gets the first, second, and destination registers for the R-Type instruction
            r2 = byte2 >> 4;
            rdest = byte2 & 15;
            ArithInstrunctionCount++;

            printDetails(opcode, instrType, r1.ToString("X"), r2.ToString("X"), rdest.ToString("X"), address.ToString("X"));

            programCounter = programCounter + 2;

        }

    }


    /// <summary>Prints the details of the given instruction.</summary>
    /// <param name="opcode">The opcode</param>
    /// <param name="instrType">Instruction type</param>
    /// <param name="r1">First register</param>
    /// <param name="r2">Second register</param>
    /// <param name="rdest">Register destination</param>
    /// <param name="address">Address of the instruction</param>
    private static void printDetails(int opcode, string instrType, string r1, string r2, string rdest, string address)
    {
        if (r1 == "FFFFFFFF")  //If any of these variables is a -1 in hex then they are not used in the current intruction and will be printed as N/A
            r1 = "N/A";
        if (r2 == "FFFFFFFF")
            r2 = "N/A";
        if (rdest == "FFFFFFFF")
            rdest = "N/A";
        if (address == "FFFFFFFF")
            address = "N/A";
        //Prints out the details of the given instruction
        Console.WriteLine(string.Format("{0, 7} {1, 4} {2, 8} {3, 9} {4, 4} {5, 4} {6, 4} {7, 8}",
                        programCounter.ToString("X").PadLeft(4, '0'), opcode.ToString("X"), instructions[opcode], instrType,
                        r1, r2, rdest, address));
    }
}