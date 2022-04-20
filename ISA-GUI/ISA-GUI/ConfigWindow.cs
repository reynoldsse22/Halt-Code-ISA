// ---------------------------------------------------------------------------
// File name: WriteResult.cs
// Project name: ISA-GUI
// ---------------------------------------------------------------------------
// Creators: Samuel Reynolds, Nick Farmer, Carlos Ortiz, & Brandon Beaudry						
// Course-Section: CSCI 4717-201
// Creation Date: 3/6/22		
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

namespace ISA_GUI
{
    /**
	* Class Name: Configurations <br>
	* Class Purpose: Creates the configuration window
	* 
	* <hr>
	* Date created: 2/27/21 <br>
	* @author Samuel Reynolds
	*/
    public partial class Configurations : Form
    {
        public ConfigCycle configurations;

        /**
	    * Method Name: WriteResult <br>
	    * Method Purpose: Class constructor
	    * 
	    * <hr>
	    * Date created: 3/6/21 <br>
	    * @author Samuel Reynolds
	    */
        public Configurations(ConfigCycle config)
        {
            InitializeComponent();
            configurations = config;
            dynamicIntAddConfigValue.Value = configurations.intAdd;
            dynamicIntSubConfigValue.Value = configurations.intSub;
            dynamicIntMultConfigValue.Value = configurations.intMult;
            dynamicIntDivConfigValue.Value = configurations.intDiv;
            dynamicFlAddConfigValue.Value = configurations.flAdd;
            dynamicFlSubConfigValue.Value = configurations.flSub;
            dynamicFlMultConfigValue.Value = configurations.dynamicFlMult;
            dynamicFlDivConfigValue.Value = configurations.dynamicFlDiv;
            dynamicEffAddressConfigValue.Value = configurations.effAddress;
            dynamicBitwiseConfigValue.Value = configurations.bitwise;
            dynamicLoadConfigValue.Value = configurations.load;
            dynamicReorderBufferSizeValue.Value = configurations.reorderbuffersize;
            dynamicStoreConfigValue.Value = configurations.store;
            dynamicShiftConfigValue.Value = configurations.shift;
            bitGlobalPredictorValue.Value = configurations.whatBitPredictor;

            staticALUValue.Value = configurations.intALU;
            staticCalcAddressValue.Value = configurations.calcAddress;
            staticFetchValue.Value = configurations.fetch;
            staticMemAccessValue.Value = configurations.memAccess;
            staticRegAccessValue.Value = configurations.regAccess;
            staticFlAdd_SubValue.Value = configurations.flAddSub;
            staticFlMultValue.Value = configurations.flMult;
            staticFlDivValue.Value = configurations.flDiv;
            staticLoad_StoreValue.Value = configurations.load_store;

             intAddFUValue.Value = configurations.intAddFUs;
             intSubFUValue.Value = configurations.intSubFUs;
             intMultFUValue.Value = configurations.intMultFUs;
             intDivFUValue.Value = configurations.intDivFus;
             flAddFUValue.Value = configurations.flAddFUs;
             flSubFUValue.Value = configurations.flSubFUs;
             flMultFUValue.Value = configurations.flMultFUs;
             flDivFUValue.Value = configurations.flDivFUs;
             shiftFUValue.Value = configurations.shiftFUs;
             branchFUValue.Value = configurations.branchFUs;
             bitwiseFUValue.Value = configurations.bitwiseFUs;
             memoryFUValue.Value = configurations.memoryFUs;


            if (configurations.dynamicPipelineSet)
            {
                staticCheckbox.Checked = false;
                dynamicCheckbox.Checked = true;
            }
            else
            {
                staticCheckbox.Checked = true;
                dynamicCheckbox.Checked = false;
            }

            if (configurations.cachingSet)
            {
                cachingCheckBox.Checked = true;
            }
            else
            {
                cachingCheckBox.Checked = false;
            }

            if (dynamicCheckbox.Checked)
            {
                staticCheckbox.Checked = false;
                foreach (Control control in staticConfigPanel.Controls)
                {
                    control.Enabled = false;
                }
                foreach (Control control in dynamicConfigPanel.Controls)
                {
                    control.Enabled = true;
                }
                foreach (Control control in dynamicFUPanel.Controls)
                {
                    control.Enabled = true;
                }
            }
            else
            {
                staticCheckbox.Checked = true;
                foreach (Control control in staticConfigPanel.Controls)
                {
                    control.Enabled = true;
                }
                foreach (Control control in dynamicConfigPanel.Controls)
                {
                    control.Enabled = false;
                }
                foreach (Control control in dynamicFUPanel.Controls)
                {
                    control.Enabled = false;
                }
            }


            if (configurations.predictionSet)
            {
                predictionBox.SelectedIndex = 1;
                bitGlobalPredictorValue.Enabled = true;
                bitPredictorLabel.Enabled = true;

            }
            else
            {
                predictionBox.SelectedIndex = 0;
                bitGlobalPredictorValue.Enabled = false;
                bitPredictorLabel.Enabled = false;
            }

        }

