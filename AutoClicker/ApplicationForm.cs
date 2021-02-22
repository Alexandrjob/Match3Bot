using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AutoClicker
{
    public partial class ApplicationForm: Form
    {
        private GameAlgorithm gameAlgorithm;
        public ApplicationForm()
        {
            InitializeComponent();
        }

        private async void ButtonStartAlgorithm_Click(object sender, EventArgs e)
        {
            await StartAsync();
        }

        private async Task StartAsync()
        {
            await Task.Run(() => Start());
        }

        private async void Start()
        {
            gameAlgorithm = new GameAlgorithm(this);
            await gameAlgorithm.StartAsync();
        }

        private void ButtonStopAlgoritm_Click(object sender, EventArgs e)
        {
            gameAlgorithm.Dispose();
        }
    }
}
