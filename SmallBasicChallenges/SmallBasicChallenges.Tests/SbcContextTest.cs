using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SmallBasicChallenges.Tests
{
    public class SbcContextTest
    {

        [Fact]
        public void TestCreate()
        {
            var mocDataService = new Mock<IDataService>();
            var dataService = mocDataService.Object;
            var context = new SbcContext(dataService);
            Assert.Same(dataService, context.DataService);
            Assert.Throws<ArgumentNullException>(() => new SbcContext(null));
        }

        [Fact]
        public void TestConnect_AddInWaitingList()
        {
            var mocDataService = new Mock<IDataService>();
            var dataService = mocDataService.Object;
            var context = new SbcContext(dataService);

            var player = "Player 1";
            var ip = "127.0.0.1";
            var game = "test";

            // The ID calculated by the context
            var playerID = String.Format("{0}-{1}-{2}", "player1", game, ip);

            Assert.Null(context.ConnectPlayer(player, ip, game));

            mocDataService.Verify(ds => ds.FindActiveSessionPlayer(playerID), Times.Once());
            mocDataService.Verify(ds => ds.GetWaitingPlayer(game, playerID), Times.Once());
            mocDataService.Verify(ds => ds.RegisterInWaitingList(game, playerID, player, ip), Times.Once());


            // Same test with a player null
            player = null;
            ip = "127.0.0.1";
            game = "test";
            playerID = String.Format("{0}-{1}-{2}", "", game, ip);

            Assert.Null(context.ConnectPlayer(player, ip, game));

            mocDataService.Verify(ds => ds.FindActiveSessionPlayer(playerID), Times.Once());
            mocDataService.Verify(ds => ds.GetWaitingPlayer(game, playerID), Times.Once());
            mocDataService.Verify(ds => ds.RegisterInWaitingList(game, playerID, player, ip), Times.Once());

            var ex = Assert.Throws<ArgumentException>(() => context.ConnectPlayer(player, ip, "unknownGame"));
            Assert.Equal("Unknown game : unknownGame", ex.Message);
        }

        [Fact]
        public void TestConnect_WithOpponent()
        {
            var player1 = "Player 1";
            var player2 = "Player 2";
            var ip = "127.0.0.1";
            var game = "test";
            var playerID1 = String.Format("{0}-{1}-{2}", "player1", game, ip);
            var playerID2 = String.Format("{0}-{1}-{2}", "player2", game, ip);

            var mocDataService = new Mock<IDataService>();
            // Simulate player1 waiting when player2 try connect
            mocDataService.Setup(ds => ds.GetWaitingPlayer(game, playerID2)).Returns(() => new SessionPlayer() {
                Game = game,
                PlayerID = playerID1,
                PlayerName = player1,
                IpAddress = ip,
                Status = SessionPlayerStatus.Waiting,
                StatusChanged = DateTime.Now
            });
            // Simulate the game creation
            mocDataService.Setup(ds => ds.CreateGameSession(game, It.IsNotNull<SessionPlayer>(), It.IsNotNull<SessionPlayer>()))
                .Returns<String, SessionPlayer, SessionPlayer>((g, p1, p2) => new GameSession {
                    Game = g,
                    Player1 = p1,
                    Player2 = p2,
                    SessionID = "SESSION",
                    Status = GameSessionStatus.Connecting,
                    StatusChanged = DateTime.Now
                });
            var dataService = mocDataService.Object;
            var context = new SbcContext(dataService);

            // First connected so go to waiting list
            Assert.Null(context.ConnectPlayer(player1, ip, game));

            // Second have an opponent so a session is started
            var session = context.ConnectPlayer(player2, ip, game);

            Assert.NotNull(session);
            Assert.NotNull(session.Player1);
            Assert.NotNull(session.Player2);
            Assert.Equal("SESSION", session.SessionID);
        }

        [Fact]
        public void TestConnect_FullConnection()
        {
            var player1 = "Player 1";
            var player2 = "Player 2";
            var ip = "127.0.0.1";
            var game = "test";
            var playerID1 = String.Format("{0}-{1}-{2}", "player1", game, ip);
            var playerID2 = String.Format("{0}-{1}-{2}", "player2", game, ip);

            SessionPlayer ps1 = null, ps2 = null;
            GameSession gs = null;

            var mocDataService = new Mock<IDataService>();
            // Simulate the player session actives
            mocDataService.Setup(ds => ds.FindActiveSessionPlayer(It.IsAny<String>()))
                .Returns<String>(id => {
                    if (ps1 != null && ps1.PlayerID == id) return ps1;
                    if (ps2 != null && ps2.PlayerID == id) return ps2;
                    return null;
                });
            mocDataService.Setup(ds => ds.GetGameSessionFromPlayer(It.IsAny<SessionPlayer>()))
                .Returns<SessionPlayer>(p => {
                    return gs;
                });
            // Simulate player1 waiting when player2 try connect
            mocDataService.Setup(ds => ds.GetWaitingPlayer(game, playerID2)).Returns(() => ps1 = new SessionPlayer() {
                Game = game,
                PlayerID = playerID1,
                PlayerName = player1,
                IpAddress = ip,
                Status = SessionPlayerStatus.Waiting,
                StatusChanged = DateTime.Now
            });
            // Simulate the game creation
            mocDataService.Setup(ds => ds.CreateGameSession(game, It.IsNotNull<SessionPlayer>(), It.IsNotNull<SessionPlayer>()))
                .Returns<String, SessionPlayer, SessionPlayer>((g, p1, p2) => {
                    ps1 = p1;
                    ps2 = p2;
                    gs = new GameSession {
                        Game = g,
                        Player1 = p1,
                        Player2 = p2,
                        SessionID = "SESSION",
                        Status = GameSessionStatus.Connecting,
                        StatusChanged = DateTime.Now
                    };
                    ps1.PlayerToken = "PT1";
                    ps1.GameSession = gs.SessionID;
                    ps2.PlayerToken = "PT2";
                    ps2.GameSession = gs.SessionID;
                    return gs;
                });
            var dataService = mocDataService.Object;
            var context = new SbcContext(dataService);

            // First connected so go to waiting list
            Assert.Null(context.ConnectPlayer(player1, ip, game));

            // Second have an opponent so a session is started
            var session = context.ConnectPlayer(player2, ip, game);

            Assert.NotNull(session);
            Assert.Same(ps1, session.Player1);
            Assert.Same(ps2, session.Player2);
            Assert.Equal("Player 1", ps1.PlayerName);
            Assert.Equal("Player 2", ps2.PlayerName);

            // All is connecting
            Assert.Equal(SessionPlayerStatus.Connecting, ps1.Status);
            Assert.Equal(SessionPlayerStatus.Connecting, ps2.Status);
            Assert.Equal(GameSessionStatus.Connecting, gs.Status);

            // The second connect before the first
            Assert.Same(session, context.ConnectPlayer(player2, ip, game));
            
            // Now player to is connected, not the other
            Assert.Equal(SessionPlayerStatus.Connecting, ps1.Status);
            Assert.Equal(SessionPlayerStatus.Connected, ps2.Status);
            Assert.Equal(GameSessionStatus.Connecting, gs.Status);

            // The second connect twice
            Assert.Same(session, context.ConnectPlayer(player2, ip, game));

            // No change
            Assert.Equal(SessionPlayerStatus.Connecting, ps1.Status);
            Assert.Equal(SessionPlayerStatus.Connected, ps2.Status);
            Assert.Equal(GameSessionStatus.Connecting, gs.Status);

            // The first connect 
            Assert.Same(session, context.ConnectPlayer(player1, ip, game));

            // All are connected
            Assert.Equal(SessionPlayerStatus.Connected, ps1.Status);
            Assert.Equal(SessionPlayerStatus.Connected, ps2.Status);
            Assert.Equal(GameSessionStatus.Connected, gs.Status);

            // The first connect twice
            Assert.Same(session, context.ConnectPlayer(player1, ip, game));

            // All are connected
            Assert.Equal(SessionPlayerStatus.Connected, ps1.Status);
            Assert.Equal(SessionPlayerStatus.Connected, ps2.Status);
            Assert.Equal(GameSessionStatus.Connected, gs.Status);
        }

        [Fact]
        public void TestConnect_AbortedSession()
        {
            var player1 = "Player 1";
            var player2 = "Player 2";
            var ip = "127.0.0.1";
            var game = "test";
            var playerID1 = String.Format("{0}-{1}-{2}", "player1", game, ip);
            var playerID2 = String.Format("{0}-{1}-{2}", "player2", game, ip);

            SessionPlayer ps1 = new SessionPlayer() {
                Game = game,
                GameSession="SESSION",
                PlayerToken = "PT1",
                PlayerID = playerID1,
                PlayerName = player1,
                IpAddress = ip,
                Status = SessionPlayerStatus.Connecting,
                StatusChanged = DateTime.Now
            };
            SessionPlayer ps2 = new SessionPlayer() {
                Game = game,
                GameSession = "SESSION",
                PlayerToken = "PT2",
                PlayerID = playerID2,
                PlayerName = player2,
                IpAddress = ip,
                Status = SessionPlayerStatus.Connecting,
                StatusChanged = DateTime.Now
            };
            GameSession gs = new GameSession {
                Game = game,
                Player1 = ps1,
                Player2 = ps2,
                SessionID = "SESSION",
                Status = GameSessionStatus.Connecting,
                StatusChanged = DateTime.Now.AddSeconds(-6)
            };

            var mocDataService = new Mock<IDataService>();
            // Simulate the player session actives
            mocDataService.Setup(ds => ds.FindActiveSessionPlayer(It.IsAny<String>()))
                .Returns<String>(id => {
                    if (ps1 != null && ps1.PlayerID == id) return ps1;
                    if (ps2 != null && ps2.PlayerID == id) return ps2;
                    return null;
                });
            mocDataService.Setup(ds => ds.GetGameSessionFromPlayer(It.IsAny<SessionPlayer>()))
                .Returns<SessionPlayer>(p => {
                    return gs;
                });
            var dataService = mocDataService.Object;
            var context = new SbcContext(dataService);

            // Connect one
            Assert.Null(context.ConnectPlayer(player1, ip, game));

            // The abort method need to be called
            mocDataService.Verify(ds => ds.AbortSession(gs), Times.Once());

        }

        [Fact]
        public void TestConnect_FindSessionFromPlayer()
        {
            var player1 = "Player 1";
            var player2 = "Player 2";
            var player3 = "Player 3";
            var ip = "127.0.0.1";
            var game = "test";
            var playerID1 = String.Format("{0}-{1}-{2}", "player1", game, ip);
            var playerID2 = String.Format("{0}-{1}-{2}", "player2", game, ip);
            var playerID3 = String.Format("{0}-{1}-{2}", "player3", game, ip);

            SessionPlayer ps1 = new SessionPlayer() {
                Game = game,
                GameSession = "SESSION",
                PlayerToken = "PT1",
                PlayerID = playerID1,
                PlayerName = player1,
                IpAddress = ip,
                Status = SessionPlayerStatus.Connecting,
                StatusChanged = DateTime.Now
            };
            SessionPlayer ps2 = new SessionPlayer() {
                Game = game,
                GameSession = "SESSION",
                PlayerToken = "PT2",
                PlayerID = playerID2,
                PlayerName = player2,
                IpAddress = ip,
                Status = SessionPlayerStatus.Connecting,
                StatusChanged = DateTime.Now
            };
            SessionPlayer ps3 = new SessionPlayer() {
                Game = game,
                GameSession = "SESSION2",
                PlayerToken = "PT2",
                PlayerID = playerID3,
                PlayerName = player3,
                IpAddress = ip,
                Status = SessionPlayerStatus.Connecting,
                StatusChanged = DateTime.Now
            };
            GameSession gs = new GameSession {
                Game = game,
                Player1 = ps1,
                Player2 = ps2,
                SessionID = "SESSION",
                Status = GameSessionStatus.Connecting,
                StatusChanged = DateTime.Now.AddSeconds(-6)
            };

            var mocDataService = new Mock<IDataService>();
            // Simulate the player session actives
            mocDataService.Setup(ds => ds.FindActiveSessionPlayer(It.IsAny<String>()))
                .Returns<String>(id => {
                    if (ps1 != null && ps1.PlayerID == id) return ps1;
                    if (ps2 != null && ps2.PlayerID == id) return ps2;
                    if (ps3 != null && ps3.PlayerID == id) return ps3;
                    return null;
                });
            mocDataService.Setup(ds => ds.GetGameSessionFromPlayer(It.IsAny<SessionPlayer>()))
                .Returns<SessionPlayer>(p => {
                    if (gs.Player1 == p || gs.Player2 == p)
                        return gs;
                    return null;
                });
            var dataService = mocDataService.Object;
            var context = new SbcContext(dataService);

            Assert.Same(gs, context.FindSessionFromPlayer(playerID1));
            Assert.Same(gs, context.FindSessionFromPlayer(playerID2));
            Assert.Null(context.FindSessionFromPlayer(playerID3));
            Assert.Null(context.FindSessionFromPlayer("XXX"));

        }

    }
}
