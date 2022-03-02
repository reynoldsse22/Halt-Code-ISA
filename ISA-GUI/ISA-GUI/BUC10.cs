// ---------------------------------------------------------------------------
// File name: BUC10.cs
// Project name: ISA-GUI
// ---------------------------------------------------------------------------
// Creators: Samuel Reynolds, Nick Farmer, Carlos Ortiz, & Brandon Beaudry						
// Course-Section: CSCI 4717-201
// Creation Date: 2/19/22		
// ---------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

// THIS CLASS IS ONLY TO BE USED TO KICKOFF THE PIPELINE AND UPDATE THE GUI //

namespace ISA_GUI
{
    /**
	* Class Name: BUC10 <br>
	* Class Purpose: To kickoff the pipeline process and update GUI elements
	* 
	* <hr>
	* Date created: 2/19/21 <br>
	* @author Samuel Reynolds
	*/
    public partial class BUC10 : Form
    {
        CentralProcessingUnit cpu = new CentralProcessingUnit();                //The CPU
        ConfigCycle config = new ConfigCycle();                                 //The configuration file
        bool halted = true;                                                     //Determines if the program has been halted yet
        List<string> program;                                                   //Holds the user inputted instructions
        Instruction[] stages = new Instruction[5];
        StringBuilder assemblyOutput = new StringBuilder();                     //Mutable string for the assembly instructions
        StringBuilder pipelineOutput = new StringBuilder(                       //Mutable string for the pipeline output
            "   Team: Beaudry, Farmer, Ortiz, Reynolds\n" +
            "Project: Pipeline ISA Implementation\n" +
            "---------------------------------------------------------------\n\n");
        StringBuilder decodedString = new StringBuilder(                        //Mutable string for the decoded instructions
            "Program  Inst Inst Instruct                            Address/\n" +
            "Counter  Flag Spec Mnemonic      Type FReg SReg DReg  Immediate\n" +
            "-------- ---- ---- -------- --------- ---- ---- ---- ----------");
        /**
	    * Method Name: BUC10 <br>
	    * Method Purpose: Class constructor
	    * 
	    * <hr>
	    * Date created: 2/19/21 <br>
	    * @author Samuel Reynolds
	    */
        public BUC10()
        {
            InitializeComponent();
            this.Focus();
        }

        /**
		 * Method Name: BUC10Load <br>
		 * Method Purpose: Clear the program on load
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 *   @param  object sender
		 *   @param  EventArgs e
		 */
        private void BUC10_Load(object sender, EventArgs e)
        {
            clearProgram();     //On load, initialize all GUI elements and values to their starting value
            stage1Text.SelectionAlignment = HorizontalAlignment.Center;
            stage2Text.SelectionAlignment = HorizontalAlignment.Center;
            stage3Text.SelectionAlignment = HorizontalAlignment.Center;
            stage4Text.SelectionAlignment = HorizontalAlignment.Center;
            stage5Text.SelectionAlignment = HorizontalAlignment.Center;
        }

        /**
		 * Method Name: runButton_Click <br>
		 * Method Purpose: Starts and runs the whole program when the run button is clicked
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 *   @param  object sender
		 *   @param  EventArgs e
		 */
        private void runButton_Click(object sender, EventArgs e)
        {
            clearProgram();         //Initialize all GUI elements and values to their starting value
            halted = false;         //Set loop control variable to false since this a fresh run of the program
            try
            {
                program = getInput();       //get the input from the user
                cpu.runCycle(program, false, ref assemblyOutput, ref decodedString, ref pipelineOutput, ref halted, ref config, ref stages);      //Run the program all the way through. Stepthrough flag is false
            }
            catch (Exception)  //Catch any errors when getting input from user or decoding invalid instructions
            {
                string message = "Invalid Instruction!";
                AssemblerListingTextBox.Text = message;
                StatsTextBox.Text = message;
                pipelineTextBox.Text = message;
                AssemblyTextBox.Text = message;
                halted = true;
                return;
            }
            updateGUI();        //update the GUI to reflect changes
        }

