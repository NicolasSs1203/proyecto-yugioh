namespace YugiohInterfaz
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label labelTitulo;
        private System.Windows.Forms.Button buttonJugar;
        private System.Windows.Forms.Button buttonSalir;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            labelTitulo = new Label();
            buttonJugar = new Button();
            buttonSalir = new Button();
            SuspendLayout();
            // 
            // labelTitulo
            // 
            labelTitulo.AutoSize = true;
            labelTitulo.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            labelTitulo.ForeColor = Color.Gold;
            labelTitulo.Location = new Point(150, 50);
            labelTitulo.Name = "labelTitulo";
            labelTitulo.Size = new Size(469, 45);
            labelTitulo.TabIndex = 0;
            labelTitulo.Text = "YU-GI-OH - MENÚ PRINCIPAL";
            // 
            // buttonJugar
            // 
            buttonJugar.BackColor = Color.Gold;
            buttonJugar.Font = new Font("Old English Text MT", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonJugar.ForeColor = Color.Black;
            buttonJugar.Location = new Point(250, 180);
            buttonJugar.Name = "buttonJugar";
            buttonJugar.Size = new Size(300, 70);
            buttonJugar.TabIndex = 1;
            buttonJugar.Text = "Jugar";
            buttonJugar.UseVisualStyleBackColor = false;
            buttonJugar.Click += buttonJugar_Click;
            // 
            // buttonSalir
            // 
            buttonSalir.BackColor = Color.Gold;
            buttonSalir.Font = new Font("Old English Text MT", 21.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            buttonSalir.ForeColor = Color.Black;
            buttonSalir.Location = new Point(250, 280);
            buttonSalir.Name = "buttonSalir";
            buttonSalir.Size = new Size(300, 70);
            buttonSalir.TabIndex = 2;
            buttonSalir.Text = "Salir";
            buttonSalir.UseVisualStyleBackColor = false;
            buttonSalir.Click += buttonSalir_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DarkSlateBlue;
            ClientSize = new Size(800, 450);
            Controls.Add(buttonSalir);
            Controls.Add(buttonJugar);
            Controls.Add(labelTitulo);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Menú Principal - Yu-Gi-Oh";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}