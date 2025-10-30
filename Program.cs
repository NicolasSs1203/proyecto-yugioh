using YuGiOh.Models.Cartas;
using YuGiOh.Enums;
using YuGiOh.Models.CartaMonstruo;

namespace YuGiOh
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== PROBANDO CARTAS DE YU-GI-OH ===\n");

            // 1. Carta Monstruo
            var dragonBlanco = new CartaMonstruo
            {
                Nombre = "Dragón Blanco de Ojos Azules",
                Descripcion = "Este dragón legendario es una poderosa máquina de destrucción.",
                Tipo = TipoCarta.Monstruo,
                Ataque = 3000,
                Defensa = 2500,
                Nivel = 8,
                Atributo = Atributo.Luz,
                TipoMonstruo = "Dragón",
                Posicion = PosicionMonstruo.Ataque,
                EstaBocaAbajo = false
            };

            // 2. Carta Mágica
            var ollaCodicia = new CartaMagica
            {
                Nombre = "Olla de la Codicia",
                Descripcion = "Roba 2 cartas de tu Deck.",
                Tipo = TipoCarta.Magica,
                TipoMagica = TipoMagica.Normal,
                EstaBocaAbajo = false
            };

            // 3. Carta Trampa
            var cilindroMagico = new CartaTrampa
            {
                Nombre = "Cilindro Mágico",
                Descripcion = "Niega el ataque de un monstruo y inflige daño igual a su ATK.",
                Tipo = TipoCarta.Trampa,
                TipoTrampa = TipoTrampa.Normal,
                EstaBocaAbajo = true
            };

            // Mostrar las cartas
            Console.WriteLine("--- CARTA MONSTRUO ---");
            Console.WriteLine($"Nombre: {dragonBlanco.Nombre}");
            Console.WriteLine($"Tipo: {dragonBlanco.Tipo}");
            Console.WriteLine($"ATK: {dragonBlanco.Ataque} / DEF: {dragonBlanco.Defensa}");
            Console.WriteLine($"Nivel: {dragonBlanco.Nivel} ⭐");
            Console.WriteLine($"Atributo: {dragonBlanco.Atributo}");
            Console.WriteLine($"Tipo Monstruo: {dragonBlanco.TipoMonstruo}");
            Console.WriteLine($"Posición: {dragonBlanco.Posicion}");
            Console.WriteLine($"Boca abajo: {dragonBlanco.EstaBocaAbajo}");

            Console.WriteLine("\n--- CARTA MÁGICA ---");
            Console.WriteLine($"Nombre: {ollaCodicia.Nombre}");
            Console.WriteLine($"Tipo: {ollaCodicia.Tipo}");
            Console.WriteLine($"Subtipo: {ollaCodicia.TipoMagica}");
            Console.WriteLine($"Descripción: {ollaCodicia.Descripcion}");
            Console.WriteLine($"Boca abajo: {ollaCodicia.EstaBocaAbajo}");

            Console.WriteLine("\n--- CARTA TRAMPA ---");
            Console.WriteLine($"Nombre: {cilindroMagico.Nombre}");
            Console.WriteLine($"Tipo: {cilindroMagico.Tipo}");
            Console.WriteLine($"Subtipo: {cilindroMagico.TipoTrampa}");
            Console.WriteLine($"Descripción: {cilindroMagico.Descripcion}");
            Console.WriteLine($"Boca abajo: {cilindroMagico.EstaBocaAbajo}");
        }
    }
}