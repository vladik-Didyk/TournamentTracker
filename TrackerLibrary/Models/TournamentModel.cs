﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models
{
    public class TournamentModel
    {
        /// <summary>
        /// The unique idetntifier for the tournament.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name given to this tournament.
        /// </summary>
        public string TournamentName { get; set; }
        /// <summary>
        /// The amount of money each team needs to put up to enter.
        /// </summary>
        public decimal EntryFee { get; set; }
        /// <summary>
        /// The set of teams that have been entered.
        /// </summary>
        public List<TeamModel> EnteredTeams { get; set; } = new List<TeamModel>();
        /// <summary>
        /// The list of prizes for the varios places.
        /// </summary>
        public List<PrizeModel> Prizes { get; set; } = new List<PrizeModel>();
        /// <summary>
        /// The matchups per round.
        /// </summary>
        public List<List<MatchupModel>> Rounds { get; set; } = new List<List<MatchupModel>>();

    }
}
