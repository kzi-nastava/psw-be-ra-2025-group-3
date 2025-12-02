using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public enum TourExecutionStatus
    {
        Active = 0,      //  u toku
        Completed = 1,   //  uspešno završena
        Abandoned = 2    //  napuštena 
    }
}
