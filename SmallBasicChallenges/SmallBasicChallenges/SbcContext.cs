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
                    player.Status = SessionPlayerStatus.Connected;
                    player.StatusChanged = DateTime.Now;
                }
                // If the two players are connected the game is ready to start
                if (player.Status == SessionPlayerStatus.Connected && opponentPlayer.Status == SessionPlayerStatus.Connected)
                {
                    gEngine.InitializeSession(session);
                    session.Status = GameSessionStatus.Connected;
                    session.StatusChanged = DateTime.Now;
                }
                // If the game is always 'connecting' after timeout
                if (session.Status == GameSessionStatus.Connecting && session.StatusChanged.AddSeconds(5) < DateTime.Now)
                {
                    // Abort the session and the player sessions
                    DataService.AbortSession(session);
                    player = null;
                    session = null;
                    opponentPlayer = null;
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
                opponent.Status = opponent.Status == SessionPlayerStatus.Waiting ? SessionPlayerStatus.Connecting : opponent.Status;
                opponent.StatusChanged = DateTime.Now;

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
        /// Current data service
        /// </summary>
        public IDataService DataService { get; private set; }

        /// <summary>
        /// Current game service
        /// </summary>
        public IGameService GameService { get; private set; }

    }

}
