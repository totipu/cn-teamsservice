using Xunit;
using StatlerWaldorfCorp.TeamService.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;


namespace StatlerWaldorfCorp.TeamService
{
    public class TeamsControllerTest
    {
        [Fact]
        public void QueryTeamListReturnsCorrectTeams()
        {
            TeamsController controller = new TeamsController(new TestMemoryTeamRepository());

            var rawTeams =  (IEnumerable<Team>) (controller.GetAllTeams() as ObjectResult).Value;
            List<Team> teams = new List<Team>(rawTeams);            

            Assert.Equal(2, teams.Count);
            Assert.Equal("one", teams[0].Name);
            Assert.Equal("two", teams[1].Name);
        }

        [Fact]
        public void GetTeamRetrivesTeam()
        {    
            TeamsController controller = new TeamsController(new TestMemoryTeamRepository());

            string sampleName = "sample";
            Guid id = Guid.NewGuid();
            Team sampleTeam = new Team(sampleName, id);
            controller.CreateTeam(sampleTeam);
        
            Team retrivedTeam = (Team)(controller.GetTeam(id) as ObjectResult).Value;
            Assert.Equal(sampleName, retrivedTeam.Name);
            Assert.Equal(id, retrivedTeam.ID);
        
        }

        [Fact]
        public void GetNonExistentTeamReturnsNotFound()
        {
            TeamsController controller = new TeamsController(new TestMemoryTeamRepository());

            Guid id = Guid.NewGuid();
            var result = controller.GetTeam(id);
            Assert.True(result is NotFoundResult);        
        }

        [Fact]
        public void CreateTeamAddsTeamToList()
        {
            TeamsController controller = new TeamsController(new TestMemoryTeamRepository());
            var teams = (IEnumerable<Team>) 
                (controller.GetAllTeams() as ObjectResult).Value;
            List<Team> original = new List<Team>(teams);

            Team t = new Team("sample");
            var result = controller.CreateTeam(t);

            var newTeamsRaw = (IEnumerable<Team>) 
                (controller.GetAllTeams() as ObjectResult).Value;
            
            List<Team> newTeams = new List<Team>(newTeamsRaw);
            Assert.Equal(original.Count + 1, newTeams.Count);

            var sampleTeam = newTeams.FirstOrDefault(
                target => target.Name == "sample");
            Assert.NotNull(sampleTeam);
        }

        [Fact]
        public void UpdateTeamModifiesTeamToList()
        {
            TeamsController controller = new TeamsController(new TestMemoryTeamRepository());
            var teams = (IEnumerable<Team>)(controller.GetAllTeams() as ObjectResult).Value;
            List<Team> original = new List<Team>(teams);

            Guid id = Guid.NewGuid();
            Team t = new Team("sample", id);
            var result = controller.CreateTeam(t);

            Team newTeam = new Team("sample2", id);
            controller.UpdateTeam(newTeam, id);

            var newTeamsRaw = (IEnumerable<Team>)(controller.GetAllTeams() as ObjectResult).Value;
            List<Team> newTeams = new List<Team>(newTeamsRaw);
            var sampleTeam = newTeams.FirstOrDefault(target => target.Name == "sample");
            Assert.Null(sampleTeam);

            Team retrivedTeam = (Team)(controller.GetTeam(id) as ObjectResult).Value;
            Assert.Equal("sample2", retrivedTeam.Name);

        }

        [Fact]
        public void UpdateNonExistentTeamReturnsNotFound()
        {
            TeamsController controller = new TeamsController(new TestMemoryTeamRepository());
            var teams = (IEnumerable<Team>)(controller.GetAllTeams() as ObjectResult).Value;
            List<Team> original = new List<Team>(teams);

            Team someTeam = new Team("Some Team", Guid.NewGuid());
            controller.CreateTeam(someTeam);

            Guid newTeamId = Guid.NewGuid();
            Team newTeam = new Team("New Team", newTeamId);
            var result = controller.UpdateTeam(newTeam, newTeamId);

            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public void DeleteTeamRemovesFromList()
        {
            TeamsController controller = new TeamsController(new TestMemoryTeamRepository());
            var teams = (IEnumerable<Team>)(controller.GetAllTeams() as ObjectResult).Value;
            int ct = teams.Count();

            string sampleName = "sample";
            Guid id = Guid.NewGuid();
            Team sampleTeam = new Team(sampleName, id);
            controller.CreateTeam(sampleTeam);

            teams = (IEnumerable<Team>)(controller.GetAllTeams() as ObjectResult).Value;
            sampleTeam = teams.FirstOrDefault(target => target.Name == sampleName);
            Assert.NotNull(sampleTeam);

            controller.DeleteTeam(id);

            teams = (IEnumerable<Team>)(controller.GetAllTeams() as ObjectResult).Value;
            sampleTeam = teams.FirstOrDefault(target => target.Name == sampleName);
            Assert.Null(sampleTeam);
        }

        [Fact]
        public void DeleteNonExistentTeamReturnsNotFound()
        {
            TeamsController controller = new TeamsController(new TestMemoryTeamRepository());
            Guid id = Guid.NewGuid();

            var result = controller.DeleteTeam(id);
            Assert.True(result is NotFoundResult);
        }
    }
}