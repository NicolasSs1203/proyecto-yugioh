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
            // Deshabilitar estilos visuales para renderizado más simple y rápido
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Usar double buffering a nivel de aplicación
            if (!SystemInformation.TerminalServerSession)
            {
                Application.EnableVisualStyles();
            }
           
            var yugi = new Jugador { Nombre = "Yugi" };
            var kaiba = new Jugador { Nombre = "Kaiba" };

            CrearMazoYugi(yugi);
            CrearMazoKaiba(kaiba);

            var juego = new Juego(yugi, kaiba);
            juego.IniciarJuego();

            // Iniciar la interfaz gráfica en lugar de la consola
            Application.Run(new MainGameForm(juego));
        }

        static void CrearMazoYugi(Jugador jugador)
        {
            // Mazo de Yugi - Agrega las cartas aquí

            // Mago Oscuro
            jugador.Baraja.Add(new CartaMonstruo
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
                EstaBocaAbajo = false,
                RutaImagen = "Resources/cartas/Mago oscuro.jpg"
            });

            // Chica maga oscura
            jugador.Baraja.Add(new CartaMonstruo
            {
                Nombre = "Chica maga oscura",
                Descripcion = "La aprendiz del mago oscuro",
                Tipo = TipoCarta.Monstruo,
                Ataque = 2000,
                Defensa = 1700,
                Nivel = 6,
                Atributo = Atributo.Oscuridad,
                TipoMonstruo = "Lanzador de Conjuros",
                Posicion = PosicionMonstruo.Ataque,
                EstaBocaAbajo = false,
                RutaImagen = "Resources/cartas/Chica maga oscura.jpg"
            });

            // Guerrero celta
            jugador.Baraja.Add(new CartaMonstruo
            {
                Nombre = "Guerrero celta",
                Descripcion = "Un guerrero ágil y fuerte",
                Tipo = TipoCarta.Monstruo,
                Ataque = 1400,
                Defensa = 1200,
                Nivel = 4,
                Atributo = Atributo.Tierra,
                TipoMonstruo = "Guerrero",
                Posicion = PosicionMonstruo.Ataque,
                EstaBocaAbajo = false,
                RutaImagen = "Resources/cartas/Guerrero celta.jpg"
            });

            // Mago del tiempo
            jugador.Baraja.Add(new CartaMonstruo
            {
                Nombre = "Mago del tiempo",
                Descripcion = "Controla el flujo del tiempo",
                Tipo = TipoCarta.Monstruo,
                Ataque = 1200,
                Defensa = 1500,
                Nivel = 3,
                Atributo = Atributo.Luz,
                TipoMonstruo = "Lanzador de Conjuros",
                Posicion = PosicionMonstruo.Ataque,
                EstaBocaAbajo = false,
                RutaImagen = "Resources/cartas/Mago del tiempo.jpg"
            });

            // Kuriboh
            jugador.Baraja.Add(new CartaMonstruo
            {
                Nombre = "Kuriboh",
                Descripcion = "Una pequeña criatura peluda",
                Tipo = TipoCarta.Monstruo,
                Ataque = 300,
                Defensa = 200,
                Nivel = 1,
                Atributo = Atributo.Tierra,
                TipoMonstruo = "Demonio",
                Posicion = PosicionMonstruo.Ataque,
                EstaBocaAbajo = false,
                RutaImagen = "Resources/cartas/Kuriboh.jpg"
            });

            // Tifon Magico
            jugador.Baraja.Add(new CartaMagica
            {
                Nombre = "Tifon Magico",
                Descripcion = "Destruye una carta mágica o trampa en el campo",
                Tipo = TipoCarta.Magica,
                TipoMagica = TipoMagica.Normal,
                EstaBocaAbajo = false,
                RutaImagen = "Resources/cartas/Tifon Magico.jpg"
            });

            // Cilindros Mágicos
            jugador.Baraja.Add(new CartaTrampa
            {
                Nombre = "Cilindros Mágicos",
                Descripcion = "Anula un ataque y recibe daño igual al ataque del monstruo atacante",
                Tipo = TipoCarta.Trampa,
                TipoTrampa = TipoTrampa.Normal,
                EstaBocaAbajo = false,
                RutaImagen = "Resources/cartas/Cilindros Magicos.jpg"
            });

            // Olla de la codicia
            jugador.Baraja.Add(new CartaMagica
            {
                Nombre = "Olla de la codicia",
                Descripcion = "Roba 2 cartas adicionales",
                Tipo = TipoCarta.Magica,
                TipoMagica = TipoMagica.Normal,
                EstaBocaAbajo = false,
                RutaImagen = "Resources/cartas/Olla de la avaricia.jpg"
            });

            // Ataque negado
            jugador.Baraja.Add(new CartaTrampa
            {
                Nombre = "Ataque negado",
                Descripcion = "Anula un ataque y destruye el monstruo atacante",
                Tipo = TipoCarta.Trampa,
                TipoTrampa = TipoTrampa.Normal,
                EstaBocaAbajo = false,
                RutaImagen = "Resources/cartas/Ataque negado.jpg"
            });

        }

        static void CrearMazoKaiba(Jugador jugador)
        {
            // Mazo de Kaiba - Agregá las cartas aquí

            // Dragón Blanco de Ojos Azules
            jugador.Baraja.Add(new CartaMonstruo
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
                EstaBocaAbajo = false,
                RutaImagen = "Resources/cartas/Dragon blanco.jpg"
            });

            // Kaibaman
            jugador.Baraja.Add(new CartaMonstruo
            {
                Nombre = "Kaibaman",
                Descripcion = "Un leal sirviente del maestro Kaiba",
                Tipo = TipoCarta.Monstruo,
                Ataque = 0,
                Defensa = 0,
                Nivel = 1,
                Atributo = Atributo.Luz,
                TipoMonstruo = "Guerrero",
                Posicion = PosicionMonstruo.Ataque,
                EstaBocaAbajo = false,
                RutaImagen = "Resources/cartas/Kaibaman.jpg"
            });

            // lord of d
            jugador.Baraja.Add(new CartaMonstruo
            {
                Nombre = "Señor de D",
                Descripcion = "Un guerrero poderoso que puede destruir cartas mágicas y trampas",
                Tipo = TipoCarta.Monstruo,
                Ataque = 1200,
                Defensa = 1000,
                Nivel = 4,
                Atributo = Atributo.Tierra,
                TipoMonstruo = "Guerrero",
                Posicion = PosicionMonstruo.Ataque,
                EstaBocaAbajo = false,
                RutaImagen = "Resources/cartas/mr Dragon.jpg"
            });

            // summoned skull
            jugador.Baraja.Add(new CartaMonstruo
            {
                Nombre = "Calavera Invocada",
                Descripcion = "Un monstruo de tipo demonio con gran poder de ataque",
                Tipo = TipoCarta.Monstruo,
                Ataque = 2500,
                Defensa = 1200,
                Nivel = 6,
                Atributo = Atributo.Oscuridad,
                TipoMonstruo = "Demonio",
                Posicion = PosicionMonstruo.Ataque,
                EstaBocaAbajo = false,
                RutaImagen = "Resources/cartas/invoca Al Craneo.jpg"
            });

                        // Tifon Magico
            jugador.Baraja.Add(new CartaMagica
            {
                Nombre = "Tifon Magico",
                Descripcion = "Destruye una carta mágica o trampa en el campo",
                Tipo = TipoCarta.Magica,
                TipoMagica = TipoMagica.Normal,
                EstaBocaAbajo = false,
                RutaImagen = "Resources/cartas/Tifon Magico.jpg"
            });

            // Cilindros Mágicos
            jugador.Baraja.Add(new CartaTrampa
            {
                Nombre = "Cilindros Mágicos",
                Descripcion = "Anula un ataque y recibe daño igual al ataque del monstruo atacante",
                Tipo = TipoCarta.Trampa,
                TipoTrampa = TipoTrampa.Normal,
                EstaBocaAbajo = false,
                RutaImagen = "Resources/cartas/Cilindros Magicos.jpg"
            });

            // Olla de la codicia
            jugador.Baraja.Add(new CartaMagica
            {
                Nombre = "Olla de la codicia",
                Descripcion = "Roba 2 cartas adicionales",
                Tipo = TipoCarta.Magica,
                TipoMagica = TipoMagica.Normal,
                EstaBocaAbajo = false,
                RutaImagen = "Resources/cartas/Olla de la avaricia.jpg"
            });

            // Ataque negado
            jugador.Baraja.Add(new CartaTrampa
            {
                Nombre = "Ataque negado",
                Descripcion = "Anula un ataque y destruye el monstruo atacante",
                Tipo = TipoCarta.Trampa,
                TipoTrampa = TipoTrampa.Normal,
                EstaBocaAbajo = false,
                RutaImagen = "Resources/cartas/Ataque negado.jpg"
            });

        }
    }
}