        /**
		 * Method Name: Configurations_Load <br>
		 * Method Purpose: On load of window - initialize
		 * 
		 * <br>
		 * Date created: 3/6/22 <br>
		 * <hr>
		 *   @param  object sender
		 *   @param  EventArgs e
		 */
        private void Configurations_Load(object sender, EventArgs e)
        {
            StartPosition = FormStartPosition.CenterScreen;
        }

        /**
		 * Method Name: applyCHangesButton_Click <br>
		 * Method Purpose: Update the configuration class with the new values the user set in the config window
		 * 
		 * <br>
		 * Date created: 3/6/22 <br>
		 * <hr>
		 *   @param  object sender
		 *   @param  EventArgs e
		 */
        private void applyChangesButton_Click(object sender, EventArgs e)
        {
            configurations.intAdd = (int)dynamicIntAddConfigValue.Value;
            configurations.intSub = (int)dynamicIntSubConfigValue.Value;
            configurations.intMult = (int)dynamicIntMultConfigValue.Value;
            configurations.intDiv = (int)dynamicIntDivConfigValue.Value;
            configurations.flAdd = (int)dynamicFlAddConfigValue.Value;
            configurations.flSub = (int)dynamicFlSubConfigValue.Value;
            configurations.dynamicFlMult = (int)dynamicFlMultConfigValue.Value;
            configurations.dynamicFlDiv = (int)dynamicFlDivConfigValue.Value;
            configurations.effAddress = (int)dynamicEffAddressConfigValue.Value;
            configurations.bitwise = (int)dynamicBitwiseConfigValue.Value;
            configurations.load = (int)dynamicLoadConfigValue.Value;
            configurations.reorderbuffersize = (int)dynamicReorderBufferSizeValue.Value;
            configurations.store = (int)dynamicStoreConfigValue.Value;
            configurations.shift = (int)dynamicShiftConfigValue.Value;

            configurations.intALU = (int)staticALUValue.Value;
            configurations.calcAddress = (int)staticCalcAddressValue.Value;
            configurations.fetch = (int)staticFetchValue.Value;
            configurations.memAccess = (int)staticMemAccessValue.Value;
            configurations.regAccess = (int)staticRegAccessValue.Value;
            configurations.flAddSub = (int)staticFlAdd_SubValue.Value;
            configurations.flMult = (int)staticFlMultValue.Value;
            configurations.flDiv = (int)staticFlDivValue.Value;
            configurations.load_store = (int)staticLoad_StoreValue.Value;

            configurations.intAddFUs = (int)intAddFUValue.Value;
            configurations.intSubFUs = (int)intSubFUValue.Value;
            configurations.intMultFUs = (int)intMultFUValue.Value;
            configurations.intDivFus = (int)intDivFUValue.Value;
            configurations.flAddFUs = (int)flAddFUValue.Value;
            configurations.flSubFUs = (int)flSubFUValue.Value;
            configurations.flMultFUs = (int)flMultFUValue.Value;
            configurations.flDivFUs = (int)flDivFUValue.Value;
            configurations.shiftFUs = (int)shiftFUValue.Value;
            configurations.branchFUs = (int)branchFUValue.Value;
            configurations.bitwiseFUs = (int)bitwiseFUValue.Value;
            configurations.memoryFUs = (int)memoryFUValue.Value;



            if (staticCheckbox.Checked)
                configurations.dynamicPipelineSet = false;
            else
                configurations.dynamicPipelineSet = true;

            if (predictionBox.SelectedIndex == 0)
            {
                configurations.predictionSet = false;
            }
            else
            {
                configurations.predictionSet = true;
                configurations.whatBitPredictor = (int)bitGlobalPredictorValue.Value;
            }

            if (cachingCheckBox.Checked)
            {
                configurations.cachingSet = true;
            }
            else
            {
                configurations.cachingSet = false;
            }

            Dispose();
        }

