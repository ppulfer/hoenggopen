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
            int numberOfPlayers = 20,
            int numberOfGames = 4,
            int gameDurationInMinutes = 20)
        {
            Tournament tournament = new Tournament();
            tournament.Games.Add(new Game {Id = 1, Name = "PingPong"});
            tournament.Games.Add(new Game {Id = 2, Name = "Dart    "});
            tournament.Games.Add(new Game {Id = 3, Name = "BierPong"});
            tournament.Games.Add(new Game {Id = 4, Name = "GeoGuessr"});

            //Create Teams
            for (int i = 0; i < numberOfPlayers / 2; i++)
            {
                Team team = new Team();
                team.Id = i + 1;
                tournament.Teams.Add(team);
            }

            tournament.Schedule = new Schedule();

            var randomGame = new Random();

            foreach (var team in tournament.Teams)
            {
                foreach (var opTeam in tournament.Teams.Where(t => t.Id != team.Id))
                {
                    if (!tournament.Schedule.Matches.Exists(x => x.Team1 == opTeam && x.Team2 == team))
                    {
                        var match = new Match {Team1 = team, Team2 = opTeam};

                        tournament.Schedule.Matches.Add(match);
                    }
                }
            }

            ShuffleGamesAndTeams(tournament);

            DateTime startTime = new DateTime(2019, 6, 8, 11, 30, 0);
            var endTime = ScheduleMatches(numberOfGames, gameDurationInMinutes, startTime, tournament);

            Console.ReadLine();
        }

        private static DateTime ScheduleMatches(int numberOfGames, int gameDurationInMinutes, DateTime startTime,
            Tournament tournament)
        {
            var tempTime = startTime;
            while (true)
            {
                if (tournament.Schedule.Matches.All(x => x.Start != null))
                {
                    foreach (var team in tournament.Teams)
                    {
                        DateTime? lastMatchStart = null;
                        int breakCounter = 0;
                        //Check if team only has 3 Games and then break
                        foreach (var match in team.Matches.OrderBy(x => x.Start))
                        {
                            if (lastMatchStart == null)
                            {
                                lastMatchStart = match.Start;
                                continue;
                            }

                            var duration = match.Start - lastMatchStart;

                            if (duration != null && duration.Value.TotalMinutes > gameDurationInMinutes * 3)
                            {
                                breakCounter++;
                                //Double Game Break
                                Console.WriteLine($"{team} - More then Triple Game Break");

                                tempTime = startTime;
                                tournament.Schedule.Matches.ForEach(x => x.Start = null);
                                tournament.Schedule.Matches = tournament.Schedule.Matches.ShuffleList();
                            }

                            lastMatchStart = match.Start;
                        }

                        if (breakCounter > 1)
                        {
                            Console.WriteLine("Two many Breaks");

                            tempTime = startTime;
                            tournament.Schedule.Matches.ForEach(x => x.Start = null);
                            tournament.Schedule.Matches = tournament.Schedule.Matches.ShuffleList();
                        }
                    }

                    if (tournament.Schedule.Matches.All(x => x.Start != null))
                    {
                        break;
                    }
                }

                var teamsPlaying = new List<int>();
                for (int i = 1; i <= numberOfGames; i++)
                {
                    var match1 = tournament.Schedule.Matches.FirstOrDefault(x => x.Start == null
                                                                                 && x.Game.Id == i &&
                                                                                 !(teamsPlaying
                                                                                       .Contains(x.Team1.Id) ||
                                                                                   teamsPlaying.Contains(x.Team2.Id)
                                                                                     ));
                    if (match1 != null)
                    {
                        match1.Start = tempTime;
                        teamsPlaying.Add(match1.Team1.Id);
                        teamsPlaying.Add(match1.Team2.Id);
                    }
                }

                if (tempTime.Hour == 13 && tempTime.Minute == 00)
                {
                    tempTime = tempTime.AddMinutes(20);
                }

                tempTime = tempTime.AddMinutes(gameDurationInMinutes);
            }

            return tempTime;
        }

        private static void ShuffleGamesAndTeams(Tournament tournament)
        {
            while (true)
            {
                tournament.Schedule.Matches.ForEach(x => { x.Game = null; });
                foreach (var team in tournament.Teams)
                {
                    team.Matches.Clear();
                }

                tournament.Schedule.Matches = tournament.Schedule.Matches.ShuffleList();
                foreach (var game in tournament.Games)
                {
                    foreach (var match in tournament.Schedule.Matches)
                    {
                        if (match.Game != null)
                            continue;

                        if (match.Team1.Matches.Select(x => x.Game).Count(x => x.Name == game.Name) < 2
                            && match.Team2.Matches.Select(x => x.Game).Count(x => x.Name == game.Name) < 2)
                        {
                            match.Game = game;
                            match.Team1.Matches.Add(match);
                            match.Team2.Matches.Add(match);
                        }
                    }
                }

                if (tournament.Teams.Any(x => x.Matches.Count != 8))
                {
                    Console.WriteLine("Not all have 8 Matches");
                    continue;
                }
                    //Conditions
                if (tournament.Teams.Any(y =>
                    y.Matches.Select(k => k.Game).GroupBy(x => x.Id).Any(z => z.Count() != 2)))
                {
                    Console.WriteLine("...wrong!");
                }
                else
                {
                    var random = new Random();
                    while (tournament.Schedule.Matches.Any(x => x.Game == null))
                    {
                        foreach (var game in tournament.Games)
                        {
                            var match = tournament.Schedule.Matches.FirstOrDefault(x => x.Game == null);
                            if (match != null)
                            {
                                match.Game = game;
                                match.Team1.Matches.Add(match);
                                match.Team2.Matches.Add(match);
                            }
                        }
                    }

                    if(tournament.Schedule.Matches.All(match => match.Game != null))
                        break;
                }
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}