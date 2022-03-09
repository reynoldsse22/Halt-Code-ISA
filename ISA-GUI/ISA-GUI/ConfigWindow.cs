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
            fetchConfigValue.Value = configurations.fetch;
            memAccessValue.Value = configurations.memAccess;
            regAccessValue.Value = configurations.regAccess;
            loadStoreValue.Value = configurations.load_store;
            intALUValue.Value = configurations.intALU;
            flAddSubValue.Value = configurations.flAddSub;
            flMultValue.Value = configurations.flMult;
            flDivValue.Value = configurations.flDiv;
            calcAddressValue.Value = configurations.calcAddress;

            if (configurations.predictionSet)
                predictionBox.SelectedIndex = 1;
            else
                predictionBox.SelectedIndex = 0;

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
            configurations.fetch = (int)fetchConfigValue.Value;
            configurations.memAccess = (int)memAccessValue.Value;
            configurations.regAccess = (int)regAccessValue.Value;
            configurations.load_store = (int)loadStoreValue.Value;
            configurations.intALU = (int)intALUValue.Value;
            configurations.flAddSub = (int)flAddSubValue.Value;
            configurations.flMult = (int)flMultValue.Value;
            configurations.flDiv = (int)flDivValue.Value;
            configurations.calcAddress = (int)calcAddressValue.Value;

            if (predictionBox.SelectedIndex == 0)
                configurations.predictionSet = false;
            else
                configurations.predictionSet = true;



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
            fetchConfigValue.Value = 1;
            memAccessValue.Value = 3;
            regAccessValue.Value = 1;
            loadStoreValue.Value = 1;
            intALUValue.Value = 1;
            flAddSubValue.Value = 2;
            flMultValue.Value = 5;
            flDivValue.Value = 10;
            calcAddressValue.Value = 1;
            predictionBox.SelectedIndex = 0;

            configurations.predictionSet = false;
            configurations.forwardingSet = false;
            configurations.fetch = (int)fetchConfigValue.Value;
            configurations.memAccess = (int)memAccessValue.Value;
            configurations.regAccess = (int)regAccessValue.Value;
            configurations.load_store = (int)loadStoreValue.Value;
            configurations.intALU = (int)intALUValue.Value;
            configurations.flAddSub = (int)flAddSubValue.Value;
            configurations.flMult = (int)flMultValue.Value;
            configurations.flDiv = (int)flDivValue.Value;
            configurations.calcAddress = (int)calcAddressValue.Value;
        }
    }
}
