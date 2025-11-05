namespace YugiohInterfaz
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label labelTitulo;
        private System.Windows.Forms.Button buttonJugar;
        private System.Windows.Forms.Button buttonSalir;
        private System.Windows.Forms.PictureBox pictureBoxFondo;

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
            pictureBoxFondo = new PictureBox();
            labelTitulo = new Label();
            buttonJugar = new Button();
            buttonSalir = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBoxFondo).BeginInit();
            SuspendLayout();

            // 
            // pictureBoxFondo
            // 
            pictureBoxFondo.Dock = DockStyle.Fill;
            pictureBoxFondo.Location = new Point(0, 0);
            pictureBoxFondo.Name = "pictureBoxFondo";
            pictureBoxFondo.Size = new Size(800, 450);
            pictureBoxFondo.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxFondo.TabIndex = 0;
            pictureBoxFondo.TabStop = false;

            // 
            // labelTitulo
            // 
            labelTitulo.BackColor = Color.Transparent;
            labelTitulo.Font = new Font("Arial Black", 26F, FontStyle.Bold);
            labelTitulo.ForeColor = Color.FromArgb(255, 215, 0);
            labelTitulo.Location = new Point(50, 30);
            labelTitulo.Name = "labelTitulo";
            labelTitulo.Size = new Size(700, 70);
            labelTitulo.TabIndex = 2;
            labelTitulo.Text = "YU-GI-OH! DUEL MONSTERS";
            labelTitulo.TextAlign = ContentAlignment.MiddleCenter;

            // 
            // buttonJugar
            // 
            buttonJugar.BackColor = Color.FromArgb(255, 215, 0);
            buttonJugar.FlatStyle = FlatStyle.Flat;
            buttonJugar.FlatAppearance.BorderSize = 4;
            buttonJugar.FlatAppearance.BorderColor = Color.FromArgb(139, 69, 19);
            buttonJugar.Font = new Font("Impact", 28F, FontStyle.Regular);
            buttonJugar.ForeColor = Color.FromArgb(40, 20, 0);
            buttonJugar.Location = new Point(200, 180);
            buttonJugar.Name = "buttonJugar";
            buttonJugar.Size = new Size(400, 90);
            buttonJugar.TabIndex = 3;
            buttonJugar.Text = "⚔ INICIAR DUELO ⚔";
            buttonJugar.UseVisualStyleBackColor = false;
            buttonJugar.Click += buttonJugar_Click;
            buttonJugar.MouseEnter += ButtonJugar_MouseEnter;
            buttonJugar.MouseLeave += ButtonJugar_MouseLeave;

            // 
            // buttonSalir
            // 
            buttonSalir.BackColor = Color.FromArgb(255, 215, 0);
            buttonSalir.FlatStyle = FlatStyle.Flat;
            buttonSalir.FlatAppearance.BorderSize = 4;
            buttonSalir.FlatAppearance.BorderColor = Color.FromArgb(139, 69, 19);
            buttonSalir.Font = new Font("Impact", 28F, FontStyle.Regular);
            buttonSalir.ForeColor = Color.FromArgb(40, 20, 0);
            buttonSalir.Location = new Point(200, 300);
            buttonSalir.Name = "buttonSalir";
            buttonSalir.Size = new Size(400, 90);
            buttonSalir.TabIndex = 4;
            buttonSalir.Text = "✖ SALIR ✖";
            buttonSalir.UseVisualStyleBackColor = false;
            buttonSalir.Click += buttonSalir_Click;
            buttonSalir.MouseEnter += ButtonSalir_MouseEnter;
            buttonSalir.MouseLeave += ButtonSalir_MouseLeave;

            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 15, 40);
            ClientSize = new Size(800, 450);
            Controls.Add(buttonSalir);
            Controls.Add(buttonJugar);
            Controls.Add(labelTitulo);
            Controls.Add(pictureBoxFondo);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Yu-Gi-Oh! Duel Monsters";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBoxFondo).EndInit();
            ResumeLayout(false);
        }

        // Cargar fondo al iniciar
        private void Form1_Load(object sender, EventArgs e)
        {
            CrearFondoYuGiOh();
        }

        // Crear fondo épico de Yu-Gi-Oh
        private void CrearFondoYuGiOh()
        {
            Bitmap fondo = new Bitmap(800, 450);
            using (Graphics g = Graphics.FromImage(fondo))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                // Degradado base (púrpura oscuro a azul)
                using (var degradado = new System.Drawing.Drawing2D.LinearGradientBrush(
                    new Rectangle(0, 0, 800, 450),
                    Color.FromArgb(15, 5, 35),
                    Color.FromArgb(45, 15, 70),
                    45F))
                {
                    g.FillRectangle(degradado, 0, 0, 800, 450);
                }

                // Agregar círculos místicos de fondo
                using (Pen circuloPen = new Pen(Color.FromArgb(40, 255, 215, 0), 2))
                {
                    g.DrawEllipse(circuloPen, 50, 50, 300, 300);
                    g.DrawEllipse(circuloPen, 450, 100, 300, 300);
                    g.DrawEllipse(circuloPen, 250, 200, 200, 200);
                }

                // Pentagrama grande (Sello del Milenio)
                DibujarPentagrama(g, 400, 225, 180, Color.FromArgb(25, 255, 215, 0));

                // Estrellas brillantes
                Random rand = new Random(42);
                for (int i = 0; i < 80; i++)
                {
                    int x = rand.Next(800);
                    int y = rand.Next(450);
                    int size = rand.Next(2, 6);
                    int alpha = rand.Next(50, 150);

                    using (SolidBrush estrella = new SolidBrush(Color.FromArgb(alpha, 255, 215, 0)))
                    {
                        g.FillEllipse(estrella, x, y, size, size);
                    }
                }

                // Ojo del Milenio simplificado
                DibujarOjoMilenio(g, 400, 100);
            }

            pictureBoxFondo.Image = fondo;
        }

        // Dibujar Ojo del Milenio
        private void DibujarOjoMilenio(Graphics g, int x, int y)
        {
            // Ojo exterior
            using (SolidBrush ojoBrush = new SolidBrush(Color.FromArgb(60, 255, 215, 0)))
            {
                g.FillEllipse(ojoBrush, x - 40, y - 20, 80, 40);
            }

            // Pupila
            using (SolidBrush pupilaBrush = new SolidBrush(Color.FromArgb(80, 139, 69, 19)))
            {
                g.FillEllipse(pupilaBrush, x - 15, y - 15, 30, 30);
            }

            // Brillo
            using (SolidBrush brilloBrush = new SolidBrush(Color.FromArgb(100, 255, 255, 200)))
            {
                g.FillEllipse(brilloBrush, x - 5, y - 8, 10, 10);
            }
        }

        // Dibujar pentagrama
        private void DibujarPentagrama(Graphics g, int cx, int cy, int radio, Color color)
        {
            PointF[] puntos = new PointF[5];
            double angulo = -Math.PI / 2;

            for (int i = 0; i < 5; i++)
            {
                puntos[i] = new PointF(
                    (float)(cx + radio * Math.Cos(angulo)),
                    (float)(cy + radio * Math.Sin(angulo))
                );
                angulo += 2 * Math.PI / 5;
            }

            using (Pen pen = new Pen(color, 3))
            {
                g.DrawLine(pen, puntos[0], puntos[2]);
                g.DrawLine(pen, puntos[2], puntos[4]);
                g.DrawLine(pen, puntos[4], puntos[1]);
                g.DrawLine(pen, puntos[1], puntos[3]);
                g.DrawLine(pen, puntos[3], puntos[0]);
            }
        }

        // Efectos hover para botón Jugar
        private void ButtonJugar_MouseEnter(object sender, EventArgs e)
        {
            buttonJugar.BackColor = Color.FromArgb(255, 50, 0);
            buttonJugar.ForeColor = Color.White;
            buttonJugar.Font = new Font("Impact", 30F, FontStyle.Regular);
            buttonJugar.FlatAppearance.BorderColor = Color.FromArgb(255, 69, 0);
        }

        private void ButtonJugar_MouseLeave(object sender, EventArgs e)
        {
            buttonJugar.BackColor = Color.FromArgb(255, 215, 0);
            buttonJugar.ForeColor = Color.FromArgb(40, 20, 0);
            buttonJugar.Font = new Font("Impact", 28F, FontStyle.Regular);
            buttonJugar.FlatAppearance.BorderColor = Color.FromArgb(139, 69, 19);
        }

        // Efectos hover para botón Salir
        private void ButtonSalir_MouseEnter(object sender, EventArgs e)
        {
            buttonSalir.BackColor = Color.FromArgb(220, 20, 60);
            buttonSalir.ForeColor = Color.White;
            buttonSalir.Font = new Font("Impact", 30F, FontStyle.Regular);
            buttonSalir.FlatAppearance.BorderColor = Color.FromArgb(139, 0, 0);
        }

        private void ButtonSalir_MouseLeave(object sender, EventArgs e)
        {
            buttonSalir.BackColor = Color.FromArgb(255, 215, 0);
            buttonSalir.ForeColor = Color.FromArgb(40, 20, 0);
            buttonSalir.Font = new Font("Impact", 28F, FontStyle.Regular);
            buttonSalir.FlatAppearance.BorderColor = Color.FromArgb(139, 69, 19);
        }
    }
}