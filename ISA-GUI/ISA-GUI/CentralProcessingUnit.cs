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
        public InstructionMemory IM;
        public StaticPipeline SP;
        public DynamicPipeline DP;
        public int cycleCount;
        public Instruction stall = new Instruction();
        public int totalHazard, structuralHazard, dataHazard, controlHazard, RAW, WAR, WAW;
        public int totalCyclesStalled, fetchStalled, decodeStalled, executeStalled, accessMemStalled, writeRegStalled;
        public bool lastBranchDecision;
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
            IM = new InstructionMemory();
            SP = new StaticPipeline();
            DP = new DynamicPipeline();
            cycleCount = 0;
            totalCyclesStalled = 0;
            fetchStalled = 0;
            decodeStalled = 0;
            executeStalled = 0;
            accessMemStalled = 0;
            writeRegStalled = 0;
            totalHazard = 0;
            structuralHazard = 0;
            dataHazard = 0;
            controlHazard = 0;
            WAR = 0;
            RAW = 0;
            WAW = 0;
            lastBranchDecision = false;
        }


        public void runStaticPipeline(List<string> input, bool stepThrough, ref StringBuilder assemblyString, ref StringBuilder decodedString,
                ref StringBuilder pipelineString, ref bool halted, ref ConfigCycle config, ref Instruction[] stages)
        {
            if (IM.instructions.Count == 0)    //if the program hasn't been started
            {
                IM.setInstructionSize(input.Count);     //Set the instruction size based on the amount of instructions given
                storeProgramInMemory(input);            //Store the program in main memory and the instruction memory unit
            }
            SP.runCycle(input, stepThrough, ref assemblyString, ref decodedString, ref pipelineString, ref halted, ref config, ref stages, ref IM, ref registers, ref dataMemory);
        }

        public void runDynamicPipeline()
        {

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

    }
}
