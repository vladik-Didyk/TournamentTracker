using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class SQLConnector : IDataConnection
    {

        private const string db = "Tournaments";
        public PersonModel CreatePerson(PersonModel model)
        {
            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString(db)))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@FirstName", model.FirstName);
                parameters.Add("@LastName", model.LastName);
                parameters.Add("@EmailAddress", model.EmailAddress);
                parameters.Add("@CellphoneNumber", model.CellphoneNumber);
                parameters.Add("@id", 1,
                    dbType: DbType.Int32,
                    direction: ParameterDirection.Output);

                connection.Execute("dbo.spPeople_Insert",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                model.Id = parameters.Get<int>("@id");

                return model;
            }
        }

        // TODO - Make the CreatePrize method actually save to the database 
        /// <summary>
        /// Saves a new prize to the database
        /// </summary>
        /// <param name="model">The prize information.</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {
            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString(db)))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@PlaceNumber", model.PlaceNumber);
                parameters.Add("@PlaceName", model.PlaceName);
                parameters.Add("@PrizeAmount", model.PrizeAmount);
                parameters.Add("@PrizePercentage", model.PrizePercentage);
                parameters.Add("@id", 1,
                    dbType: DbType.Int32,
                    direction: ParameterDirection.Output);

                connection.Execute("dbo.spPrizes_Insert",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                model.Id = parameters.Get<int>("@id");

                return model;
            }
        }
        public List<PersonModel> GetPerson_All()
        {
            List<PersonModel> output;

            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<PersonModel>("dbo.spPeople_GetAll").ToList();
            }

            return output;
        }
        public TeamModel CreateTeam(TeamModel model)
        {

            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString(db)))
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@TeamName", model.TeamName);
                parameters.Add("@id", 1,
                    dbType: DbType.Int32,
                    direction: ParameterDirection.Output);

                connection.Execute("dbo.spTeams_Insert",
                    parameters,
                    commandType: CommandType.StoredProcedure);

                model.Id = parameters.Get<int>("@id");

                foreach (PersonModel teamMember in model.TeamMembers)
                {
                    parameters = new DynamicParameters();
                    parameters.Add("@TeamId", model.Id);
                    parameters.Add("@PersonId", teamMember.Id);

                    connection.Execute("dbo.spTeamMembers_Insert",
                        parameters,
                        commandType: CommandType.StoredProcedure);
                }
                return model;
            }
        }
        public List<TeamModel> GetTeam_All()
        {
            List<TeamModel> output;

            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString(db)))
            {
                output = connection.Query<TeamModel>("dbo.spTeam_GetAll").ToList();

                foreach (TeamModel team in output)
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@TeamId", team.Id);

                    team.TeamMembers = connection.Query<PersonModel>("dbo.spTeamMembers_GetByTeam",
                        parameters,
                        commandType: CommandType.StoredProcedure).ToList();
                }
            }

            return output;
        }
        public void CreateTournament(TournamentModel model)
        {
            using (IDbConnection connection = new SqlConnection(GlobalConfig.CnnString(db)))
            {
                SaveTournament(connection, model);
                SaveTournamentPrizes(connection, model);
                SaveTournamentEntries(connection, model);
            }
        }
        private void SaveTournament(IDbConnection connection, TournamentModel model)
        {
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("@TournamentsName", model.TournamentName);
            parameters.Add("@EntryFee", model.EntryFee);
            parameters.Add("@id", 0,
                dbType: DbType.Int32,
                direction: ParameterDirection.Output);

            connection.Execute("dbo.spTournaments_Insert",
                parameters,
                commandType: CommandType.StoredProcedure);

            model.Id = parameters.Get<int>("@id");
        }
        private void SaveTournamentPrizes(IDbConnection connection, TournamentModel model)
        {
            foreach (PrizeModel prize in model.Prizes)
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@TournamentId", model.Id);
                parameters.Add("@Prizeld", prize.Id);
                parameters.Add("@id", 0,
               dbType: DbType.Int32,
               direction: ParameterDirection.Output);

                connection.Execute("dbo.spTournamentPrizes_Insert",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }
        private void SaveTournamentEntries(IDbConnection connection, TournamentModel model)
        {
            foreach (TeamModel team in model.EnteredTeams)
            {
                DynamicParameters parameters = new DynamicParameters();
                parameters.Add("@TournamentId", model.Id);
                parameters.Add("@Teamld", team.Id);
                parameters.Add("@id", 0,
               dbType: DbType.Int32,
               direction: ParameterDirection.Output);

                connection.Execute("dbo.spTournamentEntries_Insert",
                    parameters,
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}
