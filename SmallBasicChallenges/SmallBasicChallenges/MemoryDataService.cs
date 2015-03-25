using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallBasicChallenges
{

    /// <summary>
    /// Data service in the memory
    /// </summary>
    public class MemoryDataService : IDataService
    {
        List<SessionPlayer> _WaitingPlayers = new List<SessionPlayer>();
        List<SessionPlayer> _ActivePlayerSessions = new List<SessionPlayer>();
        Dictionary<String, GameSession> _GameSessions = new Dictionary<String, GameSession>();

        /// <summary>
        /// Find an active player session by the player ID
        /// </summary>
        public SessionPlayer FindActiveSessionPlayerByPlayerID(string playerID)
        {
            return _ActivePlayerSessions.Where(sp => String.Equals(playerID, sp.PlayerID, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }

        /// <summary>
        /// Get the game session of a player
        /// </summary>
        public GameSession GetGameSessionFromPlayer(SessionPlayer player)
        {
            return _GameSessions[player.GameSession];
        }

        /// <summary>
        /// Remove too old waiting player
        /// </summary>
        protected void CleanWaitingList()
        {
            // Clean the waiting list
            while (_WaitingPlayers.Count > 0 && _WaitingPlayers[0].StatusChanged < DateTime.Now.AddSeconds(-1))
                _WaitingPlayers.RemoveAt(0);
        }

        /// <summary>
        /// Search a waiting player for start a new session as an opponent with <paramref name="playerID"/>
        /// </summary>
        /// <param name="game">Game type</param>
        /// <param name="playerID">Player ID of the opponent</param>
        /// <returns>A player waiting or null</returns>
        public SessionPlayer GetWaitingPlayer(String game, String playerID)
        {
            CleanWaitingList();
            // Search a player
            return _WaitingPlayers
                .FirstOrDefault(p => 
                    String.Equals(game, p.Game, StringComparison.OrdinalIgnoreCase) 
                    && !String.Equals(playerID, p.PlayerID, StringComparison.OrdinalIgnoreCase)
                    );
        }

        /// <summary>
        /// Create a new game session from players
        /// </summary>
        /// <param name="player1">First player</param>
        /// <param name="player2">Second player</param>
        /// <returns>The new session</returns>
        public GameSession CreateGameSession(string game, SessionPlayer player1, SessionPlayer player2)
        {
            var result = new GameSession() {
                SessionID = Guid.NewGuid().ToString(),
                Player1 = player1,
                Player2 = player2,
                Game = game,
                Status = GameSessionStatus.Connecting,
                StatusChanged = DateTime.Now
            };
            if (player1.PlayerToken == null)
                player1.PlayerToken = Guid.NewGuid().ToString();
            if (player2.PlayerToken == null)
                player2.PlayerToken = Guid.NewGuid().ToString();
            player1.GameSession = result.SessionID;
            player1.PlayerNum = 1;
            player1.Game = game;
            player2.GameSession = result.SessionID;
            player2.PlayerNum = 2;
            player2.Game = game;
            if (!_ActivePlayerSessions.Contains(player1))
                _ActivePlayerSessions.Add(player1);
            if (!_ActivePlayerSessions.Contains(player2))
                _ActivePlayerSessions.Add(player2);
            _GameSessions[result.SessionID] = result;
            return result;
        }

        /// <summary>
        /// Register a player in the player list
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="playerID">ID of the player</param>
        /// <param name="playerName">The name of the player</param>
        /// <param name="ipAddress">The IP Address for security</param>
        /// <returns>The new session player</returns>
        public SessionPlayer RegisterInWaitingList(string game, string playerID, string playerName, string ipAddress)
        {
            var result = new SessionPlayer() {
                PlayerToken = null,
                PlayerID = playerID,
                PlayerName = playerName,
                IpAddress = ipAddress,
                Game = game,
                GameSession = null,
                PlayerNum = 0,
                Status = SessionPlayerStatus.Waiting,
                StatusChanged = DateTime.Now
            };
            _WaitingPlayers.Add(result);
            return result;
        }

        /// <summary>
        /// Abort a session
        /// </summary>
        /// <param name="session">Game session to abort</param>
        public void AbortSession(GameSession session)
        {
            if (session == null) throw new ArgumentNullException("session");
            _GameSessions.Remove(session.SessionID);
            _ActivePlayerSessions.Remove(session.Player1);
            _ActivePlayerSessions.Remove(session.Player2);
        }

        /// <summary>
        /// Returns the count of game session in play status
        /// </summary>
        public int GetActiveSessionsCount()
        {
            return _ActivePlayerSessions.Count;
        }

        /// <summary>
        /// Returns the count of waiting player
        /// </summary>
        public int GetWaitingCount()
        {
            CleanWaitingList();
            return _WaitingPlayers.Count;
        }

    }

}
