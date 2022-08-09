using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.Models;


namespace TrackerLibrary.DataAccess.TextHelpers
{
    public static class TextConnectorProcessor
    {
        public static string FullFilePath(this string fileName)
        {
            return $"{ConfigurationManager.AppSettings["filePath"] }\\{fileName}";
        }

        // * 1) Load the text file 
        public static List<string> LoadFile(this string file)
        {
            if (!File.Exists(file))
            {
                return new List<string>();
            }

            return File.ReadAllLines(file).ToList();
        }

        // * 2) Convert the text to List<PrizeModel> 
        public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
        {
            List<PrizeModel> output = new List<PrizeModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                PrizeModel p = new PrizeModel();
                p.Id = int.Parse(cols[0]);
                p.PlaceNumber = int.Parse(cols[1]);
                p.PlaceName = (cols[2]);
                p.PrizeAmount = decimal.Parse(cols[3]);
                p.PrizePercentage = double.Parse(cols[4]);
                output.Add(p);
            }

            return output;
        }

        public static List<PersonModel> ConvertToPersonModels(this List<string> lines)
        {

            List<PersonModel> output = new List<PersonModel>();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                PersonModel person = new PersonModel();

                person.Id = int.Parse(cols[0]);
                person.FirstName = cols[1];
                person.LastName = cols[2];
                person.EmailAddress = cols[3];
                person.CellphoneNumber = cols[4];
                output.Add(person);
            }

            return output;
        }

        // 5) Convert the prizes to list<string>
        // 6) Save the list<string> to the text file
        public static void SaveToPrizeFile(this List<PrizeModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (PrizeModel p in models)
            {
                lines.Add($"{p.Id}," +
                    $" {p.PlaceNumber}, " +
                    $"{p.PlaceName}, " +
                    $"{p.PrizeAmount}, " +
                    $"{p.PrizePercentage}");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }


        public static void SaveToPeopleFile(this List<PersonModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (PersonModel p in models)
            {
                lines.Add($"{p.Id }, " +
                    $"{p.FirstName}, " +
                    $"{p.LastName}, " +
                    $"{p.EmailAddress}, " +
                    $"{p.CellphoneNumber}");
            }

            File.AppendAllLines(fileName.FullFilePath(), lines);
        }

        public static List<TeamModel> ConvertToTeamModels(this List<string> lines, string peopleFileName)
        {
            //id,team name, list of ids separated by the pipe
            // 3, Tim's, 1|3|5

            List<TeamModel> output = new List<TeamModel>();
            List<PersonModel> people = peopleFileName.FullFilePath().LoadFile().ConvertToPersonModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                TeamModel team = new TeamModel();
                team.Id = int.Parse(cols[0]);
                team.TeamName = cols[1];

                string[] personIds = cols[2].Split('|');

                foreach (string id in personIds)
                {
                    team.TeamMembers.Add(people
                        .Where(x => x.Id == int.Parse(id))
                        .FirstOrDefault()
                        );
                }

                output.Add(team);
            }

            return output;
        }

        public static List<TournamentModel> ConvertToTournamentModels(
            this List<string> lines,
            string teamFileName,
            string peopleFileName,
            string prizeFileName)
        {
            // id = 0
            // TournamentName = 1
            // EntryFee = 2
            // EntredTeams = 3
            // Prizes = 4
            // Rounds = 5

            //id, TournamentName, EntryFee,
            //(id|id|id - Entered Teams),
            //(id|id|id - Prizes),
            //(Rounds - id^id^id|id^id^id|id^id^id)

            List<TournamentModel> output = new List<TournamentModel>();
            List<TeamModel> teams = teamFileName.FullFilePath().LoadFile().ConvertToTeamModels(peopleFileName);
            List<PrizeModel> prizes = prizeFileName.FullFilePath().LoadFile().ConvertToPrizeModels();

            foreach (string line in lines)
            {
                string[] cols = line.Split(',');

                TournamentModel tournament = new TournamentModel();
                tournament.Id = int.Parse(cols[0]);
                tournament.TournamentName = (cols[1]);
                tournament.EntryFee = int.Parse(cols[2]);

                string[] teamIds = cols[3].Split('|');

                foreach (string id in teamIds)
                {
                    tournament.EnteredTeams.Add(teams
                        .Where(x => x.Id == int.Parse(id))
                        .First());
                }

                string[] prizeIds = cols[4].Split('|');

                foreach (string id in prizeIds)
                {
                    tournament.Prizes.Add(prizes
                        .Where(x => x.Id == int.Parse(id))
                        .First());
                }

                // TODO - Capture Rounds information

                output.Add(tournament);
            }

            return output;
        }


        public static void SaveToTournamentFile(this List<TournamentModel> models, string fileName)
        {
            // id = 0
            // TournamentName = 1
            // EntryFee = 2
            // EntredTeams = 3
            // Prizes = 4
            // Rounds = 5

            List<string> lines = new List<string>();

            foreach (TournamentModel tournament in models)
            {
                lines.Add($@"{tournament.Id}, 
                   {tournament.TournamentName},
                    {tournament.EntryFee},
                    {ConvertTeamListToString(tournament.EnteredTeams)},
                    {ConvertPrizeListToString(tournament.Prizes)},
                    {ConvertRoundListToString(tournament.Rounds)}");
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        public static string ConvertTeamListToString(List<TeamModel> teams)
        {
            string output = "";

            if (teams.Count == 0)
            {
                return "";
            }

            foreach (TeamModel team in teams)
            {
                output += $"{team.Id}|";
            }
            output = output.Substring(0, output.Length - 1);

            return output;
        }
        public static string ConvertRoundListToString(List<List<MatchupModel>> rounds)
        {
            //(Rounds - id^id^id|id^id^id|id^id^id)

            string output = "";

            if (rounds.Count == 0)
            {
                return "";
            }

            foreach (List<MatchupModel> round in rounds)
            {
                output += $"{ConvertMatchupListToString(round)}|";
            }
            output = output.Substring(0, output.Length - 1);

            return output;
        }

        public static string ConvertMatchupListToString(List<MatchupModel> matchups)
        {
            string output = "";

            if (matchups.Count == 0)
            {
                return "";
            }

            foreach (MatchupModel matchup in matchups)
            {
                output += $"{matchup.Id}^";
            }
            output = output.Substring(0, output.Length - 1);

            return output;

        }

        public static string ConvertPrizeListToString(List<PrizeModel> prizes)
        {
            string output = "";

            if (prizes.Count == 0)
            {
                return "";
            }

            foreach (PrizeModel prize in prizes)
            {
                output += $"{prize.Id}|";
            }
            output = output.Substring(0, output.Length - 1);

            return output;
        }

        public static void SaveToTeamFile(this List<TeamModel> models, string fileName)
        {
            List<string> lines = new List<string>();

            foreach (TeamModel team in models)
            {
                lines.Add($"{team.Id}, " +
                    $"{team.TeamName}, " +
                    $"{ConvertPeopleListToString(team.TeamMembers)}"
                    );
            }

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }

        private static string ConvertPeopleListToString(List<PersonModel> people)
        {
            string output = "";

            if (people.Count == 0)
            {
                return "";
            }

            foreach (PersonModel person in people)
            {
                output += $"{person.Id}|";
            }
            output = output.Substring(0, output.Length - 1);

            return output;
        }
    }
}
