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
        public SbcContext(IDataService dataService)
        {
            if (dataService == null) throw new ArgumentNullException("dataService");
            this.DataService = dataService;
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
        /// Try to connect the player to a session
        /// </summary>
        public virtual GameSession ConnectPlayer(String playerName, String ipAddress, String game)
        {
            // Calculate the player ID : Name + gametype + IP
            String playerID = String.Format("{0}-{1}-{2}", CleanPlayerName(playerName), game, ipAddress);

            // Search an active session for the player
            var player = DataService.FindActiveSessionPlayerByPlayerID(playerID);

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
                    Status = SessionPlayerStatus.Waiting,
                    StatusChanged = DateTime.Now
                };

                // Create a new session game
                return DataService.CreateGameSession(game, opponent, player);
            }

            // Register the player in the wainting list
            DataService.RegisterInWaitingList(game, playerID, playerName, ipAddress);

            // The player is in the waiting list so we returns nothing
            return null;
        }

        /// <summary>
        /// Current dataservice
        /// </summary>
        public IDataService DataService { get; private set; }
    }

}
