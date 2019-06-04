using System;

namespace hoenggopen.Model
{
    public class Match
    {
        public DateTime? Start { get; set; }
        public Team Team1 { get; set; }
        public Team Team2 { get; set; }
        public Game Game { get; set; }

        public override string ToString()
        {
            return $"{Start?.ToShortTimeString()}\t{Team1} - {Team2}\t{Game}";
        }
    }
}