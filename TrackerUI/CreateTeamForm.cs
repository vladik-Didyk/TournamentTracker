using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI
{
    public partial class CreateTeamForm : Form
    {

        private List<PersonModel> availableTeamMembers = GlobalConfig.Connection.GetPerson_All();
        private List<PersonModel> selectedTeamMembers = new List<PersonModel>();


        public CreateTeamForm()
        {
            InitializeComponent();

            //CreateSampleData();

            WireUpLists();
        }



        private void CreateSampleData()
        {
            availableTeamMembers.Add(new PersonModel { FirstName = "Sergay", LastName = "Kalenik" });
            availableTeamMembers.Add(new PersonModel { FirstName = "Alex", LastName = "Monako" });

            selectedTeamMembers.Add(new PersonModel { FirstName = "Thor", LastName = "Smith" });
            selectedTeamMembers.Add(new PersonModel { FirstName = "Warren", LastName = "Strom" });

        }

        private void WireUpLists()
        {
            selectTeamMemberDropDown.DataSource = null;

            selectTeamMemberDropDown.DataSource = availableTeamMembers;
            selectTeamMemberDropDown.DisplayMember = "FullName";

            teamMembersListBox.DataSource = null;

            teamMembersListBox.DataSource = selectedTeamMembers;
            teamMembersListBox.DisplayMember = "FullName";


        }

        private void teamOneScoreLabel_Click(object sender, EventArgs e)
        {

        }

        private void teamOneScoreValue_TextChanged(object sender, EventArgs e)
        {

        }

        private void createMemberButton_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                PersonModel person = new PersonModel();

                person.FirstName = firstNameValue.Text;
                person.LastName = lastNameValue.Text;
                person.EmailAddress = emailValue.Text;
                person.CellphoneNumber = cellPhoneValue.Text;

                person = GlobalConfig.Connection.CreatePerson(person);

                selectedTeamMembers.Add(person);

                WireUpLists();

                firstNameValue.Text = "";
                lastNameValue.Text = "";
                emailValue.Text = "";
                cellPhoneValue.Text = "";
            }
            else
            {
                MessageBox.Show("You need to fill in all of the fields.");
            }
        }

        private bool ValidateForm()
        {
            // TODO - Add validation to the form

            if (firstNameValue.Text.Length == 0)
            {
                return false;
            }

            if (lastNameValue.Text.Length == 0)
            {
                return false;
            }

            if (emailValue.Text.Length == 0)
            {
                return false;
            }

            if (cellPhoneLabel.Text.Length == 0)
            {
                return false;
            }

            return true;
        }

        private void addTeamButton_Click(object sender, EventArgs e)
        {
            PersonModel person = (PersonModel)selectTeamMemberDropDown.SelectedItem;

            if (person != null)
            {
                availableTeamMembers.Remove(person);
                selectedTeamMembers.Add(person);

                WireUpLists();
            }
        }

        private void removeSelectedMemberButton_Click(object sender, EventArgs e)
        {
            PersonModel person = (PersonModel)teamMembersListBox.SelectedItem;

            if (person != null)
            {
                availableTeamMembers.Add(person);
                selectedTeamMembers.Remove(person);

                WireUpLists();
            }


        }

        private void createTeamButton_Click(object sender, EventArgs e)
        {
            TeamModel newTeam = new TeamModel();

            newTeam.TeamName = tournamentNameValue.Text;
            newTeam.TeamMembers = selectedTeamMembers;

            newTeam = GlobalConfig.Connection.CreateTeam(newTeam);

            // TODO - If we aren't closing this from after creation, reset the form
        }
    }
}
