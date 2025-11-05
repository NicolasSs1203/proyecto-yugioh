using YuGiOh.Enums;
namespace YuGiOh.Models.Cartas
{
    public abstract class Carta
    {
        public required string Nombre { get; set; }
        public required string Descripcion { get; set; }
        public required TipoCarta Tipo { get; set; }
        public bool EstaBocaAbajo { get; set; }
        public string? RutaImagen { get; set; }
    }
}