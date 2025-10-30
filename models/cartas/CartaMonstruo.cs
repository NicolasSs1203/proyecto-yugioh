using YuGiOh.Enums;
using YuGiOh.Models.Cartas;
namespace YuGiOh.Models.CartaMonstruo
{
    public class CartaMonstruo : Carta
    {
        public int Ataque { get; set; }
        public int Defensa { get; set; }
        public int Nivel { get; set; }
        public Atributo Atributo { get; set; }
        public required string TipoMonstruo { get; set; }
        public PosicionMonstruo Posicion { get; set; }
        public bool EstaBocaAbajo { get; set; }
    }
}
