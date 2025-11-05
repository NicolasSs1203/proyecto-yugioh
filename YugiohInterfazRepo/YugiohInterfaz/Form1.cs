using System;
using System.Windows.Forms;

namespace YugiohInterfaz
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void buttonJugar_Click(object sender, EventArgs e)
        {
            MessageBox.Show("¡Próximamente: Campo de Juego!", "Yu-Gi-Oh", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void buttonSalir_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(
                "¿Seguro que deseas salir del juego?",
                "Salir",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
                Application.Exit();
        }
    }
}