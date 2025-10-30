using YuGiOh.Enums;

namespace YuGiOh.Models.Cartas
{
    public class CartaMagica : Carta
    {
        public TipoMagica TipoMagica { get; set; }
        public bool EstaBocaAbajo { get; set; }
    }
}