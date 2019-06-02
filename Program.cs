using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using hoenggopen.Model;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace hoenggopen
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateTournament();


            CreateWebHostBuilder(args).Build().Run();
        }

        private static void CreateTournament(
            int numberOfPlayers = 24,
            int numberOfGames = 4,
            int gameDurationInMinutes = 15)
        {
            Tournament tournament = new Tournament();
            tournament.Games.Add(new Game {Name = "PingPong"});
            tournament.Games.Add(new Game {Name = "Dart"});
            tournament.Games.Add(new Game {Name = "BierPong"});
            tournament.Games.Add(new Game {Name = "GeoGuessr"});
            
            for (int i = 0; i < numberOfPlayers; i++)
            {
                var playerId = i + 1;
                Player player = new Player
                {
                    Id = playerId,
                    Name = "Player" + playerId
                };
                tournament.Participants.Add(player);    
            }

            var tempPlayers = tournament.Participants.ToList();
            
            //Create Teams
            for (int i = 0; i < numberOfPlayers/2; i++)
            {
                Team team = new Team();
                team.Id = i + 1;
                team.Name = "Team" + team.Id;
                team.Player1 = RemoveRandomPlayer(tempPlayers);
                team.Player2 = RemoveRandomPlayer(tempPlayers); 
                tournament.Teams.Add(team);
            }
            
            tournament.Schedule = new Schedule();

            foreach (var team in tournament.Teams)
            {
                foreach (var opTeam in tournament.Teams.Where(t => t.Id != team.Id))
                {
                    var match = new Match();
                    match.Team1 = team;
                    match.Team2 = opTeam;
                    tournament.Schedule.Matches.Add(match);
                }
            }
            
            
            
            
        }

        private static Player RemoveRandomPlayer(List<Player> tempPlayers)
        {
            var random = new Random(tempPlayers.Count);
            var randomNr = random.Next(0, tempPlayers.Count);
            var randomPLayer = tempPlayers[randomNr];
            tempPlayers.RemoveAt(randomNr);
            return randomPLayer;
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}