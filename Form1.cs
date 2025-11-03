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

        private void buttonVerCartas_Click(object sender, EventArgs e)
        {
            FormVerCartas formCartas = new FormVerCartas();
            formCartas.ShowDialog();
        }

        private void buttonJugar_Click(object sender, EventArgs e)
        {
            FormCampoDeJuego campo = new FormCampoDeJuego();
            campo.ShowDialog();
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