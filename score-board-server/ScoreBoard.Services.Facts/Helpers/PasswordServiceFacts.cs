using System;
using FluentAssertions;
using ScoreBoard.Services.Helpers;
using Xunit;

namespace ScoreBoard.Services.Facts.Helpers
{
    public class PasswordServiceFacts
    {
        private readonly IPasswordService testee;

        public PasswordServiceFacts()
        {
            testee = new PasswordService();
        }

        [Fact]
        public void CreatePasswordHashPasswordNotNullFact()
        {
            Action action = () => testee.CreatePasswordHash(null, out _, out _);

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'password')");
        }

        [Fact]
        public void CreatePasswordHashPasswordNotWhitespaceFact()
        {
            Action action = () => testee.CreatePasswordHash("", out _, out _);

            action.Should().Throw<ArgumentException>().WithMessage("Value cannot be empty or whitespace only string. (Parameter 'password')");
        }

        [Fact]
        public void CreatePasswordHashPasswordMinimum8CharactersFact()
        {
            Action action = () => testee.CreatePasswordHash("1234567", out _, out _);

            action.Should().Throw<ArgumentException>().WithMessage("Value has less than 8 characters. (Parameter 'password')");
        }

        [Fact]
        public void CreatePasswordHashCorrectHashAndSaltSizeFact()
        {
            string password = "password";

            testee.CreatePasswordHash(password, out var passwordHash, out var passwordSalt);

            passwordHash.Should().HaveCount(64);
            passwordSalt.Should().HaveCount(128);
        }

        [Fact]
        public void VerifyPasswordHashPasswordNotNullFact()
        {
            Action action = () => testee.VerifyPasswordHash(null, new byte[] { }, new byte[] { });

            action.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'password')");
        }

        [Fact]
        public void VerifyPasswordHashPasswordNotWhitespaceFact()
        {
            Action action = () => testee.VerifyPasswordHash("", new byte[] { }, new byte[] { });

            action.Should().Throw<ArgumentException>().WithMessage("Value cannot be empty or whitespace only string. (Parameter 'password')");
        }

        [Fact]
        public void VerifyPasswordHashHashSizeShouldBe64Fact()
        {
            Action action = () => testee.VerifyPasswordHash("password", new byte[] { }, new byte[] { });

            action.Should().Throw<ArgumentException>().WithMessage("Invalid length of password hash (64 bytes expected). (Parameter 'storedHash')");
        }

        [Fact]
        public void VerifyPasswordHashSaltSizeShouldBe128Fact()
        {
            Action action = () => testee.VerifyPasswordHash("password", new[]
            {
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue
            }, new byte[] { });

            action.Should().Throw<ArgumentException>().WithMessage("Invalid length of password salt (128 bytes expected). (Parameter 'storedSalt')");
        }

        [Fact]
        public void VerifyPasswordHashIsFalseFact()
        {
            bool verified = testee.VerifyPasswordHash("password", new[]
            {
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue
            }, new[]
            {
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue,
                byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue, byte.MinValue
            });

            verified.Should().Be(false);
        }

        [Fact]
        public void VerifyPasswordHashIsTrueFact()
        {
            bool verified = testee.VerifyPasswordHash("password", new byte[]
            {
                225, 35, 30, 182, 216, 239, 159, 16, 
                221, 118, 205, 96, 98, 5, 55, 93, 
                31, 16, 41, 48, 250, 224, 100, 201, 
                230, 31, 55, 176, 255, 207, 2, 139, 
                202, 152, 39, 90, 168, 40, 230, 237, 
                136, 53, 111, 75, 155, 123, 129, 224, 
                207, 185, 149, 214, 204, 17, 48, 155, 
                242, 179, 139, 184, 232, 83, 235, 243
            }, new byte[]
            {
                161, 54, 110, 201, 208, 125, 218, 159, 
                226, 244, 65, 140, 150, 78, 72, 174, 
                182, 117, 155, 69, 137, 168, 189, 210,
                89, 156, 15, 115, 188, 196, 40, 110, 
                38, 193, 201, 120, 125, 110, 15, 213, 
                88, 179, 89, 34, 145, 105, 226, 136, 
                27, 124, 53, 148, 139, 210, 99, 32, 
                109, 184, 221, 175, 145, 147, 108, 237, 
                28, 138, 83, 126, 13, 245, 255, 69, 
                103, 5, 137, 69, 235, 44, 127, 155, 
                72, 140, 179, 222, 236, 248, 0, 162, 
                74, 130, 112, 219, 84, 225, 59, 104, 
                50, 170, 216, 238, 233, 62, 82, 181, 
                144, 213, 250, 222, 239, 235, 154, 71, 
                118, 178, 162, 231, 209, 184, 175, 69, 
                89, 228, 214, 89, 97, 232, 9, 205
            });

            verified.Should().Be(true);
        }
    }
}
