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
        List<Instruction> instructionQueue;
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
        public ReservationStation intAddRS;
        public ReservationStation intSubRS;
        public ReservationStation intMultRS;
        public ReservationStation intDivRS;
        public ReservationStation floatAddRS;
        public ReservationStation floatSubRS;
        public ReservationStation floatMultRS;
        public ReservationStation floatDivRS;
        public ReservationStation bitwiseOPRS;
        public ReservationStation loadOPRS;
        public ReservationStation storeOPRS;
        public ReservationStation branchOPS;
        public ReservationStation shiftOPS;
        public Instruction fetchInstruction;

        public int totalHazard, structuralHazard, dataHazard, controlHazard, RAW, WAR, WAW;
        public int totalCyclesStalled;
        public bool lastBranchDecision;
        //private Timer timer;



        /// <summary>Initializes a new instance of the <see cref="DynamicPipeline" /> class.</summary>
        public DynamicPipeline()
        {
            fetch = new Fetch();
            CU = new ControlUnit();
            print = new Printer();
            instructionQueue = new List<Instruction>();
            alu = new ALU();
            CU = new ControlUnit();
            EU = new ExecutionUnit();
            AM = new AccessMemory();
            WR = new WriteResult();
            CommonDataBus= new CommonDataBus();
            intAddFu = new IntAddFU();  
            intSubFu = new IntSubFU();
            intMultFu = new IntMultFU();
            intDivFu = new IntDivFU();
            floatAddFu = new FloatAddFU();
            floatSubFu = new FloatSubFU();
            floatMultFu = new FloatMultFU();
            floatDivFu = new FloatDivFU();
            bitwiseOPFU = new BitwiseOPFU();
            intAddRS = new ReservationStation("intAddRS");
            intSubRS = new ReservationStation("intSubRS");
            intMultRS = new ReservationStation("intMultRS");
            intDivRS = new ReservationStation("intDivRS");
            floatAddRS = new ReservationStation("floatAddRS");
            floatSubRS = new ReservationStation("floatSubRS");
            floatMultRS = new ReservationStation("floatMultRS");
            floatDivRS = new ReservationStation("floatDivRS");
            bitwiseOPRS = new ReservationStation("bitwiseOPRS");
            loadOPRS = new ReservationStation("loadOPRS");
            storeOPRS = new ReservationStation("storeOPRS");
            branchOPS = new ReservationStation("branchOPS");        //Reservation station solely for branches\
            shiftOPS = new ReservationStation("shiftOPS");


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
            //Fetch 1 instruction and send it to be decoded
            fetchInstruction = fetch.getNextInstruction(ref IM);
            //Decode instruction and add it to the instruction queue
            CU.decode(ref IM, ref fetchInstruction);
            fetchInstruction.cycle = 1; //Makes it start at cycle 1
            instructionQueue.Add(fetchInstruction);

            //Put the instructions into the reservation station. This should be the first cycle of the pipeline
            foreach(Instruction inst in instructionQueue) //Will run through instruction queue, instruction will bne take off the queue once commited
            {
                switch (inst.cycle) //Instuction will hold which cycle it's in
                {
                    //Populates reservationStations
                    case 1:
                        populateReservationStation(inst);
                        break;

                    //Send the instruction to its corresponding functional unit as long as there are no structural harzards present
                    //Open up reservation station to allow for more instructions to flow in
                    //Execute within the functional unit
                    case 2:

                        break;

                    //Store the answer and corresponding reservation name into the data bus
                    //This should be where the Write Result and Memory Read stages will be held
                    case 3:

                        break;
                    case 4:

                        break;
                    //Take instructions from the data bus and add them to the reorder buffer where instructions will be executed based on program counter
                    case 5:

                        break;

                }
            }
            
            generateAssembly(ref assemblyString, IM);       //Populates instructionQueue and writes out assembly
            halted = true; //For testing purposes

        }

        //Will use the opcode and flags to figure out which reservation station the instruction needs to be in
        private void populateReservationStation(Instruction instruction)
        {
            switch(instruction.opcode)
            {
                case 0:
                case 1:
                    break;      //NOT IMPLEMENTED, NOT SURE WHERE TO PUT HALT AND NOP

                case 2:         //Branch Instructions
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                    if(!branchOPS.Busy)
                    {

                        instruction.cycle = 2;
                        branchOPS.instruction = instruction;
                        branchOPS.Busy = true;
                        //Vj, Vk, Qj, Qk will be implemented here
                    }
                    break;
                case 15:
                case 10:        //Store instruction
                    if(!storeOPRS.Busy)
                    {
                        instruction.cycle = 2;
                        storeOPRS.instruction = instruction;
                        storeOPRS.Busy = true;
                        //Vj, Vk, Qj, Qk will be implemented here
                    }
                    break;
                case 9:         //Load Instructions
                case 11:
                case 12:
                    if(!loadOPRS.Busy)
                    {
                        instruction.cycle = 2;
                        loadOPRS.instruction = instruction;
                        loadOPRS.Busy = true;
                        //Vj, Vk, Qj, Qk will be implemented here
                    }
                    break;
                case 13:
                case 14:
                case 16:
                case 17:
                case 18:
                case 19:
                    if(!shiftOPS.Busy)
                    {
                        instruction.cycle = 2;
                        shiftOPS.instruction = instruction;
                        shiftOPS.Busy = true;
                    }
                    break;
                case 20:            //Add instruction. Checks if it's float
                    if((instruction.instrFlag & 2) == 0)
                    {
                        if (!floatAddRS.Busy)
                        {
                            instruction.cycle = 2;
                            floatAddRS.instruction = instruction;
                            floatAddRS.Busy = true;
                        }
                    }
                    else
                    {
                        if(!intAddRS.Busy)
                        {
                            instruction.cycle = 2;
                            intAddRS.instruction = instruction;
                            intAddRS.Busy = true;
                        }
                    }
                    
                    break;
                case 21:            //Sub instruction. Checks if it's float
                    if ((instruction.instrFlag & 2) == 0)
                    {
                        if(!floatSubRS.Busy)
                        {
                            instruction.cycle = 2;
                            floatSubRS.instruction = instruction;
                            floatSubRS.Busy = true;
                        }
                    }
                    else
                    {
                        if (!intSubRS.Busy)
                        {
                            instruction.cycle = 2;
                            intSubRS.instruction = instruction;
                            intSubRS.Busy = true;
                        }
                    }
                    break;
                case 22:            //Mult instructions. Checks if it's float
                    if ((instruction.instrFlag & 2) == 0)
                    {
                        if (!floatMultRS.Busy)
                        {
                            instruction.cycle = 2;
                            floatMultRS.instruction = instruction;
                            floatMultRS.Busy = true;
                        }
                    }
                    else
                    {
                        if (!intMultRS.Busy)
                        {
                            instruction.cycle = 2;
                            intMultRS.instruction = instruction;
                            intMultRS.Busy = true;
                        }
                    }
                      
                    break;
                case 23:            //Div instructions. Checks if it's float
                    if ((instruction.instrFlag & 2) == 0)
                    {
                        if (!floatDivRS.Busy)
                        {
                            instruction.cycle = 2;
                            floatDivRS.instruction = instruction;
                            floatDivRS.Busy = true;
                        }
                    }
                    else
                    {
                        if (!intDivRS.Busy)
                        {
                            instruction.cycle = 2;
                            intDivRS.instruction = instruction;
                            intDivRS.Busy = true;
                        }
                    }
                    break;
                case 24:
                case 25:
                case 26:
                case 27:
                    if (!shiftOPS.Busy)
                    {
                        instruction.cycle = 2;
                        shiftOPS.instruction = instruction;
                        shiftOPS.Busy = true;
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
