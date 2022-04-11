using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISA_GUI
{
    internal class DynamicPipeline
    {
        public Fetch fetch;
        public ControlUnit CU;
        public Queue<Instruction> instructionQueue;
        public List<Instruction> instructionsInFlight;
        public Instruction justCommitedInstruction;
        public ALU alu;
        public ExecutionUnit EU;
        public WriteResult WR;
        public AccessMemory AM;
        public Printer printer;
        public int cycleCount;
        public Instruction stall = new Instruction();
        public CommonDataBus commonDataBus;
        public ReorderBuffer reorderBuffer;
        public IntAddFU intAddFu;
        public IntSubFU intSubFu;
        public IntMultFU intMulFu;
        public IntDivFU intDivFu;
        public FloatAddFU flAddFu;
        public FloatSubFU flSubFu;
        public FloatMultFU flMultFu;
        public FloatDivFU flDivFu;
        public BitwiseOPFU bitFu;
        public MemoryUnit memoryFu;
        public BranchFU branchFu;
        public ShiftFU shiftFu;
        public ReservationStation intAddRS;
        public ReservationStation intSubRS;
        public ReservationStation intMultRS;
        public ReservationStation intDivRS;
        public ReservationStation floatAddRS;
        public ReservationStation floatSubRS;
        public ReservationStation floatMultRS;
        public ReservationStation floatDivRS;
        public ReservationStation bitwiseOPRS;
        public ReservationStation branchOPS;
        public ReservationStation shiftOPS;
        public ReservationStation load_storeBuffer;
        public Instruction fetchInstruction;

        public int totalHazard, structuralHazard, dataHazard, controlHazard, RAW, WAR, WAW;
        public int reorderBufferDelay, reservationStationDelay, trueDependenceDelay, totalDelays, instructionID;
        public int totalCyclesStalled, numOfInstructionsInExecution, twoBitPredictionCounter;
        public bool lastBranchDecision, executiveBranchDecision, doneExecuting, executionInProgress, haltFound, commitedThisCycle, branchDecoded, instructionFetched;



        /// <summary>Initializes a new instance of the <see cref="DynamicPipeline" /> class.</summary>
        public DynamicPipeline()
        {
            fetch = new Fetch();
            CU = new ControlUnit();
            printer = new Printer();
            instructionQueue = new Queue<Instruction>();
            instructionsInFlight = new List<Instruction>();
            alu = new ALU();
            CU = new ControlUnit();
            EU = new ExecutionUnit();
            AM = new AccessMemory();
            WR = new WriteResult();
            commonDataBus = new CommonDataBus();
            reorderBuffer = new ReorderBuffer();
            intAddFu = new IntAddFU();
            intSubFu = new IntSubFU();
            intMulFu = new IntMultFU();
            intDivFu = new IntDivFU();
            flAddFu = new FloatAddFU();
            flSubFu = new FloatSubFU();
            flMultFu = new FloatMultFU();
            flDivFu = new FloatDivFU();
            bitFu = new BitwiseOPFU();
            memoryFu = new MemoryUnit();
            branchFu = new BranchFU();
            shiftFu = new ShiftFU();
            intAddRS = new ReservationStation("intAddRS");
            intSubRS = new ReservationStation("intSubRS");
            intMultRS = new ReservationStation("intMultRS");
            intDivRS = new ReservationStation("intDivRS");
            floatAddRS = new ReservationStation("floatAddRS");
            floatSubRS = new ReservationStation("floatSubRS");
            floatMultRS = new ReservationStation("floatMultRS");
            floatDivRS = new ReservationStation("floatDivRS");
            bitwiseOPRS = new ReservationStation("bitwiseOPRS");
            branchOPS = new ReservationStation("branchOPS");        //Reservation station solely for branches\
            shiftOPS = new ReservationStation("shiftOPS");
            load_storeBuffer = new ReservationStation("load_storeBuffer");


            fetchInstruction = new Instruction();
            cycleCount = 0;
            totalCyclesStalled = 0;
            totalHazard = 0;
            structuralHazard = 0;
            dataHazard = 0;
            controlHazard = 0;
            WAR = 0;
            RAW = 0;
            WAW = 0;
            instructionID = 1;
            numOfInstructionsInExecution = 0;
            twoBitPredictionCounter = 0;
            lastBranchDecision = false;
            doneExecuting = false;
            executiveBranchDecision = false;
            executionInProgress = false;
            haltFound = false;
            commitedThisCycle = false;
            branchDecoded = false;
        }

        /// <summary>Runs the cycle.</summary>
        /// <param name="input">The input.</param>
        /// <param name="stepThrough">if set to <c>true</c> [step through].</param>
        /// <param name="assemblyString">The assembly string.</param>
        /// <param name="decodedString">The decoded string.</param>
        /// <param name="pipelingString">The pipeling string.</param>
        /// <param name="halted">if set to <c>true</c> [halted].</param>	

        /// <param name="config">The configuration.</param>
        /// <param name="IM">The im.</param>
        /// <param name="registers">The registers.</param>
        /// <param name="dataMemory">The data memory.</param>
        public void runCycle(List<string> input, bool stepThrough, ref StringBuilder assemblyString, ref StringBuilder decodedString, ref StringBuilder pipelineString, 
            ref bool halted, ref ConfigCycle config, ref InstructionMemory IM, ref RegisterFile registers, ref DataMemory dataMemory)
        {
            do
            {
                intAddFu.oneCycleFU = true;
                intSubFu.oneCycleFU = true;
                intMulFu.oneCycleFU = true;
                intDivFu.oneCycleFU = true;
                flAddFu.oneCycleFU = true;
                flSubFu.oneCycleFU = true;
                flMultFu.oneCycleFU = true;
                flDivFu.oneCycleFU = true;
                bitFu.oneCycleFU = true;
                memoryFu.oneCycleFU = true;
                branchFu.oneCycleFU = true;
                shiftFu.oneCycleFU = true;

                commitedThisCycle = false;
                startOfLoop:
                cycleCount++;
                reorderBuffer.oneCommitPerCycle = true; //Control booleans to make sure stages only run once per cycle

                if (halted)
                {
                    clearDynamicPipeline();
                    return;
                }

                string result = "";
                if (instructionQueue.Count == 0)
                {
                    fillInstructionQueue(ref IM);
                }

                if(instructionsInFlight.Count == 0)
                {
                    fetchFromInstructionQueue();
                }
                

                //Put the instructions into the reservation station. This should be the first cycle of the pipeline
                foreach (Instruction inst in instructionsInFlight.ToList()) //Will run through instruction queue, instruction will bne take off the queue once commited
                {
                    switch (inst.stage) //Instuction will hold which cycle it's in
                    {
                        //Take instructions from the data bus and add them to the reorder buffer where instructions will be executed based on program counter
                        case 5:
                            if (commitedThisCycle)
                                continue;

                            int instructionIndex = reorderBuffer.checkCommit(inst, ref WR, ref dataMemory, ref lastBranchDecision, ref IM, ref registers, ref haltFound, ref commonDataBus);
                            if (instructionIndex < 0)
                            {
                                justCommitedInstruction = null;
                                continue;
                            }
                            
                            if (haltFound)
                            {
                                totalDelays = trueDependenceDelay + reservationStationDelay + reorderBufferDelay;
                                halted = true;
                            }
                            commitedThisCycle = true;
                            inst.stage5Cycle = cycleCount;
                            printer.buildDecodedString(ref decodedString, inst);      //Build the decoded instruction string
                            printer.buildDynamicPipelineString(ref pipelineString, inst);
                            justCommitedInstruction = inst;
                            bool hazardDetected = detectControlHazard(instructionIndex, ref registers, inst, ref IM, ref config);
                            if (!hazardDetected)
                            {
                                try
                                {
                                    int cdbIndex = commonDataBus.index[instructionIndex];
                                    commonDataBus.CDB.Remove(commonDataBus.CDB.ElementAt(cdbIndex).Key);
                                    commonDataBus.index.Remove(inst.ID);
                                    commonDataBus.IDIndex.Remove(cdbIndex);
                                    if (cdbIndex < (commonDataBus.CDB.Count))
                                    {
                                        Dictionary<int, int> newDict = new Dictionary<int, int>();
                                        Dictionary<int, int> newIndexDict = new Dictionary<int, int>();
                                        for (int i = 0; i < commonDataBus.index.Count; i++)
                                        {
                                            if (commonDataBus.index.ElementAt(i).Value > cdbIndex)
                                                newDict.Add(commonDataBus.index.ElementAt(i).Key, (commonDataBus.index.ElementAt(i).Value - 1));
                                            else
                                                newDict.Add(commonDataBus.index.ElementAt(i).Key, commonDataBus.index.ElementAt(i).Value);

                                            if (commonDataBus.IDIndex.ElementAt(i).Key > cdbIndex)
                                                newIndexDict.Add((commonDataBus.IDIndex.ElementAt(i).Key - 1), commonDataBus.IDIndex.ElementAt(i).Value);
                                            else
                                                newIndexDict.Add(commonDataBus.IDIndex.ElementAt(i).Key, commonDataBus.IDIndex.ElementAt(i).Value);
                                        }
                                        commonDataBus.index = newDict;
                                        commonDataBus.IDIndex = newIndexDict;
                                    }
                                    instructionsInFlight.Remove(inst);
                                    registers.clearSpecificRegisterQI(inst);
                                }
                                catch
                                {
                                    justCommitedInstruction = null;
                                    continue;
                                }
                            }
                            else
                            {
                                justCommitedInstruction = null;
                                goto startOfLoop;
                            }
                            break;
                        //Store the answer and corresponding reservation name into the data bus
                        case 4:
                            bool success = WR.writeToCDB(inst, ref commonDataBus, in inst.result);
                            if (success)
                            {
                                clearFU(inst);
                                inst.stage = 5;
                                if (inst.stage4Cycle == 0)
                                {
                                    inst.stage4Cycle = cycleCount;
                                }
                            }
                            break;

                        //This should be where the Write Result and Memory Read stages will be held
                        case 3:
                            numOfInstructionsInExecution++;
                            if (inst.stage3CycleStart == 0)
                                inst.stage3CycleStart = cycleCount;
                            memoryFu.instruction.doneExecuting = false;
                            AM.accessMemoryDynamic(ref dataMemory, ref registers, inst, ref config, out result, ref memoryFu);
                            inst.result = result;
                            inst.ASPR = memoryFu.instruction.ASPR;
                           // inst.stage2End = cycleCount - 1;        //End of the second stage
                           // inst.stage3Start = cycleCount;          //Start of the third
                            
                            if(memoryFu.instruction.doneExecuting)
                            {
                                numOfInstructionsInExecution--;
                                inst.stage3CycleEnd = cycleCount;
                                inst.stage = 4;
                            }
                            break;

                        //Send the instruction to its corresponding functional unit as long as there are no structural harzards present
                        //Open up reservation station to allow for more instructions to flow in
                        //Execute within the functional unit
                        case 2:
                            numOfInstructionsInExecution++;
                            Instruction executeInstruction = execute(inst, ref registers, ref dataMemory, ref IM, ref config, ref alu, ref result);
                            inst.result = result;
                            inst.dependantOpID1 = 0;
                            inst.dependantOpID2 = 0;
                            inst.doneExecuting = executeInstruction.doneExecuting;
                            inst.executionInProgress = executeInstruction.executionInProgress;
                            inst.justIssued = false;
                            inst.ASPR = executeInstruction.ASPR;
                            
                            if (inst.doneExecuting)
                            {
                                numOfInstructionsInExecution -= 1;
                                inst.stage2CycleEnd = cycleCount;
                                inst.stage2CycleStart = executeInstruction.stage2CycleStart;
                                if (inst.opcode == 0 || inst.opcode == 1)
                                    inst.stage = 5;
                                else if (inst.opcode == 9 || inst.opcode == 11 || inst.opcode == 12)
                                {
                                    inst.doneExecuting = true;
                                    inst.stage = 3;
                                }
                                else
                                    inst.stage = 4;
                            }
                            break;
                        //Populates reservationStations
                        case 1:
                            Instruction fetchInstruction = new Instruction();
                            fetchInstruction = decode(inst, ref IM, ref config);
                            inst.opcode = fetchInstruction.opcode; 
                            inst.r1 = fetchInstruction.r1;
                            inst.r2 = fetchInstruction.r2;
                            inst.r3 = fetchInstruction.r3;
                            inst.programCounterValue = fetchInstruction.programCounterValue;
                            inst.isFloat = fetchInstruction.isFloat;
                            inst.address = fetchInstruction.address;
                            inst.ASPR = fetchInstruction.ASPR;
                            inst.destinationReg = fetchInstruction.destinationReg;
                            inst.binInstruction = fetchInstruction.binInstruction;
                            inst.cycle = fetchInstruction.cycle;
                            inst.cycleControl = fetchInstruction.cycleControl;
                            inst.ID = fetchInstruction.ID;
                            inst.stage = fetchInstruction.stage;
                            inst.iOp1 = fetchInstruction.iOp1;
                            inst.iOp2 = fetchInstruction.iOp2;
                            inst.iOp3 = fetchInstruction.iOp3;
                            inst.fOp1 = fetchInstruction.fOp1;
                            inst.fOp2 = fetchInstruction.fOp2;
                            inst.fOp3 = fetchInstruction.fOp3;
                            try
                            {
                                Instruction populateInstruction = populateReservationStation(inst, ref registers);
                                reorderBuffer.addToReorderBuffer(inst, ref config);
                                if(instructionQueue.Count > 0)
                                    instructionQueue.Dequeue();
                                inst.functionalUnitID = populateInstruction.functionalUnitID;
                                inst.fOp1 = populateInstruction.fOp1;
                                inst.fOp2 = populateInstruction.fOp2;
                                inst.fOp3 = populateInstruction.fOp3;
                                inst.iOp1 = populateInstruction.iOp1;
                                inst.iOp2 = populateInstruction.iOp2;
                                inst.iOp3 = populateInstruction.iOp3;
                                inst.dependantOpID1 = populateInstruction.dependantOpID1;
                                inst.dependantOpID2 = populateInstruction.dependantOpID2;
                                inst.stage = 2;
                                inst.stage1Cycle = cycleCount;
                                inst.justIssued = true;
                            }
                            catch (Exception)
                            {
                                instructionsInFlight.Remove(inst);
                                reservationStationDelay++;
                            }
                            break;
                    }
                }
                if (instructionQueue.Count == 0 && cycleCount != 1)
                {
                    fillInstructionQueue(ref IM);
                }
                fetchFromInstructionQueue();

            } while (!halted && !stepThrough);
        }

        private void fillInstructionQueue(ref InstructionMemory IM)
        {
            for(int i = 0; i < 20; i++)
            {
                fetchInstruction = new Instruction();
                try
                {
                    fetchInstruction = fetch.getNextInstruction(ref IM);
                    if (fetchInstruction != null)
                    {
                        fetchInstruction.ID = instructionID++;
                        fetchInstruction.stage = 1;
                        instructionQueue.Enqueue(fetchInstruction);
                    }
                    else
                        return;
                }
                catch(Exception)
                {
                    return;
                }
            }
        }

        private void fetchFromInstructionQueue()
        {
            if(instructionQueue.Count != 0)
                instructionsInFlight.Add(instructionQueue.Peek());
            return;
        }

        private Instruction decode(Instruction instruction, ref InstructionMemory IM, ref ConfigCycle config)
        {
            CU.decode(ref IM, ref instruction, ref config);
            if(instruction.opcode >= 2 && instruction.opcode <= 8)
            {
                if((config.predictionSet && executiveBranchDecision) || instruction.opcode == 2)
                {
                    IM.ProgramCounter = instruction.address;
                    instructionID = instruction.ID + 1;
                    instructionQueue.Clear();
                }
            }
            return instruction;
        }
        
        private Instruction execute(Instruction instruction, ref RegisterFile registers,ref DataMemory memory, 
            ref InstructionMemory IM, ref ConfigCycle config, ref ALU alu, ref string result)
        {
            if (instruction.opcode == 0 || instruction.opcode == 1)
            {
                instruction.stage2CycleStart = cycleCount;
                instruction.stage2CycleEnd = cycleCount;
                instruction.doneExecuting = true;
                return instruction;
            }
            try
            {   
                if(!instruction.executionInProgress)
                    sendToFU(ref instruction);
            }
            catch(Exception)
            {
                instruction.doneExecuting = false;
                return instruction;
            }
            instruction.executionInProgress = true;
            bool dependencies = checkDependencies(ref instruction, ref commonDataBus, ref registers);

            if (dependencies)
            {
                instruction.doneExecuting = false;
                return instruction;
            }
            if (instruction.stage2CycleStart == 0)    //Makes sure this only runs once once the instruction started executing after dependencies are gone
                instruction.stage2CycleStart = cycleCount;

            switch (instruction.functionalUnitID)
            {
                case 1:
                    if(!intAddFu.instruction.stage2ExecutionFinished && !intAddFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref intAddFu.instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        intAddFu.instruction.stage2ExecutionFinished = true;
                        intAddFu.instruction.executionInProgress = true;
                        intAddFu.instruction.cycleControl--;
                    }
                    else
                        intAddFu.instruction.cycleControl--;
                    if (intAddFu.instruction.cycleControl == 0)
                    {
                        intAddFu.oneCycleFU = false;            //Makes sure this Functional Unit is only available for one cycle
                        intAddFu.instruction.doneExecuting = true;
                        intAddFu.instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        intAddFu.instruction = null;
                        return instruction;
                    }
                    break;
                case 2:
                    if (!intSubFu.instruction.stage2ExecutionFinished && !intSubFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref intSubFu.instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        intSubFu.instruction.stage2ExecutionFinished = true;
                        intSubFu.instruction.executionInProgress = true;
                        intSubFu.instruction.cycleControl--;
                    }
                    else
                        intSubFu.instruction.cycleControl--;
                    if (intSubFu.instruction.cycleControl == 0)
                    {
                        intSubFu.oneCycleFU = false;        //Makes sure this Functional Unit is only available for one cycle
                        intSubFu.instruction.doneExecuting = true;
                        intSubFu.instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        intSubFu.instruction = null;
                        return instruction;
                    }
                    break;
                case 3:
                    if (!intMulFu.instruction.stage2ExecutionFinished && !intMulFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref intMulFu.instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        intMulFu.instruction.stage2ExecutionFinished = true;
                        intMulFu.instruction.executionInProgress = true;
                        intMulFu.instruction.cycleControl--;
                    }
                    else
                        intMulFu.instruction.cycleControl--;
                    if (intMulFu.instruction.cycleControl == 0)
                    {
                        intMulFu.oneCycleFU = false;       //Makes sure this Functional Unit is only available for one cycle
                        intMulFu.instruction.doneExecuting = true;
                        intMulFu.instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        intMulFu.instruction = null;
                        return instruction;
                    }
                    break;
                case 4:
                    if (!intDivFu.instruction.stage2ExecutionFinished && !intDivFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref intDivFu.instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        intDivFu.instruction.stage2ExecutionFinished = true;
                        intDivFu.instruction.executionInProgress = true;
                        intDivFu.instruction.cycleControl--;
                    }
                    else
                        intDivFu.instruction.cycleControl--;
                    if (intDivFu.instruction.cycleControl == 0)
                    {
                        intDivFu.oneCycleFU = false;        //Makes sure this Functional Unit is only available for one cycle
                        intDivFu.instruction.doneExecuting = true;
                        intDivFu.instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        intDivFu.instruction = null;
                        return instruction;
                    }
                    break;
                case 5:
                    if (!flAddFu.instruction.stage2ExecutionFinished && !flAddFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref flAddFu.instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        flAddFu.instruction.stage2ExecutionFinished = true;
                        flAddFu.instruction.executionInProgress = true;
                        flAddFu.instruction.cycleControl--;
                    }
                    else
                        flAddFu.instruction.cycleControl--;
                    if (flAddFu.instruction.cycleControl == 0)
                    { 
                        flAddFu.oneCycleFU = false;      //Makes sure this Functional Unit is only available for one cycle
                        flAddFu.instruction.doneExecuting = true;
                        flAddFu.instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        flAddFu.instruction = null;
                        return instruction;
                    }
                    break;
                case 6:
                    if (!flSubFu.instruction.stage2ExecutionFinished && !flSubFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref flSubFu.instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        flSubFu.instruction.stage2ExecutionFinished = true;
                        flSubFu.instruction.executionInProgress = true;
                        flSubFu.instruction.cycleControl--;
                    }
                    else
                        flSubFu.instruction.cycleControl--;
                    if (flSubFu.instruction.cycleControl == 0)
                    {
                        flSubFu.oneCycleFU = false;      //Makes sure this Functional Unit is only available for one cycle
                        flSubFu.instruction.doneExecuting = true;
                        flSubFu.instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        flSubFu.instruction = null;
                        return instruction;
                    }
                    break;
                case 7:
                    if (!flMultFu.instruction.stage2ExecutionFinished && !flMultFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref flMultFu.instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        flMultFu.instruction.stage2ExecutionFinished = true;
                        flMultFu.instruction.executionInProgress = true;
                        flMultFu.instruction.cycleControl--;
                    }
                    else
                        flMultFu.instruction.cycleControl--;
                    if (flMultFu.instruction.cycleControl == 0)
                    {
                        flMultFu.oneCycleFU = false;     //Makes sure this Functional Unit is only available for one cycle
                        flMultFu.instruction.doneExecuting = true;
                        flMultFu.instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        flMultFu.instruction = null;
                        return instruction;
                    }
                    break;
                case 8:
                    if (!flDivFu.instruction.stage2ExecutionFinished && !flDivFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref flDivFu.instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        flDivFu.instruction.stage2ExecutionFinished = true;
                        flDivFu.instruction.executionInProgress = true;
                        flDivFu.instruction.cycleControl--;
                    }
                    else
                        flDivFu.instruction.cycleControl--;
                    if (flDivFu.instruction.cycleControl == 0)
                    {
                        flDivFu.oneCycleFU = false;      //Makes sure this Functional Unit is only available for one cycle
                        flDivFu.instruction.doneExecuting = true;
                        flDivFu.instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        flDivFu.instruction = null;
                        return instruction;
                    }
                    break;
                case 9:
                    if (!bitFu.instruction.stage2ExecutionFinished && !bitFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref bitFu.instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        bitFu.instruction.stage2ExecutionFinished = true;
                        bitFu.instruction.executionInProgress = true;
                        bitFu.instruction.cycleControl--;
                    }
                    else
                        bitFu.instruction.cycleControl--;
                    if (bitFu.instruction.cycleControl == 0)
                    {
                        bitFu.oneCycleFU = false;     //Makes sure this Functional Unit is only available for one cycle
                        bitFu.instruction.doneExecuting = true;
                        bitFu.instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        bitFu.instruction = null;
                        return instruction;
                    }
                    break;
                case 10:
                    if (!memoryFu.instruction.stage2ExecutionFinished && !memoryFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref memoryFu.instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        memoryFu.instruction.stage2ExecutionFinished = true;
                        memoryFu.instruction.executionInProgress = true;
                        memoryFu.instruction.cycleControl--;
                    }
                    else
                        memoryFu.instruction.cycleControl--;
                    if (memoryFu.instruction.cycleControl == 0)
                    {
                        memoryFu.oneCycleFU = false;        //Makes sure this Functional Unit is only available for one cycle
                        memoryFu.instruction.doneExecuting = true;
                        memoryFu.instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        return instruction;
                    }
                    break;
                case 11:
                    if (!branchFu.instruction.stage2ExecutionFinished && !branchFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref branchFu.instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        branchFu.instruction.stage2ExecutionFinished = true;
                        branchFu.instruction.executionInProgress = true;
                        branchFu.instruction.cycleControl--;
                    }
                    else
                        branchFu.instruction.cycleControl--;
                    if (branchFu.instruction.cycleControl == 0)
                    {
                        branchFu.oneCycleFU = false;        //Makes sure this Functional Unit is only available for one cycle
                        branchFu.instruction.doneExecuting = true;
                        branchFu.instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        branchFu.instruction = null;
                        return instruction;
                    }
                    break;
                case 12:
                    if (!shiftFu.instruction.stage2ExecutionFinished && !shiftFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref shiftFu.instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        shiftFu.instruction.stage2ExecutionFinished = true;
                        shiftFu.instruction.executionInProgress = true;
                        shiftFu.instruction.cycleControl--;
                    }
                    else
                        shiftFu.instruction.cycleControl--;
                    if (shiftFu.instruction.cycleControl == 0)
                    {
                        shiftFu.oneCycleFU = false;         //Makes sure this Functional Unit is only available for one cycle
                        shiftFu.instruction.doneExecuting = true;
                        shiftFu.instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        shiftFu.instruction = null;
                        return instruction;
                    }
                    break;
            }
            return instruction;
        }

        private void sendToFU(ref Instruction instruction)
        {
            if (instruction.opcode == 0 || instruction.opcode == 1)
                return;

            switch(instruction.functionalUnitID)
            {
                case 1:
                    if (intAddFu.instruction == null && intAddFu.oneCycleFU) 
                    {
                        intAddFu.instruction = intAddRS.instruction;
                        intAddRS.instruction = null;
                        intAddRS.Busy = false;
                        return;
                    }
                    break;
                case 2:
                    if (intSubFu.instruction == null && intSubFu.oneCycleFU)
                    {
                        intSubFu.instruction = intSubRS.instruction;
                        intSubRS.instruction = null;
                        intSubRS.Busy = false;
                        return;
                    }
                    break;
                case 3:
                    if (intMulFu.instruction == null && intMulFu.oneCycleFU)
                    {
                        intMulFu.instruction = intMultRS.instruction;
                        intMultRS.instruction = null;
                        intMultRS.Busy = false;
                        return;
                    }
                    break;
                case 4:
                    if (intDivFu.instruction == null && intDivFu.oneCycleFU)
                    {
                        intDivFu.instruction = intDivRS.instruction;
                        intDivRS.instruction = null;
                        intDivRS.Busy = false;
                        return;
                    }
                    break;
                case 5:
                    if (flAddFu.instruction == null && flAddFu.oneCycleFU)
                    {
                        flAddFu.instruction = floatAddRS.instruction;
                        floatAddRS.instruction = null;
                        floatAddRS.Busy = false;
                        return;
                    }
                    break;
                case 6:
                    if (flSubFu.instruction == null && flSubFu.oneCycleFU)
                    {
                        flSubFu.instruction = floatSubRS.instruction;
                        floatSubRS.instruction = null;
                        floatSubRS.Busy = false;
                        return;
                    }
                    break;
                case 7:
                    if (flMultFu.instruction == null && flMultFu.oneCycleFU )
                    {
                        flMultFu.instruction = floatMultRS.instruction;
                        floatMultRS.instruction = null;
                        floatMultRS.Busy = false;
                        return;
                    }
                    break;
                case 8:
                    if (flDivFu.instruction == null && flDivFu.oneCycleFU )
                    {
                        flDivFu.instruction = floatDivRS.instruction;
                        floatDivRS.instruction = null;
                        floatDivRS.Busy = false;
                        return;
                    }
                    break;
                case 9:
                    if (bitFu.instruction == null && bitFu.oneCycleFU)
                    {
                        bitFu.instruction = bitwiseOPRS.instruction;
                        bitwiseOPRS.instruction = null;
                        bitwiseOPRS.Busy = false;
                        return;
                    }
                    break;
                case 10:
                    if (memoryFu.instruction == null && memoryFu.oneCycleFU )
                    {
                        memoryFu.instruction = load_storeBuffer.instruction;
                        load_storeBuffer.instruction = null;
                        load_storeBuffer.Busy = false;
                        return;
                    }
                    break;
                case 11:
                    if (branchFu.instruction == null && branchFu.oneCycleFU )
                    {
                        branchFu.instruction = branchOPS.instruction;
                        branchOPS.instruction = null;
                        branchOPS.Busy = false;
                        return;
                    }
                    break;
                case 12:
                    if (shiftFu.instruction == null && shiftFu.oneCycleFU )
                    {
                        shiftFu.instruction = shiftOPS.instruction;
                        shiftOPS.instruction = null;
                        shiftOPS.Busy = false;
                        return;
                    }
                    break;
            }
            throw new Exception();
        }

        private bool checkDependencies(ref Instruction instruction, ref CommonDataBus CDB, ref RegisterFile registers)
        {
            switch(instruction.opcode)
            {
                case 10:
                case 12:
                    checkOperandDependencies(ref memoryFu.instruction, ref registers);
                    if (instruction.isFloat)
                    {
                        if (memoryFu.instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(memoryFu.instruction.fOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    memoryFu.instruction.fOperand1 = float.Parse(CDB.CDB[memoryFu.instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            memoryFu.instruction.fOperand1 = registers.floatRegisters[0];
                    }
                    else
                    {
                        if (memoryFu.instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(memoryFu.instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    memoryFu.instruction.iOperand1 = int.Parse(CDB.CDB[memoryFu.instruction.iOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            memoryFu.instruction.iOperand1 = registers.intRegisters[0];
                    }
                    break;
                case 13:
                    if (instruction.isFloat)
                    {
                        checkOperandDependencies(ref flSubFu.instruction, ref registers);
                        if (flSubFu.instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(flSubFu.instruction.fOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    flSubFu.instruction.fOperand1 = int.Parse(CDB.CDB[flSubFu.instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flSubFu.instruction.fOperand1 = registers.floatRegisters[flSubFu.instruction.r3];
                    }
                    else
                    {
                        checkOperandDependencies(ref intSubFu.instruction, ref registers);
                        if (intSubFu.instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(intSubFu.instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    intSubFu.instruction.iOperand1 = int.Parse(CDB.CDB[intSubFu.instruction.iOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intSubFu.instruction.iOperand1 = registers.intRegisters[intSubFu.instruction.r3];
                    }
                    break;
                case 14:
                    
                    if (instruction.isFloat)
                    {
                        checkOperandDependencies(ref flSubFu.instruction, ref registers);
                        if (flSubFu.instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(flSubFu.instruction.fOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    flSubFu.instruction.fOperand1 = int.Parse(CDB.CDB[flSubFu.instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flSubFu.instruction.fOperand1 = registers.floatRegisters[flSubFu.instruction.r1];
                        if (flSubFu.instruction.fOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(flSubFu.instruction.fOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    flSubFu.instruction.fOperand2 = float.Parse(CDB.CDB[flSubFu.instruction.fOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flSubFu.instruction.fOperand2 = registers.floatRegisters[flSubFu.instruction.r2];
                    }
                    else
                    {
                        checkOperandDependencies(ref flSubFu.instruction, ref registers);
                        if (intSubFu.instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(intSubFu.instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    intSubFu.instruction.iOperand1 = int.Parse(CDB.CDB[intSubFu.instruction.iOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intSubFu.instruction.iOperand1 = registers.intRegisters[intSubFu.instruction.r1];
                        if (intSubFu.instruction.iOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(intSubFu.instruction.iOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    intSubFu.instruction.iOperand2 = int.Parse(CDB.CDB[intSubFu.instruction.iOp2]);
                            }
                            else
                                trueDependenceDelay++;
                        }
                        else
                            intSubFu.instruction.iOperand2 = registers.intRegisters[intSubFu.instruction.r2];
                    }
                    break;
                case 15:
                    if (instruction.isFloat)
                    {
                        checkOperandDependencies(ref memoryFu.instruction, ref registers);
                        if (memoryFu.instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(memoryFu.instruction.fOp1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    memoryFu.instruction.fOperand1 = float.Parse(CDB.CDB[memoryFu.instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            memoryFu.instruction.fOperand1 = registers.floatRegisters[memoryFu.instruction.r2];
                    }
                    else
                    {
                        checkOperandDependencies(ref memoryFu.instruction, ref registers);
                        if (memoryFu.instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(memoryFu.instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    memoryFu.instruction.iOperand1 = int.Parse(CDB.CDB[memoryFu.instruction.iOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            memoryFu.instruction.iOperand1 = registers.intRegisters[memoryFu.instruction.r2];
                    }
                    break;
                case 16:
                case 17:
                case 18:
                case 19:
                    checkOperandDependencies(ref shiftFu.instruction, ref registers);
                    if (shiftFu.instruction.iOp1 != "")
                    {
                        if (CDB.CDB.ContainsKey(shiftFu.instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                        {
                            if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                shiftFu.instruction.iOperand1 = int.Parse(CDB.CDB[shiftFu.instruction.iOp1]);
                        }
                        else
                        {
                            trueDependenceDelay++;
                            return true;
                        }
                    }
                    else
                        shiftFu.instruction.iOperand1 = registers.intRegisters[shiftFu.instruction.r1];
                    if (shiftFu.instruction.iOp2 != "")
                    {
                        if (CDB.CDB.ContainsKey(shiftFu.instruction.iOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                        {
                            if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                shiftFu.instruction.iOperand2 = int.Parse(CDB.CDB[shiftFu.instruction.iOp2]);
                        }
                        else
                        {
                            trueDependenceDelay++;
                            return true;
                        }
                    }
                    else
                        shiftFu.instruction.iOperand2 = registers.intRegisters[shiftFu.instruction.r2];
                    break;
                case 20:
                    if (instruction.isFloat)
                    {
                        checkOperandDependencies(ref flAddFu.instruction, ref registers);
                        if (flAddFu.instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(flAddFu.instruction.fOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    flAddFu.instruction.fOperand1 = int.Parse(CDB.CDB[flAddFu.instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flAddFu.instruction.fOperand1 = registers.floatRegisters[flAddFu.instruction.r1];
                        if (flAddFu.instruction.fOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(flAddFu.instruction.fOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    flAddFu.instruction.fOperand2 = float.Parse(CDB.CDB[flAddFu.instruction.fOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flAddFu.instruction.fOperand2 = registers.floatRegisters[flAddFu.instruction.r2];
                    }
                    else
                    {
                        checkOperandDependencies(ref intAddFu.instruction, ref registers);
                        if (intAddFu.instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(intAddFu.instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    intAddFu.instruction.iOperand1 = int.Parse(CDB.CDB[intAddFu.instruction.iOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intAddFu.instruction.iOperand1 = registers.intRegisters[intAddFu.instruction.r1];
                        if (intAddFu.instruction.iOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(intAddFu.instruction.iOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    intAddFu.instruction.iOperand2 = int.Parse(CDB.CDB[intAddFu.instruction.iOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intAddFu.instruction.iOperand2 = registers.intRegisters[intAddFu.instruction.r2];
                    }
                    break;
                case 21:
                    if (instruction.isFloat)
                    {
                        checkOperandDependencies(ref flSubFu.instruction, ref registers);
                        if (flSubFu.instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(flSubFu.instruction.fOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    flSubFu.instruction.fOperand1 = int.Parse(CDB.CDB[flSubFu.instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flSubFu.instruction.fOperand1 = registers.floatRegisters[flSubFu.instruction.r1];
                        if (flSubFu.instruction.fOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(flSubFu.instruction.fOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    flSubFu.instruction.fOperand2 = float.Parse(CDB.CDB[flSubFu.instruction.fOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flSubFu.instruction.fOperand2 = registers.floatRegisters[flSubFu.instruction.r2];
                    }
                    else
                    {
                        checkOperandDependencies(ref intSubFu.instruction, ref registers);
                        if (intSubFu.instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(intSubFu.instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    intSubFu.instruction.iOperand1 = int.Parse(CDB.CDB[intSubFu.instruction.iOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intSubFu.instruction.iOperand1 = registers.intRegisters[intSubFu.instruction.r1];
                        if (intSubFu.instruction.iOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(intSubFu.instruction.iOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    intSubFu.instruction.iOperand2 = int.Parse(CDB.CDB[intSubFu.instruction.iOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intSubFu.instruction.iOperand2 = registers.intRegisters[intSubFu.instruction.r2];
                    }
                    break;
                case 22:
                    if (instruction.isFloat)
                    {
                        checkOperandDependencies(ref flMultFu.instruction, ref registers);
                        if (flMultFu.instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(flMultFu.instruction.fOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    flMultFu.instruction.fOperand1 = int.Parse(CDB.CDB[flMultFu.instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flMultFu.instruction.fOperand1 = registers.floatRegisters[flMultFu.instruction.r1];
                        if (flMultFu.instruction.fOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(flMultFu.instruction.fOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    flMultFu.instruction.fOperand2 = float.Parse(CDB.CDB[flMultFu.instruction.fOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flMultFu.instruction.fOperand2 = registers.floatRegisters[flMultFu.instruction.r2];
                    }
                    else
                    {
                        checkOperandDependencies(ref intMulFu.instruction, ref registers);
                        if (intMulFu.instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(intMulFu.instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    intMulFu.instruction.iOperand1 = int.Parse(CDB.CDB[intMulFu.instruction.iOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intMulFu.instruction.iOperand1 = registers.intRegisters[intMulFu.instruction.r1];
                        if (intMulFu.instruction.iOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(intMulFu.instruction.iOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    intMulFu.instruction.iOperand2 = int.Parse(CDB.CDB[intMulFu.instruction.iOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intMulFu.instruction.iOperand2 = registers.intRegisters[intMulFu.instruction.r2];
                    }
                    break;
                case 23:
                    if(instruction.isFloat)
                    {
                        checkOperandDependencies(ref flDivFu.instruction, ref registers);
                        if (flDivFu.instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(flDivFu.instruction.fOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    flDivFu.instruction.fOperand1 = int.Parse(CDB.CDB[flDivFu.instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flDivFu.instruction.fOperand1 = registers.floatRegisters[flDivFu.instruction.r1];
                        if (flDivFu.instruction.fOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(intDivFu.instruction.fOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    flDivFu.instruction.fOperand2 = float.Parse(CDB.CDB[flDivFu.instruction.fOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flDivFu.instruction.fOperand2 = registers.floatRegisters[flDivFu.instruction.r2];
                    }
                    else
                    {
                        checkOperandDependencies(ref intDivFu.instruction, ref registers);
                        if (intDivFu.instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(intDivFu.instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    intDivFu.instruction.iOperand1 = int.Parse(CDB.CDB[intDivFu.instruction.iOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intDivFu.instruction.iOperand1 = registers.intRegisters[intDivFu.instruction.r1];
                        if (intDivFu.instruction.iOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(intDivFu.instruction.iOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    intDivFu.instruction.iOperand2 = int.Parse(CDB.CDB[intDivFu.instruction.iOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intDivFu.instruction.iOperand2 = registers.intRegisters[intDivFu.instruction.r2];
                    }
                    break;
                case 24:
                case 25:
                case 26:
                    checkOperandDependencies(ref bitFu.instruction, ref registers);
                    if (bitFu.instruction.iOp1 != "")
                    {
                        if (CDB.CDB.ContainsKey(bitFu.instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                        {
                            if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                bitFu.instruction.iOperand1 = int.Parse(CDB.CDB[bitFu.instruction.iOp1]);
                        }
                        else
                        {
                            trueDependenceDelay++;
                            return true;
                        }
                    }
                    else
                        bitFu.instruction.iOperand1 = registers.intRegisters[bitFu.instruction.r1];
                    if (bitFu.instruction.iOp2 != "")
                    {
                        if (CDB.CDB.ContainsKey(bitFu.instruction.iOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                        {
                            if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                bitFu.instruction.iOperand2 = int.Parse(CDB.CDB[bitFu.instruction.iOp2]);
                        }
                        else
                        {
                            trueDependenceDelay++;
                            return true;
                        }
                    }
                    else
                        bitFu.instruction.iOperand2 = registers.intRegisters[bitFu.instruction.r2];
                    break;
                case 27:
                    checkOperandDependencies(ref bitFu.instruction, ref registers);
                    if (bitFu.instruction.iOp1 != "")
                    {
                        if (CDB.CDB.ContainsKey(bitFu.instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                        {
                            if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                bitFu.instruction.iOperand1 = int.Parse(CDB.CDB[bitFu.instruction.iOp1]);
                        }
                        else
                        {
                            trueDependenceDelay++;
                            return true;
                        }
                    }
                    else
                        bitFu.instruction.iOperand1 = registers.intRegisters[bitFu.instruction.r1];
                    break;
            }
            return false;
        }


        //Will use the opcode and flags to figure out which reservation station the instruction needs to be in
        private Instruction populateReservationStation(Instruction instruction, ref RegisterFile registers)
        {
            switch(instruction.opcode)
            {
                case 0:
                case 1:
                    return instruction;
                case 2:         //Branch Instructions
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    if(!branchOPS.Busy)
                    {
                        branchOPS.Busy = true;
                        instruction.functionalUnitID = 11;
                        branchOPS.instruction = instruction;
                        return instruction;
                        //Vj, Vk, Qj, Qk will be implemented here
                    }
                    break;
                case 15:
                    if (instruction.isFloat)
                    {
                        if (!load_storeBuffer.Busy)
                        {
                            checkOperandDependencies(ref instruction, ref registers);
                            load_storeBuffer.Busy = true;
                            registers.floatQi[instruction.r3] = "memoryFu";
                            registers.floatQiIndex[instruction.r3] = instruction.ID;
                            instruction.functionalUnitID = 10;
                            load_storeBuffer.instruction = instruction;
                            return instruction;
                        }
                    }
                    else
                    {
                        if (!load_storeBuffer.Busy)
                        {
                            checkOperandDependencies(ref instruction, ref registers);
                            load_storeBuffer.Busy = true;
                            registers.intQi[instruction.r3] = "memoryFu";
                            registers.intQiIndex[instruction.r3] = instruction.ID;
                            instruction.functionalUnitID = 10;
                            load_storeBuffer.instruction = instruction;
                            return instruction;
                        }
                    }
                    break;
                
                case 9:         //Load Instructions
                    if (!load_storeBuffer.Busy)
                    {
                        load_storeBuffer.Busy = true;
                        if(instruction.isFloat)
                        {
                            registers.floatQi[0] = "memoryFu";
                            registers.floatQiIndex[0] = instruction.ID;
                        }
                        else
                        {
                            registers.intQi[0] = "memoryFu";
                            registers.intQiIndex[0] = instruction.ID;
                        }
                        instruction.functionalUnitID = 10;
                        load_storeBuffer.instruction = instruction;
                        return instruction;
                        //Vj, Vk, Qj, Qk will be implemented here
                    }
                    break;
                case 10:
                    if (!load_storeBuffer.Busy)
                    {
                        load_storeBuffer.Busy = true;
                        checkOperandDependencies(ref instruction, ref registers);
                        instruction.functionalUnitID = 10;
                        load_storeBuffer.instruction = instruction;
                        return instruction;
                        //Vj, Vk, Qj, Qk will be implemented here
                    }
                    break;
                case 11:
                    if (!load_storeBuffer.Busy)
                    {
                        load_storeBuffer.Busy = true;
                        if (instruction.isFloat)
                        {
                            registers.floatQi[instruction.r3] = "memoryFu";
                            registers.floatQiIndex[instruction.r3] = instruction.ID;
                        }
                        else
                        {
                            registers.intQi[instruction.r3] = "memoryFu";
                            registers.intQiIndex[instruction.r3] = instruction.ID;
                        }
                        instruction.functionalUnitID = 10;
                        load_storeBuffer.instruction = instruction;
                        return instruction;
                        //Vj, Vk, Qj, Qk will be implemented here
                    }
                    break;

                case 12: 
                    if (instruction.isFloat)
                    {
                        if (!load_storeBuffer.Busy)
                        {
                            checkOperandDependencies(ref instruction, ref registers);
                            registers.floatQi[instruction.r3] = "memoryFu";
                            registers.floatQiIndex[instruction.r3] = instruction.ID;
                            load_storeBuffer.Busy = true;
                            instruction.functionalUnitID = 10;
                            load_storeBuffer.instruction = instruction;
                            return instruction;
                            //Vj, Vk, Qj, Qk will be implemented here
                        }
                    }
                    else
                    {
                        if (!load_storeBuffer.Busy)
                        {
                            checkOperandDependencies(ref instruction, ref registers);
                            load_storeBuffer.Busy = true;
                            registers.intQi[instruction.r3] = "memoryFu";
                            registers.intQiIndex[instruction.r3] = instruction.ID;
                            instruction.functionalUnitID = 10;
                            load_storeBuffer.instruction = instruction;
                            return instruction;
                            //Vj, Vk, Qj, Qk will be implemented here
                        }
                    }
                    break;
                case 13:
                    if (instruction.isFloat)
                    {
                        if (!intSubRS.Busy)
                        {
                            checkOperandDependencies(ref instruction, ref registers);
                            floatSubRS.Busy = true;
                            instruction.functionalUnitID = 2;
                            floatSubRS.instruction = instruction;
                            return instruction;
                        }
                    }
                    else
                    {
                        if (!intSubRS.Busy)
                        {
                            checkOperandDependencies(ref instruction, ref registers);
                            intSubRS.Busy = true;
                            instruction.functionalUnitID = 2;
                            intSubRS.instruction = instruction;
                            return instruction;
                        }
                    }
                    break;
                case 14:
                    if (instruction.isFloat)
                    {
                        if (!floatSubRS.Busy)
                        {
                            checkOperandDependencies(ref instruction, ref registers);
                            floatSubRS.Busy = true;
                            instruction.functionalUnitID = 2;
                            floatSubRS.instruction = instruction;
                            return instruction;
                        }
                    }
                    else
                    {
                        if (!intSubRS.Busy)
                        {
                            checkOperandDependencies(ref instruction, ref registers);
                            intSubRS.Busy = true;
                            instruction.functionalUnitID = 2;
                            intSubRS.instruction = instruction;
                            return instruction;
                        }
                    }
                    break;
                case 16:
                case 17:
                case 18:
                case 19:
                    if(!shiftOPS.Busy)
                    {
                        checkOperandDependencies(ref instruction, ref registers);
                        shiftOPS.Busy = true;
                        registers.intQi[instruction.r3] = "shiftFu";
                        registers.intQiIndex[instruction.r3] = instruction.ID;
                        instruction.functionalUnitID = 12;
                        shiftOPS.instruction = instruction;
                        return instruction;
                    }
                    break;
                case 20:            //Add instruction. Checks if it's float
                    if((instruction.isFloat))
                    {
                        if (!floatAddRS.Busy)
                        {
                            checkOperandDependencies(ref instruction, ref registers);
                            floatAddRS.Busy = true;
                            registers.floatQi[instruction.r3] = "flAddFu";
                            registers.floatQiIndex[instruction.r3] = instruction.ID;
                            instruction.functionalUnitID = 5;
                            floatAddRS.instruction = instruction;
                            return instruction;
                        }
                    }
                    else
                    {
                        if(!intAddRS.Busy)
                        {
                            checkOperandDependencies(ref instruction, ref registers);
                            intAddRS.Busy = true;
                            registers.intQi[instruction.r3] = "intAddFu";
                            registers.intQiIndex[instruction.r3] = instruction.ID;
                            instruction.functionalUnitID = 1;
                            intAddRS.instruction = instruction;
                            return instruction;
                        }
                    }
                    
                    break;
                case 21:            //Sub instruction. Checks if it's float
                    if ((instruction.isFloat))
                    {
                        if(!floatSubRS.Busy)
                        {
                            checkOperandDependencies(ref instruction, ref registers);
                            floatSubRS.Busy = true;
                            registers.floatQi[instruction.r3] = "flSubFu";
                            registers.floatQiIndex[instruction.r3] = instruction.ID;
                            instruction.functionalUnitID = 6;
                            floatSubRS.instruction = instruction;
                            return instruction;
                        }
                    }
                    else
                    {
                        if (!intSubRS.Busy)
                        {
                            checkOperandDependencies(ref instruction, ref registers);
                            intSubRS.Busy = true;
                            registers.intQi[instruction.r3] = "intSubFu";
                            registers.intQiIndex[instruction.r3] = instruction.ID;
                            instruction.functionalUnitID = 2;
                            intSubRS.instruction = instruction;
                            return instruction;
                        }
                    }
                    break;
                case 22:            //Mult instructions. Checks if it's float
                    if ((instruction.isFloat))
                    {
                        if (!floatMultRS.Busy)
                        {
                            checkOperandDependencies(ref instruction, ref registers);
                            floatMultRS.Busy = true;
                            registers.floatQi[instruction.r3] = "flMultFu";
                            registers.floatQiIndex[instruction.r3] = instruction.ID;
                            instruction.functionalUnitID = 7;
                            floatMultRS.instruction = instruction;
                            return instruction;
                        }
                    }
                    else
                    {
                        if (!intMultRS.Busy)
                        {
                            checkOperandDependencies(ref instruction, ref registers);
                            intMultRS.Busy = true;
                            registers.intQi[instruction.r3] = "intMulFu";
                            registers.intQiIndex[instruction.r3] = instruction.ID;
                            instruction.functionalUnitID = 3;
                            intMultRS.instruction = instruction;
                            return instruction;
                        }
                    }
                      
                    break;
                case 23:            //Div instructions. Checks if it's float
                    if ((instruction.isFloat))
                    {
                        if (!floatDivRS.Busy)
                        {
                            checkOperandDependencies(ref instruction, ref registers);
                            floatDivRS.Busy = true;
                            registers.floatQi[instruction.r3] = "flDivFu";
                            registers.floatQiIndex[instruction.r3] = instruction.ID;
                            instruction.functionalUnitID = 8;
                            floatDivRS.instruction = instruction;
                            return instruction;
                        }
                    }
                    else
                    {
                        if (!intDivRS.Busy)
                        {
                            checkOperandDependencies(ref instruction, ref registers);
                            intDivRS.Busy = true;
                            registers.intQi[instruction.r3] = "intDivFu";
                            registers.intQiIndex[instruction.r3] = instruction.ID;
                            instruction.functionalUnitID = 4;
                            intDivRS.instruction = instruction;
                            return instruction;
                        }
                    }
                    break;
                case 24:
                case 25:
                case 26:
                    if (!bitwiseOPRS.Busy)
                    {
                        checkOperandDependencies(ref instruction, ref registers);
                        bitwiseOPRS.Busy = true;
                        registers.intQi[instruction.r3] = "bitFu";
                        registers.intQiIndex[instruction.r3] = instruction.ID;
                        instruction.functionalUnitID = 9;
                        bitwiseOPRS.instruction = instruction;
                        return instruction;
                    }
                    break;
                case 27:
                    if (!bitwiseOPRS.Busy)
                    {
                        checkOperandDependencies(ref instruction, ref registers);
                        bitwiseOPRS.Busy = true;
                        registers.intQi[instruction.r3] = "bitFu";
                        registers.intQiIndex[instruction.r3] = instruction.ID;
                        instruction.functionalUnitID = 9;
                        bitwiseOPRS.instruction = instruction;
                        return instruction;
                    }
                    break;


            }
            throw new NotImplementedException();
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
        public bool detectControlHazard(int id, ref RegisterFile registers, Instruction instruction, ref InstructionMemory IM, ref ConfigCycle config)
        {
            if (id == -1)
                return false;

            if (instruction.opcode < 2 || instruction.opcode > 8)
                return false;


            if (config.whatBitPredictor == 2)
            {
                if (twoBitPredictionCounter == 0)
                {
                    executiveBranchDecision = false;
                }
                else if (twoBitPredictionCounter == 1)
                {
                    executiveBranchDecision = false;
                }
                else if (twoBitPredictionCounter == 2)
                {
                    executiveBranchDecision = true;
                }
                else if (twoBitPredictionCounter == 3)
                {
                    executiveBranchDecision = true;
                }
            }
            else if(config.whatBitPredictor == 1)
            {
                executiveBranchDecision = lastBranchDecision;
            }

            Instruction testInstruction = new Instruction();

            try
            {
                testInstruction = instructionsInFlight.Find(inst => inst.ID == (id + 1));
            }
            catch (Exception)
            {
                if(twoBitPredictionCounter > 0)
                    twoBitPredictionCounter -= 1;
                IM.ProgramCounter = (instruction.programCounterValue + 3);
                goto flush;
            }

            if (lastBranchDecision)
            {
                if (instruction.address != testInstruction.programCounterValue)
                {
                    if (twoBitPredictionCounter < 3)
                        twoBitPredictionCounter += 1;
                    IM.ProgramCounter = instruction.address;
                    goto flush;
                }
                if (twoBitPredictionCounter > 0)
                    twoBitPredictionCounter -= 1;
                return false;
            }
            else
            {
                if ((instruction.programCounterValue + 3) != testInstruction.programCounterValue)
                {
                    if (twoBitPredictionCounter > 0)
                        twoBitPredictionCounter -= 1;
                    IM.ProgramCounter = (instruction.programCounterValue + 3);
                    goto flush;
                }
                if (twoBitPredictionCounter < 3)
                    twoBitPredictionCounter += 1;

                return false;
            }

            flush:

            instructionID = id + 1;
            instructionsInFlight.Clear();
            instructionQueue.Clear();
            intAddFu.instruction = null;
            intSubFu.instruction = null;
            intMulFu.instruction = null;
            intDivFu.instruction = null;
            flAddFu.instruction = null;
            flSubFu.instruction = null;
            flMultFu.instruction = null;
            flDivFu.instruction = null;
            bitFu.instruction = null;
            memoryFu.instruction = null;
            branchFu.instruction = null;
            shiftFu.instruction = null;
            intAddRS.instruction = null;
            intSubRS.instruction = null;
            intMultRS.instruction = null;
            intDivRS.instruction = null;
            floatAddRS.instruction = null;
            floatSubRS.instruction = null;
            floatMultRS.instruction = null;
            floatDivRS.instruction = null;
            bitwiseOPRS.instruction = null;
            load_storeBuffer.instruction = null;
            branchOPS.instruction = null;
            shiftOPS.instruction = null;

            intAddRS.Busy = false;
            intSubRS.Busy = false;
            intMultRS.Busy = false;
            intDivRS.Busy = false;
            floatAddRS.Busy = false;
            floatSubRS.Busy = false;
            floatMultRS.Busy = false;
            floatDivRS.Busy = false;
            bitwiseOPRS.Busy = false;
            load_storeBuffer.Busy = false;
            branchOPS.Busy = false;
            shiftOPS.Busy = false;
            reorderBuffer.removeAllInstructionsAfterHazard(id);
            commonDataBus.CDB.Clear();
            commonDataBus.index.Clear();
            commonDataBus.IDIndex.Clear();

            registers.clearRegistersQI();

            return true;
        }

        private void clearFU(Instruction instruction)
        {
            switch(instruction.functionalUnitID)
            {
                case 1:
                    if(instruction.Equals(intAddFu.instruction))        //Makes sure that processing instructions do not delete different instructions in functional unit
                        intAddFu.instruction = null;
                    break;
                case 2:
                    if(instruction.Equals(intSubFu.instruction))
                        intSubFu.instruction = null;
                    break;
                case 3:
                    if(instruction.Equals(intMulFu.instruction))
                        intMulFu.instruction = null;
                    break;
                case 4:
                    if(instruction.Equals(intDivFu.instruction))
                    intDivFu.instruction = null;
                    break;
                case 5:
                    if(instruction.Equals(flAddFu.instruction))
                        flAddFu.instruction = null;
                    break;
                case 6:
                    if(instruction.Equals(flSubFu.instruction))
                        flSubFu.instruction = null;
                    break;
                case 7:
                    if(instruction.Equals(flMultFu.instruction))
                        flMultFu.instruction = null;
                    break;
                case 8:
                    if(instruction.Equals(flDivFu.instruction))
                        flDivFu.instruction = null;
                    break;
                case 9:
                    if(instruction.Equals(bitFu.instruction))
                    bitFu.instruction = null;
                    break;
                case 10:
                    if(instruction.Equals(memoryFu.instruction))
                        memoryFu.instruction = null;
                    break;
                case 11:
                    if(instruction.Equals(branchFu.instruction))
                        branchFu.instruction = null;
                    break;
                case 12:
                    if(instruction.Equals(shiftFu.instruction))
                        shiftFu.instruction = null;
                    break;
            }
        }



        private Instruction checkOperandDependencies(ref Instruction instruction, ref RegisterFile registers)
        {
            switch (instruction.opcode)
            {
                
                case 10:
                    if (instruction.isFloat)
                    {
                        if (!registers.floatQi[0].Equals("0") && instruction.ID > registers.floatQiIndex[0])
                        {
                            instruction.fOp1 = registers.floatQi[0];
                            instruction.dependantOpID1 = registers.floatQiIndex[0];
                        }
                        else
                        {
                            instruction.fOp1 = "";
                            instruction.dependantOpID1 = -1;
                        }
                    }
                    else
                    {
                        if (!registers.intQi[0].Equals("0") && instruction.ID > registers.intQiIndex[0])
                        {
                            instruction.iOp1 = registers.intQi[0];
                            instruction.dependantOpID1 = registers.intQiIndex[0];
                        }
                        else
                        {
                            instruction.iOp1 = "";
                            instruction.dependantOpID1 = -1;
                        }
                    }
                    break;
                case 12:
                    if (instruction.isFloat)
                    {
                        if (!registers.floatQi[0].Equals("0") && instruction.ID > registers.floatQiIndex[0])
                        {
                            instruction.fOp1 = registers.floatQi[0];
                            instruction.dependantOpID1 = registers.floatQiIndex[0];
                        }
                        else
                        {
                            instruction.fOp1 = "";
                            instruction.dependantOpID1 = -1;
                        }
                    }
                    else
                    {
                        if (!registers.intQi[0].Equals("0"))
                        {
                            instruction.iOp1 = registers.intQi[0];
                            instruction.dependantOpID1 = registers.intQiIndex[0];
                        }
                        else
                        {
                            instruction.iOp1 = "";
                            instruction.dependantOpID1 = -1;
                        }
                    }
                    break;
                case 13:
                    if (instruction.isFloat)
                    {
                        if (!registers.floatQi[instruction.r3].Equals("0") && instruction.ID > registers.floatQiIndex[instruction.r3])
                        {
                            instruction.fOp1 = registers.floatQi[instruction.r3];
                            instruction.dependantOpID1 = registers.floatQiIndex[instruction.r3];
                        }
                        else
                        {
                            instruction.fOp1 = "";
                            instruction.dependantOpID1 = -1;
                        }
                    }
                    else
                    {
                        if (!registers.intQi[instruction.r3].Equals("0") && instruction.ID > registers.intQiIndex[instruction.r3])
                        {
                            instruction.iOp1 = registers.intQi[instruction.r3];
                            instruction.dependantOpID1 = registers.intQiIndex[instruction.r3];
                        }
                        else
                        {
                            instruction.iOp1 = "";
                            instruction.dependantOpID1 = -1;
                        }
                    }
                    break;
                case 14:
                    if (instruction.isFloat)
                    {
                        if (!registers.floatQi[instruction.r1].Equals("0") && instruction.ID > registers.floatQiIndex[instruction.r1])
                        {
                            instruction.fOp1 = registers.floatQi[instruction.r1];
                            instruction.dependantOpID1 = registers.floatQiIndex[instruction.r1];
                        }
                        else
                        {
                            instruction.fOp1 = "";
                            instruction.dependantOpID1 = -1;
                        }

                        if (!registers.floatQi[instruction.r2].Equals("0") && instruction.ID > registers.floatQiIndex[instruction.r2])
                        {
                            instruction.fOp2 = registers.floatQi[instruction.r2];
                            instruction.dependantOpID2 = registers.floatQiIndex[instruction.r2];
                        }
                        else
                        {
                            instruction.fOp2 = "";
                            instruction.dependantOpID2 = -1;
                        }
                    }
                    else
                    {
                        if (!registers.intQi[instruction.r1].Equals("0") && instruction.ID > registers.intQiIndex[instruction.r1])
                        {
                            instruction.iOp1 = registers.intQi[instruction.r1];
                            instruction.dependantOpID1 = registers.intQiIndex[instruction.r1];
                        }
                        else
                        {
                            instruction.fOp1 = "";
                            instruction.dependantOpID1 = -1;
                        }

                        if (!registers.intQi[instruction.r2].Equals("0") && instruction.ID > registers.intQiIndex[instruction.r2])
                        {
                            instruction.iOp2 = registers.intQi[instruction.r2];
                            instruction.dependantOpID2 = registers.intQiIndex[instruction.r2];
                        }
                        else
                        {
                            instruction.fOp2 = "";
                            instruction.dependantOpID2 = -1;
                        }
                    }
                    break;
                case 15:
                    if (instruction.isFloat)
                    {
                        if (!registers.floatQi[instruction.r2].Equals("0") && instruction.ID > registers.floatQiIndex[instruction.r2])
                        {
                            instruction.fOp1 = registers.floatQi[instruction.r2];
                            instruction.dependantOpID1 = registers.floatQiIndex[instruction.r2];
                        }
                        else
                        {
                            instruction.fOp1 = "";
                            instruction.dependantOpID1 = -1;
                        }
                    }
                    else
                    {
                        if (!registers.intQi[instruction.r2].Equals("0") && instruction.ID > registers.intQiIndex[instruction.r2])
                        {
                            instruction.iOp1 = registers.intQi[instruction.r2];
                            instruction.dependantOpID1 = registers.intQiIndex[instruction.r2];
                        }
                        else
                        {
                            instruction.iOp1 = "";
                            instruction.dependantOpID1 = -1;
                        }
                    }
                    break;
                case 16:
                case 17:
                case 18:
                case 19:
                    if (!registers.intQi[instruction.r1].Equals("0") && instruction.ID > registers.intQiIndex[instruction.r1])
                    {
                        instruction.iOp1 = registers.intQi[instruction.r1];
                        instruction.dependantOpID1 = registers.intQiIndex[instruction.r1];
                    }
                    else
                    {
                        instruction.iOp1 = "";
                        instruction.dependantOpID1 = -1;
                    }

                    if (!registers.intQi[instruction.r2].Equals("0") && instruction.ID > registers.intQiIndex[instruction.r2])
                    {
                        instruction.iOp2 = registers.intQi[instruction.r2];
                        instruction.dependantOpID2 = registers.intQiIndex[instruction.r2];
                    }
                    else
                    {
                        instruction.iOp2 = "";
                        instruction.dependantOpID2 = -1;
                    }
                    break;
                case 20:            //Add instruction. Checks if it's float
                    if ((instruction.isFloat))
                    {
                        if (!registers.floatQi[instruction.r1].Equals("0") && instruction.ID > registers.floatQiIndex[instruction.r1])
                        {
                            instruction.fOp1 = registers.floatQi[instruction.r1];
                            instruction.dependantOpID1 = registers.floatQiIndex[instruction.r1];
                        }
                        else
                        {
                            instruction.fOp1 = "";
                            instruction.dependantOpID1 = -1;
                        }

                        if (!registers.floatQi[instruction.r2].Equals("0") && instruction.ID > registers.floatQiIndex[instruction.r2])
                        {
                            instruction.fOp2 = registers.floatQi[instruction.r2];
                            instruction.dependantOpID2 = registers.floatQiIndex[instruction.r2];
                        }
                        else
                        {
                            instruction.fOp2 = "";
                            instruction.dependantOpID2 = -1;
                        }
                    }
                    else
                    {
                        if (!registers.intQi[instruction.r1].Equals("0") && instruction.ID > registers.intQiIndex[instruction.r1])
                        {
                            if(registers.intQiIndex[instruction.r1] < instruction.ID)
                            {
                                instruction.iOp1 = registers.intQi[instruction.r1];
                                instruction.dependantOpID1 = registers.intQiIndex[instruction.r1];
                            }
                        }
                        else
                        {
                            instruction.iOp1 = "";
                            instruction.dependantOpID1 = -1;
                        }

                        if (!registers.intQi[instruction.r2].Equals("0") && instruction.ID > registers.intQiIndex[instruction.r2])
                        {
                            instruction.iOp2 = registers.intQi[instruction.r2];
                            instruction.dependantOpID2 = registers.intQiIndex[instruction.r2];
                        }
                        else
                        {
                            instruction.iOp2 = "";
                            instruction.dependantOpID2 = -1;
                        }
                    }

                    break;
                case 21:            //Sub instruction. Checks if it's float
                    if ((instruction.isFloat))
                    {
                        if (!registers.floatQi[instruction.r1].Equals("0") && instruction.ID > registers.floatQiIndex[instruction.r1])
                        {
                            instruction.fOp1 = registers.floatQi[instruction.r1];
                            instruction.dependantOpID1 = registers.floatQiIndex[instruction.r1];
                        }
                        else
                        {
                            instruction.fOp1 = "";
                            instruction.dependantOpID1 = -1;
                        }

                        if (!registers.floatQi[instruction.r2].Equals("0") && instruction.ID > registers.floatQiIndex[instruction.r2])
                        {
                            instruction.fOp2 = registers.floatQi[instruction.r2];
                            instruction.dependantOpID2 = registers.floatQiIndex[instruction.r2];
                        }
                        else
                        {
                            instruction.fOp2 = "";
                            instruction.dependantOpID2 = -1;
                        }
                    }
                    else
                    {
                        if (!registers.intQi[instruction.r1].Equals("0") && instruction.ID > registers.intQiIndex[instruction.r1])
                        {
                            instruction.iOp1 = registers.intQi[instruction.r1];
                            instruction.dependantOpID1 = registers.intQiIndex[instruction.r1];
                        }
                        else
                        {
                            instruction.iOp1 = "";
                            instruction.dependantOpID1 = -1;
                        }

                        if (!registers.intQi[instruction.r2].Equals("0") && instruction.ID > registers.intQiIndex[instruction.r2])
                        {
                            instruction.iOp2 = registers.intQi[instruction.r2];
                            instruction.dependantOpID2 = registers.intQiIndex[instruction.r2];
                        }
                        else
                        {
                            instruction.iOp2 = "";
                            instruction.dependantOpID2 = -1;
                        }
                    }
                    break;
                case 22:            //Mult instructions. Checks if it's float
                    if ((instruction.isFloat))
                    {
                        if (!registers.floatQi[instruction.r1].Equals("0") && instruction.ID > registers.floatQiIndex[instruction.r1])
                        {
                            instruction.fOp1 = registers.floatQi[instruction.r1];
                            instruction.dependantOpID1 = registers.floatQiIndex[instruction.r1];
                        }
                        else
                        {
                            instruction.fOp1 = "";
                            instruction.dependantOpID1 = -1;
                        }

                        if (!registers.floatQi[instruction.r2].Equals("0") && instruction.ID > registers.floatQiIndex[instruction.r2])
                        {
                            instruction.fOp2 = registers.floatQi[instruction.r2];
                            instruction.dependantOpID2 = registers.floatQiIndex[instruction.r2];
                        }
                        else
                        {
                            instruction.fOp2 = "";
                            instruction.dependantOpID2 = -1;
                        }
                    }
                    else
                    {
                        if (!registers.intQi[instruction.r1].Equals("0") && instruction.ID > registers.intQiIndex[instruction.r1])
                        {
                            instruction.iOp1 = registers.intQi[instruction.r1];
                            instruction.dependantOpID1 = registers.intQiIndex[instruction.r1];
                        }
                        else
                        {
                            instruction.iOp1 = "";
                            instruction.dependantOpID1 = -1;
                        }

                        if (!registers.intQi[instruction.r2].Equals("0") && instruction.ID > registers.intQiIndex[instruction.r2])
                        {
                            instruction.iOp2 = registers.intQi[instruction.r2];
                            instruction.dependantOpID2 = registers.intQiIndex[instruction.r2];
                        }
                        else
                        {
                            instruction.iOp2 = "";
                            instruction.dependantOpID2 = -1;
                        }
                    }

                    break;
                case 23:            //Div instructions. Checks if it's float
                    if ((instruction.isFloat))
                    {
                        if (!registers.floatQi[instruction.r1].Equals("0") && instruction.ID > registers.floatQiIndex[instruction.r1])
                        {
                            instruction.fOp1 = registers.floatQi[instruction.r1];
                            instruction.dependantOpID1 = registers.floatQiIndex[instruction.r1];
                        }
                        else
                        {
                            instruction.fOp1 = "";
                            instruction.dependantOpID1 = -1;
                        }

                        if (!registers.floatQi[instruction.r2].Equals("0") && instruction.ID > registers.floatQiIndex[instruction.r2])
                        {
                            instruction.fOp2 = registers.floatQi[instruction.r2];
                            instruction.dependantOpID2 = registers.floatQiIndex[instruction.r2];
                        }
                        else
                        {
                            instruction.fOp2 = "";
                            instruction.dependantOpID2 = -1;
                        }
                    }
                    else
                    {
                        if (!registers.intQi[instruction.r1].Equals("0") && instruction.ID > registers.intQiIndex[instruction.r1])
                        {
                            instruction.iOp1 = registers.intQi[instruction.r1];
                            instruction.dependantOpID1 = registers.intQiIndex[instruction.r1];
                        }
                        else
                        {
                            instruction.iOp1 = "";
                            instruction.dependantOpID1 = -1;
                        }

                        if (!registers.intQi[instruction.r2].Equals("0") && instruction.ID > registers.intQiIndex[instruction.r2])
                        {
                            instruction.iOp2 = registers.intQi[instruction.r2];
                            instruction.dependantOpID2 = registers.intQiIndex[instruction.r2];
                        }
                        else
                        {
                            instruction.iOp2 = "";
                            instruction.dependantOpID2 = -1;
                        }
                    }
                    break;
                case 24:
                case 25:
                case 26:
                    if (!registers.intQi[instruction.r1].Equals("0") && instruction.ID > registers.intQiIndex[instruction.r1])
                    {
                        instruction.iOp1 = registers.intQi[instruction.r1];
                        instruction.dependantOpID1 = registers.intQiIndex[instruction.r1];
                    }
                    else
                    {
                        instruction.iOp1 = "";
                        instruction.dependantOpID1 = -1;
                    }

                    if (!registers.intQi[instruction.r2].Equals("0") && instruction.ID > registers.intQiIndex[instruction.r2])
                    {
                        instruction.iOp2 = registers.intQi[instruction.r2];
                        instruction.dependantOpID2 = registers.intQiIndex[instruction.r2];
                    }
                    else
                    {
                        instruction.iOp2 = "";
                        instruction.dependantOpID2 = -1;
                    }
                    break;
                case 27:
                    if (!registers.intQi[instruction.r1].Equals("0") && instruction.ID > registers.intQiIndex[instruction.r1])
                    {
                        instruction.iOp1 = registers.intQi[instruction.r1];
                        instruction.dependantOpID1 = registers.intQiIndex[instruction.r1];
                    }
                    else
                    {
                        instruction.iOp1 = "";
                        instruction.dependantOpID1 = -1;
                    }
                    break;


            }
            return instruction;
        }


        /// <summary>Generates the assembly.</summary>
        /// <param name="IM">The instruction memory</param>
        /// <exception cref="System.NotImplementedException"></exception>
        private void generateAssembly(ref StringBuilder assemblyString, InstructionMemory IM)
        {
            Instruction instruction = new Instruction();
            while(IM.ProgramCounter < IM.instructions.Count)
            {
                instruction = fetch.getNextInstruction(ref IM);
                CU.decode(ref IM, ref instruction);
                //instructionQueue.Add(instruction);  //Fill up instruction queue
                printer.buildAssemblyString(ref assemblyString, ref instruction); //Prints assembly string to the GUI
            }
        }


        public void clearDynamicPipeline()
        {
            instructionsInFlight.Clear();
            instructionQueue.Clear();
            intAddFu.instruction = null;
            intSubFu.instruction = null;
            intMulFu.instruction = null;
            intDivFu.instruction = null;
            flAddFu.instruction = null;
            flSubFu.instruction = null;
            flMultFu.instruction = null;
            flDivFu.instruction = null;
            bitFu.instruction = null;
            memoryFu.instruction = null;
            branchFu.instruction = null;
            shiftFu.instruction = null;
            intAddRS.instruction = null;
            intSubRS.instruction = null;
            intMultRS.instruction = null;
            intDivRS.instruction = null;
            floatAddRS.instruction = null;
            floatSubRS.instruction = null;
            floatMultRS.instruction = null;
            floatDivRS.instruction = null;
            bitwiseOPRS.instruction = null;
            load_storeBuffer.instruction = null;
            branchOPS.instruction = null;
            shiftOPS.instruction = null;

            intAddRS.Busy = false;
            intSubRS.Busy = false;
            intMultRS.Busy = false;
            intDivRS.Busy = false;
            floatAddRS.Busy = false;
            floatSubRS.Busy = false;
            floatMultRS.Busy = false;
            floatDivRS.Busy = false;
            bitwiseOPRS.Busy = false;
            load_storeBuffer.Busy = false;
            branchOPS.Busy = false;
            shiftOPS.Busy = false;
            commonDataBus.CDB.Clear();
            commonDataBus.index.Clear();
            commonDataBus.IDIndex.Clear();
            reorderBuffer.reorderBuffer.Clear();
            justCommitedInstruction = null;
            instructionID = 1;
            cycleCount = 0;

            fetchInstruction = new Instruction();
            totalCyclesStalled = 0;
            totalHazard = 0;
            structuralHazard = 0;
            dataHazard = 0;
            controlHazard = 0;
            numOfInstructionsInExecution = 0;
            lastBranchDecision = false;
            doneExecuting = false;
            executionInProgress = false;
            haltFound = false;
            commitedThisCycle = false;
            reorderBuffer.reorderIndex = 1;
            executiveBranchDecision = false;
            twoBitPredictionCounter = 0;
        }
    }
}
