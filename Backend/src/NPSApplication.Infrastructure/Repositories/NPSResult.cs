namespace NPSApplication.Application.DTOs;

public class NPSResult
{
    public int TotalVotes { get; set; }
    public int Promoters { get; set; }
    public int Neutrals { get; set; }
    public int Detractors { get; set; }
    public decimal NPSScore { get; set; }
}