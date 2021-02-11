using System;
using System.Windows.Forms;

namespace AutoClicker
{
    public partial class ApplicationForm: Form
    {
        public ApplicationForm()
        {
            InitializeComponent();
        }

        private void ApplicationForm_Load(object sender, EventArgs e)
        {
        }

        private void ButtonStartAlgorithm_Click(object sender, EventArgs e)
        {
            GameAlgorithm gameAlgorithm = new GameAlgorithm(this);
            gameAlgorithm.StartAsync();
        }
    }
}
