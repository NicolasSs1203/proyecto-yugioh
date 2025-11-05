using YuGiOh.Models;
using YuGiOh.Models.Cartas;
using YuGiOh.Enums;
using System;
using System.Windows.Forms;

namespace YuGiOh
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
           
            var yugi = new Jugador { Nombre = "Yugi" };
            var kaiba = new Jugador { Nombre = "Kaiba" };

            CrearDeck(yugi);
            CrearDeck(kaiba);

            var juego = new Juego(yugi, kaiba);
            juego.IniciarJuego();

            // Iniciar la interfaz gráfica en lugar de la consola
            Application.Run(new MainGameForm(juego));
        }

        static void CrearDeck(Jugador jugador)
        {
            // Tu código existente para crear el deck...
            var monstruos = new[]
            {
                new CartaMonstruo
                {
                    Nombre = "Dragón Blanco de Ojos Azules",
                    Descripcion = "Dragón legendario",
                    Tipo = TipoCarta.Monstruo,
                    Ataque = 3000,
                    Defensa = 2500,
                    Nivel = 8,
                    Atributo = Atributo.Luz,
                    TipoMonstruo = "Dragón",
                    Posicion = PosicionMonstruo.Ataque,
                    EstaBocaAbajo = false
                },
                new CartaMonstruo
                {
                    Nombre = "Mago Oscuro",
                    Descripcion = "El mago definitivo",
                    Tipo = TipoCarta.Monstruo,
                    Ataque = 2500,
                    Defensa = 2100,
                    Nivel = 7,
                    Atributo = Atributo.Oscuridad,
                    TipoMonstruo = "Lanzador de Conjuros",
                    Posicion = PosicionMonstruo.Ataque,
                    EstaBocaAbajo = false
                },
                new CartaMonstruo
                {
                    Nombre = "Guerrero Celta",
                    Descripcion = "Guerrero de la tierra",
                    Tipo = TipoCarta.Monstruo,
                    Ataque = 1400,
                    Defensa = 1200,
                    Nivel = 4,
                    Atributo = Atributo.Tierra,
                    TipoMonstruo = "Guerrero",
                    Posicion = PosicionMonstruo.Ataque,
                    EstaBocaAbajo = false
                }
            };

            foreach (var monstruo in monstruos)
            {
                jugador.Baraja.Add(monstruo);
            }

            for (int i = 0; i < 3; i++)
            {
                var magica = new CartaMagica
                {
                    Nombre = "Olla de la Codicia",
                    Descripcion = "Roba 2 cartas.",
                    Tipo = TipoCarta.Magica,
                    TipoMagica = TipoMagica.Normal,
                    EstaBocaAbajo = false
                };
                jugador.Baraja.Add(magica);
            }

            for (int i = 0; i < 2; i++)
            {
                var trampa = new CartaTrampa
                {
                    Nombre = "Cilindro Mágico",
                    Descripcion = "Niega un ataque.",
                    Tipo = TipoCarta.Trampa,
                    TipoTrampa = TipoTrampa.Normal,
                    EstaBocaAbajo = true
                };
                jugador.Baraja.Add(trampa);
            }
        }
    }
}