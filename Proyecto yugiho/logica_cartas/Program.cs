using System;

namespace logica
{
    class Carta
    {
        private string nombre = "";
        private int ataque;
        private int defensa;
        private int nivel;
        private string tipo = "";
        private string atributo = "";
        private string posicion = "";
        public string Nombre
        {
            get => nombre;
            set => nombre = value ?? "Sin nombre";
        }

        public int Ataque
        {
            get => ataque;
            set => ataque = (value >= 0) ? value : 0;
        }

        public int Defensa
        {
            get => defensa;
            set => defensa = (value >= 0) ? value : 0;
        }

        public int Nivel
        {
            get => nivel;
            set => nivel = (value >= 0) ? value : 0;
        }

        public string Tipo
        {
            get => tipo;
            set => tipo = value ?? "Desconocido";
        }

        public string Atributo
        {
            get => atributo;
            set => atributo = value ?? "Neutro";
        }

        public string Posicion
        {
            get => posicion;
            set
            {
                if (string.Equals(value, "Ataque", StringComparison.OrdinalIgnoreCase))
                    posicion = "Ataque";
                else if (string.Equals(value, "Defensa", StringComparison.OrdinalIgnoreCase))
                    posicion = "Defensa";
                else
                    posicion = "Posición desconocida";
            }
        }

        public Carta()
        {
            nombre = "Sin nombre";
            ataque = 0;
            defensa = 0;
            nivel = 1;
            tipo = "Desconocido";
            atributo = "Neutro";
            posicion = "Ataque";
        }

        public Carta(string nombre, int ataque, int defensa, int nivel, string tipo, string atributo, string posicion)
        {
            Nombre = nombre;
            Ataque = ataque;
            Defensa = defensa;
            Nivel = nivel;
            Tipo = tipo;
            Atributo = atributo;
            Posicion = posicion;
        }

        public void Ataca()
        {
            Console.WriteLine($"El monstruo {Nombre} atacará con {Ataque} puntos");
        }

        public void Defiende()
        {
            Console.WriteLine($"El monstruo {Nombre} defenderá con {Defensa} puntos");
        }

        public void CambiarPosicion(string nuevaPosicion)
        {
                if (string.Equals(nuevaPosicion, "Ataque", StringComparison.OrdinalIgnoreCase))
    {
        Posicion = "Ataque";
        Console.WriteLine($"La carta {Nombre} fue colocada en ataque");
    }
    else if (string.Equals(nuevaPosicion, "Defensa", StringComparison.OrdinalIgnoreCase))
    {
        Posicion = "Defensa";
        Console.WriteLine($"La carta {Nombre} fue colocada en defensa");
    }
    else
    {
        Console.WriteLine("Posición no válida. Usa 'Ataque' o 'Defensa'.");
    }
        }


        public void MostrarInformacion()
        {
            Console.WriteLine($"Nombre: {Nombre}");
            Console.WriteLine($"Ataque: {Ataque}");
            Console.WriteLine($"Defensa: {Defensa}");
            Console.WriteLine($"Nivel: {Nivel}");
            Console.WriteLine($"Tipo: {Tipo}");
            Console.WriteLine($"Atributo: {Atributo}");
            Console.WriteLine($"Posición: {Posicion}");
        }
    }
}
