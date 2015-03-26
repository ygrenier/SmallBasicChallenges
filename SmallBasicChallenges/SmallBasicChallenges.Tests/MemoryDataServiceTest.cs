using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmallBasicChallenges.Tests
{
    public class MemoryDataServiceTest
    {

        [Fact]
        public void TestCreate()
        {
            var ds = new MemoryDataService();
            Assert.Equal(0, ds.GetWaitingCount());
            Assert.Equal(0, ds.GetActiveSessionsCount());
        }

        [Fact]
        public void TestWaitingList()
        {
            var ds = new MemoryDataService();

            Assert.Equal(0, ds.GetWaitingCount());
            Assert.Null(ds.GetWaitingPlayer("g1", "p1"));
            Assert.Null(ds.GetWaitingPlayer("g1", "p2"));
            Assert.Null(ds.GetWaitingPlayer("g2", "p1"));
            Assert.Null(ds.GetWaitingPlayer("g2", "p2"));

            var g1p1 = ds.RegisterInWaitingList("g1", "p1", "Player 1", "1");
            Assert.Equal(1, ds.GetWaitingCount());
            Assert.Null(ds.GetWaitingPlayer("g1", "p1"));
            Assert.Same(g1p1, ds.GetWaitingPlayer("g1", "p2"));
            Assert.Same(g1p1, ds.GetWaitingPlayer("g1", "p3"));
            Assert.Null(ds.GetWaitingPlayer("g2", "p1"));
            Assert.Null(ds.GetWaitingPlayer("g2", "p2"));
            Assert.Null(ds.GetWaitingPlayer("g2", "p3"));

            System.Threading.Thread.Sleep(300);

            var g1p2 = ds.RegisterInWaitingList("g1", "p2", "Player 2", "1");
            Assert.Equal(2, ds.GetWaitingCount());
            Assert.Same(g1p2, ds.GetWaitingPlayer("g1", "p1"));
            Assert.Same(g1p1, ds.GetWaitingPlayer("g1", "p2"));
            Assert.Same(g1p1, ds.GetWaitingPlayer("g1", "p3"));
            Assert.Null(ds.GetWaitingPlayer("g2", "p1"));
            Assert.Null(ds.GetWaitingPlayer("g2", "p2"));
            Assert.Null(ds.GetWaitingPlayer("g2", "p3"));

            System.Threading.Thread.Sleep(300);

            var g2p1 = ds.RegisterInWaitingList("g2", "p1", "Player 1", "1");
            var g1p1bis = ds.RegisterInWaitingList("g1", "p1", "Player 1", "1");
            Assert.Equal(4, ds.GetWaitingCount());
            Assert.Same(g1p2, ds.GetWaitingPlayer("g1", "p1"));
            Assert.Same(g1p1, ds.GetWaitingPlayer("g1", "p2"));
            Assert.Same(g1p1, ds.GetWaitingPlayer("g1", "p3"));
            Assert.Null(ds.GetWaitingPlayer("g2", "p1"));
            Assert.Same(g2p1, ds.GetWaitingPlayer("g2", "p2"));
            Assert.Same(g2p1, ds.GetWaitingPlayer("g2", "p3"));

            System.Threading.Thread.Sleep(300);

            var g2p2 = ds.RegisterInWaitingList("g2", "p2", "Player 2", "1");
            var g1p1ter = ds.RegisterInWaitingList("g1", "p1", "Player 1", "1");
            Assert.Equal(6, ds.GetWaitingCount());
            Assert.Same(g1p2, ds.GetWaitingPlayer("g1", "p1"));
            Assert.Same(g1p1, ds.GetWaitingPlayer("g1", "p2"));
            Assert.Same(g1p1, ds.GetWaitingPlayer("g1", "p3"));
            Assert.Same(g2p2, ds.GetWaitingPlayer("g2", "p1"));
            Assert.Same(g2p1, ds.GetWaitingPlayer("g2", "p2"));
            Assert.Same(g2p1, ds.GetWaitingPlayer("g2", "p3"));

            // Starting now the waiting list starting to decrease beacause timeout

            System.Threading.Thread.Sleep(300);
            // g1p1 is removed
            Assert.Equal(5, ds.GetWaitingCount());
            Assert.Same(g1p2, ds.GetWaitingPlayer("g1", "p1"));
            Assert.Same(g1p1bis, ds.GetWaitingPlayer("g1", "p2"));
            Assert.Same(g1p2, ds.GetWaitingPlayer("g1", "p3"));
            Assert.Same(g2p2, ds.GetWaitingPlayer("g2", "p1"));
            Assert.Same(g2p1, ds.GetWaitingPlayer("g2", "p2"));
            Assert.Same(g2p1, ds.GetWaitingPlayer("g2", "p3"));

            System.Threading.Thread.Sleep(300);
            // g1p2 is removed
            Assert.Equal(4, ds.GetWaitingCount());
            Assert.Null(ds.GetWaitingPlayer("g1", "p1"));
            Assert.Same(g1p1bis, ds.GetWaitingPlayer("g1", "p2"));
            Assert.Same(g1p1bis, ds.GetWaitingPlayer("g1", "p3"));
            Assert.Same(g2p2, ds.GetWaitingPlayer("g2", "p1"));
            Assert.Same(g2p1, ds.GetWaitingPlayer("g2", "p2"));
            Assert.Same(g2p1, ds.GetWaitingPlayer("g2", "p3"));

            System.Threading.Thread.Sleep(300);
            // g2p1 and g1p1bis are removed
            Assert.Equal(2, ds.GetWaitingCount());
            Assert.Null(ds.GetWaitingPlayer("g1", "p1"));
            Assert.Same(g1p1ter, ds.GetWaitingPlayer("g1", "p2"));
            Assert.Same(g1p1ter, ds.GetWaitingPlayer("g1", "p3"));
            Assert.Same(g2p2, ds.GetWaitingPlayer("g2", "p1"));
            Assert.Null(ds.GetWaitingPlayer("g2", "p2"));
            Assert.Same(g2p2, ds.GetWaitingPlayer("g2", "p3"));

            System.Threading.Thread.Sleep(300);
            // g2p2 and g1p1ter are removed
            Assert.Equal(0, ds.GetWaitingCount());
            Assert.Null(ds.GetWaitingPlayer("g1", "p1"));
            Assert.Null(ds.GetWaitingPlayer("g1", "p2"));
            Assert.Null(ds.GetWaitingPlayer("g1", "p3"));
            Assert.Null(ds.GetWaitingPlayer("g2", "p1"));
            Assert.Null(ds.GetWaitingPlayer("g2", "p2"));
            Assert.Null(ds.GetWaitingPlayer("g2", "p3"));

        }

        [Fact]
        public void TestGameSession()
        {
            var ds = new MemoryDataService();

            Assert.Null(ds.FindActiveSessionPlayer("p1"));

            SessionPlayer p1 = new SessionPlayer {
                PlayerID = "p1",
                PlayerName = "Player 1",
                IpAddress = "127.0.0.1"
            };
            SessionPlayer p2 = new SessionPlayer {
                PlayerID = "p2",
                PlayerName = "Player 2",
                IpAddress = "127.0.0.1"
            };

            var session = ds.CreateGameSession("game", p1, p2);

            Assert.NotNull(session.SessionID);
            Assert.NotEmpty(session.SessionID);
            Assert.Equal("game", session.Game);
            Assert.Same(p1, session.Player1);
            Assert.Same(p2, session.Player2);
            Assert.Equal(GameSessionStatus.Connecting, session.Status);

            Assert.NotNull(p1.PlayerToken);
            Assert.NotEmpty(p1.PlayerToken);
            Assert.Equal("game", p1.Game);
            Assert.Equal(session.SessionID, p1.GameSession);
            Assert.Equal(1, p1.PlayerNum);
            Assert.Equal(SessionPlayerStatus.Waiting, p1.Status);

            Assert.NotNull(p2.PlayerToken);
            Assert.NotEmpty(p2.PlayerToken);
            Assert.Equal("game", p2.Game);
            Assert.Equal(session.SessionID, p2.GameSession);
            Assert.Equal(2, p2.PlayerNum);
            Assert.Equal(SessionPlayerStatus.Waiting, p1.Status);

            Assert.Same(p1, ds.FindActiveSessionPlayer("p1"));
            Assert.Same(p2, ds.FindActiveSessionPlayer("p2"));

            Assert.Same(session, ds.GetGameSessionFromPlayer(p1));
            Assert.Same(session, ds.GetGameSessionFromPlayer(p2));

            ds.AbortSession(session);

            Assert.Null(ds.FindActiveSessionPlayer("p1"));
            Assert.Null(ds.FindActiveSessionPlayer("p2"));

            Assert.Throws<KeyNotFoundException>(() => ds.GetGameSessionFromPlayer(p1));
            Assert.Throws<KeyNotFoundException>(() => ds.GetGameSessionFromPlayer(p2));

            Assert.Throws<ArgumentNullException>(() => ds.AbortSession(null));

        }

    }
}
