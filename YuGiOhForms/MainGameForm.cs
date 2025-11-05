using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using YuGiOh.Models;
using YuGiOh.Models.Cartas;
using YuGiOh.Enums;

namespace YuGiOh
{
    public partial class MainGameForm : Form
    {
        // Importar función de Windows para deshabilitar redibujado
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        private const int WM_SETREDRAW = 0x000B;

        private Juego juego;
        private bool modoAtaque = false;
        private CartaMonstruo? monstruoAtacante = null;
        
        // Caché de imágenes para evitar cargarlas múltiples veces
        private static Dictionary<string, Image> cacheImagenes = new Dictionary<string, Image>();
        
        private void BeginUpdate()
        {
            SendMessage(this.Handle, WM_SETREDRAW, 0, 0);
        }

        private void EndUpdate()
        {
            SendMessage(this.Handle, WM_SETREDRAW, 1, 0);
            this.Refresh();
        }

        private void BeginPanelUpdate(Control panel)
        {
            SendMessage(panel.Handle, WM_SETREDRAW, 0, 0);
        }

        private void EndPanelUpdate(Control panel)
        {
            SendMessage(panel.Handle, WM_SETREDRAW, 1, 0);
            panel.Refresh();
        }
        
        private Image ObtenerImagenEnCache(string ruta)
        {
            if (!cacheImagenes.ContainsKey(ruta))
            {
                // Intentar con la ruta tal como está
                string rutaBuscada = ruta;
                if (!System.IO.File.Exists(rutaBuscada))
                {
                    // Si no existe, intentar con ruta relativa al directorio de ejecución
                    var directorioEjecucion = AppContext.BaseDirectory;
                    rutaBuscada = System.IO.Path.Combine(directorioEjecucion, ruta);
                }

                if (System.IO.File.Exists(rutaBuscada))
                {
                    cacheImagenes[ruta] = Image.FromFile(rutaBuscada);
                }
            }
            return cacheImagenes.ContainsKey(ruta) ? cacheImagenes[ruta] : null;
        }
    
        
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
        private Button btnAutores = null!;
        private TextBox txtLog = null!;

        public MainGameForm(Juego juego)
        {
            this.juego = juego;
            
            // Configuración optimizada de renderizado
            this.DoubleBuffered = true;
            this.AutoScaleMode = AutoScaleMode.Font;
            
            // Deshabilitar actualizaciones de ventana durante inicialización
            BeginUpdate();
            
            try
            {
                InicializarInterfaz();
                EstablecerFondo();
                ActualizarInterfaz();
                AgregarLog("¡DUELO INICIADO!");
            }
            finally
            {
                // Habilitar redibujado después de la inicialización
                EndUpdate();
            }
        }

