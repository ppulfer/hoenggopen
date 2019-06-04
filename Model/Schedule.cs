using System.Collections.Generic;

namespace hoenggopen.Model
{
    public class Schedule
    {
        public Schedule()
        {
            Matches = new List<Match>();
        }
        public List<Match> Matches { get; set; }
    }
}