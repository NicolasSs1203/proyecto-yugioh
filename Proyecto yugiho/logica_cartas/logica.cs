using System;

namespace logica
{
    class Program
    {
        static void Main(string[] args)
        {
            //cartas
            Carta carta1 = new Carta("Mago Oscuro", 2500, 2100, 7, "Hechicero", "Oscuridad", "Ataque");
            Carta carta2 = new Carta("Dragón Blanco de Ojos Azules", 3000, 2500, 8, "Dragón", "Luz", "Ataque");
            Carta carta3 = new Carta("Soldado del Lustre Negro", 3000, 2500, 8, "Guerrero", "Tierra", "Defensa");

            Console.WriteLine("Bienvenido al buelo yugioh");

            //elegir cartas
            Console.WriteLine("\nJugador 1, elige tu carta:");
            Console.WriteLine("1. Mago Oscuro");
            Console.WriteLine("2. Dragón Blanco de Ojos Azules");
            Console.WriteLine("3. Soldado del Lustre Negro");
            Console.Write("Elige (1, 2 o 3): ");
            string opcion1 = Console.ReadLine() ?? "";

            Carta jugador1 = opcion1 switch
            {
                "1" => carta1,
                "2" => carta2,
                "3" => carta3,
                _ => carta1
            };

            Console.WriteLine($"\nJugador 1 ha elegido: {jugador1.Nombre}");
            jugador1.CambiarPosicion("Ataque");

            Console.Clear();

            Console.WriteLine("Jugador 2, elige tu carta:");
            Console.WriteLine("1. Mago Oscuro");
            Console.WriteLine("2. Dragón Blanco de Ojos Azules");
            Console.WriteLine("3. Soldado del Lustre Negro");
            Console.Write("Elige (1, 2 o 3): ");
            string opcion2 = Console.ReadLine() ?? "";

            Carta jugador2 = opcion2 switch
            {
                "1" => carta1,
                "2" => carta2,
                "3" => carta3,
                _ => carta1
            };

            Console.WriteLine($"\nJugador 2 ha elegido: {jugador2.Nombre}");
            jugador2.CambiarPosicion("Ataque");

            Console.Clear();

            int vida1 = 4000;
            int vida2 = 4000;

            Console.WriteLine("Comienza el duelo");
            Console.WriteLine($"Jugador 1: {jugador1.Nombre} ({vida1} LP)");
            Console.WriteLine($"Jugador 2: {jugador2.Nombre} ({vida2} LP)");

            int turno = 1;
            bool juegoActivo = true;

            while (juegoActivo)
            {
                Console.WriteLine($"\nTurno {turno}");
               //jugador 1
                Console.WriteLine("\nTurno del jugador 1:");
                Console.WriteLine("1. Atacar");
                Console.WriteLine("2. Defender");
                Console.Write("Elige una opcion: ");
                string accion1 = Console.ReadLine() ?? "";

                if (accion1 == "1")
                {
                    jugador1.Ataca();
                    int daño = jugador1.Ataque - jugador2.Defensa;

                    if (daño > 0)
                    {
                        vida2 -= daño;
                        Console.WriteLine($"jugador 2 recibe {daño} puntos de daño");
                    }
                    else
                    {
                        Console.WriteLine("jugador 2 bloquea el ataque sin daño");
                    }
                }
                else if (accion1 == "2")
                {
                    jugador1.Defiende();
                }

                if (vida2 <= 0)
                {
                    Console.WriteLine("\njugador 1 ha ganado el duelo");
                    break;
                }

                //jugador 2
                Console.WriteLine("\nTurno del Jugador 2:");
                Console.WriteLine("1. Atacar");
                Console.WriteLine("2. Defender");
                Console.Write("Elige una opcion: ");
                string accion2 = Console.ReadLine() ?? "";

                if (accion2 == "1")
                {
                    jugador2.Ataca();
                    int daño = jugador2.Ataque - jugador1.Defensa;

                    if (daño > 0)
                    {
                        vida1 -= daño;
                        Console.WriteLine($"jugador 1 recibe {daño} puntos de daño");
                    }
                    else
                    {
                        Console.WriteLine("jugador 1 bloquea el ataque sin daño");
                    }
                }
                else if (accion2 == "2")
                {
                    jugador2.Defiende();
                }

                if (vida1 <= 0)
                {
                    Console.WriteLine("\njugador 2 ha ganado el duelo");
                    break;
                }

                Console.WriteLine($"\nVida Jugador 1: {vida1} LP");
                Console.WriteLine($" Vida Jugador 2: {vida2} LP");

                turno++;
            }

            Console.WriteLine("\nfin del duelo");
        }
    }
}


