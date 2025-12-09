using System;
using System.Collections.Generic;

namespace Explorer.Tours.API.Dtos;

public class AdminTourProblemDto
{
    public long Id { get; set; }
    public long TourId { get; set; }
    public string TourName { get; set; }
    public long TouristId { get; set; } 

    public long AuthorId { get; set; }
    public int Category { get; set; }
    public int Priority { get; set; }
    public string Description { get; set; }
    public DateTime Time { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int Status { get; set; }
    public string? ResolvedByTouristComment { get; set; }
    
    public bool IsOverdue { get; set; }
    public int DaysOpen { get; set; }
    
    public List<MessageDto> Messages { get; set; } = new();
}