        /**
		 * Method Name: stepthroughButton_Click <br>
		 * Method Purpose: Starts and runs program and executes only one instruction at a time when the debug button is clicked
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 *   @param  object sender
		 *   @param  EventArgs e
		 */
        private void stepthroughButton_Click(object sender, EventArgs e)
        {
            if(cpu.IM.instructions.Count > 0 && halted == true)
            {
                clearProgram();
                halted = false;
                updateGUI();
            }
            else if(halted == true)              //If halted is true, then start program from scratch
            {
                clearProgram();
                halted = false;            
                try
                {
                    List<string> program = getInput();      //get instructions from user
                    cpu.runCycle(program, true, ref assemblyOutput, ref decodedString, ref pipelineOutput, ref halted, ref config, ref stages);      //Run the program one cycle at a time. Stepthrough flag is true
                    updateGUI();        //update the GUI
                }
                catch (Exception)       //Catch any errors when getting input from user or decoding invalid instructions
                {
                    string message = "Invalid Instruction!";
                    AssemblerListingTextBox.Text = message;
                    StatsTextBox.Text = message;
                    pipelineTextBox.Text = message;
                    halted = true;
                    return;
                }  
            }
            else   //if halted is false, the program is already running. Just send one instruction at a time through the pipeline. 
            {
                try
                {
                    cpu.runCycle(program, true, ref assemblyOutput, ref decodedString, ref pipelineOutput, ref halted, ref config, ref stages);      //Run the program one cycle at a time. Stepthrough flag is true
                }
                catch (Exception)       //Catch any errors when getting input from user or decoding invalid instructions
                {
                    string message = "Invalid Instruction!";
                    AssemblerListingTextBox.Text = message;
                    StatsTextBox.Text = message;
                    pipelineTextBox.Text = message;
                    AssemblyTextBox.Text = message;
                    halted = true;
                    return;
                }
                updateGUI();        //update the GUI to reflect new changes
            }
            
        }

        /**
		 * Method Name: restartButton_Click <br>
		 * Method Purpose: Refreshes the program so that it can be started again. 
		 * 
		 * <br>
		 * Date created: 2/28/22 <br>
		 * <hr>
		 *   @param  object sender
		 *   @param  EventArgs e
		 */
        private void resetButton_Click(object sender, EventArgs e)
        {
            clearProgram();         //Initialize all GUI elements and values to their starting value
            halted = false;
            updateGUI();
        }

        /**
		 * Method Name: getInput <br>
		 * Method Purpose: Gets the instructions from the user and splits it on the space
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 * Return: program - a list of two byte instructions
		 */
        private List<string> getInput()
        {
            program = new List<string>();
            string pr = "";
            try
            {
                if(!InputBox.Text.Equals(""))                               //If the input is null, throw an exception
                {
                    pr = InputBox.Text;
                    program = pr.Split(' ').ToList();                        //Splits the given program into different instructions
                }
                else
                {
                    throw new Exception("Invalid Instruction!");
                }
                
            }
            catch (Exception ex)                                        //If any error in parsing, catch exception and pass to the parent
            {
                throw new Exception(ex.Message);
            }
            return program;
        }

