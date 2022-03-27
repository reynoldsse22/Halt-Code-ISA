using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ISA_GUI
{
    internal class StaticPipeline
    {
        public ALU alu;
        public Fetch fetch;
        public ControlUnit CU;
        public ExecutionUnit EU;
        public WriteResult WR;
        public AccessMemory AM;
        public Printer printer;
        public int cycleCount;
        public Instruction stall = new Instruction();
        public int totalHazard, structuralHazard, dataHazard, controlHazard, RAW, WAR, WAW;
        public int totalCyclesStalled, fetchStalled, decodeStalled, executeStalled, accessMemStalled, writeRegStalled;
        public bool lastBranchDecision;
        private Timer timer;

        public StaticPipeline()
        {
            alu = new ALU();
            fetch = new Fetch();
            CU = new ControlUnit();
            EU = new ExecutionUnit();
            AM = new AccessMemory();
            WR = new WriteResult();
            printer = new Printer();
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
            timer = new Timer();
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
                ref StringBuilder pipelineString, ref bool halted, ref ConfigCycle config, ref Instruction[] stages, ref InstructionMemory IM, ref RegisterFile registers,
                ref DataMemory dataMemory)
        {
            if (halted)         //If it is halted, do not run the program and return so that it can be reset.
                return;
            
                cycleCount++;   //increase cycle count

                if (stages[4] != null)        //if there is an instruction in stage 5 AND the instruction is done inside this stage
                {
                    if (stages[4].cycleControl == 0)            //if the instruction is done executing in this stage
                    {
                        stages[4].stage5End = cycleCount - 1;       //set the end cycle
                        printer.buildAssemblyString(ref assemblyString, ref stages[4]);    //Build the associated assembly syntax string for the instruction
                        printer.buildDecodedString(ref decodedString, stages[4]);      //Build the decoded instruction string
                        printer.buildPipelineString(ref pipelineString, ref stages[4]);   //Build the pipeline string
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
                        else if (WR.success)
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
                            if (AM.success)
                                structuralHazard++;
                        }
                    }
                }

            stage3:
                if (stages[2] != null)                      //If there is an instruction in stage 3
                {
                    if (EU.hazardDetected)
                    {
                        executeStalled++;
                        totalCyclesStalled++;
                    }
                    if (EU.success == false)                //If it has not been worked on yet
                    {
                        EU.hazardDetected = stage3DetectHazard(ref stages);
                        if (EU.hazardDetected)
                            goto stage3Bubble;
                        if (!EU.inProgress)
                            EU.execute(ref registers, ref dataMemory, ref alu, ref IM, ref stages[2], ref config, ref lastBranchDecision);   //EXECUTE - Execute the instruction in stage 3
                        stages[2].stage = 3;                 //set stage
                        detectControlHazard(ref stages);

                    }

                    stages[2].cycleControl--;           //If not done processing, decrement a cycle
                    if (stages[2].cycleControl <= 0)        //If done, the instruction is no longer in progress
                        EU.inProgress = false;


                }
            stage3Bubble:
                if (stages[1] != null && !CU.inProgress)     //If stage 2 has an instruction and it has already been processed
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
                        else if (EU.occupied)
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
                if (stages[0] != null && !fetch.inProgress)  //If stage 1 has an instruction and it has already been processed
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
                        else if (CU.occupied)
                        {
                            fetchStalled++;
                            totalCyclesStalled++;
                            if (CU.success)
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
                        return;

                    stages[0].stage1Start = cycleCount;     //set cycle start

                    stages[0].cycleControl--;                //If not done processing, decrement a cycle
                    if (stages[0].cycleControl <= 0)        //If processed
                        fetch.inProgress = false;           //No longer in progress

                    return;
                }
                stages[0].cycleControl--;                //If not done processing, decrement a cycle
                if (stages[0].cycleControl <= 0)        //If processed
                    fetch.inProgress = false;           //No longer in progress

                //end stage 1


              //If not halted and not in step through mode

            return;
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

                case 13:
                    //COmpare Immediate
                    if (stages[4] == null)
                        goto case13NextCheck;
                    if ((stages[1].isFloat ^ stages[4].isFloat) == false)//either both are float or both are integer instructions
                    {
                        if (stages[1].r1 == stages[4].destinationReg && ((stages[4].opcode >= 9 && stages[4].opcode <= 12) || stages[4].opcode >= 14) && stages[4].opcode != 10)
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
            if (stages[2].opcode >= 2 && stages[2].opcode <= 8)
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
                else if (stages[0] != null)
                {
                    if (lastBranchDecision && stages[2].address != (stages[0].programCounterValue))
                    {
                        totalHazard++;
                        controlHazard++;
                        stages[1] = null;
                        stages[0] = null;
                        hazard = true;
                    }
                    else if (!lastBranchDecision && stages[2].address == (stages[0].programCounterValue))
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
