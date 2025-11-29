using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Explorer.Stakeholders.API.Dtos;

public class PreferenceCreateDto
{
    public int Difficulty { get; set; }
    public int WalkingRating { get; set; }
    public int BicycleRating { get; set; }
    public int CarRating { get; set; }
    public int BoatRating { get; set; }
    public List<string> Tags { get; set; }
}