        /**
		 * Method Name: getConfig <br>
		 * Method Purpose: Return the configuration class back to parent
		 * 
		 * <br>
		 * Date created: 3/6/22 <br>
		 * <hr>
		 */
        public ConfigCycle getConfig()
        {
            return configurations;
        }

        /**
		 * Method Name: resetButton_Click <br>
		 * Method Purpose: Reset the configuration class to default values
		 * 
		 * <br>
		 * Date created: 3/6/22 <br>
		 * <hr>
		 *   @param  object sender
		 *   @param  EventArgs e
		 */
        private void resetButton_Click(object sender, EventArgs e)
        {
            staticFetchValue.Value = 1;
            staticMemAccessValue.Value = 3;
            staticRegAccessValue.Value = 1;
            staticLoad_StoreValue.Value = 1;
            staticALUValue.Value = 1;
            staticFlAdd_SubValue.Value = 2;
            staticFlMultValue.Value = 5;
            staticFlDivValue.Value = 10;
            staticCalcAddressValue.Value = 1;
            predictionBox.SelectedIndex = 0;

            dynamicIntAddConfigValue.Value = 1;
            dynamicIntSubConfigValue.Value = 1;
            dynamicIntMultConfigValue.Value = 1;
            dynamicIntDivConfigValue.Value = 2;
            dynamicFlAddConfigValue.Value = 2;
            dynamicFlSubConfigValue.Value = 2;
            dynamicFlMultConfigValue.Value = 5;
            dynamicFlDivConfigValue.Value = 10;
            dynamicBitwiseConfigValue.Value = 1;
            dynamicShiftConfigValue.Value = 1;
            dynamicLoadConfigValue.Value = 1;
            dynamicStoreConfigValue.Value = 1;
            bitGlobalPredictorValue.Value = 1;

            intAddFUValue.Value = 1;
            intSubFUValue.Value = 1;
            intMultFUValue.Value = 1;
            intDivFUValue.Value = 1;
            flAddFUValue.Value = 1;
            flSubFUValue.Value = 1;
            flMultFUValue.Value = 1;
            flDivFUValue.Value = 1;
            shiftFUValue.Value = 1;
            branchFUValue.Value = 1;
            bitwiseFUValue.Value = 1;
            memoryFUValue.Value = 2;
            

            configurations.predictionSet = false;
            configurations.forwardingSet = false;
            configurations.fetch = (int)staticFetchValue.Value;
            configurations.memAccess = (int)staticMemAccessValue.Value;
            configurations.regAccess = (int)staticRegAccessValue.Value;
            configurations.load_store = (int)staticLoad_StoreValue.Value;
            configurations.intALU = (int)staticALUValue.Value;
            configurations.flAddSub = (int)staticFlAdd_SubValue.Value;
            configurations.flMult = (int)staticFlMultValue.Value;
            configurations.flDiv = (int)staticFlDivValue.Value;
            configurations.calcAddress = (int)staticCalcAddressValue.Value;

            configurations.intAdd = (int)dynamicIntAddConfigValue.Value;
            configurations.intSub = (int)dynamicIntSubConfigValue.Value;
            configurations.intMult = (int)dynamicIntMultConfigValue.Value;
            configurations.intDiv = (int)dynamicIntDivConfigValue.Value;
            configurations.flAdd = (int)dynamicFlAddConfigValue.Value;
            configurations.flSub = (int)dynamicFlSubConfigValue.Value;
            configurations.dynamicFlMult = (int)dynamicFlMultConfigValue.Value;
            configurations.dynamicFlDiv = (int)dynamicFlDivConfigValue.Value;
            configurations.effAddress = (int)dynamicEffAddressConfigValue.Value;
            configurations.bitwise = (int)dynamicBitwiseConfigValue.Value;
            configurations.load = (int)dynamicLoadConfigValue.Value;
            configurations.reorderbuffersize = (int)dynamicReorderBufferSizeValue.Value;
            configurations.store = (int)dynamicStoreConfigValue.Value;
            configurations.shift = (int)dynamicShiftConfigValue.Value;
            setReorderBufferMinimum();
        }

