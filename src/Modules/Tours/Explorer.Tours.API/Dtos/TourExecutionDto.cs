using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Explorer.Tours.API.Dtos;

public class TourExecutionDto
{
    public long Id { get; set; }
    public long TouristId { get; set; }
    public long TourId { get; set; }
    public DateTime StartTime { get; set; }
    public int Status { get; set; }
    public double StartLatitude { get; set; }
    public double StartLongitude { get; set; }
    public DateTime? CompletionTime { get; set; }
    public DateTime? AbandonTime { get; set; }

    //
    public List<KeyPointCompletionDto> CompletedKeyPoints { get; set; } = new List<KeyPointCompletionDto>();
}

//
public class KeyPointCompletionDto
{
    public long KeyPointId { get; set; }
    public DateTime CompletedAt { get; set; }
}

