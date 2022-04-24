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
using System.IO;

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
        Assembler assembler = new Assembler();
        Disassembler disassembler = new Disassembler();
        bool halted = true;                                                     //Determines if the program has been halted yet
        List<string> program;                                                   //Holds the user inputted instructions
        Instruction[] stages = new Instruction[5];
        StringBuilder assemblyOutput = new StringBuilder();                     //Mutable string for the assembly instructions
        StringBuilder pipelineOutput = new StringBuilder(                       //Mutable string for the pipeline output
            "   Team: Beaudry, Farmer, Ortiz, Reynolds\n" +
            "Project: Static Pipeline ISA Implementation\n" +
            "---------------------------------------------------------------\n\n");
        StringBuilder decodedString = new StringBuilder(                        //Mutable string for the decoded instructions
            "Program  Inst Inst Instruct                            Address/\n" +
            "Counter  Flag Spec Mnemonic      Type FReg SReg DReg  Immediate\n" +
            "-------- ---- ---- -------- --------- ---- ---- ---- ----------");

        StringBuilder cacheString = new StringBuilder(
            "Cache Statistics\n"+
            "----------------\n"+
            "Address       Offset      index     tag    Data \n"+
            "-------       ------      -----     ---    ----");

       
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
            CenterToScreen();
            clearProgram();     //On load, initialize all GUI elements and values to their starting value
            stage1Text.SelectionAlignment = HorizontalAlignment.Center;
            stage2Text.SelectionAlignment = HorizontalAlignment.Center;
            stage3Text.SelectionAlignment = HorizontalAlignment.Center;
            stage4Text.SelectionAlignment = HorizontalAlignment.Center;
            stage5Text.SelectionAlignment = HorizontalAlignment.Center;
            currentCycleText.SelectionAlignment = HorizontalAlignment.Center;
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
                if (objectCode.SelectedTab == objectCodeBox)
                {
                    List<string> program = getMachineCode();      //get instructions from user
                }
                else
                {
                    List<string> program = getAssembly();      //get instructions from user
                }
                if (!config.dynamicPipelineSet)
                {
                    while (!halted)
                    {
                        cpu.runStaticPipeline(program, false, ref assemblyOutput, ref decodedString, ref pipelineOutput, ref halted, ref config, ref stages);      //Run the program all the way through. Stepthrough flag is false
                    }
                }
                else
                {
                    while (!halted)
                    {

                        cpu.runDynamicPipeline(program, false, ref assemblyOutput, ref decodedString, ref pipelineOutput, ref halted, ref config, ref cacheString); //Calling Dynamic Pipeline
                    }
                    //return;//ELSE STATEMENT FOR DYNAMIC PIPELINE GOES HERE
                }
            }
            catch (Exception)  //Catch any errors when getting input from user or decoding invalid instructions
            {
                string message = "Invalid Instruction!";
                AssemblerListingTextBox.Text = message;
                StatsTextBox.Text = message;
                pipelineTextBox.Text = message;
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
                    if (objectCode.SelectedTab == objectCodeBox)
                    {
                        List<string> program = getMachineCode();      //get instructions from user
                    }
                    else
                    {
                        List<string> program = getAssembly();      //get instructions from user
                    }
                    if (!config.dynamicPipelineSet)
                        cpu.runStaticPipeline(program, true, ref assemblyOutput, ref decodedString, ref pipelineOutput, ref halted, ref config, ref stages);      //Run the program one cycle at a time. Stepthrough flag is true
                    else
                        cpu.runDynamicPipeline(program, true, ref assemblyOutput, ref decodedString, ref pipelineOutput, ref halted, ref config, ref cacheString); //Calling Dynamic Pipeline

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
                    if (!config.dynamicPipelineSet)
                        cpu.runStaticPipeline(program, true, ref assemblyOutput, ref decodedString, ref pipelineOutput, ref halted, ref config, ref stages);      //Run the program one cycle at a time. Stepthrough flag is true
                    else
                        cpu.runDynamicPipeline(program, true, ref assemblyOutput, ref decodedString, ref pipelineOutput, ref halted, ref config, ref cacheString); //Calling Dynamic Pipeline
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
		 * Return: program - a list of three byte instructions
		 */
        private List<string> getMachineCode()
        {
            program = new List<string>();
            string assembly = "";
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

            assembly = disassembler.Disassemble(program);
            assemblerTextBox.Text = assembly;
            return program;
        }

        /**
		 * Method Name: getInput <br>
		 * Method Purpose: Gets the instructions from the user and splits it on the space
		 * 
		 * <br>
		 * Date created: 2/19/22 <br>
		 * <hr>
		 * Return: program - a list of three byte instructions
		 */
        private List<string> getAssembly()
        {
            List<string> assembly = new List<string>();
            program = new List<string>();
            string pr = "";
            string machineOutput = "";
            try
            {
                if (!assemblerTextBox.Text.Equals(""))                               //If the input is null, throw an exception
                {
                    pr = assemblerTextBox.Text;
                    assembly = pr.Split('\n').ToList();                        //Splits the given program into different instructions
                }
                else
                {
                    throw new Exception("Invalid Instruction!");
                }

                program = assembler.Assemble(assembly);

                for(int x = 0; x < program.Count; x++)
                {
                    if (x == 0)
                        machineOutput += program[x];
                    else
                        machineOutput += ' ' + program[x];
                }
                InputBox.Text = machineOutput;
            }
            catch (Exception)                                        //If any error in parsing, catch exception and pass to the parent
            {
                throw new Exception("Invalid Instruction!");
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
            "Project: Static Pipeline ISA Implementation\n" +
            "---------------------------------------------------------------\n\n");
            if (!config.dynamicPipelineSet)
            {
                pipelineOutput.Append(
                "                       Inst.   Decode/ Execute/  Access  Write To\n" +
                "     Instruction       Fetch  Read Reg Calc Adr  Memory  Register\n" +
                "--------------------- ------- -------- -------- ------- ---------");
            }
            else
            {
                pipelineOutput.Append(
                "                                        Memory  Writes\n" +
                "     Instruction      Issues  Executes   Read   Result  Commits\n" +
                "--------------------- ------- -------- -------- ------- ---------");
            }
                
            assemblyOutput.Clear();

            cpu.IM.ProgramCounter = 0;
            cpu.IM.CurrentInstruction = 0;
            cpu.IM.instructions.Clear();

            if(!config.dynamicPipelineSet)
            {
                cpu.SP.CU.instructionsProcessed = 0;
                cpu.SP.CU.totalInstructions = 0;
                cpu.SP.CU.ALUInstructionCount = 0;
                cpu.SP.CU.memoryInstructionCount = 0;
                cpu.SP.CU.controlInstructionCount = 0;
                resetStaticPipeline();

            }
            else
            {
                cpu.DP.CU.instructionsProcessed = 0;
                cpu.DP.CU.totalInstructions = 0;
                cpu.DP.CU.ALUInstructionCount = 0;
                cpu.DP.CU.memoryInstructionCount = 0;
                cpu.DP.CU.controlInstructionCount = 0;
                resetDynamicPipeline();
            }

            if(config.cachingSet)
            {
                cpu.DP.DC.clearCache();
                cacheString.Clear();
                cacheString.Append(
            "Cache Statistics\n" +
            "----------------\n" +
            "Address       Offset      index     tag    Data \n" +
            "-------       ------      -----     ---    ----");
            }

            StatsTextBox.Text = "";
            pipelineStatsTextBox.Text = "";
            cacheStatsTextBox.Text = "";
            currentCycleText.SelectionAlignment = HorizontalAlignment.Center;
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
		 * Method Name: resetPipeline <br>
		 * Method Purpose: Resets the pipeline functional unit variables
		 * 
		 * <br>
		 * Date created: 3/2/22 <br>
		 * <hr>
		 */
        private void resetStaticPipeline()
        {
            cpu.SP.fetch.inProgress = false;
            cpu.SP.fetch.occupied = false;
            cpu.SP.fetch.success = false;
            cpu.SP.CU.inProgress = false;
            cpu.SP.CU.occupied = false;
            cpu.SP.CU.success = false;
            cpu.SP.EU.inProgress = false;
            cpu.SP.EU.occupied = false;
            cpu.SP.EU.success = false;
            cpu.SP.AM.inProgress = false;
            cpu.SP.AM.occupied = false;
            cpu.SP.AM.success = false;
            cpu.SP.WR.inProgress = false;
            cpu.SP.WR.occupied = false;
            cpu.SP.WR.success = false;
            cpu.SP.cycleCount = 0;
        }


        private void resetDynamicPipeline()
        {
            cpu.DP.fetch.inProgress = false;
            cpu.DP.fetch.occupied = false;
            cpu.DP.fetch.success = false;
            cpu.DP.CU.inProgress = false;
            cpu.DP.CU.occupied = false;
            cpu.DP.CU.success = false;
            cpu.DP.EU.inProgress = false;
            cpu.DP.EU.occupied = false;
            cpu.DP.EU.success = false;
            cpu.DP.AM.inProgress = false;
            cpu.DP.AM.occupied = false;
            cpu.DP.AM.success = false;
            cpu.DP.WR.inProgress = false;
            cpu.DP.WR.occupied = false;
            cpu.DP.WR.success = false;
            cpu.DP.clearDynamicPipeline();
            cpu.registers.clearRegistersQI();
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

            if (!config.dynamicPipelineSet)
            {
                cpu.SP.cycleCount = 0;
                cpu.SP.WAR = 0;
                cpu.SP.WAW = 0;
                cpu.SP.RAW = 0;
                cpu.SP.fetchStalled = 0;
                cpu.SP.decodeStalled = 0;
                cpu.SP.executeStalled = 0;
                cpu.SP.accessMemStalled = 0;
                cpu.SP.writeRegStalled = 0;
                cpu.SP.dataHazard = 0;
                cpu.SP.controlHazard = 0;
                cpu.SP.structuralHazard = 0;
                cpu.SP.totalHazard = 0;
                stage1Text.Text = "";
                stage2Text.Text = "";
                stage3Text.Text = "";
                stage4Text.Text = "";
                stage5Text.Text = "";
            }
            else
            {
                cpu.DP.cycleCount = 0;
                cpu.DP.reservationStationDelay = 0;
                cpu.DP.reorderBufferDelay = 0;
                cpu.DP.trueDependenceDelay = 0;
                issueStageText.Text = "";
                commitStageText.Text = "";
            }
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
            updatePipelineGUIElements();
            if (!config.dynamicPipelineSet)
                updateStaticPipeline();
            else
                updateDynamicPipeline();
            setRegisters();
            setStatistics();
            setMemoryBox();
            if(!config.dynamicPipelineSet)
            {
                currentCycleText.Text = cpu.SP.cycleCount.ToString();

            }
            else
            {
                updateRegisterQiText();
            }
            AssemblerListingTextBox.Text = decodedString.ToString();
            pipelineTextBox.Text = pipelineOutput.ToString();
            cacheStatsTextBox.Text = cacheString.ToString();
        }

        private void updatePipelineGUIElements()
        {
            if (config.dynamicPipelineSet)
            {
                staticPanel.Visible = false;
                dynamicPanel.Visible = true;
            }
            else
            {
                staticPanel.Visible = true;
                dynamicPanel.Visible = false;
            }
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
            r7Dec.Text = cpu.registers.intRegisters[7].ToString();
            r8Dec.Text = cpu.registers.intRegisters[8].ToString();
            r9Dec.Text = cpu.registers.intRegisters[9].ToString();
            r10Dec.Text = cpu.registers.intRegisters[10].ToString();
            r11Dec.Text = cpu.registers.intRegisters[11].ToString();
            r12Dec.Text = cpu.registers.intRegisters[12].ToString();
            r13Dec.Text = cpu.registers.intRegisters[13].ToString();
            r14Dec.Text = cpu.registers.intRegisters[14].ToString();
            r15Dec.Text = cpu.registers.intRegisters[15].ToString();

            f0Dec.Text = cpu.registers.floatRegisters[0].ToString();
            f1Dec.Text = cpu.registers.floatRegisters[1].ToString();
            f2Dec.Text = cpu.registers.floatRegisters[2].ToString();
            f3Dec.Text = cpu.registers.floatRegisters[3].ToString();
            f4Dec.Text = cpu.registers.floatRegisters[4].ToString();
            f5Dec.Text = cpu.registers.floatRegisters[5].ToString();
            f6Dec.Text = cpu.registers.floatRegisters[6].ToString();
            f7Dec.Text = cpu.registers.floatRegisters[7].ToString();
            f8Dec.Text = cpu.registers.floatRegisters[8].ToString();
            f9Dec.Text = cpu.registers.floatRegisters[9].ToString();
            f10Dec.Text = cpu.registers.floatRegisters[10].ToString();
            f11Dec.Text = cpu.registers.floatRegisters[11].ToString();
            f12Dec.Text = cpu.registers.floatRegisters[12].ToString();
            f13Dec.Text = cpu.registers.floatRegisters[13].ToString();
            f14Dec.Text = cpu.registers.floatRegisters[14].ToString();
            f15Dec.Text = cpu.registers.floatRegisters[15].ToString();

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
            StringBuilder statistics = new StringBuilder("");
            StringBuilder pipelineStats = new StringBuilder("");

            if (!config.dynamicPipelineSet)
            {
                int totalInst = cpu.SP.CU.totalInstructions;
                if (totalInst == 0)
                    return;
                statistics.Append("Summary Statistics\n");
                statistics.Append("------------------\n");
                statistics.Append(String.Format("Total instructions:              {0}\n", totalInst));                       //since we always go by 2's total instructions was pretty simple
                statistics.Append(String.Format("Control instructions:            {0}, {1}%\n", cpu.SP.CU.controlInstructionCount, Math.Round((double)cpu.SP.CU.controlInstructionCount / totalInst * 100, 2)));
                statistics.Append(String.Format("Arithmetic & logic instructions: {0}, {1}%\n", cpu.SP.CU.ALUInstructionCount, Math.Round((double)cpu.SP.CU.ALUInstructionCount / totalInst * 100, 2)));
                statistics.Append(String.Format("Memory instructions:             {0}, {1}%\n", cpu.SP.CU.memoryInstructionCount, Math.Round((double)cpu.SP.CU.memoryInstructionCount / totalInst * 100, 2)));

                pipelineStats.Append("Pipeline Statistics\n");
                pipelineStats.Append("------------------\n");
                pipelineStats.Append(String.Format("Total Cycles:           {0}\n", cpu.SP.cycleCount - 1));
                pipelineStats.Append("\nHazards\n");
                pipelineStats.Append("-------\n");
                pipelineStats.Append(String.Format("structural:             {0}\n", cpu.SP.structuralHazard));
                pipelineStats.Append(String.Format("data:                   {0}\n", cpu.SP.dataHazard));
                pipelineStats.Append(String.Format("control:                {0}\n", cpu.SP.controlHazard));
                pipelineStats.Append(String.Format("Total:                  {0}\n\n", cpu.SP.structuralHazard + cpu.SP.dataHazard + cpu.SP.controlHazard));
                pipelineStats.Append("Dependencies\n");
                pipelineStats.Append("------------\n");
                pipelineStats.Append(String.Format("read-after-write:       {0}\n", cpu.SP.RAW));
                pipelineStats.Append(String.Format("write-after-read:       {0}\n", cpu.SP.WAR));
                pipelineStats.Append(String.Format("write-after-write:      {0}\n", cpu.SP.WAW));
                pipelineStats.Append(String.Format("Total:                  {0}\n\n", (cpu.SP.RAW + cpu.SP.WAR + cpu.SP.WAW)));
                pipelineStats.Append("Cycles Stalled\n");
                pipelineStats.Append("--------------\n");
                pipelineStats.Append(String.Format("instruction fetch:      {0}\n", cpu.SP.fetchStalled));
                pipelineStats.Append(String.Format("decode / read reg:      {0}\n", cpu.SP.decodeStalled));
                pipelineStats.Append(String.Format("execute / calc address: {0}\n", cpu.SP.executeStalled));
                pipelineStats.Append(String.Format("read / write memory:    {0}\n", cpu.SP.accessMemStalled));
                pipelineStats.Append(String.Format("write register:         {0}\n", cpu.SP.writeRegStalled));
                pipelineStats.Append(String.Format("Total:                  {0}\n\n", cpu.SP.totalCyclesStalled));
            }
            else
            {
                int totalInst = cpu.DP.CU.totalInstructions;
                if (totalInst == 0)
                    return;
                statistics.Append("Summary Statistics\n");
                statistics.Append("------------------\n");
                statistics.Append(String.Format("Total instructions:              {0}\n", totalInst));                       //since we always go by 2's total instructions was pretty simple
                statistics.Append(String.Format("Control instructions:            {0}, {1}%\n", cpu.DP.CU.controlInstructionCount, Math.Round((double)cpu.DP.CU.controlInstructionCount / totalInst * 100, 2)));
                statistics.Append(String.Format("Arithmetic & logic instructions: {0}, {1}%\n", cpu.DP.CU.ALUInstructionCount, Math.Round((double)cpu.DP.CU.ALUInstructionCount / totalInst * 100, 2)));
                statistics.Append(String.Format("Memory instructions:             {0}, {1}%\n", cpu.DP.CU.memoryInstructionCount, Math.Round((double)cpu.DP.CU.memoryInstructionCount / totalInst * 100, 2)));

                pipelineStats.Append("Pipeline Statistics\n");
                pipelineStats.Append("------------------\n");
                pipelineStats.Append(String.Format("Total Cycles:                    {0}\n", cpu.DP.cycleCount - 1));
                pipelineStats.Append("\nDelays\n");
                pipelineStats.Append("-------\n");
                pipelineStats.Append(String.Format("Reorder buffer delays:           {0}\n", cpu.DP.reorderBufferDelay));
                pipelineStats.Append(String.Format("Reservation station delays:      {0}\n", cpu.DP.reservationStationDelay));
                pipelineStats.Append(String.Format("True dependence delays:          {0}\n", cpu.DP.trueDependenceDelay));
                pipelineStats.Append(String.Format("Total:                           {0}\n\n", cpu.DP.reorderBufferDelay + cpu.DP.reservationStationDelay + cpu.DP.trueDependenceDelay));
            }
            StatsTextBox.Text = statistics.ToString();
            pipelineStatsTextBox.Text = pipelineStats.ToString();
        }


        /**
		 * Method Name: updatePipeline <br>
		 * Method Purpose: Sets the pipeline GUI elements
		 * 
		 * <br>
		 * Date created: 3/2/22 <br>
		 * <hr>
		 */
        private void updateStaticPipeline()
        {

            if (stages[0] != null)
            {
                stage1Text.Text = stages[0].fullAssemblySyntax;
            }
            else
                stage1Text.Text = "";
            if (stages[1] != null)
            {
                stage2Text.Text = stages[1].fullAssemblySyntax;
            }
            else
                stage2Text.Text = "";
            if (stages[2] != null)
            {
                stage3Text.Text = stages[2].fullAssemblySyntax;
            }
            else
                stage3Text.Text = "";
            if (stages[3] != null)
            {
                stage4Text.Text = stages[3].fullAssemblySyntax;
            }
            else
                stage4Text.Text = "";
            if (stages[4] != null)
            {
                stage5Text.Text = stages[4].fullAssemblySyntax;
            }
            else
                stage5Text.Text = "";

            if(!config.dynamicPipelineSet)
            {
                stage1StalledText.Text = cpu.SP.fetchStalled.ToString();
                stage2StalledText.Text = cpu.SP.decodeStalled.ToString();
                stage3StalledText.Text = cpu.SP.executeStalled.ToString();
                stage4StalledText.Text = cpu.SP.accessMemStalled.ToString();
                stage5StalledText.Text = cpu.SP.writeRegStalled.ToString();

                rawText.Text = cpu.SP.RAW.ToString();
                warText.Text = cpu.SP.WAR.ToString();
                wawText.Text = cpu.SP.WAW.ToString();
            }
           
        }


        private void updateDynamicPipeline()
        {
            string instructionsInFlight = "Reorder Buffer     ID\n";
            instructionsInFlight += "---------------------";
            string commonDataBus =        "CDB             Value\n";
            commonDataBus += "---------------------";
            foreach (Instruction inst in cpu.DP.reorderBuffer.reorderBuffer)
            {
                instructionsInFlight += string.Format("{0, 16} {1, 4}\n", inst.fullAssemblySyntax.PadRight(16, ' '), inst.ID.ToString().PadLeft(4,' '));
                if (inst.justIssued && inst.stage1Cycle == cpu.DP.cycleCount)
                    issueStageText.Text = inst.fullAssemblySyntax;
                else
                    issueStageText.Text = "";
            }

            foreach(KeyValuePair<string, string> instruction in cpu.DP.commonDataBus.CDB)
            {
                try
                {
                    commonDataBus += string.Format("{0, 16} {1, 4}\n", instruction.Key.PadRight(16, ' '), instruction.Value.ToString().PadLeft(4, ' '));

                }
                catch(Exception)
                {

                }
            }
            commonDataBusText.Text = commonDataBus;

            if (cpu.DP.justCommitedInstruction != null)
                commitStageText.Text = cpu.DP.justCommitedInstruction.fullAssemblySyntax;
            else
                commitStageText.Text = "";
            instructionInFlightText.Text = instructionsInFlight;
            reorderBufferCountText.Text = cpu.DP.reorderBuffer.reorderBuffer.Count.ToString();
            currentCycleText.Text = cpu.DP.cycleCount.ToString();
            rbdText.Text = cpu.DP.reorderBufferDelay.ToString();
            rsdText.Text = cpu.DP.reservationStationDelay.ToString();
            tddText.Text = cpu.DP.trueDependenceDelay.ToString();


            updateRegisterQiText();
            updateFunctionUnits();
        }


        private void updateRegisterQiText()
        {
            string registers =          "                                    Register Status                                       \n";
            registers +=                "---------------------------------------------------------------------------------------\n";
            registers +=                "Field -- r0        r1        r2        r3        r4        r5        r6        r7         \n";
            registers += (string.Format("Qi       {0, 9} {1, 9} {2, 9} {3, 9} {4, 9} {5, 9} {6, 9} {7, 9}\n\n", cpu.registers.intQi[0].PadRight(9, ' '), cpu.registers.intQi[1].PadRight(9, ' '),
                cpu.registers.intQi[2].PadRight(9, ' '), cpu.registers.intQi[3].PadRight(9, ' '), cpu.registers.intQi[4].PadRight(9, ' '), cpu.registers.intQi[5].PadRight(9, ' '), 
                cpu.registers.intQi[6].PadRight(9, ' '), cpu.registers.intQi[7].PadRight(9, ' '))); 
            registers +=                "Field -- r8        r9        r10       r11       r12       r13       r14       r15       \n";
            registers += (string.Format("Qi       {0, 9} {1, 9} {2, 9} {3, 9} {4, 9} {5, 9} {6, 9} {7, 9}\n\n", cpu.registers.intQi[8].PadRight(9, ' '), cpu.registers.intQi[9].PadRight(9, ' '),
                cpu.registers.intQi[10].PadRight(9, ' '), cpu.registers.intQi[11].PadRight(9, ' '), cpu.registers.intQi[12].PadRight(9, ' '), cpu.registers.intQi[13].PadRight(9, ' '),
                cpu.registers.intQi[14].PadRight(9, ' '), cpu.registers.intQi[15].PadRight(9, ' ')));
            registers +=                "Field -- f0        f1        f2        f3        f4        f5        f6        f7        \n";
            registers += (string.Format("Qi       {0, 9} {1, 9} {2, 9} {3, 9} {4, 9} {5, 9} {6, 9} {7, 9}\n\n", cpu.registers.floatQi[0].PadRight(9, ' '), cpu.registers.floatQi[1].PadRight(9, ' '),
                cpu.registers.floatQi[2].PadRight(9, ' '), cpu.registers.floatQi[3].PadRight(9, ' '), cpu.registers.floatQi[4].PadRight(9, ' '), cpu.registers.floatQi[5].PadRight(9, ' '),
                cpu.registers.floatQi[6].PadRight(9, ' '), cpu.registers.floatQi[7].PadRight(9, ' ')));
            registers +=                "Field -- f8        f9        f10       f11       f12       f13       f14       f15       \n";
            registers += (string.Format("Qi       {0, 9} {1, 9} {2, 9} {3, 9} {4, 9} {5, 9} {6, 9} {7, 9}\n\n", cpu.registers.floatQi[8].PadRight(9, ' '), cpu.registers.floatQi[9].PadRight(9, ' '),
                cpu.registers.floatQi[10].PadRight(9, ' '), cpu.registers.floatQi[11].PadRight(9, ' '), cpu.registers.floatQi[12].PadRight(9, ' '), cpu.registers.floatQi[13].PadRight(9, ' '),
                cpu.registers.floatQi[14].PadRight(9, ' '), cpu.registers.floatQi[15].PadRight(9, ' ')));

            registersQIText.Text = registers;
        }

        private void updateFunctionUnits()
        {
            if (!cpu.DP.programRanAtLeastOnce)
                return;

            string intAdd = "";
            string intSub = "";
            string intMult = "";
            string intDiv = "";
            string flAdd = "";
            string flSub = "";
            string flMult = "";
            string flDiv = "";
            string bitwise = "";
            string branch = "";
            string memory = "";
            string load_storeBuffer = "";

            intAdd += "     IntAddFU     \n";
            intAdd += "-------------------\n";
            foreach(IntAddFU fu in cpu.DP.intAddFUs)
            {
                if (fu.instruction != null)
                {
                    intAdd += fu.instruction.fullAssemblySyntax + "\n";
                }
            }
            intAdd += "\n  Reserv. Station  \n";
            intAdd += "-------------------\n";
            foreach (ReservationStation rs in cpu.DP.intAddRSs)
            {
                if (rs.instruction != null)
                    intAdd += rs.instruction.fullAssemblySyntax + "\n";
            }
            intAddText.Text = intAdd;

            intSub += "     IntSubFU     \n";
            intSub += "-------------------\n";
            foreach(IntSubFU fu in cpu.DP.intSubFUs)
            {
                if (fu.instruction != null)
                {
                    intSub += fu.instruction.fullAssemblySyntax + "\n";
                }
            }
            intSub += "\n  Reserv. Station  \n";
            intSub += "-------------------\n";
            foreach (ReservationStation rs in cpu.DP.intSubRSs)
            {
                if (rs.instruction != null)
                    intSub += rs.instruction.fullAssemblySyntax + "\n";
            }
            intSubText.Text = intSub;

            intMult += "     IntMulFU     \n";
            intMult += "-------------------\n";
            foreach(IntMultFU fu in cpu.DP.intMultFUs)
            {
                if (fu.instruction != null)
                {
                    intMult += fu.instruction.fullAssemblySyntax + "\n";
                }
            }
            intMult += "\n  Reserv. Station  \n";
            intMult += "-------------------\n";
            foreach (ReservationStation rs in cpu.DP.intMultRSs)
            {
                if (rs.instruction != null)
                    intMult += rs.instruction.fullAssemblySyntax + "\n";
            }
            intMultText.Text = intMult;

            intDiv += "     IntDivFU     \n";
            intDiv += "-------------------\n";
            foreach(IntDivFU fu in cpu.DP.intDivFUs)
            {
                if (fu.instruction != null)
                {
                    intDiv += fu.instruction.fullAssemblySyntax + "\n";
                }
            }
            intDiv += "\n  Reserv. Station  \n";
            intDiv += "-------------------\n";
            foreach (ReservationStation rs in cpu.DP.intDivRSs)
            {
                if (rs.instruction != null)
                    intDiv += rs.instruction.fullAssemblySyntax + "\n";
            }
            intDivText.Text = intDiv;

            flAdd += "      FlAddFU      \n";
            flAdd += "-------------------\n";
            foreach(FloatAddFU fu in cpu.DP.flAddFUs)
            {
                if (fu.instruction != null)
                {
                    flAdd += fu.instruction.fullAssemblySyntax + "\n";
                }
            }
            flAdd += "\n  Reserv. Station  \n";
            flAdd += "-------------------\n";
            foreach (ReservationStation rs in cpu.DP.floatAddRSs)
            {
                if (rs.instruction != null)
                    flAdd += rs.instruction.fullAssemblySyntax + "\n";
            }
            flAddText.Text = flAdd;

            flSub += "      FlSubFU      \n";
            flSub += "-------------------\n";
            foreach(FloatSubFU fu in cpu.DP.flSubFUs)
            {
                if (fu.instruction != null)
                {
                    flSub += fu.instruction.fullAssemblySyntax + "\n";
                }
            }
            flSub += "\n  Reserv. Station  \n";
            flSub += "-------------------\n";
            foreach (ReservationStation rs in cpu.DP.floatSubRSs)
            {
                if (rs.instruction != null)
                    flSub += rs.instruction.fullAssemblySyntax + "\n";
            }
            flSubText.Text = flSub;

            flMult += "      FlMultFU     \n";
            flMult += "-------------------\n";
            foreach(FloatMultFU fu in cpu.DP.flMultFUs)
            {
                if (fu.instruction != null)
                {
                    flMult += fu.instruction.fullAssemblySyntax + "\n";
                }
            }
            flMult += "\n  Reserv. Station  \n";
            flMult += "-------------------\n";
            foreach (ReservationStation rs in cpu.DP.floatMultRSs)
            {
                if (rs.instruction != null)
                    flMult += rs.instruction.fullAssemblySyntax + "\n";
            }
            flMultText.Text = flMult;

            flDiv += "      FlDivFU      \n";
            flDiv += "-------------------\n";
            foreach(FloatDivFU fu in cpu.DP.flDivFUs)
            {
                if (fu.instruction != null)
                {
                    flDiv += fu.instruction.fullAssemblySyntax + "\n";
                }
            }
            flDiv += "\n  Reserv. Station  \n";
            flDiv += "-------------------\n";
            foreach (ReservationStation rs in cpu.DP.floatDivRSs)
            {
                if (rs.instruction != null)
                    flDiv += rs.instruction.fullAssemblySyntax + "\n";
            }
            flDivText.Text = flDiv;

            bitwise += "       BitFU      \n";
            bitwise += "-------------------\n";
            foreach(BitwiseOPFU fu in cpu.DP.bitFUs)
            {
                if (fu.instruction != null)
                {
                    bitwise += fu.instruction.fullAssemblySyntax + "\n";
                }
            }
            bitwise += "\n  Reserv. Station  \n";
            bitwise += "-------------------\n";
            foreach (ReservationStation rs in cpu.DP.bitwiseRSs)
            {
                if (rs.instruction != null)
                    bitwise += rs.instruction.fullAssemblySyntax + "\n";
            }
            bitwiseText.Text = bitwise;

            branch += "      BranchFU     \n";
            branch += "-------------------\n";
            foreach(BranchFU fu in cpu.DP.branchFUs)
            {
                if (fu.instruction != null)
                {
                    branch += fu.instruction.fullAssemblySyntax + "\n";
                }
            }
            branch += "\n  Reserv. Station  \n";
            branch += "-------------------\n";
            foreach (ReservationStation rs in cpu.DP.branchRSs)
            {
                if (rs.instruction != null)
                    branch += rs.instruction.fullAssemblySyntax + "\n";
            }
            branchText.Text = branch;

            memory += "    Memory Unit   \n";
            memory += "-------------------\n";
            foreach(MemoryUnit fu in cpu.DP.memoryFUs)
            {
                if (fu.instruction != null)
                    memory += fu.instruction.fullAssemblySyntax + "\n";
            }
            memoryFUText.Text = memory;

            load_storeBuffer += " Load/Store Buffer\n";
            load_storeBuffer += "-------------------\n";
            foreach(ReservationStation rs in cpu.DP.loadStoreBuffer)
            {
                
                if (rs.instruction != null)
                    load_storeBuffer += rs.instruction.fullAssemblySyntax + "\n";
            }
            load_storeBufferText.Text = load_storeBuffer;

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


        private void delayProgram(int delayTime)
        {
            var waitTime = new TimeSpan(0, 0, 0, 0, delayTime * 100);
            var waitUntil = DateTime.Now + waitTime;
            int i = 0;
            while (DateTime.Now <= waitUntil)
            {
                i += 1;
            }
        }

        /**
		 * Method Name: RunButton_MouseHover <br>
		 * Method Purpose: Sets up a tooltip to display to the user that the run button runs the program
		 * 
		 * <br>
		 * Date created: 3/2/22 <br>
		 * <hr>
		 *   @param  object sender
		 *   @param  EventArgs e
		 */
        private void RunButton_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Run program", RunButton);
        }

        /**
		 * Method Name: stepthroughButton_MouseHover <br>
		 * Method Purpose: Sets up a tooltip to display to the user that the stepthrough button runs the program one cycle at a time
		 * 
		 * <br>
		 * Date created: 3/2/22 <br>
		 * <hr>
		 *   @param  object sender
		 *   @param  EventArgs e
		 */
        private void stepthroughButton_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Step through one cycle", stepthroughButton);
        }

        /**
		 * Method Name: resetButton_MouseHover <br>
		 * Method Purpose: Sets up a tooltip to display to the user that the reset button resets the program
		 * 
		 * <br>
		 * Date created: 3/2/22 <br>
		 * <hr>
		 *   @param  object sender
		 *   @param  EventArgs e
		 */
        private void resetButton_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Reset", resetButton);
        }

        /**
		 * Method Name: configButton_Click <br>
		 * Method Purpose: Brings up the configuration window and gets the user changes
		 * 
		 * <br>
		 * Date created: 3/6/22 <br>
		 * <hr>
		 *   @param  object sender
		 *   @param  EventArgs e
		 */
        private void configButton_Click(object sender, EventArgs e)
        {
            using (Configurations configWindow = new Configurations(config))
            {
                configWindow.StartPosition = FormStartPosition.CenterParent;
                configWindow.ShowDialog();

                config = configWindow.getConfig();
                updateGUI();
            }
        }

        private void buildButton_Click(object sender, EventArgs e)
        {
            if (objectCode.SelectedTab == objectCodeBox)
            {
                List<string> program = getMachineCode();      //get instructions from user
            }
            else
            {
                List<string> program = getAssembly();      //get instructions from user
            }
        }
        /**
        * Method Name: button1_click <br>
        * Method Purpose: load asembly fronm file
        * <br>
        * Date created:  <br>
        * <hr>
        *  
        */
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = Path.Combine(Application.StartupPath, "");
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                StreamReader reader = new StreamReader(dlg.FileName, Encoding.Default);
                if (objectCode.SelectedIndex == 0)
                {
                    assemblerTextBox.Text = reader.ReadToEnd();
                }
                else if(objectCode.SelectedIndex == 1)
                {
                    InputBox.Text = reader.ReadToEnd();
                }
                //assemblerTextBox.Text = reader.ReadToEnd();
                reader.Close();
            }

            dlg.Dispose();
        }

        private void SaveFile_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "*.txt|*.txt";
            dlg.RestoreDirectory = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(dlg.FileName, StatsTextBox.Text);
                File.AppendAllText(dlg.FileName, AssemblerListingTextBox.Text);
                File.AppendAllText(dlg.FileName, pipelineStatsTextBox.Text);
            }
            MessageBox.Show("Saved File");
        }

        private void buildButton_MouseHover(object sender, EventArgs e)
        {
            toolTip1.Show("Build", buildButton);
        }
    }
    

}
