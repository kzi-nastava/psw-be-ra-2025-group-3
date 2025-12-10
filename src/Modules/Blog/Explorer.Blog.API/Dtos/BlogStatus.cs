using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Dtos
{
    public enum BlogStatus
    {
        Draft = 0,      // Priprema
        Published = 1,  // Objavljen
        Archived = 2    // Arhiviran
    }
}