        /**
		 * Method Name: clearProgram <br>
		 * Method Purpose: Resets all values for a fresh program
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 */
        private void clearProgram()
        {
            clearPipeline();
            decodedString.Clear();
            decodedString.Append(
            "Program  Inst Inst Instruct                            Address/\n" +
            "Counter  Flag Spec Mnemonic      Type FReg SReg DReg  Immediate\n" +
            "-------- ---- ---- -------- --------- ---- ---- ---- ----------");
            pipelineOutput.Clear();
            pipelineOutput.Append(
            "   Team: Beaudry, Farmer, Ortiz, Reynolds\n" +
            "Project: Pipeline ISA Implementation\n" +
            "---------------------------------------------------------------\n\n");

            pipelineOutput.Append(
                "                Inst.     Decode    Execute   Access    Write   \n" +
                "Instruction     Fetch     Read Reg  Calc Adr  Memory    Register\n" +
                "------------    ------   ---------- --------- ------- ----------\n"
                );
            assemblyOutput.Clear();

            cpu.IM.ProgramCounter = 0;
            cpu.IM.CurrentInstruction = 0;
            cpu.IM.instructions.Clear();
            cpu.CU.instructionsProcessed = 0;
            cpu.CU.totalInstructions = 0;
            cpu.CU.ALUInstructionCount = 0;
            cpu.CU.memoryInstructionCount = 0;
            cpu.CU.controlInstructionCount = 0;
            StatsTextBox.Text = "";
            clearRegandMem();
            updateGUI();
        }

        /**
		 * Method Name: clearRegandMem <br>
		 * Method Purpose: Resets the values in the registers and memory
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 */
        private void clearRegandMem()
        {
            //Clear registers
            for (int i = 0; i < 7; i++)
            {
                cpu.registers.intRegisters[i] = 0;
                cpu.registers.floatRegisters[i] = 0;
            }
            cpu.registers.ASPR = 0;

            //clear main memory
            Array.Clear(cpu.dataMemory.MainMemory, 0, cpu.dataMemory.MainMemory.Length);
        }

        /**
        * Method Name: clearPipeline <br>
        * Method Purpose: Resets the values in the pipeline
        * 
        * <br>
        * Date created: 2/19/22 <br>
        * <hr>
        */
        private void clearPipeline()
        {
            for(int i = 0; i<5; i++)    
                stages[i] = null;
        }

        /**
		 * Method Name: updateGUI <br>
		 * Method Purpose: Updates all the GUI elements
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 */
        private void updateGUI()
        {
            updatePipeline();
            setRegisters();
            setStatistics();
            setMemoryBox();
            AssemblerListingTextBox.Text = decodedString.ToString();
            AssemblyTextBox.Text = assemblyOutput.ToString();
            pipelineTextBox.Text = pipelineOutput.ToString();
        }