        private void InicializarInterfaz()
        {
            // Suspender layout durante inicialización
            this.SuspendLayout();
            
            Text = $"Yu-Gi-Oh! - {juego.Jugador1.Nombre} vs {juego.Jugador2.Nombre}";
            Size = new Size(1000, 750);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.DarkGreen;

            // Agregar icono al formulario
            try
            {
                string iconPath = Path.Combine(AppContext.BaseDirectory, "Resources", "170902742.jpg");
                if (File.Exists(iconPath))
                {
                    using (var imgIcon = Image.FromFile(iconPath))
                    {
                        this.Icon = Icon.FromHandle(((Bitmap)imgIcon).GetHicon());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al cargar icono: {ex.Message}");
            }

            // Panel del oponente (arriba)
            panelOponente = new Panel
            {
                Dock = DockStyle.Top,
                Height = 280,
                BackColor = Color.FromArgb(150, 0, 0, 0),
                BorderStyle = BorderStyle.FixedSingle
            };
            panelOponente.Click += (s, e) => CampoOponenteClickeado();

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

            btnAutores = new Button
            {
                Text = "Autores",
                Location = new Point(10, 670),
                Size = new Size(200, 35),
                BackColor = Color.DarkSlateGray,
                ForeColor = Color.White,
                Font = new Font("Arial", 9, FontStyle.Bold)
            };
            btnAutores.Click += (s, e) => MostrarAutores();

            // Agregar controles al panel
            panelControles.Controls.AddRange(new Control[]
            {
                lblTurno, lblFase, lblVidaJugador, lblVidaOponente,
                btnFaseRobo, btnFasePrincipal, btnFaseBatalla, btnTerminarTurno, btnAutores
            });

            // Agregar paneles al formulario
            Controls.AddRange(new Control[]
            {
                panelJugador,
                panelMano,
                panelOponente,
                panelControles
            });
            
            // Reanudar layout después de la inicialización
            this.ResumeLayout(false);
        }

        private void ActualizarInterfaz()
        {
            BeginUpdate();
            
            try
            {
                ActualizarInformacion();
                ActualizarMano();
                ActualizarCampoJugador();
                ActualizarCampoOponente();
            }
            finally
            {
                EndUpdate();
            }
        }

        // Actualización rápida solo de información y campos (sin mano)
        private void ActualizarCamposRapido()
        {
            BeginUpdate();
            
            try
            {
                ActualizarInformacion();
                ActualizarCampoJugador();
                ActualizarCampoOponente();
            }
            finally
            {
                EndUpdate();
            }
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
            panelMano.SuspendLayout();
            BeginPanelUpdate(panelMano);
            try
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
            finally
            {
                EndPanelUpdate(panelMano);
                panelMano.ResumeLayout(false);
            }
        }

        private void ActualizarCampoJugador()
        {
            panelJugador.SuspendLayout();
            BeginPanelUpdate(panelJugador);
            try
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
                int yMonstruos = 20; // Arriba

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

                // ZONA DE MÁGICAS/TRAMPAS (3 espacios) - Debajo de monstruos
                int xMagicas = 250; // Mismo inicio que monstruos
                int yMagicas = 150; // Abajo

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
            finally
            {
                EndPanelUpdate(panelJugador);
                panelJugador.ResumeLayout(false);
            }
        }
        

        private void ActualizarCampoOponente()
        {
            panelOponente.SuspendLayout();
            BeginPanelUpdate(panelOponente);
            try
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
                int yMonstruos = 135;

                for (int i = 0; i < 3; i++)
                {
                    var monstruo = campo.ZonaMonstruos[i];

                    if (monstruo != null)
                    {
                        var controlCarta = CrearControlCarta(monstruo, false);
                        controlCarta.Location = new Point(xMonstruos, yMonstruos);
                        int posicion = i; // Capturar la posición en una variable local
                        controlCarta.Click += (s, e) => MonstruoOponenteClickeado(posicion);
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

                // ZONA DE MÁGICAS/TRAMPAS (fila superior)
                int xMagicas = 250;
                int yMagicas = 25; // Arriba

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
            finally
            {
                EndPanelUpdate(panelOponente);
                panelOponente.ResumeLayout(false);
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

            bool imagenCargada = false;

            // Intentar cargar imagen desde la propiedad RutaImagen usando caché
            if (!string.IsNullOrEmpty(carta.RutaImagen))
            {
                try
                {
                    var imagen = ObtenerImagenEnCache(carta.RutaImagen);
                    if (imagen != null)
                    {
                        // Usar la imagen como fondo del panel en lugar de agregar un PictureBox
                        panel.BackgroundImage = imagen;
                        panel.BackgroundImageLayout = ImageLayout.Stretch;
                        imagenCargada = true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error cargando imagen {carta.RutaImagen}: {ex.Message}");
                }
            }

            // Si no se cargó imagen, usar diseño de texto
            if (!imagenCargada)
            {
                // Nombre de la carta
                var lblNombre = new Label
                {
                    Text = carta.Nombre.Length > 10 ? carta.Nombre.Substring(0, 10) + "..." : carta.Nombre,
                    Font = new Font("Arial", 7, FontStyle.Bold),
                    ForeColor = Color.White,
                    Location = new Point(2, 2),
                    Size = new Size(76, 30),
                    TextAlign = ContentAlignment.TopCenter,
                    BackColor = Color.Transparent
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
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = Color.Transparent
                    };
                    panel.Controls.Add(lblStats);

                    var lblNivel = new Label
                    {
                        Text = $"Nivel: {monstruo.Nivel}",
                        Font = new Font("Arial", 6),
                        ForeColor = Color.White,
                        Location = new Point(2, 35),
                        Size = new Size(76, 12),
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = Color.Transparent
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
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = Color.Transparent
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
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = Color.Transparent
                    };
                    panel.Controls.Add(lblBocaAbajo);
                }
                else
                {
                    panel.Controls.Add(lblNombre);
                }
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
                Text = $"FASE PRINCIPAL - {juego.JugadorActual.Nombre}",
                Size = new Size(600, 550),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.DarkSlateGray,
                ForeColor = Color.White
            };

            // Panel de encabezado
            var panelEncabezado = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(570, 60),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(40, 40, 50)
            };

            var lblTitulo = new Label
            {
                Text = $"FASE PRINCIPAL",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.Gold,
                Location = new Point(15, 10),
                AutoSize = true
            };
            panelEncabezado.Controls.Add(lblTitulo);

            var lblJugador = new Label
            {
                Text = $"Turno de: {juego.JugadorActual.Nombre}",
                Font = new Font("Arial", 11),
                ForeColor = Color.LightCyan,
                Location = new Point(15, 35),
                AutoSize = true
            };
            panelEncabezado.Controls.Add(lblJugador);

            dialog.Controls.Add(panelEncabezado);

            // Botón Invocar Monstruo
            var btnInvocarMonstruo = new Button
            {
                Text = "⚔️ INVOCAR MONSTRUO",
                Location = new Point(20, 85),
                Size = new Size(540, 50),
                BackColor = Color.DarkGreen,
                ForeColor = Color.White,
                Font = new Font("Arial", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnInvocarMonstruo.Click += (s, e) => { MostrarDialogoInvocarMonstruo(); dialog.Close(); };
            dialog.Controls.Add(btnInvocarMonstruo);

            // Botón Colocar Mágica/Trampa
            var btnColocarMagicaTrampa = new Button
            {
                Text = "📜 COLOCAR MÁGICA / TRAMPA",
                Location = new Point(20, 145),
                Size = new Size(540, 50),
                BackColor = Color.DarkBlue,
                ForeColor = Color.White,
                Font = new Font("Arial", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnColocarMagicaTrampa.Click += (s, e) => { MostrarDialogoColocarMagicaTrampa(); dialog.Close(); };
            dialog.Controls.Add(btnColocarMagicaTrampa);

            // Botón Ver Mi Mano
            var btnVerMano = new Button
            {
                Text = "🖐️ VER MI MANO",
                Location = new Point(20, 205),
                Size = new Size(540, 50),
                BackColor = Color.DarkOrange,
                ForeColor = Color.White,
                Font = new Font("Arial", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnVerMano.Click += (s, e) => { MostrarManoCompleta(); };
            dialog.Controls.Add(btnVerMano);

            // Botón Ver Mi Campo
            var btnVerCampo = new Button
            {
                Text = "🏛️ VER MI CAMPO",
                Location = new Point(20, 265),
                Size = new Size(540, 50),
                BackColor = Color.DarkViolet,
                ForeColor = Color.White,
                Font = new Font("Arial", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnVerCampo.Click += (s, e) => { MostrarMiCampo(); };
            dialog.Controls.Add(btnVerCampo);

            // Botón Ver Campo Oponente
            var btnVerCampoOponente = new Button
            {
                Text = "👁️ VER CAMPO OPONENTE",
                Location = new Point(20, 325),
                Size = new Size(540, 50),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                Font = new Font("Arial", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnVerCampoOponente.Click += (s, e) => { MostrarCampoOponente(); };
            dialog.Controls.Add(btnVerCampoOponente);

            // Separador visual
            var panelSeparador = new Panel
            {
                Location = new Point(10, 385),
                Size = new Size(570, 2),
                BackColor = Color.Gold
            };
            dialog.Controls.Add(panelSeparador);

            // Botón Terminar Fase
            var btnTerminarFase = new Button
            {
                Text = "✓ TERMINAR FASE PRINCIPAL",
                Location = new Point(20, 400),
                Size = new Size(540, 50),
                BackColor = Color.DarkCyan,
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnTerminarFase.Click += (s, e) => dialog.Close();
            dialog.Controls.Add(btnTerminarFase);

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
            // Buscar mágicas y trampas en la mano
            var cartasMT = juego.JugadorActual.Mano
                .Where(c => c.Tipo == TipoCarta.Magica || c.Tipo == TipoCarta.Trampa)
                .ToList();

            if (cartasMT.Count == 0)
            {
                MessageBox.Show("No tienes mágicas ni trampas en tu mano.", "Colocar Mágica/Trampa");
                return;
            }

            // Mostrar diálogo para seleccionar la carta
            var dialog = new Form
            {
                Text = "Selecciona una Mágica o Trampa",
                Size = new Size(900, 300),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.DarkSlateGray,
                ForeColor = Color.White
            };

            // Panel de encabezado
            var panelEncabezado = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(870, 50),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(40, 40, 50)
            };

            var lblTitulo = new Label
            {
                Text = $"Selecciona una Mágica o Trampa ({cartasMT.Count} disponibles)",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.LightCyan,
                Location = new Point(15, 15),
                AutoSize = true
            };
            panelEncabezado.Controls.Add(lblTitulo);

            dialog.Controls.Add(panelEncabezado);

            // Panel con scroll para las cartas
            var panelCartas = new Panel
            {
                Location = new Point(10, 70),
                Size = new Size(870, 150),
                AutoScroll = true,
                BackColor = Color.FromArgb(30, 30, 40),
                BorderStyle = BorderStyle.FixedSingle
            };

            int xPos = 10;
            const int anchoTarjeta = 120;
            const int altoTarjeta = 140;

            foreach (var carta in cartasMT)
            {
                var panelCarta = new Panel
                {
                    Location = new Point(xPos, 10),
                    Size = new Size(anchoTarjeta, altoTarjeta),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = ObtenerColorCarta(carta),
                    Cursor = Cursors.Hand
                };

                // Cargar imagen
                bool imagenCargada = false;
                if (!string.IsNullOrEmpty(carta.RutaImagen))
                {
                    try
                    {
                        var imagen = ObtenerImagenEnCache(carta.RutaImagen);
                        if (imagen != null)
                        {
                            panelCarta.BackgroundImage = imagen;
                            panelCarta.BackgroundImageLayout = ImageLayout.Stretch;
                            imagenCargada = true;
                        }
                    }
                    catch { }
                }

                if (!imagenCargada)
                {
                    var lblNombre = new Label
                    {
                        Text = carta.Nombre.Length > 12 ? carta.Nombre.Substring(0, 12) + "..." : carta.Nombre,
                        Font = new Font("Arial", 7, FontStyle.Bold),
                        ForeColor = Color.White,
                        Location = new Point(2, 2),
                        Size = new Size(anchoTarjeta - 4, altoTarjeta - 4),
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = Color.Transparent,
                        AutoSize = false
                    };
                    panelCarta.Controls.Add(lblNombre);
                }

                panelCarta.Click += (s, e) =>
                {
                    dialog.Close();
                    MostrarDialogoColocarMagicaTrampaEnPosicion(carta);
                };

                panelCarta.MouseEnter += (s, e) => panelCarta.BorderStyle = BorderStyle.Fixed3D;
                panelCarta.MouseLeave += (s, e) => panelCarta.BorderStyle = BorderStyle.FixedSingle;

                panelCartas.Controls.Add(panelCarta);
                xPos += anchoTarjeta + 10;
            }

            dialog.Controls.Add(panelCartas);

            var btnCancelar = new Button
            {
                Text = "✕ Cancelar",
                Location = new Point(10, 230),
                Size = new Size(870, 40),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                Font = new Font("Arial", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancelar.Click += (s, e) => dialog.Close();

            dialog.Controls.Add(btnCancelar);

            dialog.ShowDialog();
        }

        private void MostrarDialogoColocarMagicaTrampaEnPosicion(Carta carta)
        {
            // Mostrar diálogo mejorado para elegir posición en el campo
            var dialogPosicion = new Form
            {
                Text = $"Colocar: {carta.Nombre}",
                Size = new Size(600, 500),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.DarkSlateGray,
                ForeColor = Color.White
            };

            // Panel superior con info de la carta
            var panelInfo = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(570, 100),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(40, 40, 50)
            };

            var lblNombreCarta = new Label
            {
                Text = carta.Nombre,
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.LightCyan,
                Location = new Point(10, 10),
                AutoSize = true
            };
            panelInfo.Controls.Add(lblNombreCarta);

            var lblTipo = new Label
            {
                Text = $"Tipo: {carta.Tipo}",
                Font = new Font("Arial", 11),
                ForeColor = Color.Yellow,
                Location = new Point(10, 35),
                AutoSize = true
            };
            panelInfo.Controls.Add(lblTipo);

            var lblDescripcion = new Label
            {
                Text = carta.Descripcion,
                Font = new Font("Arial", 9),
                ForeColor = Color.LightGray,
                Location = new Point(10, 58),
                Size = new Size(550, 35),
                AutoSize = false
            };
            panelInfo.Controls.Add(lblDescripcion);

            dialogPosicion.Controls.Add(panelInfo);

            // Panel para seleccionar posición
            var lblSelectPosicion = new Label
            {
                Text = "Selecciona una posición en tu zona de mágicas/trampas:",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 120),
                AutoSize = true
            };
            dialogPosicion.Controls.Add(lblSelectPosicion);

            int posicionSeleccionada = 0;
            var btnPosiciones = new Button[3];

            for (int i = 0; i < 3; i++)
            {
                int posicion = i;
                var estado = juego.JugadorActual.MiCampo.ZonaMagiaTrampa[i] == null ? "VACÍO" : "OCUPADO";
                var icono = juego.JugadorActual.MiCampo.ZonaMagiaTrampa[i] == null ? "📍" : "❌";
                var btnPosicion = new Button
                {
                    Text = $"{icono}\nPosición {i}\n{estado}",
                    Location = new Point(15 + (i * 185), 155),
                    Size = new Size(170, 90),
                    BackColor = juego.JugadorActual.MiCampo.ZonaMagiaTrampa[i] == null ? Color.DarkBlue : Color.DarkRed,
                    ForeColor = Color.White,
                    Font = new Font("Arial", 11, FontStyle.Bold),
                    Enabled = juego.JugadorActual.MiCampo.ZonaMagiaTrampa[i] == null,
                    Cursor = Cursors.Hand,
                    TextAlign = ContentAlignment.MiddleCenter
                };

                btnPosicion.Click += (s, e) =>
                {
                    // Deseleccionar anterior
                    foreach (var btn in btnPosiciones)
                        btn.BackColor = btn.Enabled ? Color.DarkBlue : Color.DarkRed;

                    // Seleccionar actual
                    posicionSeleccionada = posicion;
                    btnPosicion.BackColor = Color.Gold;
                };

                btnPosiciones[i] = btnPosicion;
                dialogPosicion.Controls.Add(btnPosicion);
            }

            // Panel separador
            var panelSeparador = new Panel
            {
                Location = new Point(10, 255),
                Size = new Size(570, 2),
                BackColor = Color.Gold
            };
            dialogPosicion.Controls.Add(panelSeparador);

            // Botones de acción
            var btnAceptar = new Button
            {
                Text = "✓ COLOCAR CARTA",
                Location = new Point(30, 270),
                Size = new Size(540, 50),
                BackColor = Color.DarkBlue,
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            var btnCancelar = new Button
            {
                Text = "✕ CANCELAR",
                Location = new Point(30, 330),
                Size = new Size(540, 50),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnAceptar.Click += (s, e) =>
            {
                // Colocar la carta mágica/trampa
                juego.JugadorActual.JugarMagiaTrampa(carta, posicionSeleccionada);

                AgregarLog($"✓ {carta.Nombre} colocada en posición {posicionSeleccionada}");
                ActualizarInterfaz();
                dialogPosicion.Close();
            };

            btnCancelar.Click += (s, e) => dialogPosicion.Close();

            dialogPosicion.Controls.Add(btnAceptar);
            dialogPosicion.Controls.Add(btnCancelar);

            dialogPosicion.ShowDialog();
        }

        private void MostrarManoCompleta()
        {
            var mano = juego.JugadorActual.Mano;
            
            if (mano.Count == 0)
            {
                MessageBox.Show("No tienes cartas en tu mano.", "Tu Mano");
                return;
            }

            var dialog = new Form
            {
                Text = $"Tu Mano - {mano.Count} cartas",
                Size = new Size(900, 600),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.DarkSlateGray,
                ForeColor = Color.White
            };

            // Panel de encabezado
            var panelEncabezado = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(870, 50),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(40, 40, 50)
            };

            var lblTitulo = new Label
            {
                Text = $"TU MANO - {mano.Count} CARTAS",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.LightCyan,
                Location = new Point(15, 12),
                AutoSize = true
            };
            panelEncabezado.Controls.Add(lblTitulo);

            dialog.Controls.Add(panelEncabezado);

            // Panel con scroll para mostrar las cartas
            var panelCartas = new Panel
            {
                Location = new Point(10, 70),
                Size = new Size(870, 450),
                AutoScroll = true,
                BackColor = Color.FromArgb(30, 30, 40),
                BorderStyle = BorderStyle.FixedSingle
            };

            int xPos = 10;
            const int anchoTarjeta = 140;
            const int altoTarjeta = 200;

            foreach (var carta in mano)
            {
                // Panel para cada carta
                var panelCarta = new Panel
                {
                    Location = new Point(xPos, 10),
                    Size = new Size(anchoTarjeta, altoTarjeta),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = ObtenerColorCarta(carta),
                    Cursor = Cursors.Hand
                };

                // Cargar imagen si existe
                bool imagenCargada = false;
                if (!string.IsNullOrEmpty(carta.RutaImagen))
                {
                    try
                    {
                        var imagen = ObtenerImagenEnCache(carta.RutaImagen);
                        if (imagen != null)
                        {
                            panelCarta.BackgroundImage = imagen;
                            panelCarta.BackgroundImageLayout = ImageLayout.Stretch;
                            imagenCargada = true;
                        }
                    }
                    catch { }
                }

                // Si no hay imagen, mostrar información de texto
                if (!imagenCargada)
                {
                    var lblNombre = new Label
                    {
                        Text = carta.Nombre.Length > 15 ? carta.Nombre.Substring(0, 15) + "..." : carta.Nombre,
                        Font = new Font("Arial", 7, FontStyle.Bold),
                        ForeColor = Color.White,
                        Location = new Point(2, 2),
                        Size = new Size(anchoTarjeta - 4, 40),
                        TextAlign = ContentAlignment.TopCenter,
                        BackColor = Color.Transparent,
                        AutoSize = false
                    };
                    panelCarta.Controls.Add(lblNombre);

                    if (carta is CartaMonstruo monstruo)
                    {
                        var lblStats = new Label
                        {
                            Text = $"ATK:{monstruo.Ataque}\nDEF:{monstruo.Defensa}",
                            Font = new Font("Arial", 7, FontStyle.Bold),
                            ForeColor = Color.Yellow,
                            Location = new Point(2, altoTarjeta - 30),
                            Size = new Size(anchoTarjeta - 4, 25),
                            TextAlign = ContentAlignment.MiddleCenter,
                            BackColor = Color.Transparent
                        };
                        panelCarta.Controls.Add(lblStats);
                    }
                }

                // Click para ver detalles
                panelCarta.Click += (s, e) => MostrarDetallesCarta(carta);
                panelCarta.MouseEnter += (s, e) => panelCarta.BorderStyle = BorderStyle.Fixed3D;
                panelCarta.MouseLeave += (s, e) => panelCarta.BorderStyle = BorderStyle.FixedSingle;

                panelCartas.Controls.Add(panelCarta);
                xPos += anchoTarjeta + 10;
            }

            dialog.Controls.Add(panelCartas);

            // Panel de información
            var panelInfo = new Panel
            {
                Location = new Point(10, 530),
                Size = new Size(870, 35),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(40, 40, 50)
            };

            var lblInfo = new Label
            {
                Text = "Haz clic en cualquier carta para ver los detalles completos",
                Font = new Font("Arial", 10),
                ForeColor = Color.LightGray,
                Location = new Point(15, 8),
                AutoSize = true
            };
            panelInfo.Controls.Add(lblInfo);

            dialog.Controls.Add(panelInfo);

            dialog.ShowDialog();
        }

        private void MostrarMiCampo()
        {
            var campo = juego.JugadorActual.MiCampo;
            var todasLasCartas = new List<(Carta carta, string zona, int posicion)>();

            // Agregar monstruos
            for (int i = 0; i < campo.ZonaMonstruos.Length; i++)
            {
                if (campo.ZonaMonstruos[i] != null)
                    todasLasCartas.Add((campo.ZonaMonstruos[i], "Monstruo", i));
            }

            // Agregar mágicas/trampas
            for (int i = 0; i < campo.ZonaMagiaTrampa.Length; i++)
            {
                if (campo.ZonaMagiaTrampa[i] != null)
                    todasLasCartas.Add((campo.ZonaMagiaTrampa[i], "Mágica/Trampa", i));
            }

            if (todasLasCartas.Count == 0)
            {
                MessageBox.Show("Tu campo está vacío.", "Mi Campo");
                return;
            }

            var dialog = new Form
            {
                Text = $"Mi Campo - {todasLasCartas.Count} cartas",
                Size = new Size(900, 600),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.DarkSlateGray,
                ForeColor = Color.White
            };

            // Panel de encabezado
            var panelEncabezado = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(870, 50),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(40, 40, 50)
            };

            var lblTitulo = new Label
            {
                Text = $"MI CAMPO - {todasLasCartas.Count} CARTAS",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.LightGreen,
                Location = new Point(15, 12),
                AutoSize = true
            };
            panelEncabezado.Controls.Add(lblTitulo);

            dialog.Controls.Add(panelEncabezado);

            // Panel con scroll para mostrar las cartas
            var panelCartas = new Panel
            {
                Location = new Point(10, 70),
                Size = new Size(870, 450),
                AutoScroll = true,
                BackColor = Color.FromArgb(30, 30, 40),
                BorderStyle = BorderStyle.FixedSingle
            };

            int xPos = 10;
            const int anchoTarjeta = 140;
            const int altoTarjeta = 200;

            foreach (var (carta, zona, posicion) in todasLasCartas)
            {
                // Panel para cada carta
                var panelCarta = new Panel
                {
                    Location = new Point(xPos, 10),
                    Size = new Size(anchoTarjeta, altoTarjeta),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = ObtenerColorCarta(carta),
                    Cursor = Cursors.Hand
                };

                // Cargar imagen si existe
                bool imagenCargada = false;
                if (!string.IsNullOrEmpty(carta.RutaImagen))
                {
                    try
                    {
                        var imagen = ObtenerImagenEnCache(carta.RutaImagen);
                        if (imagen != null)
                        {
                            panelCarta.BackgroundImage = imagen;
                            panelCarta.BackgroundImageLayout = ImageLayout.Stretch;
                            imagenCargada = true;
                        }
                    }
                    catch { }
                }

                // Si no hay imagen, mostrar información de texto
                if (!imagenCargada)
                {
                    var lblNombre = new Label
                    {
                        Text = carta.Nombre.Length > 15 ? carta.Nombre.Substring(0, 15) + "..." : carta.Nombre,
                        Font = new Font("Arial", 7, FontStyle.Bold),
                        ForeColor = Color.White,
                        Location = new Point(2, 2),
                        Size = new Size(anchoTarjeta - 4, 40),
                        TextAlign = ContentAlignment.TopCenter,
                        BackColor = Color.Transparent,
                        AutoSize = false
                    };
                    panelCarta.Controls.Add(lblNombre);

                    var lblZona = new Label
                    {
                        Text = zona,
                        Font = new Font("Arial", 6),
                        ForeColor = Color.Cyan,
                        Location = new Point(2, 45),
                        Size = new Size(anchoTarjeta - 4, 15),
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = Color.Transparent
                    };
                    panelCarta.Controls.Add(lblZona);

                    if (carta is CartaMonstruo monstruo)
                    {
                        var lblStats = new Label
                        {
                            Text = $"ATK:{monstruo.Ataque}\nDEF:{monstruo.Defensa}",
                            Font = new Font("Arial", 7, FontStyle.Bold),
                            ForeColor = Color.Yellow,
                            Location = new Point(2, altoTarjeta - 30),
                            Size = new Size(anchoTarjeta - 4, 25),
                            TextAlign = ContentAlignment.MiddleCenter,
                            BackColor = Color.Transparent
                        };
                        panelCarta.Controls.Add(lblStats);
                    }
                }

                // Click para ver detalles
                panelCarta.Click += (s, e) => MostrarDetallesCarta(carta);
                panelCarta.MouseEnter += (s, e) => panelCarta.BorderStyle = BorderStyle.Fixed3D;
                panelCarta.MouseLeave += (s, e) => panelCarta.BorderStyle = BorderStyle.FixedSingle;

                panelCartas.Controls.Add(panelCarta);
                xPos += anchoTarjeta + 10;
            }

            dialog.Controls.Add(panelCartas);

            // Panel de información
            var panelInfo = new Panel
            {
                Location = new Point(10, 530),
                Size = new Size(870, 35),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(40, 40, 50)
            };

            var lblInfo = new Label
            {
                Text = "Haz clic en cualquier carta para ver los detalles completos",
                Font = new Font("Arial", 10),
                ForeColor = Color.LightGray,
                Location = new Point(15, 8),
                AutoSize = true
            };
            panelInfo.Controls.Add(lblInfo);

            dialog.Controls.Add(panelInfo);

            dialog.ShowDialog();
        }

        private void MostrarCampoOponente()
        {
            var campo = juego.JugadorOponente.MiCampo;
            var todasLasCartas = new List<(Carta carta, string zona, int posicion)>();

            // Agregar monstruos
            for (int i = 0; i < campo.ZonaMonstruos.Length; i++)
            {
                if (campo.ZonaMonstruos[i] != null)
                    todasLasCartas.Add((campo.ZonaMonstruos[i], "Monstruo", i));
            }

            // Agregar mágicas/trampas
            for (int i = 0; i < campo.ZonaMagiaTrampa.Length; i++)
            {
                if (campo.ZonaMagiaTrampa[i] != null)
                    todasLasCartas.Add((campo.ZonaMagiaTrampa[i], "Mágica/Trampa", i));
            }

            if (todasLasCartas.Count == 0)
            {
                MessageBox.Show($"El campo de {juego.JugadorOponente.Nombre} está vacío.", "Campo Oponente");
                return;
            }

            var dialog = new Form
            {
                Text = $"Campo de {juego.JugadorOponente.Nombre} - {todasLasCartas.Count} cartas",
                Size = new Size(900, 600),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.DarkSlateGray,
                ForeColor = Color.White
            };

            // Panel de encabezado
            var panelEncabezado = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(870, 50),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(40, 40, 50)
            };

            var lblTitulo = new Label
            {
                Text = $"CAMPO DE {juego.JugadorOponente.Nombre.ToUpper()} - {todasLasCartas.Count} CARTAS",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.LightCoral,
                Location = new Point(15, 12),
                AutoSize = true
            };
            panelEncabezado.Controls.Add(lblTitulo);

            dialog.Controls.Add(panelEncabezado);

            // Panel con scroll para mostrar las cartas
            var panelCartas = new Panel
            {
                Location = new Point(10, 70),
                Size = new Size(870, 450),
                AutoScroll = true,
                BackColor = Color.FromArgb(30, 30, 40),
                BorderStyle = BorderStyle.FixedSingle
            };

            int xPos = 10;
            const int anchoTarjeta = 140;
            const int altoTarjeta = 200;

            foreach (var (carta, zona, posicion) in todasLasCartas)
            {
                // Panel para cada carta
                var panelCarta = new Panel
                {
                    Location = new Point(xPos, 10),
                    Size = new Size(anchoTarjeta, altoTarjeta),
                    BorderStyle = BorderStyle.FixedSingle,
                    BackColor = ObtenerColorCarta(carta),
                    Cursor = Cursors.Hand
                };

                // Cargar imagen si existe
                bool imagenCargada = false;
                if (!string.IsNullOrEmpty(carta.RutaImagen))
                {
                    try
                    {
                        var imagen = ObtenerImagenEnCache(carta.RutaImagen);
                        if (imagen != null)
                        {
                            panelCarta.BackgroundImage = imagen;
                            panelCarta.BackgroundImageLayout = ImageLayout.Stretch;
                            imagenCargada = true;
                        }
                    }
                    catch { }
                }

                // Si no hay imagen, mostrar información de texto
                if (!imagenCargada)
                {
                    var lblNombre = new Label
                    {
                        Text = carta.Nombre.Length > 15 ? carta.Nombre.Substring(0, 15) + "..." : carta.Nombre,
                        Font = new Font("Arial", 7, FontStyle.Bold),
                        ForeColor = Color.White,
                        Location = new Point(2, 2),
                        Size = new Size(anchoTarjeta - 4, 40),
                        TextAlign = ContentAlignment.TopCenter,
                        BackColor = Color.Transparent,
                        AutoSize = false
                    };
                    panelCarta.Controls.Add(lblNombre);

                    var lblZona = new Label
                    {
                        Text = zona,
                        Font = new Font("Arial", 6),
                        ForeColor = Color.Cyan,
                        Location = new Point(2, 45),
                        Size = new Size(anchoTarjeta - 4, 15),
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = Color.Transparent
                    };
                    panelCarta.Controls.Add(lblZona);

                    if (carta is CartaMonstruo monstruo)
                    {
                        var lblStats = new Label
                        {
                            Text = $"ATK:{monstruo.Ataque}\nDEF:{monstruo.Defensa}",
                            Font = new Font("Arial", 7, FontStyle.Bold),
                            ForeColor = Color.Yellow,
                            Location = new Point(2, altoTarjeta - 30),
                            Size = new Size(anchoTarjeta - 4, 25),
                            TextAlign = ContentAlignment.MiddleCenter,
                            BackColor = Color.Transparent
                        };
                        panelCarta.Controls.Add(lblStats);
                    }
                }

                // Click para ver detalles
                panelCarta.Click += (s, e) => MostrarDetallesCarta(carta);
                panelCarta.MouseEnter += (s, e) => panelCarta.BorderStyle = BorderStyle.Fixed3D;
                panelCarta.MouseLeave += (s, e) => panelCarta.BorderStyle = BorderStyle.FixedSingle;

                panelCartas.Controls.Add(panelCarta);
                xPos += anchoTarjeta + 10;
            }

            dialog.Controls.Add(panelCartas);

            // Panel de información
            var panelInfo = new Panel
            {
                Location = new Point(10, 530),
                Size = new Size(870, 35),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(40, 40, 50)
            };

            var lblInfo = new Label
            {
                Text = "Haz clic en cualquier carta para ver los detalles completos",
                Font = new Font("Arial", 10),
                ForeColor = Color.LightGray,
                Location = new Point(15, 8),
                AutoSize = true
            };
            panelInfo.Controls.Add(lblInfo);

            dialog.Controls.Add(panelInfo);

            dialog.ShowDialog();
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

        private void MonstruoOponenteClickeado(int posicion)
        {
            var monstruo = juego.JugadorOponente.MiCampo.ZonaMonstruos[posicion];
            
            if (monstruo == null)
            {
                AgregarLog("No hay monstruo en esa posición");
                return;
            }

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
                Text = $"Opciones - {carta.Nombre}",
                Size = new Size(550, 400),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.DarkSlateGray,
                ForeColor = Color.White
            };

            // Panel superior con información de la carta
            var panelInfo = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(520, 110),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(40, 40, 50)
            };

            var lblNombreCarta = new Label
            {
                Text = carta.Nombre,
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.LightCyan,
                Location = new Point(15, 10),
                AutoSize = true
            };
            panelInfo.Controls.Add(lblNombreCarta);

            var lblTipo = new Label
            {
                Text = $"Tipo: {carta.Tipo}",
                Font = new Font("Arial", 11),
                ForeColor = Color.Yellow,
                Location = new Point(15, 35),
                AutoSize = true
            };
            panelInfo.Controls.Add(lblTipo);

            // Info adicional según el tipo de carta
            if (carta is CartaMonstruo monstruo)
            {
                var lblMonstruoStats = new Label
                {
                    Text = $"ATK: {monstruo.Ataque} | DEF: {monstruo.Defensa} | Nivel: {monstruo.Nivel}",
                    Font = new Font("Arial", 10),
                    ForeColor = Color.LightGreen,
                    Location = new Point(15, 55),
                    AutoSize = true
                };
                panelInfo.Controls.Add(lblMonstruoStats);

                var lblDesc = new Label
                {
                    Text = monstruo.Descripcion,
                    Font = new Font("Arial", 9),
                    ForeColor = Color.LightGray,
                    Location = new Point(15, 75),
                    Size = new Size(490, 30),
                    AutoSize = false
                };
                panelInfo.Controls.Add(lblDesc);
            }
            else
            {
                var lblDesc = new Label
                {
                    Text = carta.Descripcion,
                    Font = new Font("Arial", 10),
                    ForeColor = Color.LightGray,
                    Location = new Point(15, 55),
                    Size = new Size(490, 50),
                    AutoSize = false
                };
                panelInfo.Controls.Add(lblDesc);
            }

            dialog.Controls.Add(panelInfo);

            // Etiqueta de instrucción
            var lblAccion = new Label
            {
                Text = "¿Qué deseas hacer con esta carta?",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(15, 130),
                AutoSize = true
            };
            dialog.Controls.Add(lblAccion);

            // Botón Invocar/Colocar
            var textoBoton = carta.Tipo == TipoCarta.Monstruo ? "⚔️ Invocar Monstruo" : "📜 Colocar Carta";
            var colorBoton = carta.Tipo == TipoCarta.Monstruo ? Color.DarkGreen : Color.DarkBlue;
            
            var btnColocar = new Button
            {
                Text = textoBoton,
                Location = new Point(30, 170),
                Size = new Size(480, 50),
                BackColor = colorBoton,
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Enabled = true
            };

            btnColocar.Click += (s, e) =>
            {
                dialog.Close();
                ColocarCartaEnCampo(carta);
            };

            dialog.Controls.Add(btnColocar);

            // Botón Ver Detalles
            var btnVerCarta = new Button
            {
                Text = "👁️ Ver Detalles Completos",
                Location = new Point(30, 230),
                Size = new Size(480, 50),
                BackColor = Color.DarkOrange,
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnVerCarta.Click += (s, e) =>
            {
                dialog.Close();
                MostrarDetallesCarta(carta);
            };

            dialog.Controls.Add(btnVerCarta);

            // Botón Cancelar
            var btnCancelar = new Button
            {
                Text = "✕ Cancelar",
                Location = new Point(30, 290),
                Size = new Size(480, 50),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnCancelar.Click += (s, e) => dialog.Close();

            dialog.Controls.Add(btnCancelar);

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

            // Mostrar diálogo mejorado para elegir posición en el campo
            var dialogPosicion = new Form
            {
                Text = $"Invocar: {monstruo.Nombre}",
                Size = new Size(600, 450),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.DarkSlateGray,
                ForeColor = Color.White
            };

            // Panel superior con info de la carta
            var panelInfo = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(570, 90),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(40, 40, 50)
            };

            var lblNombreCarta = new Label
            {
                Text = monstruo.Nombre,
                Font = new Font("Arial", 13, FontStyle.Bold),
                ForeColor = Color.LightCyan,
                Location = new Point(10, 10),
                AutoSize = true
            };
            panelInfo.Controls.Add(lblNombreCarta);

            var lblStats = new Label
            {
                Text = $"ATK: {monstruo.Ataque} | DEF: {monstruo.Defensa} | Nivel: {monstruo.Nivel}",
                Font = new Font("Arial", 11),
                ForeColor = Color.Yellow,
                Location = new Point(10, 35),
                AutoSize = true
            };
            panelInfo.Controls.Add(lblStats);

            var lblDescripcion = new Label
            {
                Text = monstruo.Descripcion,
                Font = new Font("Arial", 9),
                ForeColor = Color.LightGray,
                Location = new Point(10, 58),
                Size = new Size(550, 25),
                AutoSize = false
            };
            panelInfo.Controls.Add(lblDescripcion);

            dialogPosicion.Controls.Add(panelInfo);

            // Panel para seleccionar posición (3 botones visuales)
            var lblSelectPosicion = new Label
            {
                Text = "Selecciona una posición en tu zona de monstruos:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 110),
                AutoSize = true
            };
            dialogPosicion.Controls.Add(lblSelectPosicion);

            int posicionSeleccionada = 0;
            var btnPosiciones = new Button[3];

            for (int i = 0; i < 3; i++)
            {
                int posicion = i;
                var estado = juego.JugadorActual.MiCampo.ZonaMonstruos[i] == null ? "VACÍO" : "OCUPADO";
                var btnPosicion = new Button
                {
                    Text = $"Posición {i}\n({estado})",
                    Location = new Point(20 + (i * 180), 140),
                    Size = new Size(160, 80),
                    BackColor = juego.JugadorActual.MiCampo.ZonaMonstruos[i] == null ? Color.DarkGreen : Color.DarkRed,
                    ForeColor = Color.White,
                    Font = new Font("Arial", 11, FontStyle.Bold),
                    Enabled = juego.JugadorActual.MiCampo.ZonaMonstruos[i] == null,
                    Cursor = Cursors.Hand
                };

                btnPosicion.Click += (s, e) =>
                {
                    // Deseleccionar anterior
                    foreach (var btn in btnPosiciones)
                        btn.BackColor = btn.Enabled ? Color.DarkGreen : Color.DarkRed;

                    // Seleccionar actual
                    posicionSeleccionada = posicion;
                    btnPosicion.BackColor = Color.Gold;
                };

                btnPosiciones[i] = btnPosicion;
                dialogPosicion.Controls.Add(btnPosicion);
            }

            // Selector de modo
            var lblModo = new Label
            {
                Text = "Modo de invocación:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 230),
                AutoSize = true
            };
            dialogPosicion.Controls.Add(lblModo);

            var panelModos = new Panel
            {
                Location = new Point(30, 255),
                Size = new Size(540, 60),
                BackColor = Color.Transparent
            };

            var rbAtaque = new RadioButton
            {
                Text = "⚔️ Ataque",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.LightCoral,
                Location = new Point(0, 10),
                Size = new Size(250, 40),
                Checked = true
            };

            var rbDefensa = new RadioButton
            {
                Text = "🛡️ Defensa",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.LightBlue,
                Location = new Point(280, 10),
                Size = new Size(250, 40)
            };

            panelModos.Controls.Add(rbAtaque);
            panelModos.Controls.Add(rbDefensa);
            dialogPosicion.Controls.Add(panelModos);

            // Botones de acción
            var btnAceptar = new Button
            {
                Text = "✓ Invocar",
                Location = new Point(130, 330),
                Size = new Size(150, 50),
                BackColor = Color.DarkGreen,
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            var btnCancelar = new Button
            {
                Text = "✕ Cancelar",
                Location = new Point(320, 330),
                Size = new Size(150, 50),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnAceptar.Click += (s, e) =>
            {
                var modo = rbAtaque.Checked ? PosicionMonstruo.Ataque : PosicionMonstruo.Defensa;

                // Configurar el monstruo
                monstruo.Posicion = modo;
                monstruo.EstaBocaAbajo = false;

                // Invocar el monstruo
                juego.JugadorActual.JugarMonstruo(monstruo, posicionSeleccionada);

                AgregarLog($"✓ {monstruo.Nombre} invocado en posición {posicionSeleccionada} (modo {modo})");
                ActualizarInterfaz();
                dialogPosicion.Close();
            };

            btnCancelar.Click += (s, e) => dialogPosicion.Close();

            dialogPosicion.Controls.Add(btnAceptar);
            dialogPosicion.Controls.Add(btnCancelar);

            dialogPosicion.ShowDialog();
        }
        private void MostrarDetallesCarta(Carta carta)
        {
            var dialog = new Form
            {
                Text = $"Carta Ampliada - {carta.Nombre}",
                Size = new Size(750, 600),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.DarkSlateGray,
                ForeColor = Color.White
            };

            // Panel izquierdo para la imagen
            var panelImagen = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(300, 400),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(30, 30, 40)
            };

            // Intentar cargar la imagen
            if (!string.IsNullOrEmpty(carta.RutaImagen))
            {
                try
                {
                    var imagen = ObtenerImagenEnCache(carta.RutaImagen);
                    if (imagen != null)
                    {
                        var pictureCarta = new PictureBox
                        {
                            Image = imagen,
                            SizeMode = PictureBoxSizeMode.StretchImage,
                            Location = new Point(5, 5),
                            Size = new Size(290, 390)
                        };
                        panelImagen.Controls.Add(pictureCarta);
                    }
                    else
                    {
                        var lblNoImagen = new Label
                        {
                            Text = "Sin Imagen",
                            Font = new Font("Arial", 12, FontStyle.Bold),
                            ForeColor = Color.Gray,
                            Location = new Point(80, 180),
                            AutoSize = true
                        };
                        panelImagen.Controls.Add(lblNoImagen);
                    }
                }
                catch
                {
                    var lblError = new Label
                    {
                        Text = "Error al cargar imagen",
                        Font = new Font("Arial", 10),
                        ForeColor = Color.LightCoral,
                        Location = new Point(50, 180),
                        AutoSize = true
                    };
                    panelImagen.Controls.Add(lblError);
                }
            }
            else
            {
                var lblSinImagen = new Label
                {
                    Text = "Sin Imagen",
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    ForeColor = Color.Gray,
                    Location = new Point(100, 180),
                    AutoSize = true
                };
                panelImagen.Controls.Add(lblSinImagen);
            }

            dialog.Controls.Add(panelImagen);

            // Panel derecho para los detalles
            var panelDetalles = new Panel
            {
                Location = new Point(320, 10),
                Size = new Size(410, 540),
                BackColor = Color.Transparent
            };

            // Nombre de la carta
            var lblNombreCarta = new Label
            {
                Text = carta.Nombre,
                Font = new Font("Arial", 16, FontStyle.Bold),
                ForeColor = Color.LightCyan,
                Location = new Point(0, 0),
                AutoSize = true
            };
            panelDetalles.Controls.Add(lblNombreCarta);

            // Tipo de carta
            var lblTipo = new Label
            {
                Text = $"Tipo: {carta.Tipo}",
                Font = new Font("Arial", 11),
                ForeColor = Color.Yellow,
                Location = new Point(0, 35),
                AutoSize = true
            };
            panelDetalles.Controls.Add(lblTipo);

            int yPos = 65;

            // Información específica según el tipo
            if (carta is CartaMonstruo monstruo)
            {
                // Stats del monstruo
                var lblStats = new Label
                {
                    Text = $"ATK: {monstruo.Ataque}  |  DEF: {monstruo.Defensa}",
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    ForeColor = Color.LightGreen,
                    Location = new Point(0, yPos),
                    AutoSize = true
                };
                panelDetalles.Controls.Add(lblStats);
                yPos += 30;

                // Información adicional
                var lblNivel = new Label
                {
                    Text = $"Nivel: {monstruo.Nivel}",
                    Font = new Font("Arial", 10),
                    ForeColor = Color.LightBlue,
                    Location = new Point(0, yPos),
                    AutoSize = true
                };
                panelDetalles.Controls.Add(lblNivel);
                yPos += 25;

                var lblAtributo = new Label
                {
                    Text = $"Atributo: {monstruo.Atributo}",
                    Font = new Font("Arial", 10),
                    ForeColor = Color.Plum,
                    Location = new Point(0, yPos),
                    AutoSize = true
                };
                panelDetalles.Controls.Add(lblAtributo);
                yPos += 25;

                var lblTipoMonstruo = new Label
                {
                    Text = $"Tipo Monstruo: {monstruo.TipoMonstruo}",
                    Font = new Font("Arial", 10),
                    ForeColor = Color.Khaki,
                    Location = new Point(0, yPos),
                    AutoSize = true
                };
                panelDetalles.Controls.Add(lblTipoMonstruo);
                yPos += 25;

                var lblPosicion = new Label
                {
                    Text = $"Posición: {monstruo.Posicion}",
                    Font = new Font("Arial", 10),
                    ForeColor = Color.LightSalmon,
                    Location = new Point(0, yPos),
                    AutoSize = true
                };
                panelDetalles.Controls.Add(lblPosicion);
                yPos += 30;
            }
            else if (carta is CartaMagica magica)
            {
                var lblTipoMagica = new Label
                {
                    Text = $"Tipo Mágica: {magica.TipoMagica}",
                    Font = new Font("Arial", 10),
                    ForeColor = Color.Plum,
                    Location = new Point(0, yPos),
                    AutoSize = true
                };
                panelDetalles.Controls.Add(lblTipoMagica);
                yPos += 30;
            }
            else if (carta is CartaTrampa trampa)
            {
                var lblTipoTrampa = new Label
                {
                    Text = $"Tipo Trampa: {trampa.TipoTrampa}",
                    Font = new Font("Arial", 10),
                    ForeColor = Color.Khaki,
                    Location = new Point(0, yPos),
                    AutoSize = true
                };
                panelDetalles.Controls.Add(lblTipoTrampa);
                yPos += 30;
            }

            // Descripción
            var lblDescripcion = new Label
            {
                Text = "Descripción:",
                Font = new Font("Arial", 11, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(0, yPos),
                AutoSize = true
            };
            panelDetalles.Controls.Add(lblDescripcion);
            yPos += 25;

            var txtDescripcion = new TextBox
            {
                Location = new Point(0, yPos),
                Size = new Size(400, 120),
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                ReadOnly = true,
                BackColor = Color.FromArgb(40, 40, 50),
                ForeColor = Color.LightGray,
                Font = new Font("Arial", 10),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtDescripcion.Text = carta.Descripcion;
            panelDetalles.Controls.Add(txtDescripcion);

            dialog.Controls.Add(panelDetalles);

            // Botón Cerrar
            var btnCerrar = new Button
            {
                Text = "✕ Cerrar",
                Location = new Point(320, 555),
                Size = new Size(410, 40),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                Font = new Font("Arial", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCerrar.Click += (s, e) => dialog.Close();

            dialog.Controls.Add(btnCerrar);

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
            // Usar el nuevo diálogo mejorado
            MostrarDialogoColocarMagicaTrampaEnPosicion(carta);
        }
        private void RealizarAtaque(CartaMonstruo atacante, CartaMonstruo defensor, int posicionDefensor)
        {
            // Validar que la posición sea válida
            if (posicionDefensor < 0 || posicionDefensor >= 3)
            {
                MessageBox.Show("Posición de defensor inválida", "Error");
                return;
            }

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

            // Verificar si alguien llegó a 0 puntos de vida
            if (juego.JugadorActual.PuntosVida <= 0)
            {
                MessageBox.Show($"¡{juego.JugadorOponente.Nombre} GANA EL DUELO!", "FIN DEL DUELO", 
                              MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Close();
                return;
            }
            
            if (juego.JugadorOponente.PuntosVida <= 0)
            {
                MessageBox.Show($"¡{juego.JugadorActual.Nombre} GANA EL DUELO!", "FIN DEL DUELO", 
                              MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Close();
                return;
            }

            ActualizarCamposRapido();
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
                MessageBox.Show($"¡{juego.JugadorActual.Nombre} GANA EL DUELO!", "FIN DEL DUELO", 
                              MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Close();
                return;
            }

            ActualizarCamposRapido();
        }
        private void CampoOponenteClickeado()
        {
            if (!modoAtaque)
            {
                return; // No estamos en modo ataque
            }

            if (monstruoAtacante == null)
            {
                MessageBox.Show("Primero selecciona un monstruo para atacar.", "Ataque");
                return;
            }

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
        private void EstablecerFondo()
        {
            try
            {
                string directorioEjecucion = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string rutaImagen = System.IO.Path.Combine(directorioEjecucion, "Resources", "Tablero.jpg");

                if (System.IO.File.Exists(rutaImagen))
                {
                    if (!cacheImagenes.ContainsKey(rutaImagen))
                    {
                        byte[] imageData = System.IO.File.ReadAllBytes(rutaImagen);
                        using (var ms = new System.IO.MemoryStream(imageData))
                        {
                            cacheImagenes[rutaImagen] = new Bitmap(ms);
                        }
                    }
                    
                    if (cacheImagenes.ContainsKey(rutaImagen))
                    {
                        this.BackgroundImage = cacheImagenes[rutaImagen];
                        this.BackgroundImageLayout = ImageLayout.Stretch;
                        return;
                    }
                }
                
                this.BackColor = Color.DarkGreen;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cargando imagen de fondo:\n{ex.Message}", "Error");
                this.BackColor = Color.DarkGreen;
            }
        }
        private Color ObtenerColorCarta(Carta carta)
        {
            if (carta.EstaBocaAbajo)
                return Color.DarkRed;

            return carta.Tipo switch
            {
                TipoCarta.Monstruo => Color.SaddleBrown,
                TipoCarta.Magica => Color.DarkBlue,
                TipoCarta.Trampa => Color.DarkMagenta,
                _ => Color.Gray
            };
        }

        // Override para optimizar el renderizado del fondo
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.BackgroundImage != null)
            {
                e.Graphics.DrawImage(this.BackgroundImage, 0, 0, this.Width, this.Height);
            }
            else
            {
                e.Graphics.Clear(this.BackColor);
            }
        }

        // Optimizar el comportamiento de resize
        protected override void OnSizeChanged(EventArgs e)
        {
            BeginUpdate();
            try
            {
                base.OnSizeChanged(e);
            }
            finally
            {
                EndUpdate();
            }
        }

        private void MostrarAutores()
        {
            Form autoresForm = new Form
            {
                Text = "Autores",
                Width = 400,
                Height = 300,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.White,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false
            };

            Label lblContenido = new Label
            {
                Text = "Desarrollado por:",
                ForeColor = Color.Black,
                Font = new Font("Arial", 11, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            LinkLabel linkNicolas = new LinkLabel
            {
                Text = "Nicolas Camacho",
                ForeColor = Color.Blue,
                Font = new Font("Arial", 11, FontStyle.Bold | FontStyle.Underline),
                Location = new Point(20, 60),
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            linkNicolas.LinkClicked += (s, e) => 
            {
                try
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "https://github.com/NicolasSs1203",
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show("No se pudo abrir el navegador: " + ex.Message);
                }
            };

            Label lblOtros = new Label
            {
                Text = "Juan Carvajal\n" +
                       "Kevin Pardo\n" +
                       "Santiago Cuervo",
                ForeColor = Color.Black,
                Font = new Font("Arial", 11, FontStyle.Bold),
                Location = new Point(20, 90),
                AutoSize = true
            };

            Label lblApp = new Label
            {
                Text = "Yu-Gi-Oh! Card Game GUI\nVersión 1.0",
                ForeColor = Color.Black,
                Font = new Font("Arial", 10, FontStyle.Bold),
                Location = new Point(20, 180),
                AutoSize = true
            };

            Button btnCerrar = new Button
            {
                Text = "Cerrar",
                DialogResult = DialogResult.OK,
                Size = new Size(100, 35),
                Location = new Point(150, 230),
                BackColor = Color.DarkSlateGray,
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };

            autoresForm.Controls.Add(lblContenido);
            autoresForm.Controls.Add(linkNicolas);
            autoresForm.Controls.Add(lblOtros);
            autoresForm.Controls.Add(lblApp);
            autoresForm.Controls.Add(btnCerrar);
            autoresForm.ShowDialog(this);
        }








        
        
    }
}