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
            generateAssembly(ref assemblyString, IM);       //Populates instructionQueue and writes out assembly
            halted = true; //For testing purposes

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
                instructionQueue.Add(instruction);  //Fill up instruction queue
                print.buildAssemblyString(ref assemblyString, ref instruction); //Prints assembly string to the GUI
            }
        }
    }
}
