using YuGiOh.Enums;

namespace YuGiOh.Models.Cartas
{
    public class CartaTrampa : Carta
    {
        public TipoTrampa TipoTrampa { get; set; }
        public new bool EstaBocaAbajo { get; set; }
    }
}