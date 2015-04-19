using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallBasicChallenges
{

    /// <summary>
    /// The context of Small Basic Context
    /// </summary>
    public class SbcContext
    {

        /// <summary>
        /// Create a new context
        /// </summary>
        public SbcContext(IDataService dataService, IGameService gameService = null)
        {
            if (dataService == null) throw new ArgumentNullException("dataService");
            this.DataService = dataService;
            this.GameService = gameService ?? DefaultGameService.Current;
        }

        /// <summary>
        /// Clean the player name for ID
        /// </summary>
        protected virtual String CleanPlayerName(String name)
        {
            if (String.IsNullOrWhiteSpace(name)) return String.Empty;
            name = name.Trim();
            StringBuilder result = new StringBuilder();
            foreach (var c in name.ToLower())
            {
                if (Char.IsLetterOrDigit(c))
                    result.Append(c);
            }
            return result.ToString();
        }

        /// <summary>
        /// Calculate the player ID
        /// </summary>
        public virtual String CalculatePlayerID(String playerName, String ipAddress, String game)
        {
            // Calculate the player ID : Name + gametype + IP
            return String.Format("{0}-{1}-{2}", CleanPlayerName(playerName), game, ipAddress);
        }

        /// <summary>
        /// Try to connect the player to a session
        /// </summary>
        public virtual GameSession ConnectPlayer(String playerName, String ipAddress, String game)
        {
            // Search the game engine
            var gEngine = GameService.FindGame(game);
            if (gEngine == null)
                throw new ArgumentException(String.Format("Unknown game : {0}", game));

            // Calculate the player ID : Name + gametype + IP
            String playerID = CalculatePlayerID(playerName, ipAddress, game);

            // Search an active session for the player
            var player = DataService.FindActiveSessionPlayer(playerID);

            // If we have a player, get the session game
            if (player != null && player.PlayerToken != null)
            {
                // Get the session and load all players
                var session = DataService.GetGameSessionFromPlayer(player);
                // Get the opponent
                var opponentPlayer = session.GetOpponent(player);
                // If the player is connecting, then he become connected
                if (player.Status == SessionPlayerStatus.Connecting)
                {
                    SetPlayerStatus(player, SessionPlayerStatus.Connected, true);
                }
                // If the two players are connected the game is ready to start
                if (player.Status == SessionPlayerStatus.Connected && opponentPlayer.Status == SessionPlayerStatus.Connected)
                {
                    SetGameSessionStatus(session, GameSessionStatus.Connected, true);
                }
                // If the game is always 'connecting' after timeout
                if (session.Status == GameSessionStatus.Connecting && session.StatusChanged.AddSeconds(5) < DateTime.Now)
                {
                    // Abort the session and the player sessions
                    DataService.AbortSession(session);
                    DataService.SaveHistory(session, DataService.GetGameData(session.SessionID)); 
                    player = null;
                    session = null;
                    opponentPlayer = null;
                }
                else
                {
                    DataService.SaveHistory(session, DataService.GetGameData(session.SessionID)); 
                }
                // Return the session
                if (session != null)
                    return session;
            }

            // Search an opponent in the waiting list
            var opponent = DataService.GetWaitingPlayer(game, playerID);

            // If we find an opponent
            if (opponent != null)
            {
                // Prepare opponent
                opponent.PlayerToken = Guid.NewGuid().ToString();
                opponent.Game=game;
                opponent.PlayerNum = 1;
                SetPlayerStatus(opponent, opponent.Status == SessionPlayerStatus.Waiting ? SessionPlayerStatus.Connecting : opponent.Status, false);

                // Create player
                player = new SessionPlayer() {
                    PlayerToken = Guid.NewGuid().ToString(),
                    PlayerID = playerID,
                    PlayerName = playerName,
                    IpAddress = ipAddress,
                    Game = game,
                    PlayerNum = 2,
                    Status = SessionPlayerStatus.Connecting,
                    StatusChanged = DateTime.Now
                };

                // Create a new session game
                var session = DataService.CreateGameSession(game, opponent, player);
                session.Player1.GameSession = session.SessionID;
                session.Player1.PlayerNum = 1;
                session.Player2.GameSession = session.SessionID;
                session.Player2.PlayerNum = 2;
                DataService.Save(session.Player1);
                DataService.Save(session.Player2);
                DataService.SaveHistory(session, null);
                return session;
            }

            // Register the player in the wainting list
            DataService.RegisterInWaitingList(game, playerID, playerName, ipAddress);

            // The player is in the waiting list so we returns nothing
            return null;
        }

        /// <summary>
        /// Find a game session from a player id or token
        /// </summary>
        public GameSession FindSessionFromPlayer(string idOrToken)
        {
            var player = DataService.FindActiveSessionPlayer(idOrToken);
            if (player != null)
                return DataService.GetGameSessionFromPlayer(player);
            return null;
        }

        /// <summary>
        /// Change the player session status
        /// </summary>
        public void SetPlayerStatus(SessionPlayer player, SessionPlayerStatus newStatus, bool save = true)
        {
            if (player == null) throw new ArgumentNullException("player");
            if (player.Status != newStatus)
            {
                player.Status = newStatus;
                player.StatusChanged = DateTime.Now;
                if (save)
                    DataService.Save(player);
            }
        }

        /// <summary>
        /// Change the game session status
        /// </summary>
        public void SetGameSessionStatus(GameSession session, GameSessionStatus newStatus, bool save = true)
        {
            if (session == null) throw new ArgumentNullException("session");
            if (session.Status != newStatus)
            {
                session.Status = newStatus;
                session.StatusChanged = DateTime.Now;
                if (save)
                    DataService.Save(session);
            }
        }

        /// <summary>
        /// Define the winner of a the session
        /// </summary>
        public void SetWinner(GameSession session, int winner)
        {
            // Wins then end the game
            session.Winner = winner;
            SetGameSessionStatus(session, GameSessionStatus.Finished, true);
        }

        /// <summary>
        /// Calculate the current status result
        /// </summary>
        public GameStatusResult GetStatusResult(GameSession session, String playerIdOrToken)
        {
            if (session == null) throw new ArgumentNullException("session");

            // Check if we require the game
            if (session.Status >= GameSessionStatus.Playing)
            {
                var game = GameService.FindGame(session.Game);
                var result = game.BuildStatusResult(this, session, DataService.GetGameData(session.SessionID), playerIdOrToken);
                if (result != null)
                    return result;
            }

            // Get the player
            var thisPlayer = session.GetPlayer(playerIdOrToken);

            // Get the opponent
            var opponent = session.GetOpponent(thisPlayer);

            // Depending the status session
            switch (session.Status)
            {
                case GameSessionStatus.Connected:
                case GameSessionStatus.Playing:
                    return new DefaultStatusResult {
                        Token = thisPlayer.PlayerToken,
                        PlayerNum = thisPlayer.PlayerNum,
                        Opponent = opponent.PlayerName,
                        Status = session.Status.ToString().ToLower()
                    };
                case GameSessionStatus.Finished:
                    return new DefaultStatusResult {
                        Token = thisPlayer.PlayerToken,
                        PlayerNum = thisPlayer.PlayerNum,
                        Opponent = opponent.PlayerName,
                        Status = session.Winner == thisPlayer.PlayerNum ? "winner" : "looser"
                    };
                case GameSessionStatus.Aborted:
                    //break;
                case GameSessionStatus.Connecting:
                default:
                    return new GameStatusResult { Status = session.Status.ToString().ToLower() };
            }
        }

        /// <summary>
        /// Get the data game of a session
        /// </summary>
        public GameData GetGameData(GameSession session)
        {
            if (session == null) throw new ArgumentNullException("session");
            if (session.Status == GameSessionStatus.Connecting) throw new InvalidOperationException("This session is connecting, no data available.");
            var gEngine = this.GameService.FindGame(session.Game);
            GameData data = DataService.GetGameData(session.SessionID);
            if (data == null)
            {
                data = new GameData {
                    SessionID = session.SessionID,
                    CurrentPlayer = 1,
                    CurrentTurn = 1
                };
                gEngine.InitializeSession(this, session, data);
                DataService.Save(data);
            }
            if (session.Status == GameSessionStatus.Connected)
            {
                SetGameSessionStatus(session, GameSessionStatus.Playing, true);
            }
            return data;
        }

        /// <summary>
        /// Execute a play command
        /// </summary>
        public GameStatusResult GamePlay(GameSession session, String player, String command)
        {
            if (session == null) throw new ArgumentNullException("session");

            var game = GameService.FindGame(session.Game);
            var gameData = DataService.GetGameData(session.SessionID);
            var result = game.Play(this, session, gameData, player, command);
            DataService.SaveHistory(session, gameData); 
            // No result !
            if (result == null)
            {
                // We returns the status
                return GetStatusResult(session, player);
            }
            return result;
        }

        /// <summary>
        /// Current data service
        /// </summary>
        public IDataService DataService { get; private set; }

        /// <summary>
        /// Current game service
        /// </summary>
        public IGameService GameService { get; private set; }
    }

}
