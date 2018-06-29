using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using StatlerWaldorfCorp.TeamService.Models;
using System.Threading.Tasks;
using StatlerWaldorfCorp.TeamService.Persistence;

namespace StatlerWaldorfCorp.TeamService
{
    [Route("[controller]")]
    public class TeamsController : Controller
    {
        ITeamRepository repository;

        public TeamsController(ITeamRepository repo)
        {
            repository = repo;
        }

        [HttpGet]
        public IActionResult GetAllTeams()
        {
            return this.Ok(repository.GetTeams());
        }

        [HttpPost]
        public IActionResult CreateTeam([FromBody]Team newTeam)
        {
            repository.AddTeam(newTeam);
            
            return this.Created($"/teams/{newTeam.ID}", newTeam);
        }

        [HttpGet("{id}")]
        public IActionResult GetTeam(Guid id)
        {
            Team team = repository.Get(id);

            if (team != null)
            {
                return this.Ok(team);
            } else {
                return this.NotFound();
            }            
        }

        [HttpPut("{id}")]
        public IActionResult UpdateTeam([FromBody]Team team, Guid id)
        {
            team.ID = id;

            if (repository.Update(team) == null) {
                return this.NotFound();
            } else {
                return this.Ok(team);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTeam(Guid id)
        {
            Team team = repository.Delete(id);

            if (team == null) {
                return this.NotFound();
            } else {
                return this.Ok(team.ID);
            }
        }
    }

}