namespace inmobiliaria2026.Models.ViewModels;

public class MultaViewModel
{
    public int TotalDiasReserva { get; set; }
    public int CantidadDiasReservados { get; set; }
    public decimal ImporteTotalReserva { get; set; }
    public decimal Pagado { get; set; }
    public decimal Deuda { get; set; }
    public decimal Importe { get; set; }
    public long ReservaId { get; set; }
}