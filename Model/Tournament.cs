using System.Collections;
using System.Collections.Generic;

namespace hoenggopen.Model
{
    public class Tournament
    {
        public Tournament()
        {
            Teams = new List<Team>();
            Games = new List<Game>();
        }

        public Schedule Schedule { get; set; }

        public IList<Game> Games { get; }

        public IList<Team> Teams { get; }
    }
}