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
    public partial class Configurations : Form
    {
        public ConfigCycle configurations;
        public Configurations(ConfigCycle config)
        {
            InitializeComponent();
            configurations = config;
            fetchConfigValue.Value = configurations.fetch;
            memAccessValue.Value = config.memAccess;
            regAccessValue.Value = config.regAccess;
            loadStoreValue.Value = config.load_store;
            intALUValue.Value = config.intALU;
            flAddSubValue.Value = config.flAddSub;
            flMultValue.Value = config.flMult;
            flDivValue.Value = config.flDiv;
            calcAddressValue.Value = config.calcAddress;

            if (config.predictionSet)
                predictionBox.SelectedIndex = 1;
            else
                predictionBox.SelectedIndex = 0;

            if (config.forwardingSet)
                forwardingBox.SelectedIndex = 1;
            else
                forwardingBox.SelectedIndex = 0;

        }

        private void Configurations_Load(object sender, EventArgs e)
        {
            StartPosition = FormStartPosition.CenterScreen;
        }


        private void applyChangesButton_Click(object sender, EventArgs e)
        {
            configurations.fetch = (int)fetchConfigValue.Value;
            configurations.memAccess = (int)memAccessValue.Value;
            configurations.regAccess = (int)regAccessValue.Value;
            configurations.load_store = (int)loadStoreValue.Value;
            configurations.intALU = (int)intALUValue.Value;
            configurations.flAddSub = (int)flAddSubValue.Value;
            configurations.flMult = (int)flMultValue.Value;
            configurations.calcAddress = (int)calcAddressValue.Value;

            if (predictionBox.SelectedIndex == 0)
                configurations.predictionSet = false;
            else
                configurations.predictionSet = true;

            if (forwardingBox.SelectedIndex == 0)
                configurations.forwardingSet = false;
            else
                configurations.forwardingSet = true;


            Dispose();
        }

        public ConfigCycle getConfig()
        {
            return configurations;
        }

    }
}
