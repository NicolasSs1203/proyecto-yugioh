using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using YuGiOh.Models;
using YuGiOh.Models.Cartas;
using YuGiOh.Enums;

namespace YuGiOh
{
    public partial class MainGameForm : Form
    {
        private Juego juego;
        private bool modoAtaque = false;
        private CartaMonstruo? monstruoAtacante = null;
    
        
        // Paneles principales
        private Panel panelOponente = null!;
        private Panel panelJugador = null!;
        private Panel panelMano = null!;
        private Panel panelControles = null!;

        // Controles
        private Label lblTurno = null!;
        private Label lblVidaJugador = null!;
        private Label lblVidaOponente = null!;
        private Label lblFase = null!;
        private Button btnFaseRobo = null!;
        private Button btnFasePrincipal = null!;
        private Button btnFaseBatalla = null!;
        private Button btnTerminarTurno = null!;
        private TextBox txtLog = null!;

        public MainGameForm(Juego juego)
        {
            this.juego = juego;
            InicializarInterfaz();
            EstablecerFondo();
            ActualizarInterfaz();
            AgregarLog("¡DUELO INICIADO!");
        }

        private void InicializarInterfaz()
        {
            Text = $"Yu-Gi-Oh! - {juego.Jugador1.Nombre} vs {juego.Jugador2.Nombre}";
            Size = new Size(1000, 750);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.DarkGreen;

            // Panel del oponente (arriba)
            panelOponente = new Panel
            {
                Dock = DockStyle.Top,
                Height = 280,
                BackColor = Color.FromArgb(150, 0, 0, 0),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Panel del jugador (centro)
            panelJugador = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Panel de la mano (abajo)
            panelMano = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 140,
                BackColor = Color.FromArgb(150, 0, 0, 0),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Panel de controles (derecha)
            panelControles = new Panel
            {
                Dock = DockStyle.Right,
                Width = 250,
                BackColor = Color.FromArgb(200, 50, 50, 50)
            };
            /*
            // Panel de log (abajo-derecha)
            var panelLog = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 120,
                BackColor = Color.Black
            };

            txtLog = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                BackColor = Color.Black,
                ForeColor = Color.LightGreen,
                Font = new Font("Consolas", 8),
                ReadOnly = true
            };
            panelLog.Controls.Add(txtLog);
            */
            // Crear controles de información
            lblTurno = new Label
            {
                Text = "Turno: Yugi",
                ForeColor = Color.White,
                Font = new Font("Arial", 11, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };

            lblFase = new Label
            {
                Text = "Fase: Robo",
                ForeColor = Color.Yellow,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(10, 35),
                AutoSize = true
            };

            lblVidaJugador = new Label
            {
                Text = "Tu Vida: 8000",
                ForeColor = Color.LightBlue,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(10, 65),
                AutoSize = true
            };

            lblVidaOponente = new Label
            {
                Text = "Oponente: 8000",
                ForeColor = Color.LightCoral,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(10, 90),
                AutoSize = true
            };

            // Botones de fases
            btnFaseRobo = new Button
            {
                Text = "Fase de Robo",
                Location = new Point(10, 130),
                Size = new Size(200, 35),
                BackColor = Color.SteelBlue,
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            btnFaseRobo.Click += (s, e) => EjecutarFaseRobo();

            btnFasePrincipal = new Button
            {
                Text = "Fase Principal",
                Location = new Point(10, 175),
                Size = new Size(200, 35),
                BackColor = Color.Goldenrod,
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            btnFasePrincipal.Click += (s, e) => EjecutarFasePrincipal();

            btnFaseBatalla = new Button
            {
                Text = "Fase de Batalla",
                Location = new Point(10, 220),
                Size = new Size(200, 35),
                BackColor = Color.OrangeRed,
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            btnFaseBatalla.Click += (s, e) => EjecutarFaseBatalla();

            btnTerminarTurno = new Button
            {
                Text = "Terminar Turno",
                Location = new Point(10, 265),
                Size = new Size(200, 35),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            btnTerminarTurno.Click += (s, e) => TerminarTurno();

            // Agregar controles al panel
            panelControles.Controls.AddRange(new Control[]
            {
                lblTurno, lblFase, lblVidaJugador, lblVidaOponente,
                btnFaseRobo, btnFasePrincipal, btnFaseBatalla, btnTerminarTurno
            });

            // Agregar paneles al formulario
            Controls.AddRange(new Control[]
            {
                panelJugador,
                panelMano,
                panelOponente,
                panelControles
            });
        }

        private void ActualizarInterfaz()
        {
            ActualizarInformacion();
            ActualizarMano();
            ActualizarCampoJugador();
            ActualizarCampoOponente();
        }

        private void ActualizarInformacion()
        {
            var jugadorActual = juego.JugadorActual;
            var oponente = juego.JugadorOponente;

            lblTurno.Text = $"Turno {juego.NumeroTurno}: {jugadorActual.Nombre}";
            lblVidaJugador.Text = $"Tu Vida: {jugadorActual.PuntosVida}";
            lblVidaOponente.Text = $"Oponente: {oponente.PuntosVida}";
        }

        private void ActualizarMano()
        {
            panelMano.Controls.Clear();
            var mano = juego.JugadorActual.Mano;

            int x = 10;
            foreach (var carta in mano)
            {
                var controlCarta = CrearControlCarta(carta, true);
                controlCarta.Location = new Point(x, 15);
                controlCarta.Click += (s, e) => CartaManoClickeada(carta);
                panelMano.Controls.Add(controlCarta);
                x += 85;
            }

            // Etiqueta de la mano
            var lblMano = new Label
            {
                Text = $"MANO ({mano.Count} cartas)",
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold),
                Location = new Point(10, 120),
                AutoSize = true
            };
            panelMano.Controls.Add(lblMano);
        }

        private void ActualizarCampoJugador()
        {
            panelJugador.Controls.Clear();
            var campo = juego.JugadorActual.MiCampo;

            // Título del campo
            var lblCampoJugador = new Label
            {
                Text = $"CAMPO DE {juego.JugadorActual.Nombre}",
                ForeColor = Color.LightBlue,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(10, 10),
                AutoSize = true
            };
            panelJugador.Controls.Add(lblCampoJugador);

            // ZONA DE MONSTRUOS (3 espacios) - Más centrados
            int xMonstruos = 250; // Centrado mejor
            int yMonstruos = 150; // Más abajo

            for (int i = 0; i < 3; i++)
            {
                var monstruo = campo.ZonaMonstruos[i];

                if (monstruo != null)
                {
                    var controlCarta = CrearControlCarta(monstruo, true);
                    controlCarta.Location = new Point(xMonstruos, yMonstruos);
                    controlCarta.Click += (s, e) => MonstruoCampoClickeado(monstruo, i);
                    panelJugador.Controls.Add(controlCarta);
                }
                else
                {
                    var zonaVacia = new Panel
                    {
                        Size = new Size(80, 120),
                        Location = new Point(xMonstruos, yMonstruos),
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.FromArgb(50, 255, 255, 255)
                    };
                    var lblZona = new Label
                    {
                        Text = $"M{i}",
                        ForeColor = Color.Gray,
                        Location = new Point(30, 50),
                        AutoSize = true
                    };
                    zonaVacia.Controls.Add(lblZona);
                    panelJugador.Controls.Add(zonaVacia);
                }
                xMonstruos += 100; // Más espaciado
            }

            // ZONA DE MÁGICAS/TRAMPAS (3 espacios) - Encima de monstruos
            int xMagicas = 250; // Mismo inicio que monstruos
            int yMagicas = 20; // Arriba

            for (int i = 0; i < 3; i++)
            {
                var carta = campo.ZonaMagiaTrampa[i];

                if (carta != null)
                {
                    var controlCarta = CrearControlCarta(carta, true);
                    controlCarta.Size = new Size(80, 100);
                    controlCarta.Location = new Point(xMagicas, yMagicas);
                    controlCarta.Click += (s, e) => MagicaTrampaClickeada(carta, i);
                    panelJugador.Controls.Add(controlCarta);
                }
                else
                {
                    var zonaVacia = new Panel
                    {
                        Size = new Size(80, 100),
                        Location = new Point(xMagicas, yMagicas),
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.FromArgb(50, 255, 200, 100)
                    };
                    var lblZona = new Label
                    {
                        Text = $"M/T",
                        ForeColor = Color.Gray,
                        Location = new Point(25, 40),
                        AutoSize = true
                    };
                    zonaVacia.Controls.Add(lblZona);
                    panelJugador.Controls.Add(zonaVacia);
                }
                xMagicas += 100;
            }

            // Deck (izquierda)
            var panelDeck = new Panel
            {
                Size = new Size(70, 100),
                Location = new Point(30, 80),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.DarkBlue
            };
            var lblDeck = new Label
            {
                Text = $"DECK\n{juego.JugadorActual.Baraja.Count}",
                ForeColor = Color.White,
                Font = new Font("Arial", 8, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            panelDeck.Controls.Add(lblDeck);
            panelJugador.Controls.Add(panelDeck);

            // Cementerio (derecha)
            var panelCementerio = new Panel
            {
                Size = new Size(70, 100),
                Location = new Point(620, 80),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.DarkViolet
            };
            var lblCementerio = new Label
            {
                Text = $"GY\n{juego.JugadorActual.Cementerio.Count}",
                ForeColor = Color.White,
                Font = new Font("Arial", 8, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            panelCementerio.Controls.Add(lblCementerio);
            panelJugador.Controls.Add(panelCementerio);

        }
        

        private void ActualizarCampoOponente()
        {
            panelOponente.Controls.Clear();
            var campo = juego.JugadorOponente.MiCampo;

            // Título
            var lblCampoOponente = new Label
            {
                Text = $"CAMPO DE {juego.JugadorOponente.Nombre}",
                ForeColor = Color.LightCoral,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(10, 5),
                AutoSize = true
            };
            panelOponente.Controls.Add(lblCampoOponente);

            // Deck del oponente (izquierda arriba)
            var panelDeck = new Panel
            {
                Size = new Size(70, 80),
                Location = new Point(30, 25),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.DarkRed
            };
            var lblDeck = new Label
            {
                Text = $"DECK\n{juego.JugadorOponente.Baraja.Count}",
                ForeColor = Color.White,
                Font = new Font("Arial", 8, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            panelDeck.Controls.Add(lblDeck);
            panelOponente.Controls.Add(panelDeck);

            // Cementerio del oponente (derecha arriba)
            var panelCementerio = new Panel
            {
                Size = new Size(70, 80),
                Location = new Point(620, 25),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.DarkSlateGray
            };
            var lblCementerio = new Label
            {
                Text = $"GY\n{juego.JugadorOponente.Cementerio.Count}",
                ForeColor = Color.White,
                Font = new Font("Arial", 8, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            panelCementerio.Controls.Add(lblCementerio);
            panelOponente.Controls.Add(panelCementerio);

            // ZONA DE MONSTRUOS DEL OPONENTE (fila central)
            int xMonstruos = 250;
            int yMonstruos = 25;

            for (int i = 0; i < 3; i++)
            {
                var monstruo = campo.ZonaMonstruos[i];

                if (monstruo != null)
                {
                    var controlCarta = CrearControlCarta(monstruo, false);
                    controlCarta.Location = new Point(xMonstruos, yMonstruos);
                    controlCarta.Click += (s, e) => MonstruoOponenteClickeado(monstruo, i);
                    panelOponente.Controls.Add(controlCarta);
                }
                else
                {
                    var zonaVacia = new Panel
                    {
                        Size = new Size(80, 100),
                        Location = new Point(xMonstruos, yMonstruos),
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.FromArgb(50, 255, 255, 255)
                    };
                    panelOponente.Controls.Add(zonaVacia);
                }
                xMonstruos += 100;
            }

            // ZONA DE MÁGICAS/TRAMPAS (fila inferior)
            int xMagicas = 250;
            int yMagicas = 135; // Debajo de los monstruos

            for (int i = 0; i < 3; i++)
            {
                var carta = campo.ZonaMagiaTrampa[i];

                if (carta != null)
                {
                    var controlCarta = CrearControlCarta(carta, false);
                    controlCarta.Size = new Size(80, 100);
                    controlCarta.Location = new Point(xMagicas, yMagicas);
                    panelOponente.Controls.Add(controlCarta);
                }
                else
                {
                    var zonaVacia = new Panel
                    {
                        Size = new Size(80, 100),
                        Location = new Point(xMagicas, yMagicas),
                        BorderStyle = BorderStyle.FixedSingle,
                        BackColor = Color.FromArgb(50, 255, 200, 100)
                    };
                    panelOponente.Controls.Add(zonaVacia);
                }
                xMagicas += 100;
            }
        }
        

        private Control CrearControlCarta(Carta carta, bool esJugadorActual)
        {
            var panel = new Panel
            {
                Size = esJugadorActual ? new Size(80, 120) : new Size(80, 100),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = ObtenerColorCarta(carta),
                Cursor = Cursors.Hand
            };

            // Nombre de la carta
            var lblNombre = new Label
            {
                Text = carta.Nombre.Length > 10 ? carta.Nombre.Substring(0, 10) + "..." : carta.Nombre,
                Font = new Font("Arial", 7, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(2, 2),
                Size = new Size(76, 30),
                TextAlign = ContentAlignment.TopCenter
            };

            if (carta is CartaMonstruo monstruo)
            {
                var lblStats = new Label
                {
                    Text = $"ATK:{monstruo.Ataque}/DEF:{monstruo.Defensa}",
                    Font = new Font("Arial", 7, FontStyle.Bold),
                    ForeColor = Color.Yellow,
                    Location = new Point(2, esJugadorActual ? 95 : 75),
                    Size = new Size(76, 20),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                panel.Controls.Add(lblStats);

                var lblNivel = new Label
                {
                    Text = $"Nivel: {monstruo.Nivel}",
                    Font = new Font("Arial", 6),
                    ForeColor = Color.White,
                    Location = new Point(2, 35),
                    Size = new Size(76, 12),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                panel.Controls.Add(lblNivel);
            }
            else
            {
                var lblTipo = new Label
                {
                    Text = carta.Tipo.ToString(),
                    Font = new Font("Arial", 7, FontStyle.Bold),
                    ForeColor = Color.White,
                    Location = new Point(2, 35),
                    Size = new Size(76, 20),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                panel.Controls.Add(lblTipo);
            }

            // Indicador de posición/boca abajo
            if (carta.EstaBocaAbajo)
            {
                panel.BackColor = Color.DarkRed;
                var lblBocaAbajo = new Label
                {
                    Text = "?",
                    Font = new Font("Arial", 14, FontStyle.Bold),
                    ForeColor = Color.White,
                    Location = new Point(25, esJugadorActual ? 50 : 40),
                    Size = new Size(30, 30),
                    TextAlign = ContentAlignment.MiddleCenter
                };
                panel.Controls.Add(lblBocaAbajo);
            }
            else
            {
                panel.Controls.Add(lblNombre);
            }

            // Tooltip con información completa
            var tooltip = new ToolTip();
            string info = $"{carta.Nombre}\n{carta.Descripcion}";
            if (carta is CartaMonstruo m)
            {
                info += $"\nNivel: {m.Nivel} | ATK:{m.Ataque}/DEF:{m.Defensa}";
                info += $"\nPosición: {m.Posicion}";
            }
            tooltip.SetToolTip(panel, info);

            return panel;
        }

        private Color ObtenerColorCarta(Carta carta)
        {
            if (carta.EstaBocaAbajo) return Color.DarkRed;
            
            return carta.Tipo switch
            {
                TipoCarta.Monstruo => Color.SaddleBrown,
                TipoCarta.Magica => Color.DarkBlue,
                TipoCarta.Trampa => Color.DarkMagenta,
                _ => Color.Gray
            };
        }

        // Métodos para el log
        private void AgregarLog(string mensaje)
        {
            /*
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action<string>(AgregarLog), mensaje);
                return;
            }
            
            txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {mensaje}\r\n");
            txtLog.SelectionStart = txtLog.Text.Length;
            txtLog.ScrollToCaret();
            */
        }

        // Event handlers para las fases
        private void EjecutarFaseRobo()
        {
            AgregarLog($"--- FASE DE ROBO - {juego.JugadorActual.Nombre} ---");
            juego.FaseRobo();
            ActualizarInterfaz();
            lblFase.Text = "Fase: Robo (Completada)";
            AgregarLog("Fase de Robo completada");
        }

        private void EjecutarFasePrincipal()
        {
            AgregarLog($"--- FASE PRINCIPAL - {juego.JugadorActual.Nombre} ---");
            lblFase.Text = "Fase: Principal";
            
            // Aquí podrías mostrar un diálogo con opciones como en tu lógica de consola
            MostrarDialogoFasePrincipal();
        }

        private void EjecutarFaseBatalla()
        {
            AgregarLog($"--- FASE DE BATALLA - {juego.JugadorActual.Nombre} ---");

            if (juego.NumeroTurno == 1)
            {
                MessageBox.Show("No se puede atacar en el primer turno.", "Fase de Batalla");
                return;
            }

            // Activar modo ataque
            modoAtaque = true;
            monstruoAtacante = null;
            lblFase.Text = "Fase: Batalla (Selecciona atacante)";
            AgregarLog("Modo ataque activado - Selecciona un monstruo para atacar");
            MessageBox.Show("Fase de Batalla - Selecciona un monstruo para atacar", "Fase de Batalla");
        }
        

        private void TerminarTurno()
        {
            AgregarLog($"{juego.JugadorActual.Nombre} termina su turno");
            juego.CambiarTurno();
            ActualizarInterfaz();
            lblFase.Text = "Fase: Robo";
            AgregarLog($"¡Turno de {juego.JugadorActual.Nombre}!");
            
            if (juego.VerificarVictoria())
            {
                MessageBox.Show($"¡{juego.JugadorOponente.Nombre} GANA EL DUELO!", "FIN DEL DUELO", 
                              MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Close();
            }
        }

        // Diálogo para Fase Principal
        private void MostrarDialogoFasePrincipal()
        {
            var dialog = new Form
            {
                Text = $"Fase Principal - {juego.JugadorActual.Nombre}",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog
            };

            var btnInvocarMonstruo = new Button { Text = "Invocar Monstruo", Location = new Point(20, 20), Size = new Size(150, 30) };
            var btnColocarMagicaTrampa = new Button { Text = "Colocar Mágica/Trampa", Location = new Point(20, 60), Size = new Size(150, 30) };
            var btnVerMano = new Button { Text = "Ver Mi Mano", Location = new Point(20, 100), Size = new Size(150, 30) };
            var btnVerCampo = new Button { Text = "Ver Mi Campo", Location = new Point(20, 140), Size = new Size(150, 30) };
            var btnVerCampoOponente = new Button { Text = "Ver Campo Oponente", Location = new Point(20, 180), Size = new Size(150, 30) };
            var btnTerminarFase = new Button { Text = "Terminar Fase", Location = new Point(200, 220), Size = new Size(150, 30) };

            btnInvocarMonstruo.Click += (s, e) => { MostrarDialogoInvocarMonstruo(); dialog.Close(); };
            btnColocarMagicaTrampa.Click += (s, e) => { MostrarDialogoColocarMagicaTrampa(); dialog.Close(); };
            btnVerMano.Click += (s, e) => { MostrarManoCompleta(); };
            btnVerCampo.Click += (s, e) => { MessageBox.Show("Tu campo está visible en la pantalla principal"); };
            btnVerCampoOponente.Click += (s, e) => { MessageBox.Show("El campo oponente está visible en la pantalla principal"); };
            btnTerminarFase.Click += (s, e) => dialog.Close();

            dialog.Controls.AddRange(new Control[] { btnInvocarMonstruo, btnColocarMagicaTrampa, btnVerMano, btnVerCampo, btnVerCampoOponente, btnTerminarFase });
            dialog.ShowDialog();
        }

        private void MostrarDialogoInvocarMonstruo()
        {
            // Implementar diálogo para invocar monstruo similar a tu lógica de consola
            var monstruos = juego.JugadorActual.Mano.Where(c => c.Tipo == TipoCarta.Monstruo).Cast<CartaMonstruo>().ToList();
            
            if (monstruos.Count == 0)
            {
                MessageBox.Show("No tienes monstruos en tu mano.", "Invocar Monstruo");
                return;
            }

            // Aquí puedes implementar un diálogo más complejo para seleccionar monstruo y posición
            AgregarLog("Diálogo de invocación de monstruo abierto");
        }

        private void MostrarDialogoColocarMagicaTrampa()
        {
            // Similar al de invocación pero para mágicas/trampas
            AgregarLog("Diálogo para colocar mágica/trampa abierto");
        }

        private void MostrarManoCompleta()
        {
            var mano = juego.JugadorActual.Mano;
            string info = $"TU MANO ({mano.Count} cartas):\n\n";
            
            foreach (var carta in mano)
            {
                info += $"{carta.Nombre} ({carta.Tipo})";
                if (carta is CartaMonstruo m) info += $" - ATK:{m.Ataque}/DEF:{m.Defensa}";
                info += "\n";
            }
            
            MessageBox.Show(info, "Tu Mano");
        }

        // Event handlers para clicks en cartas
        private void CartaManoClickeada(Carta carta)
        {
            AgregarLog($"Carta clickeada: {carta.Nombre} (en mano)");
            // Podrías mostrar opciones específicas dependiendo del tipo de carta
            MostrarDialogoOpcionesCarta(carta);

        } 
        
        private void MonstruoCampoClickeado(CartaMonstruo monstruo, int posicion)
        {
            if (modoAtaque)
            {
                // Estamos en fase de batalla
                if (monstruoAtacante == null)
                {
                    // Seleccionar monstruo atacante
                    if (monstruo.Posicion != PosicionMonstruo.Ataque)
                    {
                        MessageBox.Show("Solo los monstruos en posición de ataque pueden atacar.", "Batalla");
                        return;
                    }

                    if (monstruo.EstaBocaAbajo)
                    {
                        MessageBox.Show("Los monstruos boca abajo no pueden atacar.", "Batalla");
                        return;
                    }

                    monstruoAtacante = monstruo;
                    lblFase.Text = "Fase: Batalla (Selecciona objetivo)";
                    AgregarLog($"Monstruo atacante seleccionado: {monstruo.Nombre}");
                    MessageBox.Show($"{monstruo.Nombre} seleccionado como atacante. Ahora selecciona el objetivo.", "Batalla");
                }
                else
                {
                    // Seleccionar monstruo objetivo
                    RealizarAtaque(monstruoAtacante, monstruo, posicion);
                }
            }
            else
            {
                // Comportamiento normal (fuera de batalla)
                AgregarLog($"Monstruo clickeado: {monstruo.Nombre} (posición {posicion})");
            }
            }

        private void MagicaTrampaClickeada(Carta carta, int posicion)
        {
            AgregarLog($"Carta {carta.Tipo} clickeada: {carta.Nombre} (posición {posicion})");
        }

        private void MonstruoOponenteClickeado(CartaMonstruo monstruo, int posicion)
        {
            if (modoAtaque && monstruoAtacante != null)
            {
                // Atacar a monstruo del oponente
                RealizarAtaque(monstruoAtacante, monstruo, posicion);
            }
            else if (modoAtaque)
            {
                // Ataque directo (click en zona vacía del oponente)
                bool hayMonstruosOponente = juego.JugadorOponente.MiCampo.ZonaMonstruos.Any(m => m != null);

                if (!hayMonstruosOponente && monstruoAtacante != null)
                {
                    RealizarAtaqueDirecto(monstruoAtacante);
                }
                else if (monstruoAtacante == null)
                {
                    AgregarLog("Primero selecciona un monstruo atacante");
                }
            }
            else
            {
                // Comportamiento normal
                AgregarLog($"Monstruo oponente clickeado: {monstruo.Nombre} (posición {posicion})");
            }
    
            }
        private void MostrarDialogoOpcionesCarta(Carta carta)
        {
            var dialog = new Form
            {
                Text = $"¿Qué hacer con {carta.Nombre}?",
                Size = new Size(300, 180),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            // Etiqueta
            var lblPregunta = new Label
            {
                Text = $"Elige una acción para:\n{carta.Nombre}",
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(250, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Botón Invocar (solo para monstruos)
            // Botón para colocar en el campo (nombre diferente según el tipo)

            var textoBoton = carta.Tipo == TipoCarta.Monstruo ? "Invocar" : "Colocar";
            var btnColocar = new Button
            {
                Text = textoBoton,
                Location = new Point(30, 70),
                Size = new Size(100, 30),
                Enabled = true // Siempre habilitado para cualquier tipo
            };

            btnColocar.Click += (s, e) =>
            {
                dialog.Close();
                ColocarCartaEnCampo(carta); // Nuevo método que maneja ambos tipos
            };


            // Botón Ver Carta
            var btnVerCarta = new Button
            {
                Text = "Ver Carta",
                Location = new Point(150, 70),
                Size = new Size(100, 30)
            };
            btnVerCarta.Click += (s, e) =>
            {
                dialog.Close();
                MostrarDetallesCarta(carta);
            };

            // Botón Cancelar
            var btnCancelar = new Button
            {
                Text = "Cancelar",
                Location = new Point(100, 110),
                Size = new Size(100, 30)
            };
            btnCancelar.Click += (s, e) => dialog.Close();

            dialog.Controls.AddRange(new Control[] { lblPregunta, btnColocar, btnVerCarta, btnCancelar });
            dialog.ShowDialog();
        }
        private void InvocarCarta(Carta carta)
        {
            if (carta.Tipo != TipoCarta.Monstruo)
            {
                MessageBox.Show("Solo los monstruos pueden ser invocados.", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var monstruo = carta as CartaMonstruo;
            if (monstruo == null) return;

            // Mostrar diálogo para elegir posición en el campo
            var dialogPosicion = new Form
            {
                Text = $"Invocar {monstruo.Nombre}",
                Size = new Size(300, 200),
                StartPosition = FormStartPosition.CenterParent
            };

            var lblPosicion = new Label
            {
                Text = "Elige posición en la zona de monstruos (0-2):",
                Location = new Point(20, 20),
                AutoSize = true
            };

            var txtPosicion = new NumericUpDown
            {
                Location = new Point(20, 50),
                Size = new Size(50, 20),
                Minimum = 0,
                Maximum = 2,
                Value = 0
            };

            var lblModo = new Label
            {
                Text = "Modo de invocación:",
                Location = new Point(20, 80),
                AutoSize = true
            };

            var comboModo = new ComboBox
            {
                Location = new Point(20, 100),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            comboModo.Items.Add("Ataque");
            comboModo.Items.Add("Defensa");
            comboModo.SelectedIndex = 0;

            var btnAceptar = new Button { Text = "Invocar", Location = new Point(20, 130), Size = new Size(80, 30) };
            var btnCancelar = new Button { Text = "Cancelar", Location = new Point(120, 130), Size = new Size(80, 30) };

            btnAceptar.Click += (s, e) =>
            {
                int posicion = (int)txtPosicion.Value;
                var modo = (string)comboModo.SelectedItem!;

                // Configurar el monstruo
                monstruo.Posicion = modo == "Ataque" ? PosicionMonstruo.Ataque : PosicionMonstruo.Defensa;
                monstruo.EstaBocaAbajo = false;

                // Invocar el monstruo usando TU método existente
                juego.JugadorActual.JugarMonstruo(monstruo, posicion);

                AgregarLog($"{monstruo.Nombre} invocado en posición {posicion} (modo {modo})");
                ActualizarInterfaz();
                dialogPosicion.Close();
            };

            btnCancelar.Click += (s, e) => dialogPosicion.Close();

            dialogPosicion.Controls.AddRange(new Control[]
            {
        lblPosicion, txtPosicion, lblModo, comboModo, btnAceptar, btnCancelar
            });
            dialogPosicion.ShowDialog();
        }
        private void MostrarDetallesCarta(Carta carta)
        {
            var dialog = new Form
            {
                Text = $"Detalles - {carta.Nombre}",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog
            };

            var txtDetalles = new TextBox
            {
                Location = new Point(10, 10),
                Size = new Size(365, 210),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                Font = new Font("Arial", 10)
            };

            // Construir información de la carta
            string detalles = $"NOMBRE: {carta.Nombre}\n";
            detalles += $"TIPO: {carta.Tipo}\n";
            detalles += $"DESCRIPCIÓN: {carta.Descripcion}\n";

            if (carta is CartaMonstruo monstruo)
            {
                detalles += $"\n--- ESTADÍSTICAS ---\n";
                detalles += $"ATK: {monstruo.Ataque} | DEF: {monstruo.Defensa}\n";
                detalles += $"NIVEL: {monstruo.Nivel}\n";
                detalles += $"ATRIBUTO: {monstruo.Atributo}\n";
                detalles += $"TIPO: {monstruo.TipoMonstruo}\n";
                detalles += $"POSICIÓN: {monstruo.Posicion}";
            }
            else if (carta is CartaMagica magica)
            {
                detalles += $"\nTIPO MÁGICA: {magica.TipoMagica}";
            }
            else if (carta is CartaTrampa trampa)
            {
                detalles += $"\nTIPO TRAMPA: {trampa.TipoTrampa}";
            }

            txtDetalles.Text = detalles;

            var btnCerrar = new Button
            {
                Text = "Cerrar",
                Location = new Point(150, 230),
                Size = new Size(100, 30)
            };
            btnCerrar.Click += (s, e) => dialog.Close();

            dialog.Controls.AddRange(new Control[] { txtDetalles, btnCerrar });
            dialog.ShowDialog();
        }
        private void ColocarCartaEnCampo(Carta carta)
        {
            if (carta.Tipo == TipoCarta.Monstruo)
            {
                InvocarCarta(carta); // Usa el método existente para monstruos
            }
            else
            {
                ColocarMagicaTrampa(carta); // Nuevo método para mágicas/trampas
            }
        }
        private void ColocarMagicaTrampa(Carta carta)
        {
            // Mostrar diálogo para elegir posición en el campo
            var dialogPosicion = new Form
            {
                Text = $"Colocar {carta.Nombre}",
                Size = new Size(300, 150),
                StartPosition = FormStartPosition.CenterParent
            };

            var lblPosicion = new Label
            {
                Text = "Elige posición en la zona de mágicas/trampas (0-2):",
                Location = new Point(20, 20),
                AutoSize = true
            };

            var txtPosicion = new NumericUpDown
            {
                Location = new Point(20, 50),
                Size = new Size(50, 20),
                Minimum = 0,
                Maximum = 2,
                Value = 0
            };

            var btnAceptar = new Button { Text = "Colocar", Location = new Point(20, 80), Size = new Size(80, 30) };
            var btnCancelar = new Button { Text = "Cancelar", Location = new Point(120, 80), Size = new Size(80, 30) };

            btnAceptar.Click += (s, e) =>
            {
                int posicion = (int)txtPosicion.Value;

                // Colocar la carta mágica/trampa usando TU método existente
                juego.JugadorActual.JugarMagiaTrampa(carta, posicion);

                AgregarLog($"{carta.Nombre} colocada en posición {posicion}");
                ActualizarInterfaz();
                dialogPosicion.Close();
            };

            btnCancelar.Click += (s, e) => dialogPosicion.Close();

            dialogPosicion.Controls.AddRange(new Control[]
            {
        lblPosicion, txtPosicion, btnAceptar, btnCancelar
            });
            dialogPosicion.ShowDialog();
        }
        private void RealizarAtaque(CartaMonstruo atacante, CartaMonstruo defensor, int posicionDefensor)
        {
            AgregarLog($"¡{atacante.Nombre} ataca a {defensor.Nombre}!");

            string resultado = "";
            int danio = 0;

            if (defensor.Posicion == PosicionMonstruo.Ataque)
            {
                if (atacante.Ataque > defensor.Ataque)
                {
                    danio = atacante.Ataque - defensor.Ataque;
                    juego.JugadorOponente.PuntosVida -= danio;
                    juego.JugadorOponente.MiCampo.ZonaMonstruos[posicionDefensor] = null;
                    juego.JugadorOponente.Cementerio.Add(defensor);
                    resultado = $"{defensor.Nombre} fue destruido. {juego.JugadorOponente.Nombre} pierde {danio} LP";
                }
                else if (atacante.Ataque < defensor.Ataque)
                {
                    danio = defensor.Ataque - atacante.Ataque;
                    juego.JugadorActual.PuntosVida -= danio;
                    var campoAtacante = juego.JugadorActual.MiCampo;
                    for (int i = 0; i < 3; i++)
                    {
                        if (campoAtacante.ZonaMonstruos[i] == atacante)
                        {
                            campoAtacante.ZonaMonstruos[i] = null;
                            break;
                        }
                    }
                    juego.JugadorActual.Cementerio.Add(atacante);
                    resultado = $"{atacante.Nombre} fue destruido. {juego.JugadorActual.Nombre} pierde {danio} LP";
                }
                else
                {
                    juego.JugadorActual.MiCampo.ZonaMonstruos = juego.JugadorActual.MiCampo.ZonaMonstruos
                        .Select(m => m == atacante ? null : m).ToArray();
                    juego.JugadorOponente.MiCampo.ZonaMonstruos[posicionDefensor] = null;
                    juego.JugadorActual.Cementerio.Add(atacante);
                    juego.JugadorOponente.Cementerio.Add(defensor);
                    resultado = "Ambos monstruos fueron destruidos";
                }
            }
            else
            {
                if (atacante.Ataque > defensor.Defensa)
                {
                    juego.JugadorOponente.MiCampo.ZonaMonstruos[posicionDefensor] = null;
                    juego.JugadorOponente.Cementerio.Add(defensor);
                    resultado = $"{defensor.Nombre} fue destruido";
                }
                else if (atacante.Ataque < defensor.Defensa)
                {
                    danio = defensor.Defensa - atacante.Ataque;
                    juego.JugadorActual.PuntosVida -= danio;
                    resultado = $"{atacante.Nombre} no puede destruir a {defensor.Nombre}. {juego.JugadorActual.Nombre} pierde {danio} LP";
                }
                else
                {
                    resultado = "El ataque fue bloqueado - ningún monstruo es destruido";
                }
            }

            AgregarLog($"Resultado: {resultado}");
            MessageBox.Show(resultado, "Resultado de Batalla");

            modoAtaque = false;
            monstruoAtacante = null;
            lblFase.Text = "Fase: Batalla (Completada)";

            if (juego.JugadorActual.PuntosVida <= 0 || juego.JugadorOponente.PuntosVida <= 0)
            {
                juego.VerificarVictoria();
            }

            ActualizarInterfaz();
        }
        private void RealizarAtaqueDirecto(CartaMonstruo atacante)
        {
            AgregarLog($"¡{atacante.Nombre} realiza ataque directo!");

            // Verificar si realmente no hay monstruos defensores
            bool hayDefensores = juego.JugadorOponente.MiCampo.ZonaMonstruos.Any(m => m != null);

            if (hayDefensores)
            {
                MessageBox.Show("No puedes realizar ataque directo mientras el oponente tenga monstruos en el campo.", "Ataque Directo");
                return;
            }

            // Ataque directo exitoso
            int danio = atacante.Ataque;
            juego.JugadorOponente.PuntosVida -= danio;

            string resultado = $"¡Ataque directo exitoso! {juego.JugadorOponente.Nombre} pierde {danio} LP";

            AgregarLog($"Resultado: {resultado}");
            MessageBox.Show(resultado, "Ataque Directo");

            // Resetear modo batalla
            modoAtaque = false;
            monstruoAtacante = null;
            lblFase.Text = "Fase: Batalla (Completada)";

            // Verificar victoria
            if (juego.JugadorOponente.PuntosVida <= 0)
            {
                juego.VerificarVictoria();
            }

            ActualizarInterfaz();
        }
        private void CampoOponenteClickeado()
        {
            if (modoAtaque && monstruoAtacante != null)
            {
                // Verificar si no hay monstruos del oponente para ataque directo
                bool hayMonstruosOponente = juego.JugadorOponente.MiCampo.ZonaMonstruos.Any(m => m != null);

                if (!hayMonstruosOponente)
                {
                    RealizarAtaqueDirecto(monstruoAtacante);
                }
                else
                {
                    MessageBox.Show("No puedes realizar ataque directo mientras el oponente tenga monstruos en el campo.", "Ataque Directo");
                }
            }
        }
        private void EstablecerFondo()
{
    try
    {
        // Opción 1: Si la imagen está en una carpeta Resources
        string rutaImagen = @"C:\Users\Nicolas\programming\yugiohGui\YuGiOhForms\Resources\Tablero.jpg";
        
        if (File.Exists(rutaImagen))
        {
            this.BackgroundImage = Image.FromFile(rutaImagen);
            this.BackgroundImageLayout = ImageLayout.Stretch; // Ajusta la imagen al formulario
        }
        else
        {
            // Opción 2: Fondo sólido si no hay imagen
            this.BackColor = Color.DarkGreen;
            Console.WriteLine("Imagen de fondo no encontrada. Usando color sólido.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error cargando imagen de fondo: {ex.Message}");
        this.BackColor = Color.DarkGreen; // Fallback
    }
}







        
        
    }
}