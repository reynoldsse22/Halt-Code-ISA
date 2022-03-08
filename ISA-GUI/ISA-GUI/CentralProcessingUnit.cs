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
            fetch = new Fetch();
            CU = new ControlUnit();
            EU = new ExecutionUnit();
            IM = new InstructionMemory();
            AM = new AccessMemory();
            WR = new WriteResult();
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

        /**
	    * Method Name: runCycle <br>
	    * Method Purpose: Runs the program through the pipeline by one cycle if in stepthrough-mode and all the way through in run mode.
	    * 
	    * <hr>
	    * Date created: 2/19/21 <br>
	    * @author Samuel Reynolds
	    */
        public void runCycle(List<string> input, bool stepThrough, ref StringBuilder assemblyString, ref StringBuilder decodedString,
                ref StringBuilder pipelineString, ref bool halted, ref ConfigCycle config, ref Instruction[] stages)
        {
            byte[] instruct = new byte[3];
            if (IM.instructions.Count == 0)    //if the program hasn't been started
            {
                IM.setInstructionSize(input.Count);     //Set the instruction size based on the amount of instructions given
                storeProgramInMemory(input);            //Store the program in main memory and the instruction memory unit
            }
            if (halted)         //If it is halted, do not run the program and return so that it can be reset.
                return;
            do                  //run once, and if the run button was pressed, keep looping until halted
            {
                cycleCount++;   //increase cycle count
                 
                if (stages[4] != null)        //if there is an instruction in stage 5 AND the instruction is done inside this stage
                {
                    if (stages[4].cycleControl == 0)            //if the instruction is done executing in this stage
                    {
                        stages[4].stage5End = cycleCount - 1;       //set the end cycle
                        buildAssemblyString(ref assemblyString, ref stages[4]);    //Build the associated assembly syntax string for the instruction
                        buildDecodedString(ref decodedString, stages[4]);      //Build the decoded instruction string
                        buildPipelineString(ref pipelineString, ref stages[4]);   //Build the pipeline string
                        WR.success = false;             //reset the stage
                        if (stages[4].opcode == 0)      //if halt is found
                            halted = true;

                        stages[4] = null;               //pop off the instruction
                        WR.occupied = false;            //There is no longer an instruction in stage 5
                    }
                }

            stage5:
                if (stages[4] != null)                  //If there is an instruction in stage 5
                {
                    if (WR.success == false)            //If it has not been worked on yet
                    {
                        WR.writeToReg(registers, ref stages[4], ref config);        //WRITE TO REGISTER FILE - execute that instruction in stage 5
                        stages[4].stage = 5;                                        //set stage
                    }

                    stages[4].cycleControl--;      //If not done processing, decrement a cycle

                    if (stages[4].cycleControl <= 0)    //If done, the instruction is no longer in progress
                        WR.inProgress = false;

                }

                if (stages[3] != null)    //If stage 4 has an instruction and it has already been processed
                {
                    if (stages[3].cycleControl <= 0)        //Check and make sure it is finished it's required cycles
                    {
                        if (!WR.occupied)                   //If stage 5 is not occupied, send the instruction in stage 4 to stage 5
                        {
                            AM.success = false;             //Reset success status
                            stages[3].stage4End = cycleCount - 1;   //set ending cycle
                            stages[4] = stages[3];                  //Send to stage 5
                            stages[3] = null;                       //Clear stage 4
                            AM.occupied = false;                    //Stage 4 no longer occupied
                            WR.occupied = true;                     //Stage 5 is now occupied
                            stages[4].stage5Start = cycleCount;     //Set starting cycle
                            goto stage5;                            //Repeat stage 5 to execute that new instruction
                        }
                        else if(WR.success)
                        {
                            accessMemStalled++;
                            totalCyclesStalled++;
                            structuralHazard++;
                        }

                    }
                }

            stage4:
                if (stages[3] != null)                      //If there is an instruction in stage 4
                {
                    if (AM.success == false)                //If it has not been worked on yet
                    {
                        AM.accessMemory(ref dataMemory, ref registers, ref stages[3], ref config);      //ACCESS MEMORY - execute that instruction in stage 4
                        stages[3].stage = 4;                //set stage
                    }

                    stages[3].cycleControl--;           //If not done processing, decrement a cycle

                    if (stages[3].cycleControl <= 0)        //If done, the instruction is no longer in progress
                        AM.inProgress = false;
                }

                if (stages[2] != null)    //If stage 3 has an instruction and it has already been processed
                {
                    if (stages[2].cycleControl <= 0)        //Check and make sure it is finished it's required cycles
                    {
                        if (!AM.occupied && !EU.hazardDetected)                   //If stage 4 is not occupied, send the instruction in stage 3 to stage 4
                        {
                            EU.success = false;                //Reset success status
                            stages[2].stage3End = cycleCount - 1;   //set ending cycle
                            stages[3] = stages[2];                  //Send to stage 4
                            stages[2] = null;                       //Clear state 3
                            EU.occupied = false;                    //Stage 3 no longer occupied
                            AM.occupied = true;                     //Stage 4 is now occupied
                            stages[3].stage4Start = cycleCount;     //Set starting cycle
                            goto stage4;                            //Repeat stage 4 to execute that new instruction
                        }
                        else if (AM.occupied)
                        {
                            executeStalled++;
                            totalCyclesStalled++;
                            if(AM.success)
                                structuralHazard++;
                        }
                    }
                }

            stage3:
                if (stages[2] != null)                      //If there is an instruction in stage 3
                {
                    if(EU.hazardDetected)
                    {
                        executeStalled++;
                        totalCyclesStalled++;
                    }
                    if (EU.success == false)                //If it has not been worked on yet
                    {
                        EU.hazardDetected = stage3DetectHazard(ref stages);
                        if (EU.hazardDetected)
                            goto stage3Bubble;
                        if(!EU.inProgress)
                            EU.execute(ref registers, ref dataMemory, ref alu, ref IM, ref stages[2], ref config, ref lastBranchDecision);   //EXECUTE - Execute the instruction in stage 3
                        stages[2].stage = 3;                 //set stage
                        detectControlHazard(ref stages);
                        
                    }
                    
                    stages[2].cycleControl--;           //If not done processing, decrement a cycle
                    if (stages[2].cycleControl <= 0)        //If done, the instruction is no longer in progress
                        EU.inProgress = false;
                        

                }
            stage3Bubble:
                if(stages[1] != null && !CU.inProgress)     //If stage 2 has an instruction and it has already been processed
                {
                    if (stages[1].cycleControl <= 0)        //Check and make sure it is finished it's required cycles
                    {
                        if (!EU.occupied && !CU.hazardDetected)                   //If stage 3 is not occupied, send the instruction in stage 2 to stage 3
                        {
                            CU.success = false;              //Reset success status
                            stages[1].stage2End = cycleCount - 1;   //set ending cycle
                            stages[2] = stages[1];                  //Send to stage 3
                            stages[1] = null;                       //Clear stage 3
                            CU.occupied = false;                    //Stage 2 no longer occupied
                            EU.occupied = true;                     //Stage 3 is now occupied
                            stages[2].stage3Start = cycleCount;     //Set starting cycle
                            goto stage3;                            //Repeat stage 3 to execute that new instruction
                        }
                        else if(EU.occupied)
                        {
                            decodeStalled++;
                            totalCyclesStalled++;
                            if (EU.success)
                                structuralHazard++;
                        }
                    }
                }
                //end stage 3
                

                stage2:
                if (stages[1] != null && cycleCount > 1)            //If there is an instruction in stage 2
                {
                    if (CU.success == false)                        //If it has not been worked on yet
                    {
                        CU.hazardDetected = stage2DetectHazard(ref stages);
                        if (CU.hazardDetected)
                            goto stage2Bubble;

                        CU.decode(ref IM, ref stages[1], ref config);      //DECODE - Decode the instruction in stage 2
                        stages[1].stage = 2;                //set stage
                    }

                    stages[1].cycleControl--;                //If not done processing, decrement a cycle
                    if (stages[1].cycleControl <= 0)         //If done processing, no longer in progress
                        CU.inProgress = false;
                }

                stage2Bubble:
                if(stages[0] != null && !fetch.inProgress)  //If stage 1 has an instruction and it has already been processed
                {
                    if (stages[0].cycleControl <= 0)        //Check and make sure it is finished it's required cycles
                    {
                        if (!CU.occupied)                   //If stage 2 is not occupied, send the instruction in stage 1 to stage 2
                        {
                            stages[0].stage1End = cycleCount - 1;   //set ending cycle
                            fetch.success = false;                  //reset stage 1
                            stages[1] = stages[0];                  //Send to stage 2
                            stages[0] = null;                       //clear stage 1
                            fetch.occupied = false;                 //Stage 1 no longer occupied
                            CU.occupied = true;                     //Stage 2 is now occupied
                            stages[1].stage2Start = cycleCount;     //Set starting cycle
                            goto stage2;
                        }
                        else if(CU.occupied)
                        {
                            fetchStalled++;
                            totalCyclesStalled++;
                            if(CU.success)
                                structuralHazard++;
                        }
                    }
                }
                //end stage 2

                stage1:
                if (stages[0] == null)              //if no instruction present
                {
                    stages[0] = fetch.getNextInstruction(ref registers, ref IM, ref config, ref stages, lastBranchDecision, config.predictionSet);        //FETCH - get the next instruction and place in stage 1
                    if (stages[0] == null)
                        continue;

                    stages[0].stage1Start = cycleCount;     //set cycle start
                   
                    stages[0].cycleControl--;                //If not done processing, decrement a cycle
                    if (stages[0].cycleControl <= 0)        //If processed
                        fetch.inProgress = false;           //No longer in progress

                    continue;
                }
                stages[0].cycleControl--;                //If not done processing, decrement a cycle
                if (stages[0].cycleControl <= 0)        //If processed
                    fetch.inProgress = false;           //No longer in progress

                //end stage 1


            } while (!halted && !stepThrough);  //If not halted and not in step through mode

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
            if (instrFlag == 2 || instrFlag == 3)
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
            string updatedAssembly = instruction.ToUpper();

            if (second != "")
                first += ",";
            if (third != "")
                second += ",";

            if(instruct.opcode >= 9)
            {
                if (instruct.instrFlag == 2)
                    updatedAssembly = "f." + updatedAssembly;
                else if (instruct.instrFlag == 1)
                    updatedAssembly += 's';
                else if (instruct.instrFlag == 3)
                    updatedAssembly = "f." + updatedAssembly + 's';
            }
            instruct.assembly1 = updatedAssembly;
            assemblyString.Append(updatedAssembly + "\t" + first + second + third + "\n");
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

            string output = (string.Format("\n{0, 7} {1,13} {2, 7} {3, 8} {4, 8} {5, 7} {6, 9}",
                            instruction.assembly1.PadRight(7),instruction.assembly2.PadRight(13), stage1.PadLeft(7), stage2.PadLeft(8), stage3.PadLeft(8), stage4.PadLeft(7), stage5.PadLeft(9)));

            pipelineString.Append(output);

        }

        /**
        * Method Name: stage2DetectHazard <br>
        * Method Purpose: Detects data hazards in the pipeline for stage 2
        * 
        * <br>
        * Date created: 3/6/22 <br>
        * <hr>
        *   @param  Instruction[] stages
        */
        public bool stage2DetectHazard(ref Instruction[] stages)
        {
            bool isThereHazard = false;
            switch (stages[1].opcode)
            {
                case 9:
                    //Load from memory
                    if (stages[3] == null)
                        goto endMethod;
                    if ((stages[1].isFloat ^ stages[3].isFloat) == false)//either both are float or both are integer instructions
                    {
                        if (stages[3].r3 == 0 || stages[3].destinationReg == 0)
                        {
                            goto setRAW;
                        }
                    }
                    break;
                case 10:
                    //Store into memory
                    if(stages[3] == null)
                        goto endMethod;
                    if ((stages[1].isFloat ^ stages[3].isFloat) == false)//either both are float or both are integer instructions
                    {
                        if (stages[3].r3 == 0 || stages[3].destinationReg == 0)
                        {
                            goto setRAW;
                        }
                    }
                    break;

                case 13:
                    //COmpare Immediate
                    if(stages[4] == null)
                        goto case13NextCheck;
                    if ((stages[1].isFloat ^ stages[4].isFloat) == false)//either both are float or both are integer instructions
                    {
                        if(stages[1].r1 == stages[4].destinationReg && ((stages[4].opcode >= 9 && stages[4].opcode <= 12) || stages[4].opcode >= 14) && stages[4].opcode != 10)
                        {
                            goto setRAW;
                        }
                    }
                    case13NextCheck:
                    if (stages[3] == null)
                        goto endMethod;
                    if ((stages[1].isFloat ^ stages[3].isFloat) == false)//either both are float or both are integer instructions
                    {
                        if (stages[1].r1 == stages[3].destinationReg && ((stages[3].opcode >= 9 && stages[3].opcode <= 12) || stages[3].opcode >= 14) && stages[3].opcode != 10)
                        {
                            goto setRAW;
                        }
                    }
                    break;
                case 14:
                    //Compare registers
                    if (stages[4] == null)
                        goto case14NextCheck;
                    if ((stages[1].isFloat ^ stages[4].isFloat) == false)//either both are float or both are integer instructions
                    {
                        if (stages[1].r1 == stages[4].destinationReg && ((stages[4].opcode >= 9 && stages[4].opcode <= 12) || stages[4].opcode >= 14) && stages[4].opcode != 10)
                        {
                            goto setRAW;
                        }
                        if (stages[1].r2 == stages[4].destinationReg && ((stages[4].opcode >= 9 && stages[4].opcode <= 12) || stages[4].opcode >= 14) && stages[4].opcode != 10)
                        {
                            goto setRAW;
                        }
                    }
                    case14NextCheck:
                    if (stages[3] == null)
                        goto endMethod;
                    if ((stages[1].isFloat ^ stages[3].isFloat) == false)//either both are float or both are integer instructions
                    {
                        if (stages[1].r1 == stages[3].destinationReg && ((stages[3].opcode >= 9 && stages[3].opcode <= 12) || stages[3].opcode >= 14) && stages[3].opcode != 10)
                        {
                            goto setRAW;
                        }
                        if (stages[1].r2 == stages[3].destinationReg && ((stages[3].opcode >= 9 && stages[3].opcode <= 12) || stages[3].opcode >= 14) && stages[3].opcode != 10)
                        {
                            goto setRAW;
                        }
                    }
                    break;
                case 15:
                    //MOV reg to reg
                    if (stages[4] == null)
                        goto case15NextCheck;
                    if ((stages[1].isFloat ^ stages[4].isFloat) == false)//either both are float or both are integer instructions
                    {
                        if (stages[1].r3 == stages[4].destinationReg && ((stages[4].opcode >= 9 && stages[4].opcode <= 12) || stages[4].opcode >= 14) && stages[4].opcode != 10)
                        {
                            goto setRAW;
                        }
                    }
                    case15NextCheck:
                    if (stages[3] == null)
                        goto endMethod;
                    if ((stages[1].isFloat ^ stages[3].isFloat) == false) //either both are float or both are integer instructions
                    {
                        if (stages[1].r3 == stages[3].destinationReg && ((stages[3].opcode >= 9 && stages[3].opcode <= 12) || stages[3].opcode >= 14) && stages[3].opcode != 10)
                        {
                            goto setRAW;
                        }
                    }
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
                    //ALU instructions
                    if (stages[4] == null)
                        goto case26NextCheck;
                    if ((stages[1].isFloat ^ stages[4].isFloat) == false) //either both are float or both are integer instructions
                    {
                        if (stages[1].r1 == stages[4].destinationReg && ((stages[4].opcode >= 9 && stages[4].opcode <= 12) || stages[4].opcode >= 14) && stages[4].opcode != 10)
                        {
                            goto setRAW;
                        }
                        if (stages[1].r2 == stages[4].destinationReg && ((stages[4].opcode >= 9 && stages[4].opcode <= 12) || stages[4].opcode >= 14) && stages[4].opcode != 10)
                        {
                            goto setRAW;
                        }
                    }
                    case26NextCheck:
                    if (stages[3] == null)
                        goto endMethod;
                    if ((stages[1].isFloat ^ stages[3].isFloat) == false)//either both are float or both are integer instructions
                    {
                        if (stages[1].r1 == stages[3].destinationReg && ((stages[3].opcode >= 9 && stages[3].opcode <= 12) || stages[3].opcode >= 14) && stages[3].opcode != 10)
                        {
                            goto setRAW;
                        }
                        if (stages[1].r2 == stages[3].destinationReg && ((stages[3].opcode >= 9 && stages[3].opcode <= 12) || stages[3].opcode >= 14) && stages[3].opcode != 10)
                        {
                            goto setRAW;
                        }
                    }
                    break;
                case 27:
                    //NOT instruction
                    if (stages[4] == null)
                        goto case27NextCheck;
                    if (stages[1].r2 == stages[4].destinationReg && ((stages[4].opcode >= 9 && stages[4].opcode <= 12) || stages[4].opcode >= 14) && stages[4].opcode != 10)
                    {
                        goto setRAW;
                    }
                    if (stages[1].r3 == stages[4].destinationReg && ((stages[4].opcode >= 9 && stages[4].opcode <= 12) || stages[4].opcode >= 14) && stages[4].opcode != 10)
                    {
                        goto setRAW;
                    }
                    case27NextCheck:
                    if (stages[3] == null)
                        goto endMethod;
                    if (stages[1].r2 == stages[3].destinationReg && ((stages[3].opcode >= 9 && stages[3].opcode <= 12) || stages[3].opcode >= 14) && stages[3].opcode != 10)
                    {
                        goto setRAW;
                    }
                    if (stages[1].r3 == stages[3].destinationReg && ((stages[3].opcode >= 9 && stages[3].opcode <= 12) || stages[3].opcode >= 14) && stages[3].opcode != 10)
                    {
                        goto setRAW;
                    }
                    break;

            }
            goto endMethod;

            setRAW:
                isThereHazard = true;
                totalHazard++;
                dataHazard++;
                RAW++;

            endMethod:
                return isThereHazard;
        }


        /**
       * Method Name: stage3DetectHazard <br>
       * Method Purpose: Detects data hazards in the pipeline for stage 3
       * 
       * <br>
       * Date created: 3/6/22 <br>
       * <hr>
       *   @param  Instruction[] stages
       */
        public bool stage3DetectHazard(ref Instruction[] stages)
        {
            bool isThereHazard = false;
            switch (stages[2].opcode)
            {
                case 13:
                    if (stages[4] == null)
                        goto case13CheckNext;
                    //Compare Immediate
                    if ((stages[2].isFloat ^ stages[4].isFloat) == false)//either both are float or both are integer instructions
                    {
                        if (stages[2].r1 == stages[4].destinationReg && ((stages[4].opcode >= 9 && stages[4].opcode <= 12) || stages[4].opcode >= 14) && stages[4].opcode != 10)
                        {
                            goto setRAW;
                        }
                    }

                    case13CheckNext:
                    if (stages[3] == null)
                        goto endMethod;
                    if ((stages[2].isFloat ^ stages[3].isFloat) == false)//either both are float or both are integer instructions
                    {
                        if (stages[2].r1 == stages[3].destinationReg && ((stages[3].opcode >= 9 && stages[3].opcode <= 12) || stages[3].opcode >= 14) && stages[3].opcode != 10)
                        {
                            goto setRAW;
                        }
                    }
                    break;
                case 14:
                case 15:
                    if (stages[4] == null)
                        goto case15NextCheck;
                    if ((stages[2].isFloat ^ stages[4].isFloat) == false)//either both are float or both are integer instructions
                    {
                        if (stages[2].r2 == stages[4].destinationReg && ((stages[4].opcode >= 9 && stages[4].opcode <= 12) || stages[4].opcode >= 14) && stages[4].opcode != 10)
                        {
                            goto setRAW;
                        }
                        if (stages[2].r3 == stages[4].destinationReg && ((stages[4].opcode >= 9 && stages[4].opcode <= 12) || stages[4].opcode >= 14) && stages[4].opcode != 10)
                        {
                            goto setRAW;
                        }
                    }
                    case15NextCheck:
                    if (stages[3] == null)
                        goto endMethod;
                    if ((stages[2].isFloat ^ stages[3].isFloat) == false)//either both are float or both are integer instructions
                    {
                        if (stages[2].r2 == stages[3].destinationReg && ((stages[3].opcode >= 9 && stages[3].opcode <= 12) || stages[3].opcode >= 14) && stages[3].opcode != 10)
                        {
                            goto setRAW;
                        }
                        if (stages[2].r3 == stages[3].destinationReg && ((stages[3].opcode >= 9 && stages[3].opcode <= 12) || stages[3].opcode >= 14) && stages[3].opcode != 10)
                        {
                            goto setRAW;
                        }
                    }
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
                    if (stages[4] == null)
                        goto case26NextCheck;
                    if ((stages[2].isFloat ^ stages[4].isFloat) == false)//either both are float or both are integer instructions
                    {
                        if (stages[2].r1 == stages[4].destinationReg && ((stages[4].opcode >= 9 && stages[4].opcode <= 12) || stages[4].opcode >= 14))
                        {
                            goto setRAW;
                        }
                        if (stages[2].r2 == stages[4].destinationReg && ((stages[4].opcode >= 9 && stages[4].opcode <= 12) || stages[4].opcode >= 14))
                        {
                            goto setRAW;
                        }
                    }
                    case26NextCheck:
                    if (stages[3] == null)
                        goto endMethod;
                    if ((stages[2].isFloat ^ stages[3].isFloat) == false)//either both are float or both are integer instructions
                    {
                        if (stages[2].r1 == stages[3].destinationReg && ((stages[3].opcode >= 9 && stages[3].opcode <= 12) || stages[3].opcode >= 14))
                        {
                            goto setRAW;
                        }
                        if (stages[2].r2 == stages[3].destinationReg && ((stages[3].opcode >= 9 && stages[3].opcode <= 12) || stages[3].opcode >= 14))
                        {
                            goto setRAW;
                        }
                    }
                    break;
                case 27:
                    if (stages[4] == null)
                        goto case27NextCheck;
                    if (stages[2].r1 == stages[4].destinationReg && ((stages[4].opcode >= 9 && stages[4].opcode <= 12) || stages[4].opcode >= 14))
                    {
                        goto setRAW;
                    }

                    case27NextCheck:
                    if (stages[3] == null)
                        goto endMethod;
                    if (stages[2].r1 == stages[3].destinationReg && ((stages[3].opcode >= 9 && stages[3].opcode <= 12) || stages[3].opcode >= 14))
                    {
                        goto setRAW;
                    }
                    break;

            }

            goto endMethod;

            setRAW:
                isThereHazard = true;
                totalHazard++;
                dataHazard++;
                RAW++;

            endMethod:
                return isThereHazard;
        }

        /**
       * Method Name: detectControlHazard <br>
       * Method Purpose: Detects control hazards and if the pipeline needs to be flushed
       * 
       * <br>
       * Date created: 3/7/22 <br>
       * <hr>
       *   @param  Instruction[] stages
       */
        public bool detectControlHazard(ref Instruction[] stages)
        {
            bool hazard = false;
            if(stages[2].opcode >= 2 && stages[2].opcode <= 8)
            {
                if (stages[1] != null)
                {
                    if (lastBranchDecision && stages[2].address != (stages[1].programCounterValue))
                    {
                        totalHazard++;
                        controlHazard++;
                        stages[1] = null;
                        stages[0] = null;
                        hazard = true;
                    }
                    else if (!lastBranchDecision && stages[2].address != (stages[1].programCounterValue))
                    {
                        totalHazard++;
                        controlHazard++;
                        stages[1] = null;
                        stages[0] = null;
                        hazard = true;
                    }
                }
                else if(stages[0] != null)
                {
                    if (lastBranchDecision && stages[2].address != (stages[0].programCounterValue))
                    {
                        totalHazard++;
                        controlHazard++;
                        stages[1] = null;
                        stages[0] = null;
                        hazard = true;
                    }
                    else if(!lastBranchDecision && stages[2].address == (stages[0].programCounterValue))
                    {
                        totalHazard++;
                        controlHazard++;
                        stages[1] = null;
                        stages[0] = null;
                        hazard = true;
                    }
                }
            }
            return hazard;
        }
    }
}
