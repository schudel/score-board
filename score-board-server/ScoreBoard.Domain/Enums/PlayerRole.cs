using System.Collections.Generic;

namespace ScoreBoard.Domain.Enums
{
    public class PlayerRole
    {
        private static readonly Dictionary<PlayerRoleEnum, PlayerRole> Dictionary = new Dictionary<PlayerRoleEnum, PlayerRole>();

        private PlayerRole(string name, PlayerRoleEnum playerRoleEnum)
        {
            Name = name;
            PlayerRoleEnum = playerRoleEnum;
            Dictionary.Add(playerRoleEnum, this);
        }

        public string Name { get; }
        public PlayerRoleEnum PlayerRoleEnum { get; }

        public static IList<PlayerRole> GetAllPlayerRoles
        {
            get
            {
                IList<PlayerRole> allPlayerRoles = new List<PlayerRole>();
                foreach (KeyValuePair<PlayerRoleEnum, PlayerRole> playerRole in Dictionary)
                {
                    allPlayerRoles.Add(playerRole.Value);
                }
                return allPlayerRoles;
            }
        }

        public static PlayerRole GetPlayerRole(PlayerRoleEnum playerRoleEnum)
        {
            foreach (var (key, playerRole) in Dictionary)
            {
                if (key == playerRoleEnum)
                {
                    return playerRole;
                }
            }
            return null;
        }

        public static PlayerRole GetPlayerRole(string playerRoleString)
        {
            foreach (var (_, playerRole) in Dictionary)
            {
                if (playerRole.Name == playerRoleString)
                {
                    return playerRole;
                }
            }
            return null;
        }

        public static readonly PlayerRole Guest = new PlayerRole("Guest", PlayerRoleEnum.Guest);
        public static readonly PlayerRole User = new PlayerRole("User", PlayerRoleEnum.User);
        public static readonly PlayerRole Admin = new PlayerRole("Admin", PlayerRoleEnum.Admin);
    }

    public enum PlayerRoleEnum
    {
        Guest = 0,
        Admin = 1,
        User = 2
    }
}
