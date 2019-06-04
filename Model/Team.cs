using System.Collections.Generic;

namespace hoenggopen.Model
{
    public class Team
    {
        public Team()
        {
            Matches = new List<Match>();
        }

        public int Id { get; set; }

        public IList<Match> Matches { get; }

        public override string ToString()
        {
            return $"Team: {Id}";
        }
    }
}