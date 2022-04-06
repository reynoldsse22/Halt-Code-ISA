using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Threading;

namespace ISA_GUI
{
    internal class DynamicPipeline
    {
        Fetch fetch;
        ControlUnit CU;
        Printer print;
        Queue<Instruction> instructionQueue;
        List<Instruction> instructionsInFlight;
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
        public IntMultFU intMultFu;
        public IntDivFU intDivFu;
        public FloatAddFU floatAddFu;
        public FloatSubFU floatSubFu;
        public FloatMultFU floatMultFu;
        public FloatDivFU floatDivFu;
        public BitwiseOPFU bitwiseOPFu;
        public MemoryUnit memoryUnitFu;
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
        public int totalCyclesStalled;
        public bool lastBranchDecision, doneExecuting, executionInProgress;



        /// <summary>Initializes a new instance of the <see cref="DynamicPipeline" /> class.</summary>
        public DynamicPipeline()
        {
            fetch = new Fetch();
            CU = new ControlUnit();
            print = new Printer();
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
            intMultFu = new IntMultFU();
            intDivFu = new IntDivFU();
            floatAddFu = new FloatAddFU();
            floatSubFu = new FloatSubFU();
            floatMultFu = new FloatMultFU();
            floatDivFu = new FloatDivFU();
            bitwiseOPFu = new BitwiseOPFU();
            memoryUnitFu = new MemoryUnit();
            branchFu = new BranchFU();
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
            lastBranchDecision = false;
            doneExecuting = false;
            executionInProgress = false;
            // timer = new Timer();
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
        public void runCycle(List<string> input, bool stepThrough, ref StringBuilder assemblyString, ref StringBuilder decodedString, ref StringBuilder pipelingString, ref bool halted, ref ConfigCycle config, ref InstructionMemory IM, ref RegisterFile registers, ref DataMemory dataMemory)
        {
            do
            {
                cycleCount++;
                if(halted)
                {
                    return;
                }

                string result = "";
                int instASPR = 0;
                if (instructionQueue.Count == 0)
                {
                    fillInstructionQueue(ref IM);
                }

               
                fetchFromInstructionQueue();
                

                //Put the instructions into the reservation station. This should be the first cycle of the pipeline
                foreach (Instruction inst in instructionsInFlight) //Will run through instruction queue, instruction will bne take off the queue once commited
                {
                    switch (inst.stage) //Instuction will hold which cycle it's in
                    {
                        //Take instructions from the data bus and add them to the reorder buffer where instructions will be executed based on program counter
                        case 5:
                            try
                            {
                                reorderBuffer.addToReorderBuffer(inst, ref config);
                            }
                            catch
                            {
                                reorderBufferDelay++;
                            }
                            int instructionIndex = reorderBuffer.checkCommit(inst, ref WR, ref dataMemory, ref lastBranchDecision, ref IM, ref registers, ref halted, ref commonDataBus);
                            bool hazardDetected = detectControlHazard(instructionIndex, ref registers);
                            if(!hazardDetected)
                            {
                                try
                                {
                                    int cdbIndex = commonDataBus.index[instructionIndex];
                                    commonDataBus.CDB.Remove(commonDataBus.CDB.ElementAt(cdbIndex).Key);
                                    instructionsInFlight.Remove(inst);
                                }
                                catch 
                                {
                                    continue;
                                }
                            }
                            break;
                        //Store the answer and corresponding reservation name into the data bus
                        case 4:
                            WR.writeToCDB(inst, ref commonDataBus, in inst.result);
                            inst.stage = 5;
                            break;

                        //This should be where the Write Result and Memory Read stages will be held
                        case 3:
                            memoryUnitFu.instruction.doneExecuting = false;
                            AM.accessMemoryDynamic(ref dataMemory, ref registers, inst, ref config, out result, ref memoryUnitFu, out instASPR);
                            inst.result = result;
                            inst.ASPR = instASPR;
                            
                            if(memoryUnitFu.instruction.doneExecuting)
                            {
                                inst.stage = 4;
                            }
                            break;

                        //Send the instruction to its corresponding functional unit as long as there are no structural harzards present
                        //Open up reservation station to allow for more instructions to flow in
                        //Execute within the functional unit
                        case 2:
                            doneExecuting = execute(inst, ref registers, ref dataMemory, ref IM, ref config, ref alu, ref lastBranchDecision, ref result, ref instASPR);
                            inst.result = result;
                            inst.ASPR = instASPR;
                            inst.dependantOpID1 = 0;
                            inst.dependantOpID2 = 0;
                            if (doneExecuting)
                            {
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
                            }
                            catch (Exception)
                            {
                                reservationStationDelay++;
                            }
                            break;
                    }
                }

                //generateAssembly(ref assemblyString, IM);       //Populates instructionQueue and writes out assembly
                //halted = true; //For testing purposes
            } while (!halted && !stepThrough);
        }


        private void fillInstructionQueue(ref InstructionMemory IM)
        {
            for(int i = 0; i < 10; i++)
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
                instructionsInFlight.Add(instructionQueue.Dequeue());
            return;
        }

        private Instruction decode(Instruction instruction, ref InstructionMemory IM, ref ConfigCycle config)
        {
            CU.decode(ref IM, ref instruction, ref config);
            return instruction;
        }
        
        private bool execute(Instruction instruction, ref RegisterFile registers,ref DataMemory memory, 
            ref InstructionMemory IM, ref ConfigCycle config, ref ALU alu, ref bool branchTaken, ref string result, ref int intASPR)
        {
            if (instruction.opcode == 0 || instruction.opcode == 1)
                return true;
            try
            {
                sendToFU(ref instruction);
            }
            catch(Exception)
            {
                return false;
            }

            bool dependencies = checkDependencies(ref instruction, ref commonDataBus, ref registers);
            if(dependencies)
            {
                return false;
            }
            switch (instruction.functionalUnitID)
            {
                case 1:
                    if(!intAddFu.instruction.executionInProgress && !intAddFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref intAddFu.instruction, ref config, ref branchTaken, out result, out intASPR);
                        intAddFu.instruction.executionInProgress = true;
                        intAddFu.instruction.cycleControl--;
                    }
                    else
                        intAddFu.instruction.cycleControl--;
                    if (intAddFu.instruction.cycleControl == 0)
                    {
                        intAddFu.instruction.doneExecuting = true;
                        intAddFu.instruction.executionInProgress = false;
                        intAddFu.instruction = null;
                        return true;
                    }
                    break;
                case 2:
                    if (!intSubFu.instruction.executionInProgress && !intSubFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref intSubFu.instruction, ref config, ref branchTaken, out result, out intASPR);
                        intSubFu.instruction.executionInProgress = true;
                        intSubFu.instruction.cycleControl--;
                    }
                    else
                        intSubFu.instruction.cycleControl--;
                    if (intSubFu.instruction.cycleControl == 0)
                    {
                        intSubFu.instruction.doneExecuting = true;
                        intSubFu.instruction.executionInProgress = false;
                        intSubFu.instruction = null;
                        return true;
                    }
                    break;
                case 3:
                    if (!intMultFu.instruction.executionInProgress && !intMultFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref intMultFu.instruction, ref config, ref branchTaken, out result, out intASPR);
                        intMultFu.instruction.executionInProgress = true;
                        intMultFu.instruction.cycleControl--;
                    }
                    else
                        intMultFu.instruction.cycleControl--;
                    if (intMultFu.instruction.cycleControl == 0)
                    {
                        intMultFu.instruction.doneExecuting = true;
                        intMultFu.instruction.executionInProgress = false;
                        intMultFu.instruction = null;
                        return true;
                    }
                    break;
                case 4:
                    if (!intDivFu.instruction.executionInProgress && !intDivFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref intDivFu.instruction, ref config, ref branchTaken, out result, out intASPR);
                        intDivFu.instruction.executionInProgress = true;
                        intDivFu.instruction.cycleControl--;
                    }
                    else
                        intDivFu.instruction.cycleControl--;
                    if (intDivFu.instruction.cycleControl == 0)
                    {
                        intDivFu.instruction.doneExecuting = true;
                        intDivFu.instruction.executionInProgress = false;
                        intDivFu.instruction = null;
                        return true;
                    }
                    break;
                case 5:
                    if (!floatAddFu.instruction.executionInProgress && !floatAddFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref floatAddFu.instruction, ref config, ref branchTaken, out result, out intASPR);
                        floatAddFu.instruction.executionInProgress = true;
                        floatAddFu.instruction.cycleControl--;
                    }
                    else
                        floatAddFu.instruction.cycleControl--;
                    if (floatAddFu.instruction.cycleControl == 0)
                    {
                        floatAddFu.instruction.doneExecuting = true;
                        floatAddFu.instruction.executionInProgress = false;
                        floatAddFu.instruction = null;
                        return true;
                    }
                    break;
                case 6:
                    if (!floatSubFu.instruction.executionInProgress && !floatSubFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref floatSubFu.instruction, ref config, ref branchTaken, out result, out intASPR);
                        floatSubFu.instruction.executionInProgress = true;
                        floatSubFu.instruction.cycleControl--;
                    }
                    else
                        floatSubFu.instruction.cycleControl--;
                    if (floatSubFu.instruction.cycleControl == 0)
                    {
                        floatSubFu.instruction.doneExecuting = true;
                        floatSubFu.instruction.executionInProgress = false;
                        floatSubFu.instruction = null;
                        return true;
                    }
                    break;
                case 7:
                    if (!floatMultFu.instruction.executionInProgress && !floatMultFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref floatMultFu.instruction, ref config, ref branchTaken, out result, out intASPR);
                        floatMultFu.instruction.executionInProgress = true;
                        floatMultFu.instruction.cycleControl--;
                    }
                    else
                        floatMultFu.instruction.cycleControl--;
                    if (floatMultFu.instruction.cycleControl == 0)
                    {
                        floatMultFu.instruction.doneExecuting = true;
                        floatMultFu.instruction.executionInProgress = false;
                        floatMultFu.instruction = null;
                        return true;
                    }
                    break;
                case 8:
                    if (!floatDivFu.instruction.executionInProgress && !floatDivFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref floatDivFu.instruction, ref config, ref branchTaken, out result, out intASPR);
                        floatDivFu.instruction.executionInProgress = true;
                        floatDivFu.instruction.cycleControl--;
                    }
                    else
                        floatDivFu.instruction.cycleControl--;
                    if (floatDivFu.instruction.cycleControl == 0)
                    {
                        floatDivFu.instruction.doneExecuting = true;
                        floatDivFu.instruction.executionInProgress = false;
                        floatDivFu.instruction = null;
                        return true;
                    }
                    break;
                case 9:
                    if (!bitwiseOPFu.instruction.executionInProgress && !bitwiseOPFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref bitwiseOPFu.instruction, ref config, ref branchTaken, out result, out intASPR);
                        bitwiseOPFu.instruction.executionInProgress = true;
                        bitwiseOPFu.instruction.cycleControl--;
                    }
                    else
                        bitwiseOPFu.instruction.cycleControl--;
                    if (bitwiseOPFu.instruction.cycleControl == 0)
                    {
                        bitwiseOPFu.instruction.doneExecuting = true;
                        bitwiseOPFu.instruction.executionInProgress = false;
                        bitwiseOPFu.instruction = null;
                        return true;
                    }
                    break;
                case 10:
                    if (!memoryUnitFu.instruction.executionInProgress && !memoryUnitFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref memoryUnitFu.instruction, ref config, ref branchTaken, out result, out intASPR);
                        memoryUnitFu.instruction.executionInProgress = true;
                        memoryUnitFu.instruction.cycleControl--;
                    }
                    else
                        memoryUnitFu.instruction.cycleControl--;
                    if (memoryUnitFu.instruction.cycleControl == 0)
                    {
                        memoryUnitFu.instruction.doneExecuting = true;
                        memoryUnitFu.instruction.executionInProgress = false;
                        return true;
                    }
                    break;
                case 11:
                    if (!branchFu.instruction.executionInProgress && !branchFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref branchFu.instruction, ref config, ref branchTaken, out result, out intASPR);
                        branchFu.instruction.executionInProgress = true;
                        branchFu.instruction.cycleControl--;
                    }
                    else
                        branchFu.instruction.cycleControl--;
                    if (branchFu.instruction.cycleControl == 0)
                    {
                        branchFu.instruction.doneExecuting = true;
                        branchFu.instruction.executionInProgress = false;
                        branchFu.instruction = null;
                        return true;
                    }
                    break;
                case 12:
                    if (!shiftFu.instruction.executionInProgress && !shiftFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref shiftFu.instruction, ref config, ref branchTaken, out result, out intASPR);
                        shiftFu.instruction.executionInProgress = true;
                        shiftFu.instruction.cycleControl--;
                    }
                    else
                        shiftFu.instruction.cycleControl--;
                    if (shiftFu.instruction.cycleControl == 0)
                    {
                        shiftFu.instruction.doneExecuting = true;
                        shiftFu.instruction.executionInProgress = false;
                        shiftFu.instruction = null;
                        return true;
                    }
                    break;
            }
            return false;
        }

        private void sendToFU(ref Instruction instruction)
        {
            if (instruction.opcode == 0 || instruction.opcode == 1)
                return;

            switch(instruction.functionalUnitID)
            {
                case 1:
                    if (intAddFu.instruction == null) 
                    {
                        intAddFu.instruction = intAddRS.instruction;
                        intAddRS.instruction = null;
                        intAddRS.Busy = false;
                        return;
                    }
                    break;
                case 2:
                    if (intSubFu.instruction == null)
                    {
                        intSubFu.instruction = intSubRS.instruction;
                        intSubRS.instruction = null;
                        intSubRS.Busy = false;
                        return;
                    }
                    break;
                case 3:
                    if (intMultFu.instruction == null)
                    {
                        intMultFu.instruction = intMultRS.instruction;
                        intMultRS.instruction = null;
                        intMultRS.Busy = false;
                        return;
                    }
                    break;
                case 4:
                    if (intDivFu.instruction == null)
                    {
                        intDivFu.instruction = intDivRS.instruction;
                        intDivRS.instruction = null;
                        intDivRS.Busy = false;
                        return;
                    }
                    break;
                case 5:
                    if (floatAddFu.instruction == null)
                    {
                        floatAddFu.instruction = floatAddRS.instruction;
                        floatAddRS.instruction = null;
                        floatAddRS.Busy = false;
                        return;
                    }
                    break;
                case 6:
                    if (floatSubFu.instruction == null)
                    {
                        floatSubFu.instruction = floatSubRS.instruction;
                        floatSubRS.instruction = null;
                        floatSubRS.Busy = false;
                        return;
                    }
                    break;
                case 7:
                    if (floatMultFu.instruction == null)
                    {
                        floatMultFu.instruction = floatMultRS.instruction;
                        floatMultRS.instruction = null;
                        floatMultRS.Busy = false;
                        return;
                    }
                    break;
                case 8:
                    if (floatDivFu.instruction == null)
                    {
                        floatDivFu.instruction = floatDivRS.instruction;
                        floatDivRS.instruction = null;
                        floatDivRS.Busy = false;
                        return;
                    }
                    break;
                case 9:
                    if (bitwiseOPFu.instruction == null)
                    {
                        bitwiseOPFu.instruction = bitwiseOPRS.instruction;
                        bitwiseOPRS.instruction = null;
                        bitwiseOPRS.Busy = false;
                        return;
                    }
                    break;
                case 10:
                    if (memoryUnitFu.instruction == null)
                    {
                        memoryUnitFu.instruction = load_storeBuffer.instruction;
                        load_storeBuffer.instruction = null;
                        load_storeBuffer.Busy = false;
                        return;
                    }
                    break;
                case 11:
                    if (branchFu.instruction == null)
                    {
                        branchFu.instruction = branchOPS.instruction;
                        branchOPS.instruction = null;
                        branchOPS.Busy = false;
                        return;
                    }
                    break;
                case 12:
                    if (shiftFu.instruction == null)
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
                    if (instruction.isFloat)
                    {
                        if (memoryUnitFu.instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(memoryUnitFu.instruction.fOp1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    memoryUnitFu.instruction.fOperand1 = float.Parse(CDB.CDB[memoryUnitFu.instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            memoryUnitFu.instruction.fOperand1 = registers.floatRegisters[0];
                    }
                    else
                    {
                        if (memoryUnitFu.instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(memoryUnitFu.instruction.iOp1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    memoryUnitFu.instruction.iOperand1 = int.Parse(CDB.CDB[memoryUnitFu.instruction.iOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            memoryUnitFu.instruction.iOperand1 = registers.intRegisters[0];
                    }
                    break;
                case 13:
                    if (instruction.isFloat)
                    {
                        if (floatSubFu.instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(floatSubFu.instruction.fOp1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    floatSubFu.instruction.fOperand1 = int.Parse(CDB.CDB[floatSubFu.instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            floatSubFu.instruction.fOperand1 = registers.floatRegisters[floatSubFu.instruction.r3];
                    }
                    else
                    {
                        if (intSubFu.instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(intSubFu.instruction.iOp1))
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
                        if (floatSubFu.instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(floatSubFu.instruction.fOp1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    floatSubFu.instruction.fOperand1 = int.Parse(CDB.CDB[floatSubFu.instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            floatSubFu.instruction.fOperand1 = registers.floatRegisters[floatSubFu.instruction.r1];
                        if (floatSubFu.instruction.fOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(floatSubFu.instruction.fOp2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    floatSubFu.instruction.fOperand2 = float.Parse(CDB.CDB[floatSubFu.instruction.fOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            floatSubFu.instruction.fOperand2 = registers.floatRegisters[floatSubFu.instruction.r2];
                    }
                    else
                    {
                        if (intSubFu.instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(intSubFu.instruction.iOp1))
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
                            if (CDB.CDB.ContainsKey(intSubFu.instruction.iOp2))
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
                        if (load_storeBuffer.instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(load_storeBuffer.instruction.fOp2))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    load_storeBuffer.instruction.fOperand1 = float.Parse(CDB.CDB[load_storeBuffer.instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            load_storeBuffer.instruction.fOperand1 = registers.floatRegisters[load_storeBuffer.instruction.r2];
                    }
                    else
                    {
                        if (load_storeBuffer.instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(load_storeBuffer.instruction.iOp1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    load_storeBuffer.instruction.iOperand1 = int.Parse(CDB.CDB[load_storeBuffer.instruction.iOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            load_storeBuffer.instruction.iOperand1 = registers.intRegisters[load_storeBuffer.instruction.r2];
                    }
                    break;
                case 16:
                case 17:
                case 18:
                case 19:
                    if (shiftFu.instruction.iOp1 != "")
                    {
                        if (CDB.CDB.ContainsKey(shiftFu.instruction.iOp1))
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
                        if (CDB.CDB.ContainsKey(shiftFu.instruction.iOp2))
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
                        if (floatAddFu.instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(floatAddFu.instruction.fOp1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    floatAddFu.instruction.fOperand1 = int.Parse(CDB.CDB[floatAddFu.instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            floatAddFu.instruction.fOperand1 = registers.floatRegisters[floatAddFu.instruction.r1];
                        if (floatAddFu.instruction.fOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(floatAddFu.instruction.fOp2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    floatAddFu.instruction.fOperand2 = float.Parse(CDB.CDB[floatAddFu.instruction.fOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            floatAddFu.instruction.fOperand2 = registers.floatRegisters[floatAddFu.instruction.r2];
                    }
                    else
                    {
                        if (intAddFu.instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(intAddFu.instruction.iOp1))
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
                            if (CDB.CDB.ContainsKey(intAddFu.instruction.iOp2))
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
                        if (floatSubFu.instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(floatSubFu.instruction.fOp1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    floatSubFu.instruction.fOperand1 = int.Parse(CDB.CDB[floatSubFu.instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            floatSubFu.instruction.fOperand1 = registers.floatRegisters[floatSubFu.instruction.r1];
                        if (floatSubFu.instruction.fOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(floatSubFu.instruction.fOp2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    floatSubFu.instruction.fOperand2 = float.Parse(CDB.CDB[floatSubFu.instruction.fOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            floatSubFu.instruction.fOperand2 = registers.floatRegisters[floatSubFu.instruction.r2];
                    }
                    else
                    {
                        if (intSubFu.instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(intSubFu.instruction.iOp1))
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
                            if (CDB.CDB.ContainsKey(intSubFu.instruction.iOp2))
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
                        if (floatMultFu.instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(floatMultFu.instruction.fOp1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    floatMultFu.instruction.fOperand1 = int.Parse(CDB.CDB[floatMultFu.instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            floatMultFu.instruction.fOperand1 = registers.floatRegisters[floatMultFu.instruction.r1];
                        if (floatMultFu.instruction.fOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(floatMultFu.instruction.fOp2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    floatMultFu.instruction.fOperand2 = float.Parse(CDB.CDB[floatMultFu.instruction.fOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            floatMultFu.instruction.fOperand2 = registers.floatRegisters[floatMultFu.instruction.r2];
                    }
                    else
                    {
                        if (intMultFu.instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(intMultFu.instruction.iOp1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    intMultFu.instruction.iOperand1 = int.Parse(CDB.CDB[intMultFu.instruction.iOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intMultFu.instruction.iOperand1 = registers.intRegisters[intMultFu.instruction.r1];
                        if (intMultFu.instruction.iOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(intMultFu.instruction.iOp2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    intMultFu.instruction.iOperand2 = int.Parse(CDB.CDB[intMultFu.instruction.iOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            intMultFu.instruction.iOperand2 = registers.intRegisters[intMultFu.instruction.r2];
                    }
                    break;
                case 23:
                    if(instruction.isFloat)
                    {
                        if (floatDivFu.instruction.fOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(floatDivFu.instruction.fOp1))
                            {
                                if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                    floatDivFu.instruction.fOperand1 = int.Parse(CDB.CDB[floatDivFu.instruction.fOp1]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            floatDivFu.instruction.fOperand1 = registers.floatRegisters[floatDivFu.instruction.r1];
                        if (floatDivFu.instruction.fOp2 != "")
                        {
                            if (CDB.CDB.ContainsKey(intDivFu.instruction.fOp2))
                            {
                                if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                    floatDivFu.instruction.fOperand2 = float.Parse(CDB.CDB[floatDivFu.instruction.fOp2]);
                            }
                            else
                            {
                                trueDependenceDelay++;
                                return true;
                            }
                        }
                        else
                            floatDivFu.instruction.fOperand2 = registers.floatRegisters[floatDivFu.instruction.r2];
                    }
                    else
                    {
                        if (intDivFu.instruction.iOp1 != "")
                        {
                            if (CDB.CDB.ContainsKey(intDivFu.instruction.iOp1))
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
                            if (CDB.CDB.ContainsKey(intDivFu.instruction.iOp2))
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
                    if (bitwiseOPFu.instruction.iOp1 != "")
                    {
                        if (CDB.CDB.ContainsKey(bitwiseOPFu.instruction.iOp1))
                        {
                            if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                bitwiseOPFu.instruction.iOperand1 = int.Parse(CDB.CDB[bitwiseOPFu.instruction.iOp1]);
                        }
                        else
                        {
                            trueDependenceDelay++;
                            return true;
                        }
                    }
                    else
                        bitwiseOPFu.instruction.iOperand1 = registers.intRegisters[bitwiseOPFu.instruction.r1];
                    if (bitwiseOPFu.instruction.iOp2 != "")
                    {
                        if (CDB.CDB.ContainsKey(bitwiseOPFu.instruction.iOp2))
                        {
                            if (instruction.dependantOpID2 == CDB.IDIndex[CDB.index[instruction.dependantOpID2]])
                                bitwiseOPFu.instruction.iOperand2 = int.Parse(CDB.CDB[bitwiseOPFu.instruction.iOp2]);
                        }
                        else
                        {
                            trueDependenceDelay++;
                            return true;
                        }
                    }
                    else
                        bitwiseOPFu.instruction.iOperand2 = registers.intRegisters[bitwiseOPFu.instruction.r2];
                    break;
                case 27:
                    if (bitwiseOPFu.instruction.iOp1 != "")
                    {
                        if (CDB.CDB.ContainsKey(bitwiseOPFu.instruction.iOp1))
                        {
                            if (instruction.dependantOpID1 == CDB.IDIndex[CDB.index[instruction.dependantOpID1]])
                                bitwiseOPFu.instruction.iOperand1 = int.Parse(CDB.CDB[bitwiseOPFu.instruction.iOp1]);
                        }
                        else
                        {
                            trueDependenceDelay++;
                            return true;
                        }
                    }
                    else
                        bitwiseOPFu.instruction.iOperand1 = registers.intRegisters[bitwiseOPFu.instruction.r1];
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
                        if (!registers.floatQi[instruction.r2].Equals("0"))
                        {
                            instruction.fOp1 = registers.floatQi[instruction.r2];
                            instruction.dependantOpID1 = registers.floatQiIndex[instruction.r2];
                        }
                        if (!load_storeBuffer.Busy)
                        {
                            load_storeBuffer.Busy = true;
                            registers.floatQi[instruction.r3] = "memoryUnitFu";
                            registers.floatQiIndex[instruction.r3] = instruction.ID;
                            instruction.functionalUnitID = 10;
                            load_storeBuffer.instruction = instruction;
                            return instruction;
                        }
                    }
                    else
                    {
                        if (!registers.intQi[instruction.r2].Equals("0"))
                        {
                            instruction.iOp1 = registers.intQi[instruction.r2];
                            instruction.dependantOpID1 = registers.intQiIndex[instruction.r2];
                        }
                        if (!load_storeBuffer.Busy)
                        {
                            load_storeBuffer.Busy = true;
                            registers.intQi[instruction.r3] = "memoryUnitFu";
                            registers.intQiIndex[0] = instruction.ID;
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
                            registers.floatQi[0] = "memoryUnitFu";
                            registers.floatQiIndex[0] = instruction.ID;
                        }
                        else
                        {
                            registers.intQi[0] = "memoryUnitFu";
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

                        if(instruction.isFloat)
                        {
                            if (!registers.floatQi[0].Equals("0"))
                            {
                                instruction.fOp1 = registers.floatQi[0];
                                instruction.dependantOpID1 = registers.floatQiIndex[0];
                            }
                        }
                        else
                        {
                            if (!registers.intQi[0].Equals("0"))
                            {
                                instruction.iOp1 = registers.intQi[0];
                                instruction.dependantOpID1 = registers.intQiIndex[0];
                            }
                        }
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
                            registers.floatQi[instruction.r3] = "memoryUnitFu";
                            registers.floatQiIndex[instruction.r3] = instruction.ID;
                        }
                        else
                        {
                            registers.intQi[instruction.r3] = "memoryUnitFu";
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
                            if (!registers.floatQi[0].Equals("0"))
                            {
                                instruction.fOp1 = registers.floatQi[0];
                                instruction.dependantOpID1 = registers.floatQiIndex[0];
                            }
                            registers.floatQi[instruction.r3] = "memoryUnitFu";
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
                            if (!registers.intQi[0].Equals("0"))
                            {
                                instruction.iOp1 = registers.intQi[0];
                                instruction.dependantOpID1 = registers.intQiIndex[0];
                            }
                            load_storeBuffer.Busy = true;
                            registers.intQi[instruction.r3] = "memoryUnitFu";
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
                        if (!registers.floatQi[instruction.r3].Equals("0"))
                        {
                            instruction.fOp1 = registers.floatQi[instruction.r3];
                            instruction.dependantOpID1 = registers.floatQiIndex[instruction.r3];
                        }
                        if (!intSubRS.Busy)
                        {
                            floatSubRS.Busy = true;
                            instruction.functionalUnitID = 2;
                            floatSubRS.instruction = instruction;
                            return instruction;
                        }
                    }
                    else
                    {
                        if (!registers.intQi[instruction.r3].Equals("0"))
                        {
                            instruction.iOp1 = registers.intQi[instruction.r3];
                            instruction.dependantOpID1 = registers.intQiIndex[instruction.r3];
                        }
                        if (!intSubRS.Busy)
                        {
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
                        if (!registers.floatQi[instruction.r1].Equals("0"))
                        {
                            instruction.fOp1 = registers.floatQi[instruction.r1];
                            instruction.dependantOpID1 = registers.floatQiIndex[instruction.r1];
                        }

                        if (!registers.floatQi[instruction.r2].Equals("0"))
                        {
                            instruction.fOp2 = registers.floatQi[instruction.r2];
                            instruction.dependantOpID2 = registers.floatQiIndex[instruction.r2];
                        }
                        if (!floatSubRS.Busy)
                        {
                            floatSubRS.Busy = true;
                            instruction.functionalUnitID = 2;
                            floatSubRS.instruction = instruction;
                            return instruction;
                        }
                    }
                    else
                    {
                        if (!registers.intQi[instruction.r1].Equals("0"))
                        {
                            instruction.iOp1 = registers.intQi[instruction.r1];
                            instruction.dependantOpID1 = registers.intQiIndex[instruction.r1];
                        }

                        if (!registers.intQi[instruction.r2].Equals("0"))
                        {
                            instruction.iOp2 = registers.intQi[instruction.r2];
                            instruction.dependantOpID2 = registers.intQiIndex[instruction.r2];
                        }
                        if (!intSubRS.Busy)
                        {
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
                        if (!registers.intQi[instruction.r1].Equals("0"))
                        {
                            instruction.iOp1 = registers.intQi[instruction.r1];
                            instruction.dependantOpID1 = registers.intQiIndex[instruction.r1];
                        }

                        if (!registers.intQi[instruction.r2].Equals("0"))
                        {
                            instruction.iOp2 = registers.intQi[instruction.r2];
                            instruction.dependantOpID2 = registers.intQiIndex[instruction.r2];
                        }
                        shiftOPS.Busy = true;
                        registers.intQi[instruction.r3] = "shiftOPS";
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
                            if (!registers.floatQi[instruction.r1].Equals("0"))
                            {
                                instruction.fOp1 = registers.floatQi[instruction.r1];
                                instruction.dependantOpID1 = registers.floatQiIndex[instruction.r1];
                            }

                            if (!registers.floatQi[instruction.r2].Equals("0"))
                            {
                                instruction.fOp2 = registers.floatQi[instruction.r2];
                                instruction.dependantOpID2 = registers.floatQiIndex[instruction.r2];
                            }
                            floatAddRS.Busy = true;
                            registers.floatQi[instruction.r3] = "floatAddRS";
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
                            if (!registers.intQi[instruction.r1].Equals("0"))
                            {
                                instruction.iOp1 = registers.intQi[instruction.r1];
                                instruction.dependantOpID1 = registers.intQiIndex[instruction.r1];
                            }

                            if (!registers.intQi[instruction.r2].Equals("0"))
                            {
                                instruction.iOp2 = registers.intQi[instruction.r2];
                                instruction.dependantOpID2 = registers.intQiIndex[instruction.r2];
                            }
                            intAddRS.Busy = true;
                            registers.intQi[instruction.r3] = "intAddRS";
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
                            if (!registers.floatQi[instruction.r1].Equals("0"))
                            {
                                instruction.fOp1 = registers.floatQi[instruction.r1];
                                instruction.dependantOpID1 = registers.floatQiIndex[instruction.r1];
                            }

                            if (!registers.floatQi[instruction.r2].Equals("0"))
                            {
                                instruction.fOp2 = registers.floatQi[instruction.r2];
                                instruction.dependantOpID2 = registers.floatQiIndex[instruction.r2];
                            }
                            floatSubRS.Busy = true;
                            registers.floatQi[instruction.r3] = "floatSubRS";
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
                            if (!registers.intQi[instruction.r1].Equals("0"))
                            {
                                instruction.iOp1 = registers.intQi[instruction.r1];
                                instruction.dependantOpID1 = registers.intQiIndex[instruction.r1];
                            }

                            if (!registers.intQi[instruction.r2].Equals("0"))
                            {
                                instruction.iOp2 = registers.intQi[instruction.r2];
                                instruction.dependantOpID2 = registers.intQiIndex[instruction.r2];
                            }
                            intSubRS.Busy = true;
                            registers.intQi[instruction.r3] = "intSubRS";
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
                            if (!registers.floatQi[instruction.r1].Equals("0"))
                            {
                                instruction.fOp1 = registers.floatQi[instruction.r1];
                                instruction.dependantOpID1 = registers.floatQiIndex[instruction.r1];
                            }

                            if (!registers.floatQi[instruction.r2].Equals("0"))
                            {
                                instruction.fOp2 = registers.floatQi[instruction.r2];
                                instruction.dependantOpID2 = registers.floatQiIndex[instruction.r2];
                            }
                            floatMultRS.Busy = true;
                            registers.floatQi[instruction.r3] = "floatMultRS";
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
                            if (!registers.intQi[instruction.r1].Equals("0"))
                            {
                                instruction.iOp1 = registers.intQi[instruction.r1];
                                instruction.dependantOpID1 = registers.intQiIndex[instruction.r1];
                            }

                            if (!registers.intQi[instruction.r2].Equals("0"))
                            {
                                instruction.iOp2 = registers.intQi[instruction.r2];
                                instruction.dependantOpID2 = registers.intQiIndex[instruction.r2];
                            }
                            intMultRS.Busy = true;
                            registers.intQi[instruction.r3] = "intMultRS";
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
                            if (!registers.floatQi[instruction.r1].Equals("0"))
                            {
                                instruction.fOp1 = registers.floatQi[instruction.r1];
                                instruction.dependantOpID1 = registers.floatQiIndex[instruction.r1];
                            }

                            if (!registers.floatQi[instruction.r2].Equals("0"))
                            {
                                instruction.fOp2 = registers.floatQi[instruction.r2];
                                instruction.dependantOpID2 = registers.floatQiIndex[instruction.r2];
                            }
                            floatDivRS.Busy = true;
                            registers.floatQi[instruction.r3] = "floatDivRS";
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
                            if (!registers.intQi[instruction.r1].Equals("0"))
                            {
                                instruction.iOp1 = registers.intQi[instruction.r1];
                                instruction.dependantOpID1 = registers.intQiIndex[instruction.r1];
                            }

                            if (!registers.intQi[instruction.r2].Equals("0"))
                            {
                                instruction.iOp2 = registers.intQi[instruction.r2];
                                instruction.dependantOpID2 = registers.intQiIndex[instruction.r2];
                            }
                            intDivRS.Busy = true;
                            registers.intQi[instruction.r3] = "intDivRS";
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
                        if (!registers.intQi[instruction.r1].Equals("0"))
                        {
                            instruction.iOp1 = registers.intQi[instruction.r1];
                            instruction.dependantOpID1 = registers.intQiIndex[instruction.r1];
                        }

                        if (!registers.intQi[instruction.r2].Equals("0"))
                        {
                            instruction.iOp2 = registers.intQi[instruction.r2];
                            instruction.dependantOpID2 = registers.intQiIndex[instruction.r2];
                        }
                        bitwiseOPRS.Busy = true;
                        registers.intQi[instruction.r3] = "bitwiseOPRS";
                        registers.intQiIndex[instruction.r3] = instruction.ID;
                        instruction.functionalUnitID = 9;
                        bitwiseOPRS.instruction = instruction;
                        return instruction;
                    }
                    break;
                case 27:
                    if (!bitwiseOPRS.Busy)
                    {
                        if (!registers.intQi[instruction.r1].Equals("0"))
                        {
                            instruction.iOp1 = registers.intQi[instruction.r1];
                            instruction.dependantOpID1 = registers.intQiIndex[instruction.r1];
                        }
                        bitwiseOPRS.Busy = true;
                        registers.intQi[instruction.r3] = "bitwiseOPRS";
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
        public bool detectControlHazard(int id, ref RegisterFile registers)
        {
            if (id == -1 || lastBranchDecision == false)
                return false;
            if ((instructionsInFlight[(id - 1)].programCounterValue != instructionsInFlight[id].address) && lastBranchDecision == true)
            {
                instructionID = id + 1;
                instructionsInFlight.Clear();
                instructionQueue.Clear();
                intAddFu.instruction = null;
                intSubFu.instruction = null;
                intMultFu.instruction = null;
                intDivFu.instruction = null;
                floatAddFu.instruction = null;
                floatSubFu.instruction = null;
                floatMultFu.instruction = null;
                floatDivFu.instruction = null;
                bitwiseOPFu.instruction = null;
                memoryUnitFu.instruction = null;
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

                registers.clearRegistersQI();

                return true;
            }
            return false;
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
                print.buildAssemblyString(ref assemblyString, ref instruction); //Prints assembly string to the GUI
            }
        }
    }
}
