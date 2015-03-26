using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmallBasicChallenges.Tests
{
    public class DefaultGameServiceTest
    {

        [Fact]
        public void TestCurrent()
        {
            var curr = DefaultGameService.Current;
            Assert.NotNull(curr);
            Assert.Same(curr, DefaultGameService.Current);
        }

        [Fact]
        public void TestRegisterEnginesFromAssemblies()
        {
            var dgs = new DefaultGameService();

            dgs.RegisterEnginesFromAssemblies(null);
            dgs.RegisterEnginesFromAssemblies(new System.Reflection.Assembly[] { this.GetType().Assembly });

            Assert.Equal(1, dgs.GetGames().Count());

            Assert.IsType<MockGameService>(dgs.GetGames().Single());
        }

        [Fact]
        public void TestGames()
        {
            var dgs = new DefaultGameService();

            Assert.Equal(1, dgs.GetGames().Count());

        }

        [Fact]
        public void TestFindGame()
        {
            var dgs = new DefaultGameService();

            Assert.IsType<MockGameService>(dgs.FindGame("test"));
            Assert.IsType<MockGameService>(dgs.FindGame("Test1"));
            Assert.IsType<MockGameService>(dgs.FindGame("TEST2"));
            Assert.Null(dgs.FindGame("test3"));

        }

    }
}
