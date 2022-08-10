using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreateTournamentsForm : Form, IPrizeRequester , ITeamRequester
    {

        List<TeamModel> availableTeams = GlobalConfig.Connection.GetTeam_All();
        List<TeamModel> selectedTeams = new List<TeamModel>();
        List<PrizeModel> selectedPrizes = new List<PrizeModel>();


        public CreateTournamentsForm()
        {
            InitializeComponent();
            WireUpLists();
        }

        private void WireUpLists()
        {
            selectTeamDropDown.DataSource = null;
            selectTeamDropDown.DataSource = availableTeams;
            selectTeamDropDown.DisplayMember = "TeamName";

            tournamentTeamsListBox.DataSource = null;
            tournamentTeamsListBox.DataSource = selectedTeams;
            tournamentTeamsListBox.DisplayMember = "TeamName";

            prizesListBox.DataSource = null;
            prizesListBox.DataSource = selectedPrizes;
            prizesListBox.DisplayMember = "PlaceName";
        }


        public void PrizeComplete(PrizeModel model) // Get back the form a PrizeModel
        {

            // Take the PrizeModel and put it into our list of selected prizes
            selectedPrizes.Add(model);
            WireUpLists();
        }

        private void createPrizeButton_Click(object sender, EventArgs e)
        {
            // Call the CreatePrizeForm
            CreatePrizeForm prizeForm = new CreatePrizeForm(this);
            prizeForm.Show();

        }

        private void addTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel team = (TeamModel)selectTeamDropDown.SelectedItem;

            if (team != null)
            {
                availableTeams.Remove(team);
                selectedTeams.Add(team);

                WireUpLists();
            }
        }

        public void TeamComplete(TeamModel model)
        {
            selectedTeams.Add(model);
            WireUpLists();
        }

        private void createNewTeamLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CreateTeamForm teamForm = new CreateTeamForm(this);
            teamForm.Show();
        }

        private void removeSelectedPlayerButton_Click(object sender, EventArgs e)
        {
            TeamModel team = (TeamModel)tournamentTeamsListBox.SelectedItem;

            if (team != null)
            {
                selectedTeams.Remove(team);
                availableTeams.Add(team);

                WireUpLists();
            }
        }

        private void removeSelectedPrizeButton_Click(object sender, EventArgs e)
        {
            PrizeModel prize = (PrizeModel)prizesListBox.SelectedItem;

            if (prize !=null)
            {
                selectedPrizes.Remove(prize);

                WireUpLists();
            }
        }

        private void flowLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void createTournamentButton_Click(object sender, EventArgs e)
        {
            //Vaidate data
            decimal fee = 0;

            bool feeAceeptable = decimal.TryParse(entryFeeValue.Text, out fee);

            if (!feeAceeptable)
            {
                MessageBox.Show("You need to enter a valid Entry Fee.", 
                    "Invalid Fee", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Error);

                return;
            }
            // Create our tournament model
            TournamentModel tournamen = new TournamentModel();

            tournamen.TournamentName = tournamentNameValue.Text;
            tournamen.EntryFee = fee;
            
            tournamen.Prizes = selectedPrizes;
            tournamen.EnteredTeams = selectedTeams;

            // Wire our matchups
            TournamentLogic.CreateRounds(tournamen);

            // Create Tournament entry 
            // Create all of the prizes entries
            // Create all of the team entries 
            GlobalConfig.Connection.CreateTournament(tournamen);

        }
    }
}
