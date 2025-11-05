using YuGiOh.Models.Cartas;
using YuGiOh.Enums;

namespace YuGiOh.Models
{
    public class Combate
    {
        public static void AtacarMonstruo(
            Jugador atacante, 
            CartaMonstruo monstruoAtacante, 
            Jugador defensor, 
            CartaMonstruo monstruoDefensor,
            int posicionAtacante,
            int posicionDefensor)
        {
            Console.WriteLine($"\n--- COMBATE ---");
            Console.WriteLine($"{atacante.Nombre}: {monstruoAtacante.Nombre} (ATK: {monstruoAtacante.Ataque})");
            Console.WriteLine($"VS");
            Console.WriteLine($"{defensor.Nombre}: {monstruoDefensor.Nombre} " +
                            $"({(monstruoDefensor.Posicion == PosicionMonstruo.Ataque ? $"ATK: {monstruoDefensor.Ataque}" : $"DEF: {monstruoDefensor.Defensa}")})");

            if (monstruoDefensor.Posicion == PosicionMonstruo.Ataque)
            {
                AtaqueVsAtaque(atacante, monstruoAtacante, defensor, monstruoDefensor, posicionAtacante, posicionDefensor);
            }
            else
            {
                AtaqueVsDefensa(atacante, monstruoAtacante, defensor, monstruoDefensor, posicionAtacante, posicionDefensor);
            }
        }

        private static void AtaqueVsAtaque(
            Jugador atacante,
            CartaMonstruo monstruoAtacante,
            Jugador defensor,
            CartaMonstruo monstruoDefensor,
            int posicionAtacante,
            int posicionDefensor)
        {
            if (monstruoAtacante.Ataque > monstruoDefensor.Ataque)
            {
                int diferencia = monstruoAtacante.Ataque - monstruoDefensor.Ataque;
                Console.WriteLine($"¡{monstruoAtacante.Nombre} destruye a {monstruoDefensor.Nombre}!");
                Console.WriteLine($"{defensor.Nombre} recibe {diferencia} de daño.");
                
                DestruirMonstruo(defensor, monstruoDefensor, posicionDefensor);
                defensor.RecibirDanio(diferencia);
            }
            else if (monstruoAtacante.Ataque < monstruoDefensor.Ataque)
            {
                int diferencia = monstruoDefensor.Ataque - monstruoAtacante.Ataque;
                Console.WriteLine($"¡{monstruoDefensor.Nombre} destruye a {monstruoAtacante.Nombre}!");
                Console.WriteLine($"{atacante.Nombre} recibe {diferencia} de daño.");
                
                DestruirMonstruo(atacante, monstruoAtacante, posicionAtacante);
                atacante.RecibirDanio(diferencia);
            }
            else
            {
                Console.WriteLine("¡Ambos monstruos tienen el mismo ATK!");
                Console.WriteLine("Ambos monstruos son destruidos.");
                
                DestruirMonstruo(atacante, monstruoAtacante, posicionAtacante);
                DestruirMonstruo(defensor, monstruoDefensor, posicionDefensor);
            }
        }

        private static void AtaqueVsDefensa(
            Jugador atacante,
            CartaMonstruo monstruoAtacante,
            Jugador defensor,
            CartaMonstruo monstruoDefensor,
            int posicionAtacante,
            int posicionDefensor)
        {
            if (monstruoAtacante.Ataque > monstruoDefensor.Defensa)
            {
                Console.WriteLine($"¡{monstruoAtacante.Nombre} destruye a {monstruoDefensor.Nombre}!");
                Console.WriteLine($"{defensor.Nombre} no recibe daño (estaba en defensa).");
                
                DestruirMonstruo(defensor, monstruoDefensor, posicionDefensor);
            }
            else if (monstruoAtacante.Ataque < monstruoDefensor.Defensa)
            {
                int diferencia = monstruoDefensor.Defensa - monstruoAtacante.Ataque;
                Console.WriteLine($"¡{monstruoDefensor.Nombre} resiste el ataque!");
                Console.WriteLine($"{atacante.Nombre} recibe {diferencia} de daño.");
                
                atacante.RecibirDanio(diferencia);
            }
            else
            {
                Console.WriteLine($"{monstruoDefensor.Nombre} resiste el ataque.");
                Console.WriteLine("No hay daño ni destrucción.");
            }
        }

        public static void AtaqueDirecto(Jugador atacante, CartaMonstruo monstruoAtacante, Jugador defensor)
        {
            Console.WriteLine($"\n--- ATAQUE DIRECTO ---");
            Console.WriteLine($"{atacante.Nombre}: {monstruoAtacante.Nombre} (ATK: {monstruoAtacante.Ataque})");
            Console.WriteLine($"¡Ataque directo a {defensor.Nombre}!");
            
            defensor.RecibirDanio(monstruoAtacante.Ataque);
        }

        private static void DestruirMonstruo(Jugador jugador, CartaMonstruo monstruo, int posicion)
        {
            jugador.MiCampo.RemoverMonstruo(posicion);
            jugador.Cementerio.Add(monstruo);
            Console.WriteLine($"{monstruo.Nombre} fue enviado al cementerio de {jugador.Nombre}.");
        }
    }
}