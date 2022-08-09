using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;
using System.Collections.Generic;
using System.Linq;
using System;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        private const string PrizesFile = "PrizeModels.csv";
        private const string PeopleFile = "PersonModels.csv";
        private const string TeamFile = "TeamModels.csv";
        private const string TournamentFile = "TournamentModels.csv";

        public PersonModel CreatePerson(PersonModel model)
        {
            List<PersonModel> people = PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();

            int currentId = 1;

            if (people.Count > 0)
            {
                currentId = people.OrderByDescending(x => x.Id).First().Id + 1;
            }

            model.Id = currentId;

            people.Add(model);
            people.SaveToPeopleFile(PeopleFile);

            return model;

        }

        // TODO - Wire up the CreatePrize for text files.
        public PrizeModel CreatePrize(PrizeModel model)
        {
            // 1) Load the text file
            // 2) Convert the text to List<PrizeModel>
            List<PrizeModel> prizes = PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

            // 3) Find the max ID
            int currentId = 1;

            if (prizes.Count > 0)
            {
                currentId = prizes
                    .OrderByDescending(x => x.Id)
                    .First().Id + 1;
            }
            model.Id = currentId;

            // 4) Add the new record with the new ID (max + 1)
            prizes.Add(model);

            // 5) Convert the prizes to list<string>
            // 6) Save the list<string> to the text file
            prizes.SaveToPrizeFile(PrizesFile);

            return model;
        }

        public List<PersonModel> GetPerson_All()
        {
            return PeopleFile.FullFilePath().LoadFile().ConvertToPersonModels();
        }
        public TeamModel CreateTeam(TeamModel model)
        {
            List<TeamModel> teams = TeamFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);

            int currentId = 1;

            if (teams.Count > 0)
            {
                currentId = teams
                    .OrderByDescending(x => x.Id)
                    .First().Id + 1;
            }

            model.Id = currentId;

            teams.Add(model);

            teams.SaveToTeamFile(TeamFile);

            return model;
        }

        public List<TeamModel> GetTeam_All()
        {
             return TeamFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);

        }

        public void CreateTournament(TournamentModel model)
        {
                List<TournamentModel> tournaments = TournamentFile
                .FullFilePath()
                .LoadFile()
                .ConvertToTournamentModels(TeamFile, PeopleFile, PrizesFile);

            int currentId = 1;

            if (tournaments.Count > 0)
            {
                currentId = tournaments
                    .OrderByDescending(x => x.Id)
                    .First().Id + 1;
            }

            model.Id = currentId;
            tournaments.Add(model);

            tournaments.SaveToTournamentFile(TournamentFile);
        }
    }
}
