using System.Collections;
using System.Collections.Generic;

namespace hoenggopen.Model
{
    public class Tournament
    {
        public Tournament()
        {
            Participants = new List<Player>();
            Teams = new List<Team>();
        }
        public Schedule Schedule { get; set; }
     
        public IList<Player> Participants { get; }
        public IList<Team> Teams { get; }
    }
}