using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace ScoreBoard.Skills.Facts
{
    public class RankSorterFacts
    {
        [Fact]
        public void SortAlreadySortedFact()
        {
            IEnumerable<string> people = new[] { "One", "Two", "Three" };
            int[] ranks = { 1, 2, 3 };

            RankSorter.Sort(ref people, ref ranks);

            people.Should().BeEquivalentTo("One", "Two", "Three");
            ranks.Should().BeEquivalentTo(new[] {1, 2, 3});
        }

        [Fact]
        public void SortUnsortedFact()
        {
            IEnumerable<string> people = new[] { "Five", "Two1", "Two2", "One", "Four" };
            int[] ranks = { 5, 2, 2, 1, 4 };

            RankSorter.Sort(ref people, ref ranks);

            people.Should().BeEquivalentTo("One", "Two1", "Two2", "Four", "Five");
            ranks.Should().BeEquivalentTo(new[] {1, 2, 2, 4, 5});
        }
    }
}