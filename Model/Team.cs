namespace hoenggopen.Model
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }

        public override string ToString()
        {
            return $"Team: {Name} Player1: {Player1} Player2: {Player2}";
        }
    }
}