using Xunit;
using StatlerWaldorfCorp.TeamService.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using StatlerWaldorfCorp.TeamService.Persistence;
using StatlerWaldorfCorp.TeamService.LocationClient;

namespace StatlerWaldorfCorp.TeamService
{
    public class MembersControllerTest
    {
        [Fact]
        public void CreateMemberAddsMemberToTeam()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository, new MemoryLocationClient());

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestTeam", teamId);
            repository.AddTeam(team);

            Guid newMemberId = Guid.NewGuid();
            Member newMember = new Member(newMemberId);
            controller.CreateMember(newMember, teamId);

            Assert.True(team.Members.Contains(newMember));
        
        }

        [Fact]
        public void CreateMemberToNonexistantTeamReturnsNotFound()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository, new MemoryLocationClient());

            Guid teamId = Guid.NewGuid();            

            Guid newMemberId = Guid.NewGuid();
            Member newMember = new Member(newMemberId);
            var result = controller.CreateMember(newMember, teamId);

            Assert.True(result is NotFoundResult);        
        }
        
        [Fact]
        public void GetMembersReturnsMembers()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository, new MemoryLocationClient());

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestTeam", teamId);
            var debugTeam = repository.AddTeam(team);

            Guid firstMemberId = Guid.NewGuid();
            Member newMember = new Member("Marko", "Marković", firstMemberId);
            controller.CreateMember(newMember, teamId);

            Guid secondMemberId = Guid.NewGuid();
            newMember = new Member("Darko", "Darković", secondMemberId);
            controller.CreateMember(newMember, teamId);

            ICollection<Member> members = 
                (ICollection<Member>)(controller.GetMembers(teamId) as ObjectResult).Value;

            Assert.Equal(2, members.Count());
            Assert.NotNull(members.Where(m => m.ID == firstMemberId).First().ID);
            Assert.NotNull(members.Where(m => m.ID == secondMemberId).First().ID);
        }

        [Fact]
        public void GetMembersForNewTeamIsEmpty()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository, new MemoryLocationClient());

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestTeam", teamId);
            var debugTeam = repository.AddTeam(team);

            ICollection<Member> members = 
                (ICollection<Member>)(controller.GetMembers(teamId) as ObjectResult).Value;

            Assert.Empty(members);
        }

        [Fact]
        public void GetMembersForNonExistantTeamReturnsNotFound()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository, new MemoryLocationClient());

            Guid teamId = Guid.NewGuid();

            var result = controller.GetMembers(teamId);
            Assert.True(result is NotFoundResult);
        }

        // GetMember tests

        [Fact]
        public async void GetExistingMemberReturnsMember()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository, new MemoryLocationClient());

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestTeam", teamId);
            var debugTeam = repository.AddTeam(team);

            Guid memberId = Guid.NewGuid();
            Member newMember = new Member("Marko", "Marković", memberId);
            controller.CreateMember(newMember, teamId);

            Member member = (Member)(await controller.GetMember(teamId, memberId) as ObjectResult).Value;
            Assert.Equal(member.ID, newMember.ID);
        }

        [Fact]
        public async void GetNonExistantTeamReturnsNotFound()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository, new MemoryLocationClient());

            var result = await controller.GetMember(Guid.NewGuid(), Guid.NewGuid());
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public async void GetNonExistantMemberReturnsNotFound()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository, new MemoryLocationClient());

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestTeam", teamId);
            var debugTeam = repository.AddTeam(team);

            var result = await controller.GetMember(teamId, Guid.NewGuid());
            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public void UpdateMemberOverwrites()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository, new MemoryLocationClient());

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestTeam", teamId);
            var debugTeam = repository.AddTeam(team);

            Guid memberId = Guid.NewGuid();
            Member newMember = new Member("Marko", "Marković", memberId);
            controller.CreateMember(newMember, teamId);

            team = repository.Get(teamId);

            Member updatedMember = new Member(memberId);
            updatedMember.FirstName = "Darko";
            updatedMember.LastName = "Darković";
            controller.UpdateMember(updatedMember, teamId, memberId);

            team = repository.Get(teamId);
            Member testMember = team.Members.Where(m => m.ID == memberId).First();

            Assert.Equal("Darko", testMember.FirstName);
            Assert.Equal("Darković", testMember.LastName);            
        }

        [Fact]
        public void UpdateMembertoNonexistantMemberReturnsNoMatch()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MembersController controller = new MembersController(repository, new MemoryLocationClient());

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestTeam", teamId);
            var debugTeam = repository.AddTeam(team);

            Guid memberId = Guid.NewGuid();
            Member newMember = new Member("Marko", "Marković", memberId);
            controller.CreateMember(newMember, teamId);

            Guid nonMatchedGuid = Guid.NewGuid();
            Member updatedMember = new Member(memberId);
            updatedMember.FirstName = "Darko";
            updatedMember.LastName = "Darković";
            var result = controller.UpdateMember(updatedMember, teamId, nonMatchedGuid);

            Assert.True(result is NotFoundResult);
        }

        [Fact]
        public async void AddsLocation()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MemoryLocationClient locationClient = new MemoryLocationClient();
            MembersController controller = new MembersController(repository, locationClient);

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestTeam", teamId);
            var debugTeam = repository.AddTeam(team);

            Guid memberId = Guid.NewGuid();
            Member newMember = new Member("Marko", "Marković", memberId);
            controller.CreateMember(newMember, teamId);

            await locationClient.AddLocation(memberId, new LocationRecord() {
                Timestamp = 1, Altitude = 123.45f
            });

            Assert.True(locationClient.MemberLocationHistory.ContainsKey(memberId));
        }

        [Fact]
        public async void GetNewMemberReturnsMemberWithEmptyLastLocation()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MemoryLocationClient locationClient = new MemoryLocationClient();
            MembersController controller = new MembersController(repository, locationClient);

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestTeam", teamId);
            var debugTeam = repository.AddTeam(team);

            Guid memberId = Guid.NewGuid();
            Member newMember = new Member("Marko", "Marković", memberId);
            controller.CreateMember(newMember, teamId);

            LocatedMember member = 
                (LocatedMember)(await controller.GetMember(teamId, memberId) as ObjectResult).Value;

            Assert.Equal(member.ID, newMember.ID);
            Assert.Null(member.LastLocation);
        }

        [Fact]
        public async void GetMemberReturnsLastLocation()
        {
            ITeamRepository repository = new TestMemoryTeamRepository();
            MemoryLocationClient locationClient = new MemoryLocationClient();
            MembersController controller = new MembersController(repository, locationClient);

            Guid teamId = Guid.NewGuid();
            Team team = new Team("TestTeam", teamId);
            var debugTeam = repository.AddTeam(team);

            Guid memberId = Guid.NewGuid();
            Member newMember = new Member("Marko", "Marković", memberId);
            controller.CreateMember(newMember, teamId);

            await locationClient.AddLocation(memberId, new LocationRecord() {
                Timestamp = 1, Altitude = 123.45f
            });

            await locationClient.AddLocation(memberId, new LocationRecord() {
                Timestamp = 3, Altitude = 123.47f
            });

            await locationClient.AddLocation(memberId, new LocationRecord() {
                Timestamp = 2, Altitude = 123.46f
            });

            LocatedMember member = 
                (LocatedMember)(await controller.GetMember(teamId, memberId) as ObjectResult).Value;

            Assert.Equal(member.ID, newMember.ID);
            Assert.NotNull(member.LastLocation);
            Assert.Equal(123.47f, member.LastLocation.Altitude);
        }
    }    
}