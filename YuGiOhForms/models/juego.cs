using YuGiOh.Models.Cartas;
using YuGiOh.Enums;
using System.Linq;

namespace YuGiOh.Models
{
    public class Juego
    {
        public Jugador Jugador1 { get; set; }
        public Jugador Jugador2 { get; set; }
        public Jugador JugadorActual { get; private set; }
        public Jugador JugadorOponente { get; private set; }
        public int NumeroTurno { get; private set; }
        public bool JuegoTerminado { get; private set; }

        public Juego(Jugador jugador1, Jugador jugador2)
        {
            Jugador1 = jugador1;
            Jugador2 = jugador2;
            JugadorActual = jugador1; 
            JugadorOponente = jugador2;
            NumeroTurno = 1;
            JuegoTerminado = false;
        }

     
        public void IniciarJuego()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("       ¡DUELO DE YU-GI-OH!");
            Console.WriteLine("========================================");
            Console.WriteLine($"{Jugador1.Nombre} VS {Jugador2.Nombre}\n");

         
            Console.WriteLine("--- Robando mano inicial ---");
            for (int i = 0; i < 5; i++)
            {
                Jugador1.RobarCartaSinRestriccion();
                Jugador2.RobarCartaSinRestriccion();
            }

            Console.WriteLine($"\n{Jugador1.Nombre} y {Jugador2.Nombre} robaron 5 cartas cada uno.");
            Console.WriteLine($"\n¡{JugadorActual.Nombre} comienza!");
        }

        public void CambiarTurno()
        {
 
            var temp = JugadorActual;
            JugadorActual = JugadorOponente;
            JugadorOponente = temp;

            // Resetear la restricción de robo para el nuevo turno
            JugadorActual.YaRoboEsteTurno = false;

            NumeroTurno++;

            Console.WriteLine("\n========================================");
            Console.WriteLine($"   TURNO {NumeroTurno} - {JugadorActual.Nombre}");
            Console.WriteLine("========================================");
        }


