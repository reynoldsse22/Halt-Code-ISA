﻿namespace ISA_GUI
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
            this.r7Dec = new System.Windows.Forms.RichTextBox();
            this.r7Hex = new System.Windows.Forms.RichTextBox();
            this.r8Dec = new System.Windows.Forms.RichTextBox();
            this.r8Hex = new System.Windows.Forms.RichTextBox();
            this.r9Dec = new System.Windows.Forms.RichTextBox();
            this.r9Hex = new System.Windows.Forms.RichTextBox();
            this.r10Dec = new System.Windows.Forms.RichTextBox();
            this.r10Hex = new System.Windows.Forms.RichTextBox();
            this.r11Dec = new System.Windows.Forms.RichTextBox();
            this.r11Hex = new System.Windows.Forms.RichTextBox();
            this.r12Dec = new System.Windows.Forms.RichTextBox();
            this.r12Hex = new System.Windows.Forms.RichTextBox();
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
            this.label2.Location = new System.Drawing.Point(557, 175);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Register 0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(557, 206);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Register 1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(557, 239);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Register 2";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(557, 270);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Register 3";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(557, 302);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Register 4";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(557, 332);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Register 5";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(557, 363);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Register 6";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(557, 397);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(55, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "Register 7";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(557, 430);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(55, 13);
            this.label10.TabIndex = 16;
            this.label10.Text = "Register 8";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(557, 461);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(55, 13);
            this.label11.TabIndex = 17;
            this.label11.Text = "Register 9";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(554, 492);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(61, 13);
            this.label12.TabIndex = 18;
            this.label12.Text = "Register 10";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(554, 522);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(61, 13);
            this.label13.TabIndex = 19;
            this.label13.Text = "Register 11";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(554, 553);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(61, 13);
            this.label14.TabIndex = 20;
            this.label14.Text = "Register 12";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(579, 585);
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
            this.r0Hex.Location = new System.Drawing.Point(619, 169);
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
            this.r0Dec.Location = new System.Drawing.Point(747, 169);
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
            this.r1Dec.Location = new System.Drawing.Point(747, 200);
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
            this.r1Hex.Location = new System.Drawing.Point(619, 200);
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
            this.r2Dec.Location = new System.Drawing.Point(747, 233);
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
            this.r2Hex.Location = new System.Drawing.Point(619, 233);
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
            this.r3Dec.Location = new System.Drawing.Point(747, 264);
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
            this.r3Hex.Location = new System.Drawing.Point(619, 264);
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
            this.r4Dec.Location = new System.Drawing.Point(747, 295);
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
            this.r4Hex.Location = new System.Drawing.Point(619, 295);
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
            this.r5Dec.Location = new System.Drawing.Point(747, 326);
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
            this.r5Hex.Location = new System.Drawing.Point(619, 326);
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
            this.r6Dec.Location = new System.Drawing.Point(747, 357);
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
            this.r6Hex.Location = new System.Drawing.Point(619, 357);
            this.r6Hex.Name = "r6Hex";
            this.r6Hex.ReadOnly = true;
            this.r6Hex.Size = new System.Drawing.Size(124, 25);
            this.r6Hex.TabIndex = 40;
            this.r6Hex.Text = "";
            // 
            // r7Dec
            // 
            this.r7Dec.BackColor = System.Drawing.SystemColors.Menu;
            this.r7Dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r7Dec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r7Dec.Location = new System.Drawing.Point(747, 391);
            this.r7Dec.Name = "r7Dec";
            this.r7Dec.ReadOnly = true;
            this.r7Dec.Size = new System.Drawing.Size(124, 25);
            this.r7Dec.TabIndex = 43;
            this.r7Dec.Text = "";
            // 
            // r7Hex
            // 
            this.r7Hex.BackColor = System.Drawing.SystemColors.Menu;
            this.r7Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r7Hex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r7Hex.Location = new System.Drawing.Point(619, 391);
            this.r7Hex.Name = "r7Hex";
            this.r7Hex.ReadOnly = true;
            this.r7Hex.Size = new System.Drawing.Size(124, 25);
            this.r7Hex.TabIndex = 42;
            this.r7Hex.Text = "";
            // 
            // r8Dec
            // 
            this.r8Dec.BackColor = System.Drawing.SystemColors.Menu;
            this.r8Dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r8Dec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r8Dec.Location = new System.Drawing.Point(747, 422);
            this.r8Dec.Name = "r8Dec";
            this.r8Dec.ReadOnly = true;
            this.r8Dec.Size = new System.Drawing.Size(124, 25);
            this.r8Dec.TabIndex = 45;
            this.r8Dec.Text = "";
            // 
            // r8Hex
            // 
            this.r8Hex.BackColor = System.Drawing.SystemColors.Menu;
            this.r8Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r8Hex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r8Hex.Location = new System.Drawing.Point(619, 422);
            this.r8Hex.Name = "r8Hex";
            this.r8Hex.ReadOnly = true;
            this.r8Hex.Size = new System.Drawing.Size(124, 25);
            this.r8Hex.TabIndex = 44;
            this.r8Hex.Text = "";
            // 
            // r9Dec
            // 
            this.r9Dec.BackColor = System.Drawing.SystemColors.Menu;
            this.r9Dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r9Dec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r9Dec.Location = new System.Drawing.Point(747, 455);
            this.r9Dec.Name = "r9Dec";
            this.r9Dec.ReadOnly = true;
            this.r9Dec.Size = new System.Drawing.Size(124, 25);
            this.r9Dec.TabIndex = 47;
            this.r9Dec.Text = "";
            // 
            // r9Hex
            // 
            this.r9Hex.BackColor = System.Drawing.SystemColors.Menu;
            this.r9Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r9Hex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r9Hex.Location = new System.Drawing.Point(619, 455);
            this.r9Hex.Name = "r9Hex";
            this.r9Hex.ReadOnly = true;
            this.r9Hex.Size = new System.Drawing.Size(124, 25);
            this.r9Hex.TabIndex = 46;
            this.r9Hex.Text = "";
            // 
            // r10Dec
            // 
            this.r10Dec.BackColor = System.Drawing.SystemColors.Menu;
            this.r10Dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r10Dec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r10Dec.Location = new System.Drawing.Point(747, 486);
            this.r10Dec.Name = "r10Dec";
            this.r10Dec.ReadOnly = true;
            this.r10Dec.Size = new System.Drawing.Size(124, 25);
            this.r10Dec.TabIndex = 49;
            this.r10Dec.Text = "";
            // 
            // r10Hex
            // 
            this.r10Hex.BackColor = System.Drawing.SystemColors.Menu;
            this.r10Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r10Hex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r10Hex.Location = new System.Drawing.Point(619, 486);
            this.r10Hex.Name = "r10Hex";
            this.r10Hex.ReadOnly = true;
            this.r10Hex.Size = new System.Drawing.Size(124, 25);
            this.r10Hex.TabIndex = 48;
            this.r10Hex.Text = "";
            // 
            // r11Dec
            // 
            this.r11Dec.BackColor = System.Drawing.SystemColors.Menu;
            this.r11Dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r11Dec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r11Dec.Location = new System.Drawing.Point(747, 517);
            this.r11Dec.Name = "r11Dec";
            this.r11Dec.ReadOnly = true;
            this.r11Dec.Size = new System.Drawing.Size(124, 25);
            this.r11Dec.TabIndex = 51;
            this.r11Dec.Text = "";
            // 
            // r11Hex
            // 
            this.r11Hex.BackColor = System.Drawing.SystemColors.Menu;
            this.r11Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r11Hex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r11Hex.Location = new System.Drawing.Point(619, 517);
            this.r11Hex.Name = "r11Hex";
            this.r11Hex.ReadOnly = true;
            this.r11Hex.Size = new System.Drawing.Size(124, 25);
            this.r11Hex.TabIndex = 50;
            this.r11Hex.Text = "";
            // 
            // r12Dec
            // 
            this.r12Dec.BackColor = System.Drawing.SystemColors.Menu;
            this.r12Dec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r12Dec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r12Dec.Location = new System.Drawing.Point(747, 548);
            this.r12Dec.Name = "r12Dec";
            this.r12Dec.ReadOnly = true;
            this.r12Dec.Size = new System.Drawing.Size(124, 25);
            this.r12Dec.TabIndex = 53;
            this.r12Dec.Text = "";
            // 
            // r12Hex
            // 
            this.r12Hex.BackColor = System.Drawing.SystemColors.Menu;
            this.r12Hex.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r12Hex.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r12Hex.Location = new System.Drawing.Point(619, 548);
            this.r12Hex.Name = "r12Hex";
            this.r12Hex.ReadOnly = true;
            this.r12Hex.Size = new System.Drawing.Size(124, 25);
            this.r12Hex.TabIndex = 52;
            this.r12Hex.Text = "";
            // 
            // asprDec
            // 
            this.asprDec.BackColor = System.Drawing.SystemColors.Menu;
            this.asprDec.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.asprDec.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.asprDec.Location = new System.Drawing.Point(747, 579);
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
            this.asprHex.Location = new System.Drawing.Point(619, 579);
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
            this.cirDec.Location = new System.Drawing.Point(748, 106);
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
            this.cirHex.Location = new System.Drawing.Point(620, 106);
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
            this.pcDec.Location = new System.Drawing.Point(748, 136);
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
            this.pcHex.Location = new System.Drawing.Point(620, 136);
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
            // BUC10
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1475, 770);
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
            this.Controls.Add(this.r12Dec);
            this.Controls.Add(this.r12Hex);
            this.Controls.Add(this.r11Dec);
            this.Controls.Add(this.r11Hex);
            this.Controls.Add(this.r10Dec);
            this.Controls.Add(this.r10Hex);
            this.Controls.Add(this.r9Dec);
            this.Controls.Add(this.r9Hex);
            this.Controls.Add(this.r8Dec);
            this.Controls.Add(this.r8Hex);
            this.Controls.Add(this.r7Dec);
            this.Controls.Add(this.r7Hex);
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
        private System.Windows.Forms.RichTextBox r7Dec;
        private System.Windows.Forms.RichTextBox r7Hex;
        private System.Windows.Forms.RichTextBox r8Dec;
        private System.Windows.Forms.RichTextBox r8Hex;
        private System.Windows.Forms.RichTextBox r9Dec;
        private System.Windows.Forms.RichTextBox r9Hex;
        private System.Windows.Forms.RichTextBox r10Dec;
        private System.Windows.Forms.RichTextBox r10Hex;
        private System.Windows.Forms.RichTextBox r11Dec;
        private System.Windows.Forms.RichTextBox r11Hex;
        private System.Windows.Forms.RichTextBox r12Dec;
        private System.Windows.Forms.RichTextBox r12Hex;
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
    }
}
