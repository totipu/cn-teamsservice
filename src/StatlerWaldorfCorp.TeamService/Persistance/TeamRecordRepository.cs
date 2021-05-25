using System;
using System.Linq;
using System.Collections.Generic;
using StatlerWaldorfCorp.TeamService.Models;
using Microsoft.EntityFrameworkCore;

namespace StatlerWaldorfCorp.TeamService.Persistence
{
    public class TeamRecordRepository : ITeamRepository
    {
        private TeamDbContext context;

        public TeamRecordRepository(TeamDbContext context)
        {
            this.context = context;
        }

        public IEnumerable<Team> GetTeams() {
            return this.context.Teams.Include(t => t.Members).ToList();
        }

        public Team Add (Team t) {
            this.context.Add(t);
            this.context.SaveChanges();
            return t;
        }

        public Team Get(Guid id)
        {
            return this.context.Teams.Include(t => t.Members).FirstOrDefault( t => t.ID == id);            
        }

        public Team Update (Team t) {
            this.context.Update(t);
            this.context.SaveChanges();
            
            return t;
        }

        public Team Delete(Guid id) {
            
            var q = this.context.Teams.Where(t => t.ID == id);
            Team team = null;

            if (q.Count() > 0) {
                team = q.First();
                this.context.Teams.Remove(team);
                this.context.SaveChanges();
            }

            return team;
        }
    }
    
}