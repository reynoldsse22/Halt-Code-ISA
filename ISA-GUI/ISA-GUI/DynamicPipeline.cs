using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ISA_GUI
{
    internal class DynamicPipeline
    {
        Fetch fetch;
        ControlUnit CU;
        Printer print;
        List<Instruction> instructionQueue;


        /// <summary>Initializes a new instance of the <see cref="DynamicPipeline" /> class.</summary>
        public DynamicPipeline()
        {
            fetch = new Fetch();
            CU = new ControlUnit();
            print = new Printer();
            instructionQueue = new List<Instruction>();
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
