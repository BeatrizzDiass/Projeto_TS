using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Projeto_TS
{
    public partial class FormRegistar : Form
    {
        public FormRegistar()
        {
            InitializeComponent();
            //this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
        }

        private void buttonRegistar_Click(object sender, EventArgs e)
        {
            FormChat formChat = new FormChat();
            formChat.Show();
            this.Hide(); // Oculta o form atual depois de abrir o outro
        }
    }
}
