using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projeto_TS
{
    public partial class FormInicio : Form
    {
        public FormInicio()
        {
            InitializeComponent();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        private void FormInicio_Load(object sender, EventArgs e)
        {
            AbrirForm();
        }

        private async void AbrirForm()
        {
            await Task.Delay(5000); // Espera 5 segundos

            FormLogin formLogin = new FormLogin();
            formLogin.Show();
            this.Hide(); // Oculta o form atual depois de abrir o outro
        }

    }
}
