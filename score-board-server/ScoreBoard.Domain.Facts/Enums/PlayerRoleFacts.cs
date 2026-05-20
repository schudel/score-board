using System.Collections.Generic;
using FluentAssertions;
using ScoreBoard.Domain.Enums;
using Xunit;

namespace ScoreBoard.Domain.Facts.Enums
{
    public class PlayerRoleFacts
    {
        [Fact]
        public void GetAllPlayerRolesFact()
        {
            IList<PlayerRole> allMatchStates = PlayerRole.GetAllPlayerRoles;
            allMatchStates.Should().HaveCount(3);
        }

        [Theory]
        [InlineData(PlayerRoleEnum.Admin, "Admin")]
        [InlineData(PlayerRoleEnum.User, "User")]
        [InlineData(PlayerRoleEnum.Guest, "Guest")]
        public void GetPlayerRoleByEnumFact(PlayerRoleEnum playerRoleEnum, string name)
        {
            PlayerRole playerRole = PlayerRole.GetPlayerRole(playerRoleEnum);
            playerRole.Name.Should().Be(name);
        }

        [Fact]
        public void GetPlayerRoleWithNullFact()
        {
            PlayerRole matchState = PlayerRole.GetPlayerRole(null);
            matchState.Should().BeNull();
        }

        [Theory]
        [InlineData("Admin", PlayerRoleEnum.Admin)]
        [InlineData("User", PlayerRoleEnum.User)]
        [InlineData("Guest", PlayerRoleEnum.Guest)]
        public void GetPlayerRoleByStringFact(string name, PlayerRoleEnum matchStateEnum)
        {
            PlayerRole playerRole = PlayerRole.GetPlayerRole(name);
            playerRole.PlayerRoleEnum.Should().Be(matchStateEnum);
        }

        [Fact]
        public void GetPlayerRoleWithEmptyStringFact()
        {
            PlayerRole playerRole = PlayerRole.GetPlayerRole("");
            playerRole.Should().BeNull();
        }
    }
}
