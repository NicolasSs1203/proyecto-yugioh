using YuGiOh.Models.Cartas;

namespace YuGiOh.Models
{
    public class Campo
    {
        // 3 espacios para monstruos (null = espacio vacío)
        public CartaMonstruo?[] ZonaMonstruos { get; set; } = new CartaMonstruo?[3];
        
        // 3 espacios para magia/trampa (null = espacio vacío)
        public Carta?[] ZonaMagiaTrampa { get; set; } = new Carta?[3];

        // Método: Colocar monstruo en una posición
        public bool ColocarMonstruo(CartaMonstruo monstruo, int posicion)
        {
            if (posicion < 0 || posicion >= 3)
            {
                Console.WriteLine("Posición inválida. Debe ser 0, 1 o 2.");
                return false;
            }

            if (ZonaMonstruos[posicion] != null)
            {
                Console.WriteLine($"Ya hay un monstruo en la posición {posicion}.");
                return false;
            }

            ZonaMonstruos[posicion] = monstruo;
            Console.WriteLine($"{monstruo.Nombre} colocado en posición {posicion}.");
            return true;
        }

        // Método: Colocar carta mágica/trampa
        public bool ColocarMagiaTrampa(Carta carta, int posicion)
        {
            if (posicion < 0 || posicion >= 3)
            {
                Console.WriteLine("Posición inválida. Debe ser 0, 1 o 2.");
                return false;
            }

            if (ZonaMagiaTrampa[posicion] != null)
            {
                Console.WriteLine($"Ya hay una carta en la posición {posicion}.");
                return false;
            }

            ZonaMagiaTrampa[posicion] = carta;
            Console.WriteLine($"{carta.Nombre} colocada en posición {posicion}.");
            return true;
        }

        // Método: Remover monstruo
        public CartaMonstruo? RemoverMonstruo(int posicion)
        {
            if (posicion < 0 || posicion >= 3)
            {
                Console.WriteLine("Posición invalida.");
                return null;
            }

            var monstruo = ZonaMonstruos[posicion];
            if (monstruo == null)
            {
                Console.WriteLine("No hay monstruo en esa posición.");
                return null;
            }

            ZonaMonstruos[posicion] = null;
            Console.WriteLine($"{monstruo.Nombre} removido de posición {posicion}.");
            return monstruo;
        }

        // Método: Mostrar el campo
        public void MostrarCampo()
        {
            Console.WriteLine("\n=== CAMPO ===");
            
            Console.WriteLine("Zona de Monstruos:");
            for (int i = 0; i < 3; i++)
            {
                if (ZonaMonstruos[i] != null)
                {
                    var m = ZonaMonstruos[i]!;
                    Console.WriteLine($"  [{i}] {m.Nombre} (ATK: {m.Ataque} / DEF: {m.Defensa}) - {m.Posicion}");
                }
                else
                {
                    Console.WriteLine($"  [{i}] (vacío)");
                }
            }

            Console.WriteLine("\nZona de Magia/Trampa:");
            for (int i = 0; i < 3; i++)
            {
                if (ZonaMagiaTrampa[i] != null)
                {
                    Console.WriteLine($"  [{i}] {ZonaMagiaTrampa[i]!.Nombre}");
                }
                else
                {
                    Console.WriteLine($"  [{i}] (vacío)");
                }
            }
        }
    }
}