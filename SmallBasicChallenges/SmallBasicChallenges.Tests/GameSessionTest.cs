using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmallBasicChallenges.Tests
{
    public class GameSessionTest
    {

        [Fact]
        public void TestCreate()
        {
            var gs = new GameSession();
            Assert.Null(gs.Game);
            Assert.Null(gs.SessionID);
            Assert.Null(gs.Player1);
            Assert.Null(gs.Player2);
        }

        [Fact]
        public void TestGetPlayerFromID()
        {
            var gs = new GameSession();

            Assert.Null(gs.GetPlayerFromID("P1"));
            Assert.Null(gs.GetPlayerFromID("P2"));
            Assert.Null(gs.GetPlayerFromID("P3"));
            Assert.Null(gs.GetPlayerFromID(null));

            var p1 = new SessionPlayer {
                PlayerID = "p1",
                PlayerName = "Player 1"
            };
            gs.Player1 = p1;

            Assert.Same(p1, gs.GetPlayerFromID("P1"));
            Assert.Null(gs.GetPlayerFromID("P2"));
            Assert.Null(gs.GetPlayerFromID("P3"));
            Assert.Null(gs.GetPlayerFromID(null));

            var p2 = new SessionPlayer {
                PlayerID = "p2",
                PlayerName = "Player 2"
            };
            gs.Player2 = p2;

            Assert.Same(p1, gs.GetPlayerFromID("P1"));
            Assert.Same(p2, gs.GetPlayerFromID("P2"));
            Assert.Null(gs.GetPlayerFromID("P3"));
            Assert.Null(gs.GetPlayerFromID(null));

            gs.Player1 = null;

            Assert.Null(gs.GetPlayerFromID("P1"));
            Assert.Same(p2, gs.GetPlayerFromID("P2"));
            Assert.Null(gs.GetPlayerFromID("P3"));
            Assert.Null(gs.GetPlayerFromID(null));
        }

        [Fact]
        public void TestGetOpponent()
        {
            var gs = new GameSession();
            var p1 = new SessionPlayer {
                PlayerID = "p1",
                PlayerToken = "p1",
                PlayerName = "Player 1"
            };
            var p2 = new SessionPlayer {
                PlayerID = "p2",
                PlayerToken = "p2",
                PlayerName = "Player 2"
            };
            var p3 = new SessionPlayer {
                PlayerID = "p3",
                PlayerToken = "p3",
                PlayerName = "Player 3"
            };
            // p4 same token than p1
            var p4 = new SessionPlayer {
                PlayerID = "p4",
                PlayerToken = "p1",
                PlayerName = "Player 4"
            };
            // p5 same token than p2
            var p5 = new SessionPlayer {
                PlayerID = "p5",
                PlayerToken = "p2",
                PlayerName = "Player 5"
            };

            Assert.Null(gs.GetOpponent(p1));
            Assert.Null(gs.GetOpponent(p2));
            Assert.Null(gs.GetOpponent(p3));
            Assert.Null(gs.GetOpponent(p4));
            Assert.Null(gs.GetOpponent(p5));
            Assert.Null(gs.GetOpponent(null));

            gs.Player1 = p1;

            Assert.Null(gs.GetOpponent(p1));
            Assert.Null(gs.GetOpponent(p2));
            Assert.Null(gs.GetOpponent(p3));
            Assert.Null(gs.GetOpponent(p4));
            Assert.Null(gs.GetOpponent(p5));
            Assert.Null(gs.GetOpponent(null));

            gs.Player2 = p2;

            Assert.Same(p2, gs.GetOpponent(p1));
            Assert.Same(p1, gs.GetOpponent(p2));
            Assert.Null(gs.GetOpponent(p3));
            Assert.Same(p2, gs.GetOpponent(p4));
            Assert.Same(p1, gs.GetOpponent(p5));
            Assert.Null(gs.GetOpponent(null));

            gs.Player1 = null;

            Assert.Null(gs.GetOpponent(p1));
            Assert.Null(gs.GetOpponent(p2));
            Assert.Null(gs.GetOpponent(p3));
            Assert.Null(gs.GetOpponent(p4));
            Assert.Null(gs.GetOpponent(p5));
            Assert.Null(gs.GetOpponent(null));

        }

    }
}