        private void staticCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if(staticCheckbox.Checked)
                dynamicCheckbox.Checked = false;
            else
            {
                dynamicCheckbox.Checked = true;
            }
        }

        private void dynamicCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (dynamicCheckbox.Checked)
            {
                staticCheckbox.Checked = false;
                foreach(Control control in staticConfigPanel.Controls)
                {
                    control.Enabled = false;
                }
                foreach (Control control in dynamicConfigPanel.Controls)
                {
                    control.Enabled = true;
                }
                foreach(Control control in dynamicFUPanel.Controls)
                {
                    control.Enabled = true;
                }
            }
            else
            {
                staticCheckbox.Checked = true;
                foreach (Control control in staticConfigPanel.Controls)
                {
                    control.Enabled = true;
                }
                foreach (Control control in dynamicConfigPanel.Controls)
                {
                    control.Enabled = false;
                }
                foreach (Control control in dynamicFUPanel.Controls)
                {
                    control.Enabled = false;
                }
            }
        }

        private void predictionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(predictionBox.SelectedIndex == 1)
            {
                bitGlobalPredictorValue.Enabled = true;
                bitPredictorLabel.Enabled = true;
            }
            else
            {
                bitGlobalPredictorValue.Enabled = false;
                bitPredictorLabel.Enabled = false;
            }
        }

        private void setReorderBufferMinimum()
        {
            int min = (int)(intAddFUValue.Value + intSubFUValue.Value + intMultFUValue.Value + intDivFUValue.Value +
                flAddFUValue.Value + flSubFUValue.Value + flMultFUValue.Value + flDivFUValue.Value + shiftFUValue.Value + branchFUValue.Value +
                bitwiseFUValue.Value + memoryFUValue.Value);

            dynamicReorderBufferSizeValue.Minimum = min;
            dynamicReorderBufferSizeValue.Value = min;
        }

        private void intAddFUValue_ValueChanged(object sender, EventArgs e)
        {
            setReorderBufferMinimum();
        }

        private void intSubFUValue_ValueChanged(object sender, EventArgs e)
        {
            setReorderBufferMinimum();
        }

        private void intMultFUValue_ValueChanged(object sender, EventArgs e)
        {
            setReorderBufferMinimum();
        }

        private void intDivFUValue_ValueChanged(object sender, EventArgs e)
        {
            setReorderBufferMinimum();
        }

        private void flAddFUValue_ValueChanged(object sender, EventArgs e)
        {
            setReorderBufferMinimum();
        }

        private void flSubFUValue_ValueChanged(object sender, EventArgs e)
        {
            setReorderBufferMinimum();
        }

        private void flMultFUValue_ValueChanged(object sender, EventArgs e)
        {
            setReorderBufferMinimum();
        }

        private void flDivFUValue_ValueChanged(object sender, EventArgs e)
        {
            setReorderBufferMinimum();
        }

        private void bitwiseFUValue_ValueChanged(object sender, EventArgs e)
        {
            setReorderBufferMinimum();
        }

        private void shiftFUValue_ValueChanged(object sender, EventArgs e)
        {
            setReorderBufferMinimum();
        }

        private void branchFUValue_ValueChanged(object sender, EventArgs e)
        {
            setReorderBufferMinimum();
        }

        private void memoryFUValue_ValueChanged(object sender, EventArgs e)
        {
            setReorderBufferMinimum();
        }
    }
}
