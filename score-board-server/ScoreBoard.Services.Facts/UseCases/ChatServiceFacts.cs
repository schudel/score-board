using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ScoreBoard.Domain.Models;
using ScoreBoard.Fakes;
using ScoreBoard.Fakes.Infrastructure.Repositories;
using ScoreBoard.Services.UseCases;
using Xunit;

namespace ScoreBoard.Services.Facts.UseCases
{
    public class ChatServiceFacts
    {
        private readonly IChatService testee;

        public ChatServiceFacts()
        {
            testee = new ChatService(new ChatRepositoryFake());
            testee.Initialize("");
        }

        [Fact]
        public async Task GetByExistingIdFact()
        {
            string id = FakeData.Chat1Id;
            Chat chat = await testee.GetById(id).ConfigureAwait(false);

            chat.Id.Should().Be(Guid.Parse(id));
        }

        [Fact]
        public async Task GetByNoValidIdFact()
        {
            await Task.Delay(0);
            const string id = "NotAValidId";
            Func<Task> f = async () => await testee.GetById(id).ConfigureAwait(false);

            f.Should().Throw<Exception>();
        }

        [Fact]
        public async Task GetByNonExistingIdFact()
        {
            string id = "40F3089B-1219-4AB6-B371-B612BB6CF0C0";
            Chat chat = await testee.GetById(id).ConfigureAwait(false);

            chat.Should().BeNull();
        }

        [Fact]
        public async Task GetAllFact()
        {
            ICollection<Chat> chats = await testee.GetAll().ConfigureAwait(false);

            chats.Should().NotBeNull();
            chats.Should().HaveCount(3);
        }

        [Fact]
        public async Task AddFact()
        {
            long initCount = await testee.Count().ConfigureAwait(false);
            await testee.Add(new Chat { Id = Guid.NewGuid(), Message = "A Message"});
            long newCount = await testee.Count().ConfigureAwait(false);

            initCount.Should().Be(newCount - 1);
        }

        [Fact]
        public async Task AddNullAsGameFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.Add(null).ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("Chat is required.");
        }

        [Fact]
        public async Task UpdateFact()
        {
            string id = FakeData.Chat1Id;
            Chat chat = await testee.GetById(id).ConfigureAwait(false);
            const string message = "A New Message";
            chat.Message = message;
            await testee.Update(id, chat).ConfigureAwait(false);
            chat = await testee.GetById(id).ConfigureAwait(false);

            chat.Message.Should().Be(message);
        }

        [Fact]
        public async Task UpdateNoValidIdFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.Update("NoValidId", new Chat()).ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("No valid Id: \"NoValidId\"");
        }

        [Fact]
        public async Task UpdateNonExistingIdFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.Update("0EF28727-EBA4-46C1-B5C7-3B2A5F300106", new Chat()).ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("Chat not found");
        }

        [Fact]
        public async Task RemoveByIdFact()
        {
            long initCount = await testee.Count().ConfigureAwait(false);
            await testee.Remove(FakeData.Chat1Id);
            long newCount = await testee.Count().ConfigureAwait(false);

            initCount.Should().Be(newCount + 1);
        }

        [Fact]
        public async Task RemoveNoValidIdFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.Remove("NotAValidId").ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("No valid Id: \"NotAValidId\"");
        }

        [Fact]
        public async Task RemoveNoValidChatFact()
        {
            await Task.Delay(0);
            Chat chat = null;
            // ReSharper disable once ExpressionIsAlwaysNull
            Func<Task> f = async () => await testee.Remove(chat).ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("Chat is required.");
        }

        [Fact]
        public async Task RemoveByChatFact()
        {
            long initCount = await testee.Count().ConfigureAwait(false);
            Chat chat = await testee.GetById(FakeData.Chat1Id).ConfigureAwait(false);
            await testee.Remove(chat);
            long newCount = await testee.Count().ConfigureAwait(false);

            initCount.Should().Be(newCount + 1);
        }

        [Fact]
        public async Task CountFact()
        {
            long initCount = await testee.Count().ConfigureAwait(false);

            initCount.Should().Be(3);
        }

        [Fact]
        public async Task GetByPlayerIdFact()
        {
            ICollection<Chat> chats = await testee.GetByPlayerId(FakeData.AdminId).ConfigureAwait(false);

            chats.Should().HaveCount(1);
            chats.ElementAt(0).Id.Should().Be(FakeData.Chat1Id);
        }

        [Fact]
        public async Task GetByPlayerIdWithNoValidIdFact()
        {
            await Task.Delay(0);
            Func<Task> f = async () => await testee.GetByPlayerId("NotAValidId").ConfigureAwait(false);

            f.Should().Throw<Exception>().WithMessage("No valid Id: \"NotAValidId\"");
        }

        [Fact]
        public async Task GetByPlayerIdWithNonExistingIdFact()
        {
            ICollection<Chat> chats = await testee.GetByPlayerId("0EF28727-EBA4-46C1-B5C7-3B2A5F300106").ConfigureAwait(false);

            chats.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData(1, 0)]
        [InlineData(2, 0)]
        [InlineData(3, 0)]
        [InlineData(1, 1)]
        [InlineData(2, 1)]
        [InlineData(1, 2)]
        public async Task GetSpecificEntriesFact(int amount, int skip)
        {
            ICollection<Chat> chats = await testee.GetSpecificEntries(amount, skip).ConfigureAwait(false);

            chats.Should().HaveCount(amount);
        }
    }
}
