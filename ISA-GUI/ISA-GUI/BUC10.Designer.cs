namespace ISA_GUI
{
    partial class BUC10
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BUC10));
            this.CPU = new System.Windows.Forms.Label();
            this.MemoryText = new System.Windows.Forms.RichTextBox();
            this.ZFlagBox = new System.Windows.Forms.RichTextBox();
            this.cFlagBox = new System.Windows.Forms.RichTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.r0Hex = new System.Windows.Forms.RichTextBox();
            this.r0Dec = new System.Windows.Forms.RichTextBox();
            this.r1Dec = new System.Windows.Forms.RichTextBox();
            this.r1Hex = new System.Windows.Forms.RichTextBox();
            this.r2Dec = new System.Windows.Forms.RichTextBox();
            this.r2Hex = new System.Windows.Forms.RichTextBox();
            this.r3Dec = new System.Windows.Forms.RichTextBox();
            this.r3Hex = new System.Windows.Forms.RichTextBox();
            this.r4Dec = new System.Windows.Forms.RichTextBox();
            this.r4Hex = new System.Windows.Forms.RichTextBox();
            this.r5Dec = new System.Windows.Forms.RichTextBox();
            this.r5Hex = new System.Windows.Forms.RichTextBox();
            this.r6Dec = new System.Windows.Forms.RichTextBox();
            this.r6Hex = new System.Windows.Forms.RichTextBox();
            this.f0Dec = new System.Windows.Forms.RichTextBox();
            this.f0Hex = new System.Windows.Forms.RichTextBox();
            this.f1Dec = new System.Windows.Forms.RichTextBox();
            this.f1Hex = new System.Windows.Forms.RichTextBox();
            this.f2Dec = new System.Windows.Forms.RichTextBox();
            this.f2Hex = new System.Windows.Forms.RichTextBox();
            this.f3Dec = new System.Windows.Forms.RichTextBox();
            this.f3Hex = new System.Windows.Forms.RichTextBox();
            this.f4Dec = new System.Windows.Forms.RichTextBox();
            this.f4Hex = new System.Windows.Forms.RichTextBox();
            this.f5Dec = new System.Windows.Forms.RichTextBox();
            this.f5Hex = new System.Windows.Forms.RichTextBox();
            this.asprDec = new System.Windows.Forms.RichTextBox();
            this.asprHex = new System.Windows.Forms.RichTextBox();
            this.cirDec = new System.Windows.Forms.RichTextBox();
            this.cirHex = new System.Windows.Forms.RichTextBox();
            this.pcDec = new System.Windows.Forms.RichTextBox();
            this.pcHex = new System.Windows.Forms.RichTextBox();
            this.Output = new System.Windows.Forms.TabControl();
            this.pipelineTab = new System.Windows.Forms.TabPage();
            this.pipelineTextBox = new System.Windows.Forms.RichTextBox();
            this.sourceCodeBox = new System.Windows.Forms.TabPage();
            this.AssemblerListingTextBox = new System.Windows.Forms.RichTextBox();
            this.AssemblyTab = new System.Windows.Forms.TabPage();
            this.AssemblyTextBox = new System.Windows.Forms.RichTextBox();
            this.summaryStatsBox = new System.Windows.Forms.TabPage();
            this.StatsTextBox = new System.Windows.Forms.RichTextBox();
            this.objectCode = new System.Windows.Forms.TabControl();
            this.objectCodeBox = new System.Windows.Forms.TabPage();
            this.InputBox = new System.Windows.Forms.RichTextBox();
            this.debugButton = new System.Windows.Forms.PictureBox();
            this.RunButton = new System.Windows.Forms.PictureBox();
            this.f6Dec = new System.Windows.Forms.RichTextBox();
            this.f6Hex = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Output.SuspendLayout();
            this.pipelineTab.SuspendLayout();
            this.sourceCodeBox.SuspendLayout();
            this.AssemblyTab.SuspendLayout();
            this.summaryStatsBox.SuspendLayout();
            this.objectCode.SuspendLayout();
            this.objectCodeBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.debugButton)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RunButton)).BeginInit();
            this.SuspendLayout();
            // 
            // CPU
            // 
            this.CPU.AutoSize = true;
            this.CPU.Location = new System.Drawing.Point(571, 79);
            this.CPU.Name = "CPU";
            this.CPU.Size = new System.Drawing.Size(29, 13);
            this.CPU.TabIndex = 0;
            this.CPU.Text = "CPU";
            // 
            // MemoryText
            // 
            this.MemoryText.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MemoryText.Location = new System.Drawing.Point(878, 78);
            this.MemoryText.Name = "MemoryText";
            this.MemoryText.ReadOnly = true;
            this.MemoryText.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.MemoryText.Size = new System.Drawing.Size(585, 676);
            this.MemoryText.TabIndex = 2;
            this.MemoryText.Text = "";
            // 
            // ZFlagBox
            // 
            this.ZFlagBox.BackColor = System.Drawing.SystemColors.Menu;
            this.ZFlagBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ZFlagBox.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ZFlagBox.Location = new System.Drawing.Point(657, 73);
            this.ZFlagBox.Name = "ZFlagBox";
            this.ZFlagBox.ReadOnly = true;
            this.ZFlagBox.Size = new System.Drawing.Size(25, 25);
            this.ZFlagBox.TabIndex = 4;
            this.ZFlagBox.Text = "";
            // 
            // cFlagBox
            // 
            this.cFlagBox.BackColor = System.Drawing.SystemColors.Menu;
            this.cFlagBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cFlagBox.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cFlagBox.Location = new System.Drawing.Point(719, 73);
            this.cFlagBox.Name = "cFlagBox";
            this.cFlagBox.ReadOnly = true;
            this.cFlagBox.Size = new System.Drawing.Size(25, 25);
            this.cFlagBox.TabIndex = 6;
            this.cFlagBox.Text = "";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(594, 199);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "R0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(594, 230);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "R1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(594, 263);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(21, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "R2";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(594, 294);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(21, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "R3";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(594, 326);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "R4";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(594, 356);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(21, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "R5";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(594, 387);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(21, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "R6";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(594, 421);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(19, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "F0";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(594, 454);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(19, 13);
            this.label10.TabIndex = 16;
            this.label10.Text = "F1";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(594, 485);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(19, 13);
            this.label11.TabIndex = 17;
            this.label11.Text = "F2";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(594, 517);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(19, 13);
            this.label12.TabIndex = 18;
            this.label12.Text = "F3";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(594, 547);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(19, 13);
            this.label13.TabIndex = 19;
            this.label13.Text = "F4";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(594, 578);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(19, 13);
            this.label14.TabIndex = 20;
            this.label14.Text = "F5";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(580, 169);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(36, 13);
            this.label15.TabIndex = 21;
            this.label15.Text = "ASPR";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(591, 112);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(25, 13);
            this.label16.TabIndex = 22;
            this.label16.Text = "CIR";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(594, 141);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(21, 13);
            this.label17.TabIndex = 23;
            this.label17.Text = "PC";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(639, 79);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(14, 13);
            this.label19.TabIndex = 25;
            this.label19.Text = "Z";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(701, 79);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(14, 13);
            this.label21.TabIndex = 27;
            this.label21.Text = "C";
            // 
            // r0Hex
            // 
            this.r0Hex.BackColor = System.Drawing.SystemColors.Menu;
            this.r0Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r0Hex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r0Hex.Location = new System.Drawing.Point(619, 194);
            this.r0Hex.Name = "r0Hex";
            this.r0Hex.ReadOnly = true;
            this.r0Hex.Size = new System.Drawing.Size(124, 25);
            this.r0Hex.TabIndex = 28;
            this.r0Hex.Text = "";
            // 
            // r0Dec
            // 
            this.r0Dec.BackColor = System.Drawing.SystemColors.Menu;
            this.r0Dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r0Dec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r0Dec.Location = new System.Drawing.Point(747, 194);
            this.r0Dec.Name = "r0Dec";
            this.r0Dec.ReadOnly = true;
            this.r0Dec.Size = new System.Drawing.Size(124, 25);
            this.r0Dec.TabIndex = 29;
            this.r0Dec.Text = "";
            // 
            // r1Dec
            // 
            this.r1Dec.BackColor = System.Drawing.SystemColors.Menu;
            this.r1Dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r1Dec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r1Dec.Location = new System.Drawing.Point(747, 225);
            this.r1Dec.Name = "r1Dec";
            this.r1Dec.ReadOnly = true;
            this.r1Dec.Size = new System.Drawing.Size(124, 25);
            this.r1Dec.TabIndex = 31;
            this.r1Dec.Text = "";
            // 
            // r1Hex
            // 
            this.r1Hex.BackColor = System.Drawing.SystemColors.Menu;
            this.r1Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r1Hex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r1Hex.Location = new System.Drawing.Point(619, 225);
            this.r1Hex.Name = "r1Hex";
            this.r1Hex.ReadOnly = true;
            this.r1Hex.Size = new System.Drawing.Size(124, 25);
            this.r1Hex.TabIndex = 30;
            this.r1Hex.Text = "";
            // 
            // r2Dec
            // 
            this.r2Dec.BackColor = System.Drawing.SystemColors.Menu;
            this.r2Dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r2Dec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r2Dec.Location = new System.Drawing.Point(747, 258);
            this.r2Dec.Name = "r2Dec";
            this.r2Dec.ReadOnly = true;
            this.r2Dec.Size = new System.Drawing.Size(124, 25);
            this.r2Dec.TabIndex = 33;
            this.r2Dec.Text = "";
            // 
            // r2Hex
            // 
            this.r2Hex.BackColor = System.Drawing.SystemColors.Menu;
            this.r2Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r2Hex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r2Hex.Location = new System.Drawing.Point(619, 258);
            this.r2Hex.Name = "r2Hex";
            this.r2Hex.ReadOnly = true;
            this.r2Hex.Size = new System.Drawing.Size(124, 25);
            this.r2Hex.TabIndex = 32;
            this.r2Hex.Text = "";
            // 
            // r3Dec
            // 
            this.r3Dec.BackColor = System.Drawing.SystemColors.Menu;
            this.r3Dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r3Dec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r3Dec.Location = new System.Drawing.Point(747, 289);
            this.r3Dec.Name = "r3Dec";
            this.r3Dec.ReadOnly = true;
            this.r3Dec.Size = new System.Drawing.Size(124, 25);
            this.r3Dec.TabIndex = 35;
            this.r3Dec.Text = "";
            // 
            // r3Hex
            // 
            this.r3Hex.BackColor = System.Drawing.SystemColors.Menu;
            this.r3Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r3Hex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r3Hex.Location = new System.Drawing.Point(619, 289);
            this.r3Hex.Name = "r3Hex";
            this.r3Hex.ReadOnly = true;
            this.r3Hex.Size = new System.Drawing.Size(124, 25);
            this.r3Hex.TabIndex = 34;
            this.r3Hex.Text = "";
            // 
            // r4Dec
            // 
            this.r4Dec.BackColor = System.Drawing.SystemColors.Menu;
            this.r4Dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r4Dec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r4Dec.Location = new System.Drawing.Point(747, 320);
            this.r4Dec.Name = "r4Dec";
            this.r4Dec.ReadOnly = true;
            this.r4Dec.Size = new System.Drawing.Size(124, 25);
            this.r4Dec.TabIndex = 37;
            this.r4Dec.Text = "";
            // 
            // r4Hex
            // 
            this.r4Hex.BackColor = System.Drawing.SystemColors.Menu;
            this.r4Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r4Hex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r4Hex.Location = new System.Drawing.Point(619, 320);
            this.r4Hex.Name = "r4Hex";
            this.r4Hex.ReadOnly = true;
            this.r4Hex.Size = new System.Drawing.Size(124, 25);
            this.r4Hex.TabIndex = 36;
            this.r4Hex.Text = "";
            // 
            // r5Dec
            // 
            this.r5Dec.BackColor = System.Drawing.SystemColors.Menu;
            this.r5Dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r5Dec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r5Dec.Location = new System.Drawing.Point(747, 351);
            this.r5Dec.Name = "r5Dec";
            this.r5Dec.ReadOnly = true;
            this.r5Dec.Size = new System.Drawing.Size(124, 25);
            this.r5Dec.TabIndex = 39;
            this.r5Dec.Text = "";
            // 
            // r5Hex
            // 
            this.r5Hex.BackColor = System.Drawing.SystemColors.Menu;
            this.r5Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r5Hex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r5Hex.Location = new System.Drawing.Point(619, 351);
            this.r5Hex.Name = "r5Hex";
            this.r5Hex.ReadOnly = true;
            this.r5Hex.Size = new System.Drawing.Size(124, 25);
            this.r5Hex.TabIndex = 38;
            this.r5Hex.Text = "";
            // 
            // r6Dec
            // 
            this.r6Dec.BackColor = System.Drawing.SystemColors.Menu;
            this.r6Dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r6Dec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r6Dec.Location = new System.Drawing.Point(747, 382);
            this.r6Dec.Name = "r6Dec";
            this.r6Dec.ReadOnly = true;
            this.r6Dec.Size = new System.Drawing.Size(124, 25);
            this.r6Dec.TabIndex = 41;
            this.r6Dec.Text = "";
            // 
            // r6Hex
            // 
            this.r6Hex.BackColor = System.Drawing.SystemColors.Menu;
            this.r6Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r6Hex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r6Hex.Location = new System.Drawing.Point(619, 382);
            this.r6Hex.Name = "r6Hex";
            this.r6Hex.ReadOnly = true;
            this.r6Hex.Size = new System.Drawing.Size(124, 25);
            this.r6Hex.TabIndex = 40;
            this.r6Hex.Text = "";
            // 
            // f0Dec
            // 
            this.f0Dec.BackColor = System.Drawing.SystemColors.Menu;
            this.f0Dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.f0Dec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.f0Dec.Location = new System.Drawing.Point(747, 416);
            this.f0Dec.Name = "f0Dec";
            this.f0Dec.ReadOnly = true;
            this.f0Dec.Size = new System.Drawing.Size(124, 25);
            this.f0Dec.TabIndex = 43;
            this.f0Dec.Text = "";
            // 
            // f0Hex
            // 
            this.f0Hex.BackColor = System.Drawing.SystemColors.Menu;
            this.f0Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.f0Hex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.f0Hex.Location = new System.Drawing.Point(619, 416);
            this.f0Hex.Name = "f0Hex";
            this.f0Hex.ReadOnly = true;
            this.f0Hex.Size = new System.Drawing.Size(124, 25);
            this.f0Hex.TabIndex = 42;
            this.f0Hex.Text = "";
            // 
            // f1Dec
            // 
            this.f1Dec.BackColor = System.Drawing.SystemColors.Menu;
            this.f1Dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.f1Dec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.f1Dec.Location = new System.Drawing.Point(747, 447);
            this.f1Dec.Name = "f1Dec";
            this.f1Dec.ReadOnly = true;
            this.f1Dec.Size = new System.Drawing.Size(124, 25);
            this.f1Dec.TabIndex = 45;
            this.f1Dec.Text = "";
            // 
            // f1Hex
            // 
            this.f1Hex.BackColor = System.Drawing.SystemColors.Menu;
            this.f1Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.f1Hex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.f1Hex.Location = new System.Drawing.Point(619, 447);
            this.f1Hex.Name = "f1Hex";
            this.f1Hex.ReadOnly = true;
            this.f1Hex.Size = new System.Drawing.Size(124, 25);
            this.f1Hex.TabIndex = 44;
            this.f1Hex.Text = "";
            // 
            // f2Dec
            // 
            this.f2Dec.BackColor = System.Drawing.SystemColors.Menu;
            this.f2Dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.f2Dec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.f2Dec.Location = new System.Drawing.Point(747, 480);
            this.f2Dec.Name = "f2Dec";
            this.f2Dec.ReadOnly = true;
            this.f2Dec.Size = new System.Drawing.Size(124, 25);
            this.f2Dec.TabIndex = 47;
            this.f2Dec.Text = "";
            // 
            // f2Hex
            // 
            this.f2Hex.BackColor = System.Drawing.SystemColors.Menu;
            this.f2Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.f2Hex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.f2Hex.Location = new System.Drawing.Point(619, 480);
            this.f2Hex.Name = "f2Hex";
            this.f2Hex.ReadOnly = true;
            this.f2Hex.Size = new System.Drawing.Size(124, 25);
            this.f2Hex.TabIndex = 46;
            this.f2Hex.Text = "";
            // 
            // f3Dec
            // 
            this.f3Dec.BackColor = System.Drawing.SystemColors.Menu;
            this.f3Dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.f3Dec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.f3Dec.Location = new System.Drawing.Point(747, 511);
            this.f3Dec.Name = "f3Dec";
            this.f3Dec.ReadOnly = true;
            this.f3Dec.Size = new System.Drawing.Size(124, 25);
            this.f3Dec.TabIndex = 49;
            this.f3Dec.Text = "";
            // 
            // f3Hex
            // 
            this.f3Hex.BackColor = System.Drawing.SystemColors.Menu;
            this.f3Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.f3Hex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.f3Hex.Location = new System.Drawing.Point(619, 511);
            this.f3Hex.Name = "f3Hex";
            this.f3Hex.ReadOnly = true;
            this.f3Hex.Size = new System.Drawing.Size(124, 25);
            this.f3Hex.TabIndex = 48;
            this.f3Hex.Text = "";
            // 
            // f4Dec
            // 
            this.f4Dec.BackColor = System.Drawing.SystemColors.Menu;
            this.f4Dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.f4Dec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.f4Dec.Location = new System.Drawing.Point(747, 542);
            this.f4Dec.Name = "f4Dec";
            this.f4Dec.ReadOnly = true;
            this.f4Dec.Size = new System.Drawing.Size(124, 25);
            this.f4Dec.TabIndex = 51;
            this.f4Dec.Text = "";
            // 
            // f4Hex
            // 
            this.f4Hex.BackColor = System.Drawing.SystemColors.Menu;
            this.f4Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.f4Hex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.f4Hex.Location = new System.Drawing.Point(619, 542);
            this.f4Hex.Name = "f4Hex";
            this.f4Hex.ReadOnly = true;
            this.f4Hex.Size = new System.Drawing.Size(124, 25);
            this.f4Hex.TabIndex = 50;
            this.f4Hex.Text = "";
            // 
            // f5Dec
            // 
            this.f5Dec.BackColor = System.Drawing.SystemColors.Menu;
            this.f5Dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.f5Dec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.f5Dec.Location = new System.Drawing.Point(747, 573);
            this.f5Dec.Name = "f5Dec";
            this.f5Dec.ReadOnly = true;
            this.f5Dec.Size = new System.Drawing.Size(124, 25);
            this.f5Dec.TabIndex = 53;
            this.f5Dec.Text = "";
            // 
            // f5Hex
            // 
            this.f5Hex.BackColor = System.Drawing.SystemColors.Menu;
            this.f5Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.f5Hex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.f5Hex.Location = new System.Drawing.Point(619, 573);
            this.f5Hex.Name = "f5Hex";
            this.f5Hex.ReadOnly = true;
            this.f5Hex.Size = new System.Drawing.Size(124, 25);
            this.f5Hex.TabIndex = 52;
            this.f5Hex.Text = "";
            // 
            // asprDec
            // 
            this.asprDec.BackColor = System.Drawing.SystemColors.Menu;
            this.asprDec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.asprDec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.asprDec.Location = new System.Drawing.Point(747, 164);
            this.asprDec.Name = "asprDec";
            this.asprDec.ReadOnly = true;
            this.asprDec.Size = new System.Drawing.Size(124, 25);
            this.asprDec.TabIndex = 55;
            this.asprDec.Text = "";
            // 
            // asprHex
            // 
            this.asprHex.BackColor = System.Drawing.SystemColors.Menu;
            this.asprHex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.asprHex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.asprHex.Location = new System.Drawing.Point(619, 164);
            this.asprHex.Name = "asprHex";
            this.asprHex.ReadOnly = true;
            this.asprHex.Size = new System.Drawing.Size(124, 25);
            this.asprHex.TabIndex = 54;
            this.asprHex.Text = "";
            // 
            // cirDec
            // 
            this.cirDec.BackColor = System.Drawing.SystemColors.Menu;
            this.cirDec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cirDec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cirDec.Location = new System.Drawing.Point(747, 107);
            this.cirDec.Name = "cirDec";
            this.cirDec.ReadOnly = true;
            this.cirDec.Size = new System.Drawing.Size(124, 25);
            this.cirDec.TabIndex = 57;
            this.cirDec.Text = "";
            // 
            // cirHex
            // 
            this.cirHex.BackColor = System.Drawing.SystemColors.Menu;
            this.cirHex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.cirHex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cirHex.Location = new System.Drawing.Point(619, 107);
            this.cirHex.Name = "cirHex";
            this.cirHex.ReadOnly = true;
            this.cirHex.Size = new System.Drawing.Size(124, 25);
            this.cirHex.TabIndex = 56;
            this.cirHex.Text = "";
            // 
            // pcDec
            // 
            this.pcDec.BackColor = System.Drawing.SystemColors.Menu;
            this.pcDec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pcDec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pcDec.Location = new System.Drawing.Point(747, 137);
            this.pcDec.Name = "pcDec";
            this.pcDec.ReadOnly = true;
            this.pcDec.Size = new System.Drawing.Size(124, 25);
            this.pcDec.TabIndex = 59;
            this.pcDec.Text = "";
            // 
            // pcHex
            // 
            this.pcHex.BackColor = System.Drawing.SystemColors.Menu;
            this.pcHex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pcHex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pcHex.Location = new System.Drawing.Point(619, 137);
            this.pcHex.Name = "pcHex";
            this.pcHex.ReadOnly = true;
            this.pcHex.Size = new System.Drawing.Size(124, 25);
            this.pcHex.TabIndex = 58;
            this.pcHex.Text = "";
            // 
            // Output
            // 
            this.Output.Controls.Add(this.pipelineTab);
            this.Output.Controls.Add(this.sourceCodeBox);
            this.Output.Controls.Add(this.AssemblyTab);
            this.Output.Controls.Add(this.summaryStatsBox);
            this.Output.Location = new System.Drawing.Point(13, 299);
            this.Output.Name = "Output";
            this.Output.SelectedIndex = 0;
            this.Output.Size = new System.Drawing.Size(532, 459);
            this.Output.TabIndex = 61;
            // 
            // pipelineTab
            // 
            this.pipelineTab.Controls.Add(this.pipelineTextBox);
            this.pipelineTab.Location = new System.Drawing.Point(4, 22);
            this.pipelineTab.Name = "pipelineTab";
            this.pipelineTab.Padding = new System.Windows.Forms.Padding(3);
            this.pipelineTab.Size = new System.Drawing.Size(524, 433);
            this.pipelineTab.TabIndex = 3;
            this.pipelineTab.Text = "Pipeline Statistics";
            this.pipelineTab.UseVisualStyleBackColor = true;
            // 
            // pipelineTextBox
            // 
            this.pipelineTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.pipelineTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.pipelineTextBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pipelineTextBox.Location = new System.Drawing.Point(3, 5);
            this.pipelineTextBox.Name = "pipelineTextBox";
            this.pipelineTextBox.ReadOnly = true;
            this.pipelineTextBox.Size = new System.Drawing.Size(519, 427);
            this.pipelineTextBox.TabIndex = 2;
            this.pipelineTextBox.Text = "";
            // 
            // sourceCodeBox
            // 
            this.sourceCodeBox.Controls.Add(this.AssemblerListingTextBox);
            this.sourceCodeBox.Location = new System.Drawing.Point(4, 22);
            this.sourceCodeBox.Name = "sourceCodeBox";
            this.sourceCodeBox.Padding = new System.Windows.Forms.Padding(3);
            this.sourceCodeBox.Size = new System.Drawing.Size(524, 433);
            this.sourceCodeBox.TabIndex = 0;
            this.sourceCodeBox.Text = "Assembler Listing";
            this.sourceCodeBox.UseVisualStyleBackColor = true;
            // 
            // AssemblerListingTextBox
            // 
            this.AssemblerListingTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.AssemblerListingTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.AssemblerListingTextBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AssemblerListingTextBox.Location = new System.Drawing.Point(0, 5);
            this.AssemblerListingTextBox.Name = "AssemblerListingTextBox";
            this.AssemblerListingTextBox.ReadOnly = true;
            this.AssemblerListingTextBox.Size = new System.Drawing.Size(521, 428);
            this.AssemblerListingTextBox.TabIndex = 0;
            this.AssemblerListingTextBox.Text = "";
            // 
            // AssemblyTab
            // 
            this.AssemblyTab.Controls.Add(this.AssemblyTextBox);
            this.AssemblyTab.Location = new System.Drawing.Point(4, 22);
            this.AssemblyTab.Name = "AssemblyTab";
            this.AssemblyTab.Padding = new System.Windows.Forms.Padding(3);
            this.AssemblyTab.Size = new System.Drawing.Size(524, 433);
            this.AssemblyTab.TabIndex = 2;
            this.AssemblyTab.Text = "Assembly";
            this.AssemblyTab.UseVisualStyleBackColor = true;
            // 
            // AssemblyTextBox
            // 
            this.AssemblyTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.AssemblyTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.AssemblyTextBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AssemblyTextBox.Location = new System.Drawing.Point(0, 5);
            this.AssemblyTextBox.Name = "AssemblyTextBox";
            this.AssemblyTextBox.ReadOnly = true;
            this.AssemblyTextBox.Size = new System.Drawing.Size(521, 428);
            this.AssemblyTextBox.TabIndex = 1;
            this.AssemblyTextBox.Text = "";
            // 
            // summaryStatsBox
            // 
            this.summaryStatsBox.Controls.Add(this.StatsTextBox);
            this.summaryStatsBox.Location = new System.Drawing.Point(4, 22);
            this.summaryStatsBox.Name = "summaryStatsBox";
            this.summaryStatsBox.Padding = new System.Windows.Forms.Padding(3);
            this.summaryStatsBox.Size = new System.Drawing.Size(524, 433);
            this.summaryStatsBox.TabIndex = 1;
            this.summaryStatsBox.Text = "Summary Statistics";
            this.summaryStatsBox.UseVisualStyleBackColor = true;
            // 
            // StatsTextBox
            // 
            this.StatsTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.StatsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.StatsTextBox.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatsTextBox.Location = new System.Drawing.Point(2, 5);
            this.StatsTextBox.Name = "StatsTextBox";
            this.StatsTextBox.ReadOnly = true;
            this.StatsTextBox.Size = new System.Drawing.Size(519, 428);
            this.StatsTextBox.TabIndex = 1;
            this.StatsTextBox.Text = "";
            // 
            // objectCode
            // 
            this.objectCode.Controls.Add(this.objectCodeBox);
            this.objectCode.Location = new System.Drawing.Point(13, 64);
            this.objectCode.Name = "objectCode";
            this.objectCode.SelectedIndex = 0;
            this.objectCode.Size = new System.Drawing.Size(532, 216);
            this.objectCode.TabIndex = 62;
            // 
            // objectCodeBox
            // 
            this.objectCodeBox.Controls.Add(this.InputBox);
            this.objectCodeBox.Location = new System.Drawing.Point(4, 22);
            this.objectCodeBox.Name = "objectCodeBox";
            this.objectCodeBox.Padding = new System.Windows.Forms.Padding(3);
            this.objectCodeBox.Size = new System.Drawing.Size(524, 190);
            this.objectCodeBox.TabIndex = 0;
            this.objectCodeBox.Text = "Object Code";
            this.objectCodeBox.UseVisualStyleBackColor = true;
            // 
            // InputBox
            // 
            this.InputBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.InputBox.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InputBox.Location = new System.Drawing.Point(1, 5);
            this.InputBox.Name = "InputBox";
            this.InputBox.Size = new System.Drawing.Size(523, 182);
            this.InputBox.TabIndex = 0;
            this.InputBox.Text = "";
            // 
            // debugButton
            // 
            this.debugButton.ErrorImage = global::ISA_GUI.Properties.Resources.icons8_bug_48;
            this.debugButton.Image = global::ISA_GUI.Properties.Resources.icons8_bug_48;
            this.debugButton.InitialImage = global::ISA_GUI.Properties.Resources.icons8_bug_48;
            this.debugButton.Location = new System.Drawing.Point(71, 8);
            this.debugButton.Name = "debugButton";
            this.debugButton.Size = new System.Drawing.Size(52, 52);
            this.debugButton.TabIndex = 64;
            this.debugButton.TabStop = false;
            this.debugButton.Click += new System.EventHandler(this.debugButton_Click);
            // 
            // RunButton
            // 
            this.RunButton.ErrorImage = global::ISA_GUI.Properties.Resources.Play2;
            this.RunButton.Image = global::ISA_GUI.Properties.Resources.Play2;
            this.RunButton.InitialImage = global::ISA_GUI.Properties.Resources.Play2;
            this.RunButton.Location = new System.Drawing.Point(13, 8);
            this.RunButton.Name = "RunButton";
            this.RunButton.Size = new System.Drawing.Size(52, 52);
            this.RunButton.TabIndex = 63;
            this.RunButton.TabStop = false;
            this.RunButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // f6Dec
            // 
            this.f6Dec.BackColor = System.Drawing.SystemColors.Menu;
            this.f6Dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.f6Dec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.f6Dec.Location = new System.Drawing.Point(747, 604);
            this.f6Dec.Name = "f6Dec";
            this.f6Dec.ReadOnly = true;
            this.f6Dec.Size = new System.Drawing.Size(124, 25);
            this.f6Dec.TabIndex = 67;
            this.f6Dec.Text = "";
            // 
            // f6Hex
            // 
            this.f6Hex.BackColor = System.Drawing.SystemColors.Menu;
            this.f6Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.f6Hex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.f6Hex.Location = new System.Drawing.Point(619, 604);
            this.f6Hex.Name = "f6Hex";
            this.f6Hex.ReadOnly = true;
            this.f6Hex.Size = new System.Drawing.Size(124, 25);
            this.f6Hex.TabIndex = 66;
            this.f6Hex.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(594, 609);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 13);
            this.label1.TabIndex = 65;
            this.label1.Text = "F6";
            // 
            // BUC10
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1475, 770);
            this.Controls.Add(this.f6Dec);
            this.Controls.Add(this.f6Hex);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.debugButton);
            this.Controls.Add(this.RunButton);
            this.Controls.Add(this.objectCode);
            this.Controls.Add(this.Output);
            this.Controls.Add(this.pcDec);
            this.Controls.Add(this.pcHex);
            this.Controls.Add(this.cirDec);
            this.Controls.Add(this.cirHex);
            this.Controls.Add(this.asprDec);
            this.Controls.Add(this.asprHex);
            this.Controls.Add(this.f5Dec);
            this.Controls.Add(this.f5Hex);
            this.Controls.Add(this.f4Dec);
            this.Controls.Add(this.f4Hex);
            this.Controls.Add(this.f3Dec);
            this.Controls.Add(this.f3Hex);
            this.Controls.Add(this.f2Dec);
            this.Controls.Add(this.f2Hex);
            this.Controls.Add(this.f1Dec);
            this.Controls.Add(this.f1Hex);
            this.Controls.Add(this.f0Dec);
            this.Controls.Add(this.f0Hex);
            this.Controls.Add(this.r6Dec);
            this.Controls.Add(this.r6Hex);
            this.Controls.Add(this.r5Dec);
            this.Controls.Add(this.r5Hex);
            this.Controls.Add(this.r4Dec);
            this.Controls.Add(this.r4Hex);
            this.Controls.Add(this.r3Dec);
            this.Controls.Add(this.r3Hex);
            this.Controls.Add(this.r2Dec);
            this.Controls.Add(this.r2Hex);
            this.Controls.Add(this.r1Dec);
            this.Controls.Add(this.r1Hex);
            this.Controls.Add(this.r0Dec);
            this.Controls.Add(this.r0Hex);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cFlagBox);
            this.Controls.Add(this.ZFlagBox);
            this.Controls.Add(this.CPU);
            this.Controls.Add(this.MemoryText);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "BUC10";
            this.Text = "BUC/10";
            this.Load += new System.EventHandler(this.BUC10_Load);
            this.Output.ResumeLayout(false);
            this.pipelineTab.ResumeLayout(false);
            this.sourceCodeBox.ResumeLayout(false);
            this.AssemblyTab.ResumeLayout(false);
            this.summaryStatsBox.ResumeLayout(false);
            this.objectCode.ResumeLayout(false);
            this.objectCodeBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.debugButton)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RunButton)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label CPU;
        private System.Windows.Forms.RichTextBox MemoryText;
        private System.Windows.Forms.RichTextBox ZFlagBox;
        private System.Windows.Forms.RichTextBox cFlagBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.RichTextBox r0Hex;
        private System.Windows.Forms.RichTextBox r0Dec;
        private System.Windows.Forms.RichTextBox r1Dec;
        private System.Windows.Forms.RichTextBox r1Hex;
        private System.Windows.Forms.RichTextBox r2Dec;
        private System.Windows.Forms.RichTextBox r2Hex;
        private System.Windows.Forms.RichTextBox r3Dec;
        private System.Windows.Forms.RichTextBox r3Hex;
        private System.Windows.Forms.RichTextBox r4Dec;
        private System.Windows.Forms.RichTextBox r4Hex;
        private System.Windows.Forms.RichTextBox r5Dec;
        private System.Windows.Forms.RichTextBox r5Hex;
        private System.Windows.Forms.RichTextBox r6Dec;
        private System.Windows.Forms.RichTextBox r6Hex;
        private System.Windows.Forms.RichTextBox f0Dec;
        private System.Windows.Forms.RichTextBox f0Hex;
        private System.Windows.Forms.RichTextBox f1Dec;
        private System.Windows.Forms.RichTextBox f1Hex;
        private System.Windows.Forms.RichTextBox f2Dec;
        private System.Windows.Forms.RichTextBox f2Hex;
        private System.Windows.Forms.RichTextBox f3Dec;
        private System.Windows.Forms.RichTextBox f3Hex;
        private System.Windows.Forms.RichTextBox f4Dec;
        private System.Windows.Forms.RichTextBox f4Hex;
        private System.Windows.Forms.RichTextBox f5Dec;
        private System.Windows.Forms.RichTextBox f5Hex;
        private System.Windows.Forms.RichTextBox asprDec;
        private System.Windows.Forms.RichTextBox asprHex;
        private System.Windows.Forms.RichTextBox cirDec;
        private System.Windows.Forms.RichTextBox cirHex;
        private System.Windows.Forms.RichTextBox pcDec;
        private System.Windows.Forms.RichTextBox pcHex;
        private System.Windows.Forms.TabControl Output;
        private System.Windows.Forms.TabPage sourceCodeBox;
        private System.Windows.Forms.TabPage summaryStatsBox;
        private System.Windows.Forms.TabControl objectCode;
        private System.Windows.Forms.PictureBox RunButton;
        public System.Windows.Forms.TabPage objectCodeBox;
        private System.Windows.Forms.RichTextBox InputBox;
        private System.Windows.Forms.RichTextBox AssemblerListingTextBox;
        private System.Windows.Forms.RichTextBox StatsTextBox;
        private System.Windows.Forms.TabPage AssemblyTab;
        private System.Windows.Forms.RichTextBox AssemblyTextBox;
        private System.Windows.Forms.PictureBox debugButton;
        private System.Windows.Forms.TabPage pipelineTab;
        private System.Windows.Forms.RichTextBox pipelineTextBox;
        private System.Windows.Forms.RichTextBox f6Dec;
        private System.Windows.Forms.RichTextBox f6Hex;
        private System.Windows.Forms.Label label1;
    }
}