        /**
		 * Method Name: setRegisters <br>
		 * Method Purpose: Updates all the register values in the GUI
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 */
        private void setRegisters()
        {
            //Initialize the hexidecimal text field to 0
            //Pad left ensures that the value will be 4 digits.
            r0Hex.Text = "0x" + cpu.registers.intRegisters[0].ToString("X").PadLeft(6, '0');
            r1Hex.Text = "0x" + cpu.registers.intRegisters[1].ToString("X").PadLeft(6, '0');
            r2Hex.Text = "0x" + cpu.registers.intRegisters[2].ToString("X").PadLeft(6, '0');
            r3Hex.Text = "0x" + cpu.registers.intRegisters[3].ToString("X").PadLeft(6, '0');
            r4Hex.Text = "0x" + cpu.registers.intRegisters[4].ToString("X").PadLeft(6, '0');
            r5Hex.Text = "0x" + cpu.registers.intRegisters[5].ToString("X").PadLeft(6, '0');
            r6Hex.Text = "0x" + cpu.registers.intRegisters[6].ToString("X").PadLeft(6, '0');
            f0Hex.Text = "0x" + (BitConverter.ToString(BitConverter.GetBytes(cpu.registers.floatRegisters[0]))).PadLeft(6, '0').Replace("-", "").Remove(0,2);
            f1Hex.Text = "0x" + (BitConverter.ToString(BitConverter.GetBytes(cpu.registers.floatRegisters[1]))).PadLeft(6, '0').Replace("-", "").Remove(0,2);
            f2Hex.Text = "0x" + (BitConverter.ToString(BitConverter.GetBytes(cpu.registers.floatRegisters[2]))).PadLeft(6, '0').Replace("-", "").Remove(0, 2);
            f3Hex.Text = "0x" + (BitConverter.ToString(BitConverter.GetBytes(cpu.registers.floatRegisters[3]))).PadLeft(6, '0').Replace("-", "").Remove(0, 2);
            f4Hex.Text = "0x" + (BitConverter.ToString(BitConverter.GetBytes(cpu.registers.floatRegisters[4]))).PadLeft(6, '0').Replace("-", "").Remove(0, 2);
            f5Hex.Text = "0x" + (BitConverter.ToString(BitConverter.GetBytes(cpu.registers.floatRegisters[5]))).PadLeft(6, '0').Replace("-", "").Remove(0, 2);
            f6Hex.Text = "0x" + (BitConverter.ToString(BitConverter.GetBytes(cpu.registers.floatRegisters[6]))).PadLeft(6, '0').Replace("-", "").Remove(0, 2);
            asprHex.Text = "0x" + cpu.registers.ASPR.ToString("X").PadLeft(6, '0');
            cirHex.Text = "0x" + cpu.IM.CurrentInstruction.ToString("X").PadLeft(6, '0');
            pcHex.Text = "0x" + cpu.IM.ProgramCounter.ToString("X").PadLeft(6, '0');

            //Initialize the decimal register text fields to 0
            r0Dec.Text = cpu.registers.intRegisters[0].ToString();
            r1Dec.Text = cpu.registers.intRegisters[1].ToString();
            r2Dec.Text = cpu.registers.intRegisters[2].ToString();
            r3Dec.Text = cpu.registers.intRegisters[3].ToString();
            r4Dec.Text = cpu.registers.intRegisters[4].ToString();
            r5Dec.Text = cpu.registers.intRegisters[5].ToString();
            r6Dec.Text = cpu.registers.intRegisters[6].ToString();
            f0Dec.Text = cpu.registers.floatRegisters[0].ToString();
            f1Dec.Text = cpu.registers.floatRegisters[1].ToString();
            f2Dec.Text = cpu.registers.floatRegisters[2].ToString();
            f3Dec.Text = cpu.registers.floatRegisters[3].ToString();
            f4Dec.Text = cpu.registers.floatRegisters[4].ToString();
            f5Dec.Text = cpu.registers.floatRegisters[5].ToString();
            f6Dec.Text = cpu.registers.floatRegisters[6].ToString();
            asprDec.Text = cpu.registers.ASPR.ToString();
            cirDec.Text = cpu.IM.CurrentInstruction.ToString();
            pcDec.Text = cpu.IM.ProgramCounter.ToString();

            //Initialize the Z and C flags based on the ASPR register
            if ((cpu.registers.ASPR & 2) == 2)
                ZFlagBox.Text = 1.ToString();
            else
                ZFlagBox.Text = 0.ToString();

            if ((cpu.registers.ASPR & 1) == 1)
                cFlagBox.Text = 1.ToString();
            else
                cFlagBox.Text = 0.ToString();
        }

        /**
		 * Method Name: setStatistics <br>
		 * Method Purpose: Sets the statistics field in the GUI
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 */
        private void setStatistics()
        {
            int totalInst = cpu.CU.totalInstructions;
            if (totalInst == 0)
                return;
            StringBuilder statistics = new StringBuilder("");
            statistics.Append("Summary Statistics\n");
            statistics.Append("------------------\n");
            statistics.Append(String.Format("Total instructions:              {0}\n", totalInst));                       //since we always go by 2's total instructions was pretty simple
            statistics.Append(String.Format("Control instructions:            {0}, {1}%\n", cpu.CU.controlInstructionCount, Math.Round((double)cpu.CU.controlInstructionCount / totalInst * 100, 2)));
            statistics.Append(String.Format("Arithmetic & logic instructions: {0}, {1}%\n", cpu.CU.ALUInstructionCount, Math.Round((double)cpu.CU.ALUInstructionCount / totalInst * 100, 2)));
            statistics.Append(String.Format("Memory instructions:             {0}, {1}%\n", cpu.CU.memoryInstructionCount, Math.Round((double)cpu.CU.memoryInstructionCount / totalInst * 100, 2)));

            StatsTextBox.Text = statistics.ToString();
        }


