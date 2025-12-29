using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos;

public class LocationCheckResultDto
{
    public bool KeyPointCompleted { get; set; }
    public long? CompletedKeyPointId { get; set; }
    public DateTime LastActivity { get; set; }
    public int TotalCompletedKeyPoints { get; set; }
    public double ProgressPercentage { get; set; }
}
