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
        public CommonDataBus CommonDataBus;
        public IntAddFU intAddFu;
        public IntSubFU intSubFu;
        public IntMultFU intMultFu;
        public IntDivFU intDivFu;
        public FloatAddFU floatAddFu;
        public FloatSubFU floatSubFu;
        public FloatMultFU floatMultFu;
        public FloatDivFU floatDivFu;
        public BitwiseOPFU bitwiseOPFU;
        public MemoryUnitFU memoryUnitFU;
        public BranchFU branchFU;
        public ShiftFU shiftFU;
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
        public int reorderBufferDelay, reservationStationDelay, trueDependenceDelay, totalDelays;
        public int totalCyclesStalled;
        public bool lastBranchDecision;



        /// <summary>Initializes a new instance of the <see cref="DynamicPipeline" /> class.</summary>
        public DynamicPipeline()
        {
            fetch = new Fetch();
            CU = new ControlUnit();
            print = new Printer();
            instructionQueue = new Queue<Instruction>();
            alu = new ALU();
            CU = new ControlUnit();
            EU = new ExecutionUnit();
            AM = new AccessMemory();
            WR = new WriteResult();
            CommonDataBus = new CommonDataBus();
            intAddFu = new IntAddFU();
            intSubFu = new IntSubFU();
            intMultFu = new IntMultFU();
            intDivFu = new IntDivFU();
            floatAddFu = new FloatAddFU();
            floatSubFu = new FloatSubFU();
            floatMultFu = new FloatMultFU();
            floatDivFu = new FloatDivFU();
            bitwiseOPFU = new BitwiseOPFU();
            memoryUnitFU = new MemoryUnitFU();
            branchFU = new BranchFU();
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
            lastBranchDecision = false;
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
                if (instructionQueue.Count == 0)
                {
                    fillInstructionQueue(ref IM);
                }

                //Put the instructions into the reservation station. This should be the first cycle of the pipeline
                foreach (Instruction inst in instructionQueue) //Will run through instruction queue, instruction will bne take off the queue once commited
                {
                    switch (inst.stage) //Instuction will hold which cycle it's in
                    {
                        //Take instructions from the data bus and add them to the reorder buffer where instructions will be executed based on program counter
                        case 5:

                            break;

                        case 4:

                            break;

                        //Store the answer and corresponding reservation name into the data bus
                        //This should be where the Write Result and Memory Read stages will be held
                        case 3:

                            break;

                        //Send the instruction to its corresponding functional unit as long as there are no structural harzards present
                        //Open up reservation station to allow for more instructions to flow in
                        //Execute within the functional unit
                        case 2:


                            if (inst.opcode == 0 || inst.opcode == 1)
                                inst.stage = 5;
                            else if (inst.opcode == 9 || inst.opcode == 11 || inst.opcode == 12)
                                inst.stage = 4;
                            else
                                inst.stage = 5;
                            break;
                        //Populates reservationStations
                        case 1:
                            fetchFromInstructionQueue();
                            try
                            {
                                inst.functionalUnitID = populateReservationStation(inst, ref registers);
                                inst.stage = 2;
                            }
                            catch (Exception)
                            {
                                reservationStationDelay++;
                            }
                            break;

                    }
                }

                generateAssembly(ref assemblyString, IM);       //Populates instructionQueue and writes out assembly
                halted = true; //For testing purposes
            } while (!halted && !stepThrough);
        }


        private void fillInstructionQueue(ref InstructionMemory IM)
        {
            for(int i = 0; i < 10; i++)
            {
                fetchInstruction = new Instruction();
                fetchInstruction = fetch.getNextInstruction(ref IM);
                instructionQueue.Enqueue(fetchInstruction);
            }
        }

        private void fetchFromInstructionQueue()
        {
            instructionsInFlight.Add(instructionQueue.Dequeue());
        }


        
        private bool execute(Instruction instruction, ref RegisterFile registers,ref DataMemory memory, 
            ref InstructionMemory IM, ref ConfigCycle config, ref ALU alu, ref bool branchTaken, ref string result)
        {
            try
            {
                sendToFU(instruction);
            }
            catch(Exception e)
            {
                return false;
            }
            switch (instruction.functionalUnitID)
            {
                case 1:
                    if(!intAddFu.instruction.executionInProgress && !intAddFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref intAddFu.instruction, ref config, ref branchTaken, ref result);
                        intAddFu.instruction.executionInProgress = true;
                        intAddFu.instruction.cycleControl--;
                    }
                    else
                        intAddFu.instruction.cycleControl--;
                    if (intAddFu.instruction.cycleControl == 0)
                    {
                        intAddFu.instruction.doneExecuting = true;
                        return true;
                    }
                    break;
                case 2:
                    if (!intSubFu.instruction.executionInProgress && !intSubFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref intSubFu.instruction, ref config, ref branchTaken, ref result);
                        intSubFu.instruction.executionInProgress = true;
                        intSubFu.instruction.cycleControl--;
                    }
                    else
                        intSubFu.instruction.cycleControl--;
                    if (intSubFu.instruction.cycleControl == 0)
                    {
                        intSubFu.instruction.doneExecuting = true;
                        return true;
                    }
                    break;
                case 3:
                    if (!intMultFu.instruction.executionInProgress && !intMultFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref intMultFu.instruction, ref config, ref branchTaken, ref result);
                        intMultFu.instruction.executionInProgress = true;
                        intMultFu.instruction.cycleControl--;
                    }
                    else
                        intMultFu.instruction.cycleControl--;
                    if (intMultFu.instruction.cycleControl == 0)
                    {
                        intMultFu.instruction.doneExecuting = true;
                        return true;
                    }
                    break;
                case 4:
                    if (!intDivFu.instruction.executionInProgress && !intDivFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref intDivFu.instruction, ref config, ref branchTaken, ref result);
                        intDivFu.instruction.executionInProgress = true;
                        intDivFu.instruction.cycleControl--;
                    }
                    else
                        intDivFu.instruction.cycleControl--;
                    if (intDivFu.instruction.cycleControl == 0)
                    {
                        intDivFu.instruction.doneExecuting = true;
                        return true;
                    }
                    break;
                case 5:
                    if (!floatAddFu.instruction.executionInProgress && !floatAddFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref floatAddFu.instruction, ref config, ref branchTaken, ref result);
                        floatAddFu.instruction.executionInProgress = true;
                        floatAddFu.instruction.cycleControl--;
                    }
                    else
                        floatAddFu.instruction.cycleControl--;
                    if (floatAddFu.instruction.cycleControl == 0)
                    {
                        floatAddFu.instruction.doneExecuting = true;
                        return true;
                    }
                    break;
                case 6:
                    if (!floatSubFu.instruction.executionInProgress && !floatSubFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref floatSubFu.instruction, ref config, ref branchTaken, ref result);
                        floatSubFu.instruction.executionInProgress = true;
                        floatSubFu.instruction.cycleControl--;
                    }
                    else
                        floatSubFu.instruction.cycleControl--;
                    if (floatSubFu.instruction.cycleControl == 0)
                    {
                        floatSubFu.instruction.doneExecuting = true;
                        return true;
                    }
                    break;
                case 7:
                    if (!floatMultFu.instruction.executionInProgress && !floatMultFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref floatMultFu.instruction, ref config, ref branchTaken, ref result);
                        floatMultFu.instruction.executionInProgress = true;
                        floatMultFu.instruction.cycleControl--;
                    }
                    else
                        floatMultFu.instruction.cycleControl--;
                    if (floatMultFu.instruction.cycleControl == 0)
                    {
                        floatMultFu.instruction.doneExecuting = true;
                        return true;
                    }
                    break;
                case 8:
                    if (!floatDivFu.instruction.executionInProgress && !floatDivFu.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref floatDivFu.instruction, ref config, ref branchTaken, ref result);
                        floatDivFu.instruction.executionInProgress = true;
                        floatDivFu.instruction.cycleControl--;
                    }
                    else
                        floatDivFu.instruction.cycleControl--;
                    if (floatDivFu.instruction.cycleControl == 0)
                    {
                        floatDivFu.instruction.doneExecuting = true;
                        return true;
                    }
                    break;
                case 9:
                    if (!bitwiseOPFU.instruction.executionInProgress && !bitwiseOPFU.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref bitwiseOPFU.instruction, ref config, ref branchTaken, ref result);
                        bitwiseOPFU.instruction.executionInProgress = true;
                        bitwiseOPFU.instruction.cycleControl--;
                    }
                    else
                        bitwiseOPFU.instruction.cycleControl--;
                    if (bitwiseOPFU.instruction.cycleControl == 0)
                    {
                        bitwiseOPFU.instruction.doneExecuting = true;
                        return true;
                    }
                    break;
                case 10:
                    if (!memoryUnitFU.instruction.executionInProgress && !memoryUnitFU.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref memoryUnitFU.instruction, ref config, ref branchTaken, ref result);
                        memoryUnitFU.instruction.executionInProgress = true;
                        memoryUnitFU.instruction.cycleControl--;
                    }
                    else
                        memoryUnitFU.instruction.cycleControl--;
                    if (memoryUnitFU.instruction.cycleControl == 0)
                    {
                        memoryUnitFU.instruction.doneExecuting = true;
                        return true;
                    }
                    break;
                case 11:
                    if (!branchFU.instruction.executionInProgress && !branchFU.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref branchFU.instruction, ref config, ref branchTaken, ref result);
                        branchFU.instruction.executionInProgress = true;
                        branchFU.instruction.cycleControl--;
                    }
                    else
                        branchFU.instruction.cycleControl--;
                    if (branchFU.instruction.cycleControl == 0)
                    {
                        branchFU.instruction.doneExecuting = true;
                        return true;
                    }
                    break;
                case 12:
                    if (!shiftFU.instruction.executionInProgress && !shiftFU.instruction.doneExecuting)
                    {
                        EU.executeDynamic(ref registers, ref memory, ref alu, ref IM, ref shiftFU.instruction, ref config, ref branchTaken, ref result);
                        shiftFU.instruction.executionInProgress = true;
                        shiftFU.instruction.cycleControl--;
                    }
                    else
                        shiftFU.instruction.cycleControl--;
                    if (shiftFU.instruction.cycleControl == 0)
                    {
                        shiftFU.instruction.doneExecuting = true;
                        return true;
                    }
                    break;
            }
            return false;
        }

        private void sendToFU(Instruction instruction)
        {
            switch(instruction.functionalUnitID)
            {
                case 1:
                    if (intAddFu.instruction == null) 
                    {
                        intAddFu.instruction = intAddRS.instruction;
                        intAddRS = null;
                        return;
                    }
                    break;
                case 2:
                    if (intSubFu.instruction == null)
                    {
                        intSubFu.instruction = intSubRS.instruction;
                        intSubFu = null;
                        return;
                    }
                    break;
                case 3:
                    if (intMultFu.instruction == null)
                    {
                        intMultFu.instruction = intMultRS.instruction;
                        intMultFu = null;
                        return;
                    }
                    break;
                case 4:
                    if (intDivFu.instruction == null)
                    {
                        intDivFu.instruction = intDivRS.instruction;
                        intDivFu = null;
                        return;
                    }
                    break;
                case 5:
                    if (floatAddFu.instruction == null)
                    {
                        floatAddFu.instruction = floatAddRS.instruction;
                        floatAddFu = null;
                        return;
                    }
                    break;
                case 6:
                    if (floatSubFu.instruction == null)
                    {
                        floatSubFu.instruction = floatSubRS.instruction;
                        floatSubFu = null;
                        return;
                    }
                    break;
                case 7:
                    if (floatMultFu.instruction == null)
                    {
                        floatMultFu.instruction = floatMultRS.instruction;
                        floatMultFu = null;
                        return;
                    }
                    break;
                case 8:
                    if (floatDivFu.instruction == null)
                    {
                        floatDivFu.instruction = floatDivRS.instruction;
                        floatDivFu = null;
                        return;
                    }
                    break;
                case 9:
                    if (bitwiseOPFU.instruction == null)
                    {
                        bitwiseOPFU.instruction = bitwiseOPRS.instruction;
                        bitwiseOPFU = null;
                        return;
                    }
                    break;
                case 10:
                    if (memoryUnitFU.instruction == null)
                    {
                        memoryUnitFU.instruction = load_storeBuffer.instruction;
                        memoryUnitFU = null;
                        return;
                    }
                    break;
                case 11:
                    if (branchFU.instruction == null)
                    {
                        branchFU.instruction = branchOPS.instruction;
                        branchFU = null;
                        return;
                    }
                    break;
                case 12:
                    if (shiftFU.instruction == null)
                    {
                        shiftFU.instruction = shiftOPS.instruction;
                        shiftFU = null;
                        return;
                    }
                    break;
            }
            throw new Exception();
        }

        private bool checkDependencies(Instruction instruction)
        {
            switch(instruction.opcode)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
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
                case 27:
                    break;
            }
            return true;
        }


        private void stage3(Instruction instruction)
        {

        }


        //Will use the opcode and flags to figure out which reservation station the instruction needs to be in
        private int populateReservationStation(Instruction instruction, ref RegisterFile registers)
        {
            switch(instruction.opcode)
            {
                case 0:
                case 1:
                    return -1;
                case 2:         //Branch Instructions
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    if(!branchOPS.Busy)
                    {
                        branchOPS.instruction = instruction;
                        branchOPS.Busy = true;
                        return 11;
                        //Vj, Vk, Qj, Qk will be implemented here
                    }
                    break;
                case 15:
                    if (instruction.isFloat)
                    {
                        if (registers.floatQi[instruction.r2].Equals("0"))
                        {
                            instruction.fOp1 = registers.floatQi[instruction.r2];
                        }
                        else
                            instruction.fOp1 = instruction.r2.ToString();
                        if (!intSubRS.Busy)
                        {
                            intSubRS.instruction = instruction;
                            intSubRS.Busy = true;
                            return 2;
                        }
                    }
                    break;
                case 10:
                case 12:        //Store instruction
                    if (instruction.isFloat)
                    {
                        if (!load_storeBuffer.Busy)
                        {
                            if (registers.floatQi[0].Equals("0"))
                            {
                                instruction.fOp1 = registers.floatQi[0];
                            }
                            else
                                instruction.fOp1 = "0";
                            load_storeBuffer.instruction = instruction;
                            load_storeBuffer.Busy = true;
                            return 10;
                            //Vj, Vk, Qj, Qk will be implemented here
                        }
                    }
                    break;
                case 9:         //Load Instructions
                case 11:
                    if (!load_storeBuffer.Busy)
                    {
                        load_storeBuffer.instruction = instruction;
                        load_storeBuffer.Busy = true;
                        return 10;
                        //Vj, Vk, Qj, Qk will be implemented here
                    }
                    break;
                
                case 13:
                case 14:
                    if (instruction.isFloat)
                    {
                        if (registers.floatQi[instruction.r1].Equals("0"))
                        {
                            instruction.fOp1 = registers.floatQi[instruction.r1];
                        }
                        else
                            instruction.fOp1 = instruction.r1.ToString();

                        if (registers.floatQi[instruction.r2].Equals("0"))
                        {
                            instruction.fOp2 = registers.floatQi[instruction.r2];
                        }
                        else
                            instruction.fOp2 = instruction.r1.ToString();
                        if (!intSubRS.Busy)
                        {
                            intSubRS.instruction = instruction;
                            intSubRS.Busy = true;
                            return 2;
                        }
                    }
                    break;
                case 16:
                case 17:
                case 18:
                case 19:
                    if(!shiftOPS.Busy)
                    {
                        if (registers.intQi[instruction.r1].Equals("0"))
                        {
                            instruction.iOp1 = registers.intQi[instruction.r1];
                        }
                        else
                            instruction.iOp1 = instruction.r1.ToString();

                        if (registers.intQi[instruction.r2].Equals("0"))
                        {
                            instruction.iOp2 = registers.intQi[instruction.r2];
                        }
                        else
                            instruction.iOp2 = instruction.r1.ToString();
                        shiftOPS.instruction = instruction;
                        shiftOPS.Busy = true;
                        return 12;
                    }
                    break;
                case 20:            //Add instruction. Checks if it's float
                    if((instruction.isFloat))
                    {
                        if (!floatAddRS.Busy)
                        {
                            if (registers.floatQi[instruction.r1].Equals("0"))
                            {
                                instruction.fOp1 = registers.floatQi[instruction.r1];
                            }
                            else
                                instruction.fOp1 = instruction.r1.ToString();

                            if (registers.floatQi[instruction.r2].Equals("0"))
                            {
                                instruction.fOp2 = registers.floatQi[instruction.r2];
                            }
                            else
                                instruction.fOp2 = instruction.r1.ToString();
                            floatAddRS.instruction = instruction;
                            floatAddRS.Busy = true;
                            return 5;
                        }
                    }
                    else
                    {
                        if(!intAddRS.Busy)
                        {
                            if (registers.intQi[instruction.r1].Equals("0"))
                            {
                                instruction.iOp1 = registers.intQi[instruction.r1];
                            }
                            else
                                instruction.iOp1 = instruction.r1.ToString();

                            if (registers.intQi[instruction.r2].Equals("0"))
                            {
                                instruction.iOp2 = registers.intQi[instruction.r2];
                            }
                            else
                                instruction.iOp2 = instruction.r1.ToString();
                            intAddRS.instruction = instruction;
                            intAddRS.Busy = true;
                            return 1;
                        }
                    }
                    
                    break;
                case 21:            //Sub instruction. Checks if it's float
                    if ((instruction.isFloat))
                    {
                        if(!floatSubRS.Busy)
                        {
                            if (registers.floatQi[instruction.r1].Equals("0"))
                            {
                                instruction.fOp1 = registers.floatQi[instruction.r1];
                            }
                            else
                                instruction.fOp1 = instruction.r1.ToString();

                            if (registers.floatQi[instruction.r2].Equals("0"))
                            {
                                instruction.fOp2 = registers.floatQi[instruction.r2];
                            }
                            else
                                instruction.fOp2 = instruction.r1.ToString();
                            floatSubRS.instruction = instruction;
                            floatSubRS.Busy = true;
                            return 6;
                        }
                    }
                    else
                    {
                        if (!intSubRS.Busy)
                        {
                            if (registers.intQi[instruction.r1].Equals("0"))
                            {
                                instruction.iOp1 = registers.intQi[instruction.r1];
                            }
                            else
                                instruction.iOp1 = instruction.r1.ToString();

                            if (registers.intQi[instruction.r2].Equals("0"))
                            {
                                instruction.iOp2 = registers.intQi[instruction.r2];
                            }
                            else
                                instruction.iOp2 = instruction.r1.ToString();
                            intSubRS.instruction = instruction;
                            intSubRS.Busy = true;
                            return 2;
                        }
                    }
                    break;
                case 22:            //Mult instructions. Checks if it's float
                    if ((instruction.isFloat))
                    {
                        if (!floatMultRS.Busy)
                        {
                            if (registers.floatQi[instruction.r1].Equals("0"))
                            {
                                instruction.fOp1 = registers.floatQi[instruction.r1];
                            }
                            else
                                instruction.fOp1 = instruction.r1.ToString();

                            if (registers.floatQi[instruction.r2].Equals("0"))
                            {
                                instruction.fOp2 = registers.floatQi[instruction.r2];
                            }
                            else
                                instruction.fOp2 = instruction.r1.ToString();
                            floatMultRS.instruction = instruction;
                            floatMultRS.Busy = true;
                            return 7;
                        }
                    }
                    else
                    {
                        if (!intMultRS.Busy)
                        {
                            if (registers.intQi[instruction.r1].Equals("0"))
                            {
                                instruction.iOp1 = registers.intQi[instruction.r1];
                            }
                            else
                                instruction.iOp1 = instruction.r1.ToString();

                            if (registers.intQi[instruction.r2].Equals("0"))
                            {
                                instruction.iOp2 = registers.intQi[instruction.r2];
                            }
                            else
                                instruction.iOp2 = instruction.r1.ToString();
                            intMultRS.instruction = instruction;
                            intMultRS.Busy = true;
                            return 3;
                        }
                    }
                      
                    break;
                case 23:            //Div instructions. Checks if it's float
                    if ((instruction.isFloat))
                    {
                        if (!floatDivRS.Busy)
                        {
                            if (registers.floatQi[instruction.r1].Equals("0"))
                            {
                                instruction.fOp1 = registers.floatQi[instruction.r1];
                            }
                            else
                                instruction.fOp1 = instruction.r1.ToString();

                            if (registers.floatQi[instruction.r2].Equals("0"))
                            {
                                instruction.fOp2 = registers.floatQi[instruction.r2];
                            }
                            else
                                instruction.fOp2 = instruction.r1.ToString();
                            floatDivRS.instruction = instruction;
                            floatDivRS.Busy = true;
                            return 8;
                        }
                    }
                    else
                    {
                        if (!intDivRS.Busy)
                        {
                            if (registers.intQi[instruction.r1].Equals("0"))
                            {
                                instruction.iOp1 = registers.intQi[instruction.r1];
                            }
                            else
                                instruction.iOp1 = instruction.r1.ToString();

                            if (registers.intQi[instruction.r2].Equals("0"))
                            {
                                instruction.iOp2 = registers.intQi[instruction.r2];
                            }
                            else
                                instruction.iOp2 = instruction.r1.ToString();
                            intDivRS.instruction = instruction;
                            intDivRS.Busy = true;
                            return 4;
                        }
                    }
                    break;
                case 24:
                case 25:
                case 26:
                    if (!bitwiseOPRS.Busy)
                    {
                        if (registers.intQi[instruction.r1].Equals("0"))
                        {
                            instruction.iOp1 = registers.intQi[instruction.r1];
                        }
                        else
                            instruction.iOp1 = instruction.r1.ToString();

                        if (registers.intQi[instruction.r2].Equals("0"))
                        {
                            instruction.iOp2 = registers.intQi[instruction.r2];
                        }
                        else
                            instruction.iOp2 = instruction.r1.ToString();
                        bitwiseOPRS.instruction = instruction;
                        bitwiseOPRS.Busy = true;
                        return 9;
                    }
                    break;
                case 27:
                    if (!bitwiseOPRS.Busy)
                    {
                        if (registers.intQi[instruction.r1].Equals("0"))
                        {
                            instruction.iOp1 = registers.intQi[instruction.r1];
                        }
                        else
                            instruction.iOp1 = instruction.r1.ToString();
                        bitwiseOPRS.instruction = instruction;
                        bitwiseOPRS.Busy = true;
                        return 9;
                    }
                    break;


            }
            throw new NotImplementedException();
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