        private void updatePipeline()
        {
            int instructionHex;

                if(stages[0] != null)
                {
                    instructionHex = (stages[0].binInstruction[2] + (stages[0].binInstruction[1] << 8) + (stages[0].binInstruction[0] << 16));
                    stage1Text.Text = Regex.Replace(instructionHex.ToString("X").PadLeft(6, '0'), @"(.{2})", "$1 ");
                }
                if (stages[1] != null)
                {
                    instructionHex = (stages[1].binInstruction[2] + (stages[1].binInstruction[1] << 8) + (stages[1].binInstruction[0] << 16));
                    stage2Text.Text = Regex.Replace(instructionHex.ToString("X").PadLeft(6, '0'), @"(.{2})", "$1 ");
            }
                if (stages[2] != null)
                {
                    instructionHex = (stages[2].binInstruction[2] + (stages[2].binInstruction[1] << 8) + (stages[2].binInstruction[0] << 16));
                    stage3Text.Text = Regex.Replace(instructionHex.ToString("X").PadLeft(6, '0'), @"(.{2})", "$1 ");
            }
                if (stages[3] != null)
                {
                    instructionHex = (stages[3].binInstruction[2] + (stages[3].binInstruction[1] << 8) + (stages[3].binInstruction[0] << 16));
                    stage4Text.Text = Regex.Replace(instructionHex.ToString("X").PadLeft(6, '0'), @"(.{2})", "$1 ");
            }
                if (stages[4] != null)
                {
                    instructionHex = (stages[4].binInstruction[2] + (stages[4].binInstruction[1] << 8) + (stages[4].binInstruction[0] << 16));
                    stage5Text.Text = Regex.Replace(instructionHex.ToString("X").PadLeft(6, '0'), @"(.{2})", "$1 ");
            }
            
            
        }

        /**
		 * Method Name: setMemoryBox <br>
		 * Method Purpose: Sets the GUI with the contents of Main Memory
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 */
        private void setMemoryBox()
        {
            int offset = 0;
            StringBuilder line = new StringBuilder("ADDRESS  |  0  1  2  3  4  5  6  7  8  9  A  B  C  D  E  F |  ASCII ENCODING  |\n");
            line.Append("--------------------------------------------------------------------------------\n");
            int index = 0;
            for (int i = 0; i < (2000); i++)
            {
                line.Append("0x" + offset.ToString("x").PadLeft(6, '0').ToUpper() + " ");
                line.Append("| ");
                for (int j = 0; j < 16; j++)
                {
                    if (cpu.IM.instructions.Any())
                        line.Append(cpu.dataMemory.MainMemory[index].ToString("x").PadLeft(2, '0').ToUpper());
                    else
                        line.Append("00");
                    line.Append(" ");
                    index++;
                }
                line.Append("| ");
                index -= 16;
                for (int j = 0; j < 16; j++)
                {
                    if (cpu.IM.instructions.Any())
                        if ((cpu.dataMemory.MainMemory[index] < 32) || (cpu.dataMemory.MainMemory[index] > 255))
                            line.Append('.');
                        else
                            line.Append(Convert.ToChar(cpu.dataMemory.MainMemory[index]));
                    else
                        line.Append('.');
                    index++;
                }
                offset += 16;
                line.Append(" |\n");
            }

            MemoryText.Text = line.ToString();
        }

        private void RunButton_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Run program", RunButton);
        }

        private void stepthroughButton_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Step through one cycle", stepthroughButton);
        }

        private void resetButton_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Reset", resetButton);
        }
    }

}
