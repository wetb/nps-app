namespace NPSApplication.Application.DTOs;

public class NPSResultResponse
{
    public int TotalVotes { get; set; }
    public int Promoters { get; set; }
    public int Neutrals { get; set; }
    public int Detractors { get; set; }
    public decimal NPSScore { get; set; }
    public decimal PromotersPercentage { get; set; }
    public decimal NeutralsPercentage { get; set; }
    public decimal DetractorsPercentage { get; set; }
    public string NPSCategory { get; set; } = string.Empty;

    public void CalculatePercentages()
    {
        if (TotalVotes > 0)
        {
            PromotersPercentage = Math.Round((decimal)Promoters / TotalVotes * 100, 2);
            NeutralsPercentage = Math.Round((decimal)Neutrals / TotalVotes * 100, 2);
            DetractorsPercentage = Math.Round((decimal)Detractors / TotalVotes * 100, 2);
        }

        // Categorizar NPS
        NPSCategory = NPSScore switch
        {
            >= 75 => "Excelente",
            >= 50 => "Muy Bueno",
            >= 0 => "Bueno",
            >= -50 => "Necesita Mejorar",
            _ => "Crítico"
        };
    }
}