        public void FaseRobo()
        {
            Console.WriteLine("\n--- FASE DE ROBO ---");
            

            if (NumeroTurno == 1)
            {
                Console.WriteLine($"{JugadorActual.Nombre} no roba en el primer turno.");
                return;
            }

            JugadorActual.RobarCarta();
        }


public void FasePrincipal()
{
    Console.WriteLine("\n--- FASE PRINCIPAL ---");
    
    bool monstruoInvocado = false;  
    bool faseTerminada = false;

    while (!faseTerminada)
    {
        Console.WriteLine($"\n{JugadorActual.Nombre}, ¿qué deseas hacer?");
        Console.WriteLine("[1] Ver mi mano");
        Console.WriteLine("[2] Jugar monstruo" + (monstruoInvocado ? " (ya invocaste este turno)" : ""));
        Console.WriteLine("[3] Colocar carta mágica/trampa");
        Console.WriteLine("[4] Ver mi campo");
        Console.WriteLine("[5] Ver campo enemigo");
        Console.WriteLine("[6] Terminar fase");
        Console.Write("> ");

        string? opcion = Console.ReadLine();

        switch (opcion)
        {
            case "1":
                JugadorActual.MostrarMano();
                break;

            case "2":
                if (monstruoInvocado)
                {
                    Console.WriteLine("Ya invocaste un monstruo este turno.");
                    break;
                }
                if (InvocarMonstruo())
                {
                    monstruoInvocado = true;
                }
                break;

            case "3":
                ColocarMagiaTrampa();
                break;

            case "4":
                JugadorActual.MostrarEstado();
                break;

            case "5":
                JugadorOponente.MostrarEstado();
                break;

            case "6":
                faseTerminada = true;
                Console.WriteLine($"{JugadorActual.Nombre} termina la Fase Principal.");
                break;

            default:
                Console.WriteLine("Opción inválida.");
                break;
        }
    }
}


private bool InvocarMonstruo()
{

    var monstruos = JugadorActual.Mano
        .Where(c => c.Tipo == TipoCarta.Monstruo)
        .Cast<CartaMonstruo>()
        .ToList();

    if (monstruos.Count == 0)
    {
        Console.WriteLine("No tienes monstruos en tu mano.");
        return false;
    }

 
    Console.WriteLine("\nMonstruos en tu mano:");
    for (int i = 0; i < monstruos.Count; i++)
    {
        var m = monstruos[i];
        Console.WriteLine($"[{i + 1}] {m.Nombre} (Nivel {m.Nivel}, ATK:{m.Ataque}/DEF:{m.Defensa})");
    }
    Console.WriteLine("[0] Cancelar");
    Console.Write("Elige un monstruo: ");

    if (!int.TryParse(Console.ReadLine(), out int indice) || indice < 0 || indice > monstruos.Count)
    {
        Console.WriteLine("Opción inválida.");
        return false;
    }

    if (indice == 0) return false;

    var monstruoElegido = monstruos[indice - 1];

  
    Console.WriteLine("\n¿En qué posición del campo? (0, 1, 2)");
    JugadorActual.MiCampo.MostrarCampo();
    Console.Write("> ");

    if (!int.TryParse(Console.ReadLine(), out int posicionCampo) || posicionCampo < 0 || posicionCampo > 2)
    {
        Console.WriteLine("Posición inválida.");
        return false;
    }

    if (JugadorActual.MiCampo.ZonaMonstruos[posicionCampo] != null)
    {
        Console.WriteLine("Esa posición ya está ocupada.");
        return false;
    }


    Console.WriteLine("\n¿En qué posición?");
    Console.WriteLine("[1] Ataque");
    Console.WriteLine("[2] Defensa");
    Console.Write("> ");

    string? posicionMonstruo = Console.ReadLine();
    if (posicionMonstruo == "1")
    {
        monstruoElegido.Posicion = PosicionMonstruo.Ataque;
    }
    else if (posicionMonstruo == "2")
    {
        monstruoElegido.Posicion = PosicionMonstruo.Defensa;
    }
    else
    {
        Console.WriteLine("Opción inválida.");
        return false;
    }


    JugadorActual.JugarMonstruo(monstruoElegido, posicionCampo);
    return true;
}


private void ColocarMagiaTrampa()
{

    var magiasTrampas = JugadorActual.Mano
        .Where(c => c.Tipo == TipoCarta.Magica || c.Tipo == TipoCarta.Trampa)
        .ToList();

    if (magiasTrampas.Count == 0)
    {
        Console.WriteLine("No tienes cartas mágicas/trampa en tu mano.");
        return;
    }


    Console.WriteLine("\nCartas mágicas/trampa en tu mano:");
    for (int i = 0; i < magiasTrampas.Count; i++)
    {
        var c = magiasTrampas[i];
        Console.WriteLine($"[{i + 1}] {c.Nombre} ({c.Tipo})");
    }
    Console.WriteLine("[0] Cancelar");
    Console.Write("Elige una carta: ");

    if (!int.TryParse(Console.ReadLine(), out int indice) || indice < 0 || indice > magiasTrampas.Count)
    {
        Console.WriteLine("Opción inválida.");
        return;
    }

    if (indice == 0) return;

    var cartaElegida = magiasTrampas[indice - 1];


    Console.WriteLine("\n¿En qué posición del campo? (0, 1, 2)");
    Console.Write("> ");

    if (!int.TryParse(Console.ReadLine(), out int posicionCampo) || posicionCampo < 0 || posicionCampo > 2)
    {
        Console.WriteLine("Posición inválida.");
        return;
    }


    JugadorActual.JugarMagiaTrampa(cartaElegida, posicionCampo);
}

        
        public void FaseBatalla()
        {
            Console.WriteLine("\n--- FASE DE BATALLA ---");
            
            if (NumeroTurno == 1)
            {
                Console.WriteLine("No se puede atacar en el primer turno.");
                return;
            }

            Console.WriteLine($"{JugadorActual.Nombre} puede atacar con sus monstruos.");
           
        }

 
        public bool VerificarVictoria()
        {
            if (Jugador1.PuntosVida <= 0)
            {
                Console.WriteLine("\n========================================");
                Console.WriteLine($"   ¡{Jugador2.Nombre} GANA EL DUELO!");
                Console.WriteLine("========================================");
                JuegoTerminado = true;
                return true;
            }

            if (Jugador2.PuntosVida <= 0)
            {
                Console.WriteLine("\n========================================");
                Console.WriteLine($"   ¡{Jugador1.Nombre} GANA EL DUELO!");
                Console.WriteLine("========================================");
                JuegoTerminado = true;
                return true;
            }

      
            if (Jugador1.Baraja.Count == 0)
            {
                Console.WriteLine("\n========================================");
                Console.WriteLine($"   ¡{Jugador2.Nombre} GANA! (Deck Out)");
                Console.WriteLine("========================================");
                JuegoTerminado = true;
                return true;
            }

            if (Jugador2.Baraja.Count == 0)
            {
                Console.WriteLine("\n========================================");
                Console.WriteLine($"   ¡{Jugador1.Nombre} GANA! (Deck Out)");
                Console.WriteLine("========================================");
                JuegoTerminado = true;
                return true;
            }

            return false;
        }

  
        public void EjecutarTurno()
        {
            FaseRobo();
            FasePrincipal();
            JugadorActual.MostrarEstado();
            FaseBatalla();
            
       
            if (VerificarVictoria())
            {
                return;
            }

            Console.WriteLine($"\n{JugadorActual.Nombre} termina su turno.");
        }
    }
}