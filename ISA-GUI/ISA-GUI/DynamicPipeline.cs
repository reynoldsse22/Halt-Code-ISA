using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ISA_GUI
{
    internal class DynamicPipeline
    {
        public DataCache DC;
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

        public IntAddFU[] intAddFUs;
        public IntSubFU[] intSubFUs;
        public IntMultFU[] intMultFUs;
        public IntDivFU[] intDivFUs;
        public FloatAddFU[] flAddFUs;
        public FloatSubFU[] flSubFUs;
        public FloatMultFU[] flMultFUs;
        public FloatDivFU[] flDivFUs;
        public BitwiseOPFU[] bitFUs;
        public MemoryUnit[] memoryFUs;
        public BranchFU[] branchFUs;
        public ShiftFU[] shiftFUs;

        public ReservationStation[] intAddRSs;
        public ReservationStation[] intSubRSs;
        public ReservationStation[] intMultRSs;
        public ReservationStation[] intDivRSs;
        public ReservationStation[] floatAddRSs;
        public ReservationStation[] floatSubRSs;
        public ReservationStation[] floatMultRSs;
        public ReservationStation[] floatDivRSs;
        public ReservationStation[] bitwiseRSs;
        public ReservationStation[] branchRSs;
        public ReservationStation[] shiftRSs;
        public ReservationStation[] loadStoreBuffer;
        public Instruction fetchInstruction;

        public int totalHazard, structuralHazard, dataHazard, controlHazard, RAW, WAR, WAW;
        public int reorderBufferDelay, reservationStationDelay, trueDependenceDelay, totalDelays, instructionID;
        public int totalCyclesStalled, numOfInstructionsInExecution, twoBitPredictionCounter;
        public bool lastBranchDecision, executiveBranchDecision, doneExecuting, executionInProgress, haltFound, commitedThisCycle, branchDecoded, programRanAtLeastOnce;
        string result;



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

            fetchInstruction = new Instruction();
            cycleCount = 0;
            totalCyclesStalled = 0;
            totalHazard = 0;
            structuralHazard = 0;
            dataHazard = 0;
            controlHazard = 0;
            reservationStationDelay = 0;
            trueDependenceDelay = 0;
            reorderBufferDelay = 0;
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
            programRanAtLeastOnce = false;
            result = "";
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
        public void runCycle(List<string> input, bool stepThrough, ref StringBuilder assemblyString, ref StringBuilder decodedString, ref StringBuilder cacheString, ref StringBuilder pipelineString, 
            ref bool halted, ref ConfigCycle config, ref InstructionMemory IM, ref RegisterFile registers, ref DataMemory dataMemory)
        {
            programRanAtLeastOnce = true;
            if (cycleCount == 0)
            {
                DC = new DataCache(config);
                initializeFUs(config);
            }

            do
            {
                setCycleLimit();

                commitedThisCycle = false;
                startOfLoop:
                cycleCount++;
                reorderBuffer.oneCommitPerCycle = true; //Control booleans to make sure stages only run once per cycle

                if (halted)
                {
                    clearDynamicPipeline();
                    return;
                }

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

                            int instructionIndex = reorderBuffer.checkCommit(inst, ref WR, ref dataMemory, ref lastBranchDecision, ref IM, 
                                    ref registers, ref haltFound, ref commonDataBus, ref DC, ref cacheString, ref memoryFUs, ref config);

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
                            if(inst.opcode == 10)
                            {
                                printer.buildCacheDataString(ref cacheString, inst, ref DC);
                                if (memoryFUs[inst.functionalUnitIndex].instruction.doneExecuting)
                                {
                                    clearFU(inst);
                                }
                            }
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
                            memoryFUs[inst.functionalUnitIndex].instruction.doneExecuting = false;
                            AM.accessMemoryDynamic(ref dataMemory, ref registers, inst, ref config, out result, ref memoryFUs[inst.functionalUnitIndex], ref DC);
                            if (!result.Equals(""))
                            {
                                inst.result = result;
                            }
                            inst.ASPR = memoryFUs[inst.functionalUnitIndex].instruction.ASPR;
                           // inst.stage2End = cycleCount - 1;        //End of the second stage
                           // inst.stage3Start = cycleCount;          //Start of the third
                            
                            if (memoryFUs[inst.functionalUnitIndex].instruction.doneExecuting)
                            {
                                numOfInstructionsInExecution--;
                                inst.stage3CycleEnd = cycleCount;
                                inst.stage = 4;
                                if (inst.opcode == 9)
                                {
                                    printer.buildCacheDataString(ref cacheString, inst, ref DC);
                                }
                                if(inst.opcode == 10)
                                {
                                    memoryFUs[inst.functionalUnitIndex].instruction.doneExecuting = false;
                                    memoryFUs[inst.functionalUnitIndex].instruction.executionInProgress = false;
                                    inst.stage = 5;
                                }
                            }
                            break;

                        //Send the instruction to its corresponding functional unit as long as there are no structural harzards present
                        //Open up reservation station to allow for more instructions to flow in
                        //Execute within the functional unit
                        case 2:
                            numOfInstructionsInExecution++;
                            Instruction executeInstruction = execute(inst, ref registers, ref dataMemory, ref IM, ref config, ref alu, ref result);
                            if(inst.doneExecuting)
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
                                if (inst.opcode == 0 || inst.opcode == 1 || (inst.opcode >= 2 && inst.opcode <= 8))
                                    inst.stage = 5;
                                else if (inst.opcode >= 9 && inst.opcode <= 12)
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


        private void initializeFUs(ConfigCycle config)
        {
            intAddRSs = new ReservationStation[config.intAddFUs];
            intSubRSs = new ReservationStation[config.intSubFUs];
            intMultRSs = new ReservationStation[config.intMultFUs];
            intDivRSs = new ReservationStation[config.intDivFus];
            floatAddRSs = new ReservationStation[config.flAddFUs];
            floatSubRSs = new ReservationStation[config.flSubFUs];
            floatMultRSs = new ReservationStation[config.flMultFUs];
            floatDivRSs = new ReservationStation[config.flDivFUs];
            branchRSs = new ReservationStation[config.branchFUs];
            bitwiseRSs = new ReservationStation[config.bitwiseFUs];
            shiftRSs = new ReservationStation[config.shiftFUs];

            intAddFUs = new IntAddFU[config.intAddFUs];
            for(int i = 0; i < config.intAddFUs; i++)
            {
                intAddFUs[i] = new IntAddFU("intAddFU"+i);
                intAddFUs[i].indexer = i;
                intAddFUs[i].instruction = null;
                intAddRSs[i] = new ReservationStation("intAddRS"+i);
                intAddRSs[i].instruction = null;
                intAddRSs[i].arrayIndex = i;
            }

            intSubFUs = new IntSubFU[config.intSubFUs];
            for (int i = 0; i < config.intSubFUs; i++)
            {
                intSubFUs[i] = new IntSubFU("intSubFU" + i);
                intSubFUs[i].indexer = i;
                intSubFUs[i].instruction = null;
                intSubRSs[i] = new ReservationStation("intSubRS" + i);
                intSubRSs[i].instruction = null;
                intSubRSs[i].arrayIndex = i;
            }

            intMultFUs = new IntMultFU[config.intMultFUs];
            for (int i = 0; i < config.intMultFUs; i++)
            {
                intMultFUs[i] = new IntMultFU("intMulFU" + i);
                intMultFUs[i].indexer = i;
                intMultFUs[i].indexer = i;
                intMultRSs[i] = new ReservationStation("intMulRS" + i);
                intMultRSs[i].instruction = null;
                intMultRSs[i].arrayIndex = i;
            }

            intDivFUs = new IntDivFU[config.intDivFus];
            for (int i = 0; i < config.intDivFus; i++)
            {
                intDivFUs[i] = new IntDivFU("intDivFU" + i);
                intDivFUs[i].indexer = i;
                intDivFUs[i].instruction = null;
                intDivRSs[i] = new ReservationStation("intDivRS" + i);
                intDivRSs[i].instruction = null;
                intDivRSs[i].arrayIndex = i;
            }

            flAddFUs = new FloatAddFU[config.flAddFUs];
            for (int i = 0; i < config.flAddFUs; i++)
            {
                flAddFUs[i] = new FloatAddFU("intAddFU" + i);
                flAddFUs[i].indexer = i;
                flAddFUs[i].instruction = null;
                floatAddRSs[i] = new ReservationStation("flAddRS" + i);
                floatAddRSs[i].instruction = null;
                floatAddRSs[i].arrayIndex = i;
            }

            flSubFUs = new FloatSubFU[config.flSubFUs];
            for (int i = 0; i < config.flSubFUs; i++)
            {
                flSubFUs[i] = new FloatSubFU("flSubFU" + i);
                flSubFUs[i].indexer = i;
                flSubFUs[i].instruction = null;
                floatSubRSs[i] = new ReservationStation("flSubRS" + i);
                floatSubRSs[i].instruction = null;
                floatSubRSs[i].arrayIndex = i;
            }

            flMultFUs = new FloatMultFU[config.flMultFUs];
            for (int i = 0; i < config.flMultFUs; i++)
            {
                flMultFUs[i] = new FloatMultFU("flMulFU" + i);
                flMultFUs[i].indexer = i;
                flMultFUs[i].instruction = null;
                floatMultRSs[i] = new ReservationStation("flMulRS" + i);
                floatMultRSs[i].arrayIndex = i;
                floatMultRSs[i].instruction = null;
            }

            flDivFUs = new FloatDivFU[config.flDivFUs];
            for (int i = 0; i < config.flDivFUs; i++)
            {
                flDivFUs[i] = new FloatDivFU("flDivFU" + i);
                flDivFUs[i].indexer = i;
                floatDivRSs[i] = new ReservationStation("flDivRS" + i);
                floatDivRSs[i].arrayIndex = i;
            }

            bitFUs = new BitwiseOPFU[config.bitwiseFUs];
            for (int i = 0; i < config.bitwiseFUs; i++)
            {
                bitFUs[i] = new BitwiseOPFU("bitwiseFU" + i);
                bitFUs[i].indexer = i;
                bitFUs[i].instruction = null;
                bitwiseRSs[i] = new ReservationStation("bitwiseRS" + i);
                bitwiseRSs[i].arrayIndex = i;
                bitwiseRSs[i].instruction = null;
            }

            branchFUs = new BranchFU[config.branchFUs];
            for (int i = 0; i < config.branchFUs; i++)
            {
                branchFUs[i] = new BranchFU("branchFU" + i);
                branchFUs[i].indexer = i;
                branchFUs[i].instruction = null;
                branchRSs[i] = new ReservationStation("branchRS" + i);
                branchRSs[i].arrayIndex = i;
                branchRSs[i].instruction = null;
            }

            shiftFUs = new ShiftFU[config.shiftFUs];
            for (int i = 0; i < config.shiftFUs; i++)
            {
                shiftFUs[i] = new ShiftFU("shiftFU" + i);
                shiftFUs[i].indexer = i;
                shiftFUs[i].instruction = null;
                shiftRSs[i] = new ReservationStation("shiftRS" + i);
                shiftRSs[i].arrayIndex = i;
                shiftRSs[i].instruction = null;
            }

            memoryFUs = new MemoryUnit[config.memoryFUs];
            for (int i = 0; i < config.memoryFUs; i++)
            {
                memoryFUs[i] = new MemoryUnit("memoryFU" + i);
                memoryFUs[i].instruction = null;
                memoryFUs[i].indexer = i;
            }

            loadStoreBuffer = new ReservationStation[config.memoryFUs + 5];
            for (int i = 0; i < (config.memoryFUs + 5); i++)
            {
                loadStoreBuffer[i] = new ReservationStation("load_storeBuffer" + i);
                loadStoreBuffer[i].Busy = false;
                loadStoreBuffer[i].instruction = null;
                loadStoreBuffer[i].arrayIndex = i;
            }
        }

        private void setCycleLimit()
        {
            foreach (IntAddFU fu in intAddFUs)
                fu.oneCycleFU = true;
            foreach (IntSubFU fu in intSubFUs)
                fu.oneCycleFU = true;
            foreach (IntMultFU fu in intMultFUs)
                fu.oneCycleFU = true;
            foreach (IntDivFU fu in intDivFUs)
                fu.oneCycleFU = true;
            foreach (FloatAddFU fu in flAddFUs)
                fu.oneCycleFU = true;
            foreach (FloatSubFU fu in flSubFUs)
                fu.oneCycleFU = true;
            foreach (FloatMultFU fu in flMultFUs)
                fu.oneCycleFU = true;
            foreach (FloatDivFU fu in flDivFUs)
                fu.oneCycleFU = true;
            foreach (BitwiseOPFU fu in bitFUs)
                fu.oneCycleFU = true;
            foreach (BranchFU fu in branchFUs)
                fu.oneCycleFU = true;
            foreach (ShiftFU fu in shiftFUs)
                fu.oneCycleFU = true;
            foreach (MemoryUnit fu in memoryFUs)
                fu.oneCycleFU = true;
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
                    sendToFU(ref instruction, ref config);
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
                    if(!intAddFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished && !intAddFUs[instruction.functionalUnitIndex].instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref intAddFUs[instruction.functionalUnitIndex].instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        intAddFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished = true;
                        intAddFUs[instruction.functionalUnitIndex].instruction.executionInProgress = true;
                        intAddFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    }
                    else
                        intAddFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    if (intAddFUs[instruction.functionalUnitIndex].instruction.cycleControl == 0)
                    {
                        intAddFUs[instruction.functionalUnitIndex].oneCycleFU = false;            //Makes sure this Functional Unit is only available for one cycle
                        intAddFUs[instruction.functionalUnitIndex].instruction.doneExecuting = true;
                        intAddFUs[instruction.functionalUnitIndex].instruction.executionInProgress = false;
                        result = intAddFUs[instruction.functionalUnitIndex].instruction.result;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        intAddFUs[instruction.functionalUnitIndex].instruction = null;
                        return instruction;
                    }
                    break;
                case 2:
                    if (!intSubFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished && !intSubFUs[instruction.functionalUnitIndex].instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref intSubFUs[instruction.functionalUnitIndex].instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        intSubFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished = true;
                        intSubFUs[instruction.functionalUnitIndex].instruction.executionInProgress = true;
                        intSubFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    }
                    else
                        intSubFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    if (intSubFUs[instruction.functionalUnitIndex].instruction.cycleControl == 0)
                    {
                        result = intSubFUs[instruction.functionalUnitIndex].instruction.result;
                        intSubFUs[instruction.functionalUnitIndex].oneCycleFU = false;        //Makes sure this Functional Unit is only available for one cycle
                        intSubFUs[instruction.functionalUnitIndex].instruction.doneExecuting = true;
                        intSubFUs[instruction.functionalUnitIndex].instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        intSubFUs[instruction.functionalUnitIndex].instruction = null;
                        return instruction;
                    }
                    break;
                case 3:
                    if (!intMultFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished && !intMultFUs[instruction.functionalUnitIndex].instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref intMultFUs[instruction.functionalUnitIndex].instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        intMultFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished = true;
                        intMultFUs[instruction.functionalUnitIndex].instruction.executionInProgress = true;
                        intMultFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    }
                    else
                        intMultFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    if (intMultFUs[instruction.functionalUnitIndex].instruction.cycleControl == 0)
                    {
                        result = intMultFUs[instruction.functionalUnitIndex].instruction.result;
                        intMultFUs[instruction.functionalUnitIndex].oneCycleFU = false;       //Makes sure this Functional Unit is only available for one cycle
                        intMultFUs[instruction.functionalUnitIndex].instruction.doneExecuting = true;
                        intMultFUs[instruction.functionalUnitIndex].instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        intMultFUs[instruction.functionalUnitIndex].instruction = null;
                        return instruction;
                    }
                    break;
                case 4:
                    if (!intDivFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished && !intDivFUs[instruction.functionalUnitIndex].instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref intDivFUs[instruction.functionalUnitIndex].instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        intDivFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished = true;
                        intDivFUs[instruction.functionalUnitIndex].instruction.executionInProgress = true;
                        intDivFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    }
                    else
                        intDivFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    if (intDivFUs[instruction.functionalUnitIndex].instruction.cycleControl == 0)
                    {
                        result = intDivFUs[instruction.functionalUnitIndex].instruction.result;
                        intDivFUs[instruction.functionalUnitIndex].oneCycleFU = false;        //Makes sure this Functional Unit is only available for one cycle
                        intDivFUs[instruction.functionalUnitIndex].instruction.doneExecuting = true;
                        intDivFUs[instruction.functionalUnitIndex].instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        intDivFUs[instruction.functionalUnitIndex].instruction = null;
                        return instruction;
                    }
                    break;
                case 5:
                    if (!flAddFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished && !flAddFUs[instruction.functionalUnitIndex].instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref flAddFUs[instruction.functionalUnitIndex].instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        flAddFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished = true;
                        flAddFUs[instruction.functionalUnitIndex].instruction.executionInProgress = true;
                        flAddFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    }
                    else
                        flAddFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    if (flAddFUs[instruction.functionalUnitIndex].instruction.cycleControl == 0)
                    {
                        result = flAddFUs[instruction.functionalUnitIndex].instruction.result;
                        flAddFUs[instruction.functionalUnitIndex].oneCycleFU = false;      //Makes sure this Functional Unit is only available for one cycle
                        flAddFUs[instruction.functionalUnitIndex].instruction.doneExecuting = true;
                        flAddFUs[instruction.functionalUnitIndex].instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        flAddFUs[instruction.functionalUnitIndex].instruction = null;
                        return instruction;
                    }
                    break;
                case 6:
                    if (!flSubFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished && !flSubFUs[instruction.functionalUnitIndex].instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref flSubFUs[instruction.functionalUnitIndex].instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        flSubFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished = true;
                        flSubFUs[instruction.functionalUnitIndex].instruction.executionInProgress = true;
                        flSubFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    }
                    else
                        flSubFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    if (flSubFUs[instruction.functionalUnitIndex].instruction.cycleControl == 0)
                    {
                        result = flSubFUs[instruction.functionalUnitIndex].instruction.result;
                        flSubFUs[instruction.functionalUnitIndex].oneCycleFU = false;      //Makes sure this Functional Unit is only available for one cycle
                        flSubFUs[instruction.functionalUnitIndex].instruction.doneExecuting = true;
                        flSubFUs[instruction.functionalUnitIndex].instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        flSubFUs[instruction.functionalUnitIndex].instruction = null;
                        return instruction;
                    }
                    break;
                case 7:
                    if (!flMultFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished && !flMultFUs[instruction.functionalUnitIndex].instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref flMultFUs[instruction.functionalUnitIndex].instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        flMultFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished = true;
                        flMultFUs[instruction.functionalUnitIndex].instruction.executionInProgress = true;
                        flMultFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    }
                    else
                        flMultFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    if (flMultFUs[instruction.functionalUnitIndex].instruction.cycleControl == 0)
                    {
                        result = flMultFUs[instruction.functionalUnitIndex].instruction.result;
                        flMultFUs[instruction.functionalUnitIndex].oneCycleFU = false;     //Makes sure this Functional Unit is only available for one cycle
                        flMultFUs[instruction.functionalUnitIndex].instruction.doneExecuting = true;
                        flMultFUs[instruction.functionalUnitIndex].instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        flMultFUs[instruction.functionalUnitIndex].instruction = null;
                        return instruction;
                    }
                    break;
                case 8:
                    if (!flDivFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished && !flDivFUs[instruction.functionalUnitIndex].instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref flDivFUs[instruction.functionalUnitIndex].instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        flDivFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished = true;
                        flDivFUs[instruction.functionalUnitIndex].instruction.executionInProgress = true;
                        flDivFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    }
                    else
                        flDivFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    if (flDivFUs[instruction.functionalUnitIndex].instruction.cycleControl == 0)
                    {
                        result = flDivFUs[instruction.functionalUnitIndex].instruction.result;
                        flDivFUs[instruction.functionalUnitIndex].oneCycleFU = false;      //Makes sure this Functional Unit is only available for one cycle
                        flDivFUs[instruction.functionalUnitIndex].instruction.doneExecuting = true;
                        flDivFUs[instruction.functionalUnitIndex].instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        flDivFUs[instruction.functionalUnitIndex].instruction = null;
                        return instruction;
                    }
                    break;
                case 9:
                    if (!bitFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished && !bitFUs[instruction.functionalUnitIndex].instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref bitFUs[instruction.functionalUnitIndex].instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        bitFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished = true;
                        bitFUs[instruction.functionalUnitIndex].instruction.executionInProgress = true;
                        bitFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    }
                    else
                        bitFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    if (bitFUs[instruction.functionalUnitIndex].instruction.cycleControl == 0)
                    {
                        result = bitFUs[instruction.functionalUnitIndex].instruction.result;
                        bitFUs[instruction.functionalUnitIndex].oneCycleFU = false;     //Makes sure this Functional Unit is only available for one cycle
                        bitFUs[instruction.functionalUnitIndex].instruction.doneExecuting = true;
                        bitFUs[instruction.functionalUnitIndex].instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        bitFUs[instruction.functionalUnitIndex].instruction = null;
                        return instruction;
                    }
                    break;
                case 10:
                    if (!memoryFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished && !memoryFUs[instruction.functionalUnitIndex].instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref memoryFUs[instruction.functionalUnitIndex].instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        memoryFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished = true;
                        memoryFUs[instruction.functionalUnitIndex].instruction.executionInProgress = true;
                        memoryFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    }
                    else
                        memoryFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    if (memoryFUs[instruction.functionalUnitIndex].instruction.cycleControl == 0)
                    {
                        result = memoryFUs[instruction.functionalUnitIndex].instruction.result;
                        memoryFUs[instruction.functionalUnitIndex].oneCycleFU = false;        //Makes sure this Functional Unit is only available for one cycle
                        memoryFUs[instruction.functionalUnitIndex].instruction.doneExecuting = true;
                        memoryFUs[instruction.functionalUnitIndex].instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        return instruction;
                    }
                    break;
                case 11:
                    if (!branchFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished && !branchFUs[instruction.functionalUnitIndex].instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref branchFUs[instruction.functionalUnitIndex].instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        branchFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished = true;
                        branchFUs[instruction.functionalUnitIndex].instruction.executionInProgress = true;
                        branchFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    }
                    else
                        branchFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    if (branchFUs[instruction.functionalUnitIndex].instruction.cycleControl == 0)
                    {
                        result = branchFUs[instruction.functionalUnitIndex].instruction.result;
                        branchFUs[instruction.functionalUnitIndex].oneCycleFU = false;        //Makes sure this Functional Unit is only available for one cycle
                        branchFUs[instruction.functionalUnitIndex].instruction.doneExecuting = true;
                        branchFUs[instruction.functionalUnitIndex].instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        branchFUs[instruction.functionalUnitIndex].instruction = null;
                        return instruction;
                    }
                    break;
                case 12:
                    if (!shiftFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished && !shiftFUs[instruction.functionalUnitIndex].instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref shiftFUs[instruction.functionalUnitIndex].instruction, ref config, out result);
                        instruction.doneExecuting = false;
                        instruction.executionInProgress = true;
                        instruction.stage2ExecutionFinished = true;
                        shiftFUs[instruction.functionalUnitIndex].instruction.stage2ExecutionFinished = true;
                        shiftFUs[instruction.functionalUnitIndex].instruction.executionInProgress = true;
                        shiftFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    }
                    else
                        shiftFUs[instruction.functionalUnitIndex].instruction.cycleControl--;
                    if (shiftFUs[instruction.functionalUnitIndex].instruction.cycleControl == 0)
                    {
                        result = shiftFUs[instruction.functionalUnitIndex].instruction.result;
                        shiftFUs[instruction.functionalUnitIndex].oneCycleFU = false;         //Makes sure this Functional Unit is only available for one cycle
                        shiftFUs[instruction.functionalUnitIndex].instruction.doneExecuting = true;
                        shiftFUs[instruction.functionalUnitIndex].instruction.executionInProgress = false;
                        instruction.doneExecuting = true;
                        instruction.executionInProgress = false;
                        shiftFUs[instruction.functionalUnitIndex].instruction = null;
                        return instruction;
                    }
                    break;
            }
            return instruction;
        }

        private void sendToFU(ref Instruction instruction, ref ConfigCycle config)
        {
            if (instruction.opcode == 0 || instruction.opcode == 1)
                return;

            switch(instruction.functionalUnitID)
            {
                case 1:
                    for(int i = 0; i < config.intAddFUs; i++)
                    {
                        if (intAddFUs[i].instruction == null && intAddFUs[i].oneCycleFU)
                        {
                            intAddFUs[i].instruction = intAddRSs[instruction.reservationStationIndex].instruction;
                            intAddFUs[i].instruction.functionalUnitIndex = i;
                            intAddRSs[instruction.reservationStationIndex].instruction = null;
                            intAddRSs[instruction.reservationStationIndex].Busy = false;
                            return;
                        }
                    }
                    break;
                case 2:
                    for (int i = 0; i < config.intSubFUs; i++)
                    {
                        if (intSubFUs[i].instruction == null && intSubFUs[i].oneCycleFU)
                        {
                            intSubFUs[i].instruction = intSubRSs[instruction.reservationStationIndex].instruction;
                            intSubFUs[i].instruction.functionalUnitIndex = i;
                            intSubRSs[instruction.reservationStationIndex].instruction = null;
                            intSubRSs[instruction.reservationStationIndex].Busy = false;
                            return;
                        }
                    }
                    break;
                case 3:
                    for (int i = 0; i < config.intMultFUs; i++)
                    {
                        if (intMultFUs[i].instruction == null && intMultFUs[i].oneCycleFU)
                        {
                            intMultFUs[i].instruction = intMultRSs[instruction.reservationStationIndex].instruction;
                            intMultFUs[i].instruction.functionalUnitIndex = i;
                            intMultRSs[instruction.reservationStationIndex].instruction = null;
                            intMultRSs[instruction.reservationStationIndex].Busy = false;
                            return;
                        }
                    }
                    break;
                case 4:
                    for (int i = 0; i < config.intDivFus; i++)
                    {
                        if (intDivFUs[i].instruction == null && intDivFUs[i].oneCycleFU)
                        {
                            intDivFUs[i].instruction = intDivRSs[instruction.reservationStationIndex].instruction;
                            intDivFUs[i].instruction.functionalUnitIndex = i;
                            intDivRSs[instruction.reservationStationIndex].instruction = null;
                            intDivRSs[instruction.reservationStationIndex].Busy = false;
                            return;
                        }
                    }
                    break;
                case 5:
                    for (int i = 0; i < config.flAddFUs; i++)
                    {
                        if (flAddFUs[i].instruction == null && flAddFUs[i].oneCycleFU)
                        {
                            flAddFUs[i].instruction = floatAddRSs[instruction.reservationStationIndex].instruction;
                            flAddFUs[i].instruction.functionalUnitIndex = i;
                            floatAddRSs[instruction.reservationStationIndex].instruction = null;
                            floatAddRSs[instruction.reservationStationIndex].Busy = false;
                            return;
                        }
                    }
                    break;
                case 6:
                    for (int i = 0; i < config.flSubFUs; i++)
                    {
                        if (flSubFUs[i].instruction == null && flSubFUs[i].oneCycleFU)
                        {
                            flSubFUs[i].instruction = floatSubRSs[instruction.reservationStationIndex].instruction;
                            flSubFUs[i].instruction.functionalUnitIndex = i;
                            floatSubRSs[instruction.reservationStationIndex].instruction = null;
                            floatSubRSs[instruction.reservationStationIndex].Busy = false;
                            return;
                        }
                    }
                    break;
                case 7:
                    for (int i = 0; i < config.flMultFUs; i++)
                    {
                        if (flMultFUs[i].instruction == null && flSubFUs[i].oneCycleFU)
                        {
                            flMultFUs[i].instruction = floatMultRSs[instruction.reservationStationIndex].instruction;
                            flMultFUs[i].instruction.functionalUnitIndex = i;
                            floatMultRSs[instruction.reservationStationIndex].instruction = null;
                            floatMultRSs[instruction.reservationStationIndex].Busy = false;
                            return;
                        }
                    }
                    break;
                case 8:
                    for (int i = 0; i < config.flDivFUs; i++)
                    {
                        if (flDivFUs[i].instruction == null && flDivFUs[i].oneCycleFU)
                        {
                            flDivFUs[i].instruction = floatDivRSs[instruction.reservationStationIndex].instruction;
                            flDivFUs[i].instruction.functionalUnitIndex = i;
                            floatDivRSs[instruction.reservationStationIndex].instruction = null;
                            floatDivRSs[instruction.reservationStationIndex].Busy = false;
                            return;
                        }
                    }
                    break;
                case 9:
                    for (int i = 0; i < config.bitwiseFUs; i++)
                    {
                        if (bitFUs[i].instruction == null && bitFUs[i].oneCycleFU)
                        {
                            bitFUs[i].instruction = bitwiseRSs[instruction.reservationStationIndex].instruction;
                            bitFUs[i].instruction.functionalUnitIndex = i;
                            bitwiseRSs[instruction.reservationStationIndex].instruction = null;
                            bitwiseRSs[instruction.reservationStationIndex].Busy = false;
                            return;
                        }
                    }
                    break;
                case 10:
                    for (int i = 0; i < config.memoryFUs; i++)
                    {
                        if (memoryFUs[i].instruction == null && memoryFUs[i].oneCycleFU)
                        {
                            memoryFUs[i].instruction = loadStoreBuffer[instruction.reservationStationIndex].instruction;
                            memoryFUs[i].instruction.functionalUnitIndex = i;
                            loadStoreBuffer[instruction.reservationStationIndex].instruction = null;
                            loadStoreBuffer[instruction.reservationStationIndex].Busy = false;
                            return;
                        }
                    }
                    break;
                case 11:
                    for (int i = 0; i < config.branchFUs; i++)
                    {
                        if (branchFUs[i].instruction == null && branchFUs[i].oneCycleFU)
                        {
                            branchFUs[i].instruction = branchRSs[instruction.reservationStationIndex].instruction;
                            branchFUs[i].instruction.functionalUnitIndex = i;
                            branchRSs[instruction.reservationStationIndex].instruction = null;
                            branchRSs[instruction.reservationStationIndex].Busy = false;
                            return;
                        }
                    }
                    break;
                case 12:
                    for (int i = 0; i < config.shiftFUs; i++)
                    {
                        if (shiftFUs[i].instruction == null && shiftFUs[i].oneCycleFU)
                        {
                            shiftFUs[i].instruction = shiftRSs[instruction.reservationStationIndex].instruction;
                            shiftFUs[i].instruction.functionalUnitIndex = i;
                            shiftRSs[instruction.reservationStationIndex].instruction = null;
                            shiftRSs[instruction.reservationStationIndex].Busy = false;
                            return;
                        }
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
                    checkOperandDependencies(ref memoryFUs[instruction.functionalUnitIndex].instruction, ref registers);
                    if (instruction.isFloat)
                    {
                        if (memoryFUs[instruction.functionalUnitIndex].instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(memoryFUs[instruction.functionalUnitIndex].instruction.fOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    memoryFUs[instruction.functionalUnitIndex].instruction.fOperand1 = float.Parse(CDB.CDB[memoryFUs[instruction.functionalUnitIndex].instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            memoryFUs[instruction.functionalUnitIndex].instruction.fOperand1 = registers.floatRegisters[0];
                    }
                    else
                    {
                        if (memoryFUs[instruction.functionalUnitIndex].instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(memoryFUs[instruction.functionalUnitIndex].instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    memoryFUs[instruction.functionalUnitIndex].instruction.iOperand1 = int.Parse(CDB.CDB[memoryFUs[instruction.functionalUnitIndex].instruction.iOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            memoryFUs[instruction.functionalUnitIndex].instruction.iOperand1 = registers.intRegisters[0];
                    }
                    break;
                case 13:
                    if (instruction.isFloat)
                    {
                        checkOperandDependencies(ref flSubFUs[instruction.functionalUnitIndex].instruction, ref registers);
                        if (flSubFUs[instruction.functionalUnitIndex].instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(flSubFUs[instruction.functionalUnitIndex].instruction.fOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    flSubFUs[instruction.functionalUnitIndex].instruction.fOperand1 = int.Parse(CDB.CDB[flSubFUs[instruction.functionalUnitIndex].instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flSubFUs[instruction.functionalUnitIndex].instruction.fOperand1 = registers.floatRegisters[flSubFUs[instruction.functionalUnitIndex].instruction.r3];
                    }
                    else
                    {
                        checkOperandDependencies(ref intSubFUs[instruction.functionalUnitIndex].instruction, ref registers);
                        if (intSubFUs[instruction.functionalUnitIndex].instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(intSubFUs[instruction.functionalUnitIndex].instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    intSubFUs[instruction.functionalUnitIndex].instruction.iOperand1 = int.Parse(CDB.CDB[intSubFUs[instruction.functionalUnitIndex].instruction.iOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intSubFUs[instruction.functionalUnitIndex].instruction.iOperand1 = registers.intRegisters[intSubFUs[instruction.functionalUnitIndex].instruction.r3];
                    }
                    break;
                case 14:
                    
                    if (instruction.isFloat)
                    {
                        checkOperandDependencies(ref flSubFUs[instruction.functionalUnitIndex].instruction, ref registers);
                        if (flSubFUs[instruction.functionalUnitIndex].instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(flSubFUs[instruction.functionalUnitIndex].instruction.fOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    flSubFUs[instruction.functionalUnitIndex].instruction.fOperand1 = int.Parse(CDB.CDB[flSubFUs[instruction.functionalUnitIndex].instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flSubFUs[instruction.functionalUnitIndex].instruction.fOperand1 = registers.floatRegisters[flSubFUs[instruction.functionalUnitIndex].instruction.r1];
                        if (flSubFUs[instruction.functionalUnitIndex].instruction.fOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(flSubFUs[instruction.functionalUnitIndex].instruction.fOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    flSubFUs[instruction.functionalUnitIndex].instruction.fOperand2 = float.Parse(CDB.CDB[flSubFUs[instruction.functionalUnitIndex].instruction.fOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flSubFUs[instruction.functionalUnitIndex].instruction.fOperand2 = registers.floatRegisters[flSubFUs[instruction.functionalUnitIndex].instruction.r2];
                    }
                    else
                    {
                        checkOperandDependencies(ref flSubFUs[instruction.functionalUnitIndex].instruction, ref registers);
                        if (intSubFUs[instruction.functionalUnitIndex].instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(intSubFUs[instruction.functionalUnitIndex].instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    intSubFUs[instruction.functionalUnitIndex].instruction.iOperand1 = int.Parse(CDB.CDB[intSubFUs[instruction.functionalUnitIndex].instruction.iOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intSubFUs[instruction.functionalUnitIndex].instruction.iOperand1 = registers.intRegisters[intSubFUs[instruction.functionalUnitIndex].instruction.r1];
                        if (intSubFUs[instruction.functionalUnitIndex].instruction.iOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(intSubFUs[instruction.functionalUnitIndex].instruction.iOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    intSubFUs[instruction.functionalUnitIndex].instruction.iOperand2 = int.Parse(CDB.CDB[intSubFUs[instruction.functionalUnitIndex].instruction.iOp2]);
                            }
                            else
                                trueDependenceDelay++;
                        }
                        else
                            intSubFUs[instruction.functionalUnitIndex].instruction.iOperand2 = registers.intRegisters[intSubFUs[instruction.functionalUnitIndex].instruction.r2];
                    }
                    break;
                case 15:
                    if (instruction.isFloat)
                    {
                        checkOperandDependencies(ref memoryFUs[instruction.functionalUnitIndex].instruction, ref registers);
                        if (memoryFUs[instruction.functionalUnitIndex].instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(memoryFUs[instruction.functionalUnitIndex].instruction.fOp1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    memoryFUs[instruction.functionalUnitIndex].instruction.fOperand1 = float.Parse(CDB.CDB[memoryFUs[instruction.functionalUnitIndex].instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            memoryFUs[instruction.functionalUnitIndex].instruction.fOperand1 = registers.floatRegisters[memoryFUs[instruction.functionalUnitIndex].instruction.r2];
                    }
                    else
                    {
                        checkOperandDependencies(ref memoryFUs[instruction.functionalUnitIndex].instruction, ref registers);
                        if (memoryFUs[instruction.functionalUnitIndex].instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(memoryFUs[instruction.functionalUnitIndex].instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    memoryFUs[instruction.functionalUnitIndex].instruction.iOperand1 = int.Parse(CDB.CDB[memoryFUs[instruction.functionalUnitIndex].instruction.iOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            memoryFUs[instruction.functionalUnitIndex].instruction.iOperand1 = registers.intRegisters[memoryFUs[instruction.functionalUnitIndex].instruction.r2];
                    }
                    break;
                case 16:
                case 17:
                case 18:
                case 19:
                    checkOperandDependencies(ref shiftFUs[instruction.functionalUnitIndex].instruction, ref registers);
                    if (shiftFUs[instruction.functionalUnitIndex].instruction.iOp1 != "")
                    {
                        if (CDB.CDB.ContainsKey(shiftFUs[instruction.functionalUnitIndex].instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                        {
                            if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                shiftFUs[instruction.functionalUnitIndex].instruction.iOperand1 = int.Parse(CDB.CDB[shiftFUs[instruction.functionalUnitIndex].instruction.iOp1]);
                        }
                        else
                        {
                            trueDependenceDelay++;
                            return true;
                        }
                    }
                    else
                        shiftFUs[instruction.functionalUnitIndex].instruction.iOperand1 = registers.intRegisters[shiftFUs[instruction.functionalUnitIndex].instruction.r1];
                    if (shiftFUs[instruction.functionalUnitIndex].instruction.iOp2 != "")
                    {
                        if (CDB.CDB.ContainsKey(shiftFUs[instruction.functionalUnitIndex].instruction.iOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                        {
                            if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                shiftFUs[instruction.functionalUnitIndex].instruction.iOperand2 = int.Parse(CDB.CDB[shiftFUs[instruction.functionalUnitIndex].instruction.iOp2]);
                        }
                        else
                        {
                            trueDependenceDelay++;
                            return true;
                        }
                    }
                    else
                        shiftFUs[instruction.functionalUnitIndex].instruction.iOperand2 = registers.intRegisters[shiftFUs[instruction.functionalUnitIndex].instruction.r2];
                    break;
                case 20:
                    if (instruction.isFloat)
                    {
                        checkOperandDependencies(ref flAddFUs[instruction.functionalUnitIndex].instruction, ref registers);
                        if (flAddFUs[instruction.functionalUnitIndex].instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(flAddFUs[instruction.functionalUnitIndex].instruction.fOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    flAddFUs[instruction.functionalUnitIndex].instruction.fOperand1 = float.Parse(CDB.CDB[flAddFUs[instruction.functionalUnitIndex].instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flAddFUs[instruction.functionalUnitIndex].instruction.fOperand1 = registers.floatRegisters[flAddFUs[instruction.functionalUnitIndex].instruction.r1];
                        if (flAddFUs[instruction.functionalUnitIndex].instruction.fOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(flAddFUs[instruction.functionalUnitIndex].instruction.fOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    flAddFUs[instruction.functionalUnitIndex].instruction.fOperand2 = float.Parse(CDB.CDB[flAddFUs[instruction.functionalUnitIndex].instruction.fOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flAddFUs[instruction.functionalUnitIndex].instruction.fOperand2 = registers.floatRegisters[flAddFUs[instruction.functionalUnitIndex].instruction.r2];
                    }
                    else
                    {
                        checkOperandDependencies(ref intAddFUs[instruction.functionalUnitIndex].instruction, ref registers);
                        if (intAddFUs[instruction.functionalUnitIndex].instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(intAddFUs[instruction.functionalUnitIndex].instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    intAddFUs[instruction.functionalUnitIndex].instruction.iOperand1 = int.Parse(CDB.CDB[intAddFUs[instruction.functionalUnitIndex].instruction.iOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intAddFUs[instruction.functionalUnitIndex].instruction.iOperand1 = registers.intRegisters[intAddFUs[instruction.functionalUnitIndex].instruction.r1];
                        if (intAddFUs[instruction.functionalUnitIndex].instruction.iOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(intAddFUs[instruction.functionalUnitIndex].instruction.iOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    intAddFUs[instruction.functionalUnitIndex].instruction.iOperand2 = int.Parse(CDB.CDB[intAddFUs[instruction.functionalUnitIndex].instruction.iOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intAddFUs[instruction.functionalUnitIndex].instruction.iOperand2 = registers.intRegisters[intAddFUs[instruction.functionalUnitIndex].instruction.r2];
                    }
                    break;
                case 21:
                    if (instruction.isFloat)
                    {
                        checkOperandDependencies(ref flSubFUs[instruction.functionalUnitIndex].instruction, ref registers);
                        if (flSubFUs[instruction.functionalUnitIndex].instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(flSubFUs[instruction.functionalUnitIndex].instruction.fOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    flSubFUs[instruction.functionalUnitIndex].instruction.fOperand1 = float.Parse(CDB.CDB[flSubFUs[instruction.functionalUnitIndex].instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flSubFUs[instruction.functionalUnitIndex].instruction.fOperand1 = registers.floatRegisters[flSubFUs[instruction.functionalUnitIndex].instruction.r1];
                        if (flSubFUs[instruction.functionalUnitIndex].instruction.fOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(flSubFUs[instruction.functionalUnitIndex].instruction.fOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    flSubFUs[instruction.functionalUnitIndex].instruction.fOperand2 = float.Parse(CDB.CDB[flSubFUs[instruction.functionalUnitIndex].instruction.fOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flSubFUs[instruction.functionalUnitIndex].instruction.fOperand2 = registers.floatRegisters[flSubFUs[instruction.functionalUnitIndex].instruction.r2];
                    }
                    else
                    {
                        checkOperandDependencies(ref intSubFUs[instruction.functionalUnitIndex].instruction, ref registers);
                        if (intSubFUs[instruction.functionalUnitIndex].instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(intSubFUs[instruction.functionalUnitIndex].instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    intSubFUs[instruction.functionalUnitIndex].instruction.iOperand1 = int.Parse(CDB.CDB[intSubFUs[instruction.functionalUnitIndex].instruction.iOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intSubFUs[instruction.functionalUnitIndex].instruction.iOperand1 = registers.intRegisters[intSubFUs[instruction.functionalUnitIndex].instruction.r1];
                        if (intSubFUs[instruction.functionalUnitIndex].instruction.iOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(intSubFUs[instruction.functionalUnitIndex].instruction.iOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    intSubFUs[instruction.functionalUnitIndex].instruction.iOperand2 = int.Parse(CDB.CDB[intSubFUs[instruction.functionalUnitIndex].instruction.iOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intSubFUs[instruction.functionalUnitIndex].instruction.iOperand2 = registers.intRegisters[intSubFUs[instruction.functionalUnitIndex].instruction.r2];
                    }
                    break;
                case 22:
                    if (instruction.isFloat)
                    {
                        checkOperandDependencies(ref flMultFUs[instruction.functionalUnitIndex].instruction, ref registers);
                        if (flMultFUs[instruction.functionalUnitIndex].instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(flMultFUs[instruction.functionalUnitIndex].instruction.fOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    flMultFUs[instruction.functionalUnitIndex].instruction.fOperand1 = float.Parse(CDB.CDB[flMultFUs[instruction.functionalUnitIndex].instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flMultFUs[instruction.functionalUnitIndex].instruction.fOperand1 = registers.floatRegisters[flMultFUs[instruction.functionalUnitIndex].instruction.r1];
                        if (flMultFUs[instruction.functionalUnitIndex].instruction.fOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(flMultFUs[instruction.functionalUnitIndex].instruction.fOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    flMultFUs[instruction.functionalUnitIndex].instruction.fOperand2 = float.Parse(CDB.CDB[flMultFUs[instruction.functionalUnitIndex].instruction.fOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flMultFUs[instruction.functionalUnitIndex].instruction.fOperand2 = registers.floatRegisters[flMultFUs[instruction.functionalUnitIndex].instruction.r2];
                    }
                    else
                    {
                        checkOperandDependencies(ref intMultFUs[instruction.functionalUnitIndex].instruction, ref registers);
                        if (intMultFUs[instruction.functionalUnitIndex].instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(intMultFUs[instruction.functionalUnitIndex].instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    intMultFUs[instruction.functionalUnitIndex].instruction.iOperand1 = int.Parse(CDB.CDB[intMultFUs[instruction.functionalUnitIndex].instruction.iOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intMultFUs[instruction.functionalUnitIndex].instruction.iOperand1 = registers.intRegisters[intMultFUs[instruction.functionalUnitIndex].instruction.r1];
                        if (intMultFUs[instruction.functionalUnitIndex].instruction.iOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(intMultFUs[instruction.functionalUnitIndex].instruction.iOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    intMultFUs[instruction.functionalUnitIndex].instruction.iOperand2 = int.Parse(CDB.CDB[intMultFUs[instruction.functionalUnitIndex].instruction.iOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intMultFUs[instruction.functionalUnitIndex].instruction.iOperand2 = registers.intRegisters[intMultFUs[instruction.functionalUnitIndex].instruction.r2];
                    }
                    break;
                case 23:
                    if(instruction.isFloat)
                    {
                        checkOperandDependencies(ref flDivFUs[instruction.functionalUnitIndex].instruction, ref registers);
                        if (flDivFUs[instruction.functionalUnitIndex].instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(flDivFUs[instruction.functionalUnitIndex].instruction.fOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    flDivFUs[instruction.functionalUnitIndex].instruction.fOperand1 = float.Parse(CDB.CDB[flDivFUs[instruction.functionalUnitIndex].instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flDivFUs[instruction.functionalUnitIndex].instruction.fOperand1 = registers.floatRegisters[flDivFUs[instruction.functionalUnitIndex].instruction.r1];
                        if (flDivFUs[instruction.functionalUnitIndex].instruction.fOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(flDivFUs[instruction.functionalUnitIndex].instruction.fOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    flDivFUs[instruction.functionalUnitIndex].instruction.fOperand2 = float.Parse(CDB.CDB[flDivFUs[instruction.functionalUnitIndex].instruction.fOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            flDivFUs[instruction.functionalUnitIndex].instruction.fOperand2 = registers.floatRegisters[flDivFUs[instruction.functionalUnitIndex].instruction.r2];
                    }
                    else
                    {
                        checkOperandDependencies(ref intDivFUs[instruction.functionalUnitIndex].instruction, ref registers);
                        if (intDivFUs[instruction.functionalUnitIndex].instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(intDivFUs[instruction.functionalUnitIndex].instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    intDivFUs[instruction.functionalUnitIndex].instruction.iOperand1 = int.Parse(CDB.CDB[intDivFUs[instruction.functionalUnitIndex].instruction.iOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intDivFUs[instruction.functionalUnitIndex].instruction.iOperand1 = registers.intRegisters[intDivFUs[instruction.functionalUnitIndex].instruction.r1];
                        if (intDivFUs[instruction.functionalUnitIndex].instruction.iOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(intDivFUs[instruction.functionalUnitIndex].instruction.iOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    intDivFUs[instruction.functionalUnitIndex].instruction.iOperand2 = int.Parse(CDB.CDB[intDivFUs[instruction.functionalUnitIndex].instruction.iOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intDivFUs[instruction.functionalUnitIndex].instruction.iOperand2 = registers.intRegisters[intDivFUs[instruction.functionalUnitIndex].instruction.r2];
                    }
                    break;
                case 24:
                case 25:
                case 26:
                    checkOperandDependencies(ref bitFUs[instruction.functionalUnitIndex].instruction, ref registers);
                    if (bitFUs[instruction.functionalUnitIndex].instruction.iOp1 != "")
                    {
                        if (CDB.CDB.ContainsKey(bitFUs[instruction.functionalUnitIndex].instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                        {
                            if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                bitFUs[instruction.functionalUnitIndex].instruction.iOperand1 = int.Parse(CDB.CDB[bitFUs[instruction.functionalUnitIndex].instruction.iOp1]);
                        }
                        else
                        {
                            trueDependenceDelay++;
                            return true;
                        }
                    }
                    else
                        bitFUs[instruction.functionalUnitIndex].instruction.iOperand1 = registers.intRegisters[bitFUs[instruction.functionalUnitIndex].instruction.r1];
                    if (bitFUs[instruction.functionalUnitIndex].instruction.iOp2 != "")
                    {
                        if (CDB.CDB.ContainsKey(bitFUs[instruction.functionalUnitIndex].instruction.iOp2) && CDB.index.ContainsKey(instruction.dependantOpID2))
                        {
                            if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                bitFUs[instruction.functionalUnitIndex].instruction.iOperand2 = int.Parse(CDB.CDB[bitFUs[instruction.functionalUnitIndex].instruction.iOp2]);
                        }
                        else
                        {
                            trueDependenceDelay++;
                            return true;
                        }
                    }
                    else
                        bitFUs[instruction.functionalUnitIndex].instruction.iOperand2 = registers.intRegisters[bitFUs[instruction.functionalUnitIndex].instruction.r2];
                    break;
                case 27:
                    checkOperandDependencies(ref bitFUs[instruction.functionalUnitIndex].instruction, ref registers);
                    if (bitFUs[instruction.functionalUnitIndex].instruction.iOp1 != "")
                    {
                        if (CDB.CDB.ContainsKey(bitFUs[instruction.functionalUnitIndex].instruction.iOp1) && CDB.index.ContainsKey(instruction.dependantOpID1))
                        {
                            if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                bitFUs[instruction.functionalUnitIndex].instruction.iOperand1 = int.Parse(CDB.CDB[bitFUs[instruction.functionalUnitIndex].instruction.iOp1]);
                        }
                        else
                        {
                            trueDependenceDelay++;
                            return true;
                        }
                    }
                    else
                        bitFUs[instruction.functionalUnitIndex].instruction.iOperand1 = registers.intRegisters[bitFUs[instruction.functionalUnitIndex].instruction.r1];
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
                    foreach(ReservationStation branch in branchRSs)
                    {
                        if (!branch.Busy)
                        {
                            branch.Busy = true;
                            instruction.functionalUnitID = 11;
                            instruction.reservationStationIndex = branch.arrayIndex;
                            branch.instruction = instruction;
                            return instruction;
                            //Vj, Vk, Qj, Qk will be implemented here
                        }
                    }
                    break;
                case 15:
                    if (instruction.isFloat)
                    {
                        foreach(ReservationStation loadstoreBuff in loadStoreBuffer)
                        {
                            if (!loadstoreBuff.Busy)
                            {
                                if (registers.floatQi[instruction.r3] == "0")
                                {
                                    checkOperandDependencies(ref instruction, ref registers);
                                    loadstoreBuff.Busy = true;
                                    registers.floatQi[instruction.r3] = "memoryFU";
                                    registers.floatQiIndex[instruction.r3] = instruction.ID;
                                    instruction.functionalUnitID = 10;
                                    instruction.reservationStationIndex = loadstoreBuff.arrayIndex;
                                    loadstoreBuff.instruction = instruction;
                                    return instruction;
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (ReservationStation loadstoreBuff in loadStoreBuffer)
                        {
                            if (!loadstoreBuff.Busy)
                            {
                                if (registers.intQi[instruction.r3] == "0")
                                {
                                    checkOperandDependencies(ref instruction, ref registers);
                                    loadstoreBuff.Busy = true;
                                    registers.intQi[instruction.r3] = "memoryFU";
                                    registers.intQiIndex[instruction.r3] = instruction.ID;
                                    instruction.functionalUnitID = 10;
                                    instruction.reservationStationIndex = loadstoreBuff.arrayIndex;
                                    loadstoreBuff.instruction = instruction;
                                    return instruction;
                                }
                            }
                        }
                    }
                    break;
                
                case 9:         //Load Instructions
                    foreach (ReservationStation loadstoreBuff in loadStoreBuffer)
                    {
                        if (!loadstoreBuff.Busy)
                        {
                            if (instruction.isFloat)
                            {
                                if (registers.floatQi[0] != "0")
                                    break;
                                registers.floatQi[0] = "memoryFU";
                                registers.floatQiIndex[0] = instruction.ID;
                            }
                            else
                            {
                                if (registers.intQi[0] != "0")
                                    break;
                                registers.intQi[0] = "memoryFU";
                                registers.intQiIndex[0] = instruction.ID;
                            }
                            loadstoreBuff.Busy = true;
                            instruction.functionalUnitID = 10;
                            instruction.reservationStationIndex = loadstoreBuff.arrayIndex;
                            loadstoreBuff.instruction = instruction;
                            return instruction;
                            //Vj, Vk, Qj, Qk will be implemented here
                        }
                    }
                    break;
                case 10:
                    foreach (ReservationStation loadstoreBuff in loadStoreBuffer)
                    {
                        if (!loadstoreBuff.Busy)
                        {
                            loadstoreBuff.Busy = true;
                            checkOperandDependencies(ref instruction, ref registers);
                            instruction.functionalUnitID = 10;
                            instruction.reservationStationIndex = loadstoreBuff.arrayIndex;
                            loadstoreBuff.instruction = instruction;
                            return instruction;
                            //Vj, Vk, Qj, Qk will be implemented here
                        }
                    }
                    break;
                case 11:
                    foreach (ReservationStation loadstoreBuff in loadStoreBuffer)
                    {
                        if (!loadstoreBuff.Busy)
                        {
                            if (instruction.isFloat)
                            {
                                if (registers.floatQi[instruction.r3] != "0")
                                    break;
                                registers.floatQi[instruction.r3] = "memoryFU";
                                registers.floatQiIndex[instruction.r3] = instruction.ID;
                            }
                            else
                            {
                                if (registers.intQi[instruction.r3] != "0")
                                    break;
                                registers.intQi[instruction.r3] = "memoryFU";
                                registers.intQiIndex[instruction.r3] = instruction.ID;
                            }
                            loadstoreBuff.Busy = true;
                            instruction.functionalUnitID = 10;
                            instruction.reservationStationIndex = loadstoreBuff.arrayIndex;
                            loadstoreBuff.instruction = instruction;
                            return instruction;
                            //Vj, Vk, Qj, Qk will be implemented here
                        }
                    }
                    break;

                case 12: 
                    if (instruction.isFloat)
                    {
                        foreach (ReservationStation loadstoreBuff in loadStoreBuffer)
                        {
                            if (!loadstoreBuff.Busy)
                            {
                                if (registers.floatQi[instruction.r3] == "0")
                                {
                                    checkOperandDependencies(ref instruction, ref registers);
                                    registers.floatQi[instruction.r3] = "memoryFU";
                                    registers.floatQiIndex[instruction.r3] = instruction.ID;
                                    loadstoreBuff.Busy = true;
                                    instruction.functionalUnitID = 10;
                                    instruction.reservationStationIndex = loadstoreBuff.arrayIndex;
                                    loadstoreBuff.instruction = instruction;
                                    return instruction;
                                    //Vj, Vk, Qj, Qk will be implemented here
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (ReservationStation loadstoreBuff in loadStoreBuffer)
                        {
                            if (!loadstoreBuff.Busy)
                            {
                                if (registers.intQi[instruction.r3] == "0")
                                {
                                    checkOperandDependencies(ref instruction, ref registers);
                                    loadstoreBuff.Busy = true;
                                    registers.intQi[instruction.r3] = "memoryFU";
                                    registers.intQiIndex[instruction.r3] = instruction.ID;
                                    instruction.functionalUnitID = 10;
                                    instruction.reservationStationIndex = loadstoreBuff.arrayIndex;
                                    loadstoreBuff.instruction = instruction;
                                    return instruction;
                                    //Vj, Vk, Qj, Qk will be implemented here
                                }         
                            }
                        }
                    }
                    break;
                case 13:
                case 14:
                    if (instruction.isFloat)
                    {
                        foreach (ReservationStation flSub in floatSubRSs)
                        {
                            if (!flSub.Busy && registers.floatQi[instruction.r3] == "0")
                            {
                                checkOperandDependencies(ref instruction, ref registers);
                                flSub.Busy = true;
                                instruction.functionalUnitID = 2;
                                instruction.reservationStationIndex = flSub.arrayIndex;
                                flSub.instruction = instruction;
                                return instruction;
                            }
                        }
                    }
                    else
                    {
                        foreach (ReservationStation intSub in intSubRSs)
                        {
                            if (!intSub.Busy && registers.intQi[instruction.r3] == "0")
                            {
                                checkOperandDependencies(ref instruction, ref registers);
                                intSub.Busy = true;
                                instruction.functionalUnitID = 2;
                                instruction.reservationStationIndex = intSub.arrayIndex;
                                intSub.instruction = instruction;
                                return instruction;
                            }
                        }
                    }
                    break;
                case 16:
                case 17:
                case 18:
                case 19:
                    foreach (ReservationStation shift in shiftRSs)
                    {
                        if (!shift.Busy && registers.intQi[instruction.r3] == "0")
                        {
                            checkOperandDependencies(ref instruction, ref registers);
                            shift.Busy = true;
                            registers.intQi[instruction.r3] = "shiftFU";
                            registers.intQiIndex[instruction.r3] = instruction.ID;
                            instruction.functionalUnitID = 12;
                            instruction.reservationStationIndex = shift.arrayIndex;
                            shift.instruction = instruction;
                            return instruction;
                        }
                    }
                    break;
                case 20:            //Add instruction. Checks if it's float
                    if((instruction.isFloat))
                    {
                        foreach(ReservationStation flAdd in floatAddRSs)
                        {
                            if (!flAdd.Busy && registers.intQi[instruction.r3] == "0")
                            {
                                checkOperandDependencies(ref instruction, ref registers);
                                flAdd.Busy = true;
                                registers.floatQi[instruction.r3] = "flAddFU";
                                registers.floatQiIndex[instruction.r3] = instruction.ID;
                                instruction.functionalUnitID = 5;
                                instruction.reservationStationIndex = flAdd.arrayIndex;
                                flAdd.instruction = instruction;
                                return instruction;
                            }
                        }
                    }
                    else
                    {
                        foreach(ReservationStation intAdd in intAddRSs)
                        {
                            if (!intAdd.Busy && registers.intQi[instruction.r3] == "0")
                            {
                                checkOperandDependencies(ref instruction, ref registers);
                                intAdd.Busy = true;
                                registers.intQi[instruction.r3] = "intAddFU";
                                registers.intQiIndex[instruction.r3] = instruction.ID;
                                instruction.functionalUnitID = 1;
                                instruction.reservationStationIndex = intAdd.arrayIndex;
                                intAdd.instruction = instruction;
                                return instruction;
                            }
                        }
                    }
                    
                    break;
                case 21:            //Sub instruction. Checks if it's float
                    if ((instruction.isFloat))
                    {
                        foreach(ReservationStation flSub in floatSubRSs)
                        {
                            if (!flSub.Busy && registers.intQi[instruction.r3] == "0")
                            {
                                checkOperandDependencies(ref instruction, ref registers);
                                flSub.Busy = true;
                                registers.floatQi[instruction.r3] = "flSubFU";
                                registers.floatQiIndex[instruction.r3] = instruction.ID;
                                instruction.functionalUnitID = 6;
                                instruction.reservationStationIndex = flSub.arrayIndex;
                                flSub.instruction = instruction;
                                return instruction;
                            }
                        }
                    }
                    else
                    {
                        foreach(ReservationStation intSub in intSubRSs)
                        {
                            if (!intSub.Busy && registers.intQi[instruction.r3] == "0")
                            {
                                checkOperandDependencies(ref instruction, ref registers);
                                intSub.Busy = true;
                                registers.intQi[instruction.r3] = "intSubFU";
                                registers.intQiIndex[instruction.r3] = instruction.ID;
                                instruction.functionalUnitID = 2;
                                instruction.reservationStationIndex = intSub.arrayIndex;
                                intSub.instruction = instruction;
                                return instruction;
                            }
                        }
                    }
                    break;
                case 22:            //Mult instructions. Checks if it's float
                    if ((instruction.isFloat))
                    {
                        foreach(ReservationStation flMult in floatMultRSs)
                        {
                            if (!flMult.Busy && registers.intQi[instruction.r3] == "0")
                            {
                                checkOperandDependencies(ref instruction, ref registers);
                                flMult.Busy = true;
                                registers.floatQi[instruction.r3] = "flMulFU";
                                registers.floatQiIndex[instruction.r3] = instruction.ID;
                                instruction.functionalUnitID = 7;
                                instruction.reservationStationIndex = flMult.arrayIndex;
                                flMult.instruction = instruction;
                                return instruction;
                            }
                        }
                    }
                    else
                    {
                        foreach(ReservationStation intMult in intMultRSs)
                        {
                            if (!intMult.Busy && registers.intQi[instruction.r3] == "0")
                            {
                                checkOperandDependencies(ref instruction, ref registers);
                                intMult.Busy = true;
                                registers.intQi[instruction.r3] = "intMulFU";
                                registers.intQiIndex[instruction.r3] = instruction.ID;
                                instruction.functionalUnitID = 3;
                                instruction.reservationStationIndex = intMult.arrayIndex;
                                intMult.instruction = instruction;
                                return instruction;
                            }
                        }
                    }
                    break;
                case 23:            //Div instructions. Checks if it's float
                    if ((instruction.isFloat))
                    {
                        foreach(ReservationStation flDiv in floatDivRSs)
                        {
                            if (!flDiv.Busy && registers.intQi[instruction.r3] == "0")
                            {
                                checkOperandDependencies(ref instruction, ref registers);
                                flDiv.Busy = true;
                                registers.floatQi[instruction.r3] = "flDivFU";
                                registers.floatQiIndex[instruction.r3] = instruction.ID;
                                instruction.functionalUnitID = 8;
                                instruction.reservationStationIndex = flDiv.arrayIndex;
                                flDiv.instruction = instruction;
                                return instruction;
                            }
                        }
                    }
                    else
                    {
                        foreach(ReservationStation intDiv in intDivRSs)
                        {
                            if (!intDiv.Busy && registers.intQi[instruction.r3] == "0")
                            {
                                checkOperandDependencies(ref instruction, ref registers);
                                intDiv.Busy = true;
                                registers.intQi[instruction.r3] = "intDivFU";
                                registers.intQiIndex[instruction.r3] = instruction.ID;
                                instruction.functionalUnitID = 4;
                                instruction.reservationStationIndex = intDiv.arrayIndex;
                                intDiv.instruction = instruction;
                                return instruction;
                            }
                        }
                    }
                    break;
                case 24:
                case 25:
                case 26:
                case 27:
                    foreach (ReservationStation bitwise in bitwiseRSs)
                    {
                        if (!bitwise.Busy && registers.intQi[instruction.r3] == "0")
                        {
                            checkOperandDependencies(ref instruction, ref registers);
                            bitwise.Busy = true;
                            registers.intQi[instruction.r3] = "bitwiseFU";
                            registers.intQiIndex[instruction.r3] = instruction.ID;
                            instruction.functionalUnitID = 9;
                            instruction.reservationStationIndex = bitwise.arrayIndex;
                            bitwise.instruction = instruction;
                            return instruction;
                        }
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
            clearFunctionalUnits();
            clearReservationStations();
            reorderBuffer.removeAllInstructionsAfterHazard(id);
            commonDataBus.CDB.Clear();
            commonDataBus.index.Clear();
            commonDataBus.IDIndex.Clear();

            registers.clearRegistersQI();

            return true;
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
            clearFunctionalUnits();
            clearReservationStations();
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


        private void clearFU(Instruction instruction)
        {
            switch (instruction.functionalUnitID)
            {
                case 1:
                    foreach (IntAddFU fu in intAddFUs)
                    {
                        if (instruction.Equals(fu.instruction))
                            fu.instruction = null;
                    }
                    break;
                case 2:
                    foreach (IntSubFU fu in intSubFUs)
                    {
                        if (instruction.Equals(fu.instruction))
                            fu.instruction = null;
                    }
                    break;
                case 3:
                    foreach (IntMultFU fu in intMultFUs)
                    {
                        if (instruction.Equals(fu.instruction))
                            fu.instruction = null;
                    }
                    break;
                case 4:
                    foreach (IntDivFU fu in intDivFUs)
                    {
                        if (instruction.Equals(fu.instruction))
                            fu.instruction = null;
                    }
                    break;
                case 5:
                    foreach (FloatAddFU fu in flAddFUs)
                    {
                        if (instruction.Equals(fu.instruction))
                            fu.instruction = null;
                    }
                    break;
                case 6:
                    foreach (FloatSubFU fu in flSubFUs)
                    {
                        if (instruction.Equals(fu.instruction))
                            fu.instruction = null;
                    }
                    break;
                case 7:
                    foreach (FloatMultFU fu in flMultFUs)
                    {
                        if (instruction.Equals(fu.instruction))
                            fu.instruction = null;
                    }
                    break;
                case 8:
                    foreach (FloatDivFU fu in flDivFUs)
                    {
                        if (instruction.Equals(fu.instruction))
                            fu.instruction = null;
                    }
                    break;
                case 9:
                    foreach (BitwiseOPFU fu in bitFUs)
                    {
                        if (instruction.Equals(fu.instruction))
                            fu.instruction = null;
                    }
                    break;
                case 10:
                    foreach (MemoryUnit fu in memoryFUs)
                    {
                        if (instruction.Equals(fu.instruction))
                            fu.instruction = null;
                    }
                    break;
                case 11:
                    foreach (BranchFU fu in branchFUs)
                    {
                        if (instruction.Equals(fu.instruction))
                            fu.instruction = null;
                    }
                    break;
                case 12:
                    foreach (ShiftFU fu in shiftFUs)
                    {
                        if (instruction.Equals(fu.instruction))
                            fu.instruction = null;
                    }
                    break;
            }
        }


        public void clearFunctionalUnits()
        {
            if (programRanAtLeastOnce)
            {
                foreach (IntAddFU fu in intAddFUs)
                {
                    fu.instruction = null;
                }
                foreach (IntSubFU fu in intSubFUs)
                {
                    fu.instruction = null;
                }
                foreach (IntMultFU fu in intMultFUs)
                {
                    fu.instruction = null;
                }
                foreach (IntDivFU fu in intDivFUs)
                {
                    fu.instruction = null;
                }
                foreach (FloatAddFU fu in flAddFUs)
                {
                    fu.instruction = null;
                }
                foreach (FloatSubFU fu in flSubFUs)
                {
                    fu.instruction = null;
                }
                foreach (FloatMultFU fu in flMultFUs)
                {
                    fu.instruction = null;
                }
                foreach (FloatDivFU fu in flDivFUs)
                {
                    fu.instruction = null;
                }
                foreach (BranchFU fu in branchFUs)
                {
                    fu.instruction = null;
                }
                foreach (BitwiseOPFU fu in bitFUs)
                {
                    fu.instruction = null;
                }
                foreach (ShiftFU fu in shiftFUs)
                {
                    fu.instruction = null;
                }
                foreach (MemoryUnit fu in memoryFUs)
                {
                    fu.instruction = null;
                }
            }
        }

        public void clearReservationStations()
        {
            if (programRanAtLeastOnce)
            {
                foreach (ReservationStation rs in intAddRSs)
                {
                    rs.instruction = null;
                    rs.Busy = false;
                }
                foreach (ReservationStation rs in intSubRSs)
                {
                    rs.instruction = null;
                    rs.Busy = false;
                }
                foreach (ReservationStation rs in intMultRSs)
                {
                    rs.instruction = null;
                    rs.Busy = false;
                }
                foreach (ReservationStation rs in intDivRSs)
                {
                    rs.instruction = null;
                    rs.Busy = false;
                }
                foreach (ReservationStation rs in floatAddRSs)
                {
                    rs.instruction = null;
                    rs.Busy = false;
                }
                foreach (ReservationStation rs in floatSubRSs)
                {
                    rs.instruction = null;
                    rs.Busy = false;
                }
                foreach (ReservationStation rs in floatMultRSs)
                {
                    rs.instruction = null;
                    rs.Busy = false;
                }
                foreach (ReservationStation rs in floatDivRSs)
                {
                    rs.instruction = null;
                    rs.Busy = false;
                }
                foreach (ReservationStation rs in branchRSs)
                {
                    rs.instruction = null;
                    rs.Busy = false;
                }
                foreach (ReservationStation rs in bitwiseRSs)
                {
                    rs.instruction = null;
                    rs.Busy = false;
                }
                foreach (ReservationStation rs in shiftRSs)
                {
                    rs.instruction = null;
                    rs.Busy = false;
                }
                foreach (ReservationStation rs in loadStoreBuffer)
                {
                    rs.instruction = null;
                    rs.Busy = false;
                }
            }
        }
    }
}
