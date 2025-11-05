using YuGiOh.Models.Cartas;
using YuGiOh.Enums;

namespace YuGiOh.Models
{
    public class Jugador
    {
        public required string Nombre { get; set; }
        public int PuntosVida { get; set; } = 8000;
        
        public List<Carta> Mano { get; set; } = new List<Carta>();
        public List<Carta> Baraja { get; set; } = new List<Carta>();
        public List<Carta> Cementerio { get; set; } = new List<Carta>();
        
        public Campo MiCampo { get; set; } = new Campo();
        public bool YaRoboEsteTurno { get; set; } = false;

        public void RobarCarta()
        {
            if (YaRoboEsteTurno)
            {
                Console.WriteLine($"{Nombre} ya robó una carta este turno.");
                return;
            }

            if (Baraja.Count > 0)
            {
                var carta = Baraja[0];
                Baraja.RemoveAt(0);
                Mano.Add(carta);
                Console.WriteLine($"{Nombre} robó: {carta.Nombre}");
                YaRoboEsteTurno = true;
            }
            else
            {
                Console.WriteLine($"{Nombre} no tiene mas cartas en el deck");
            }
        }

        public void RobarCartaSinRestriccion()
        {
            if (Baraja.Count > 0)
            {
                var carta = Baraja[0];
                Baraja.RemoveAt(0);
                Mano.Add(carta);
                Console.WriteLine($"{Nombre} robó: {carta.Nombre}");
            }
            else
            {
                Console.WriteLine($"{Nombre} no tiene mas cartas en el deck");
            }
        }

        public void DescartarCarta(Carta carta)
        {
            if (Mano.Contains(carta))
            {
                Mano.Remove(carta);
                Cementerio.Add(carta);
                Console.WriteLine($"{Nombre} descartó: {carta.Nombre}");
            }
            else
            {
                Console.WriteLine($"{Nombre} no tiene esa carta en la mano!");
            }
        }

        public void JugarMonstruo(CartaMonstruo monstruo, int posicion)
        {
            if (!Mano.Contains(monstruo))
            {
                Console.WriteLine($"{Nombre} no tiene {monstruo.Nombre} en la mano!");
                return;
            }

            if (MiCampo.ColocarMonstruo(monstruo, posicion))
            {
                Mano.Remove(monstruo);
                Console.WriteLine($"{Nombre} jugó {monstruo.Nombre} desde la mano.");
            }
        }

        public void JugarMagiaTrampa(Carta carta, int posicion)
        {
            if (!Mano.Contains(carta))
            {
                Console.WriteLine($"{Nombre} no tiene {carta.Nombre} en la mano!");
                return;
            }

            if (carta.Tipo == TipoCarta.Monstruo)
            {
                Console.WriteLine("Usa JugarMonstruo() para jugar monstruos.");
                return;
            }

            if (MiCampo.ColocarMagiaTrampa(carta, posicion))
            {
                Mano.Remove(carta);
                Console.WriteLine($"{Nombre} colocó {carta.Nombre} desde la mano.");
            }
        }

        public void RecibirDanio(int danio)
        {
            PuntosVida -= danio;
            Console.WriteLine($"{Nombre} recibió {danio} de daño. LP: {PuntosVida}");
            
            if (PuntosVida <= 0)
            {
                Console.WriteLine($"¡{Nombre} ha perdido el duelo!");
            }
        }

        public void MostrarMano()
        {
            Console.WriteLine($"\n=== Mano de {Nombre} ===");
            if (Mano.Count == 0)
            {
                Console.WriteLine("(Mano vacía)");
            }
            else
            {
                for (int i = 0; i < Mano.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {Mano[i].Nombre} ({Mano[i].Tipo})");
                }
            }
        }

        public void MostrarEstado()
        {
            Console.WriteLine($"\n========== {Nombre} ==========");
            Console.WriteLine($"LP: {PuntosVida}");
            Console.WriteLine($"Cartas en mano: {Mano.Count}");
            Console.WriteLine($"Cartas en deck: {Baraja.Count}");
            Console.WriteLine($"Cartas en cementerio: {Cementerio.Count}");
            MiCampo.MostrarCampo();
        }
    }
}