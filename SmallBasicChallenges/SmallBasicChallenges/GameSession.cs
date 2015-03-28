using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallBasicChallenges
{

    /// <summary>
    /// A game session
    /// </summary>
    public class GameSession
    {
        /// <summary>
        /// Find a player from the id or the token
        /// </summary>
        public SessionPlayer GetPlayer(String idOrToken)
        {
            if (String.IsNullOrWhiteSpace(idOrToken)) return null;
            if (Player1 != null && (String.Equals(idOrToken, Player1.PlayerID, StringComparison.OrdinalIgnoreCase) || String.Equals(idOrToken, Player1.PlayerToken, StringComparison.OrdinalIgnoreCase)))
                return Player1;
            if (Player2 != null && (String.Equals(idOrToken, Player2.PlayerID, StringComparison.OrdinalIgnoreCase)|| String.Equals(idOrToken, Player2.PlayerToken, StringComparison.OrdinalIgnoreCase)))
                return Player2;
            return null;
        }

        /// <summary>
        /// Get the opponent of a player
        /// </summary>
        /// <param name="player">Player searching for the opponent</param>
        /// <returns>Returns the opponent, or null if the player is not in the game</returns>
        public SessionPlayer GetOpponent(SessionPlayer player)
        {
            if (player == null || Player1 == null || Player2 == null) return null;
            if (player == Player1 || player.PlayerToken == Player1.PlayerToken)
                return Player2;
            if (player == Player2 || player.PlayerToken == Player2.PlayerToken)
                return Player1;
            return null;
        }

        /// <summary>
        /// Unique session ID
        /// </summary>
        public String SessionID { get; set; }

        /// <summary>
        /// The first player
        /// </summary>
        public SessionPlayer Player1 { get; set; }

        /// <summary>
        /// The second player
        /// </summary>
        public SessionPlayer Player2 { get; set; }

        /// <summary>
        /// Type of the game
        /// </summary>
        public String Game { get; set; }

        /// <summary>
        /// Status of the session
        /// </summary>
        public GameSessionStatus Status { get; set; }

        /// <summary>
        /// Date of the player status changed last time
        /// </summary>
        public DateTime StatusChanged { get; set; }

        /// <summary>
        /// Number of the player wich wins the game
        /// </summary>
        /// <remarks>
        /// While the game is not finished, contains 0
        /// </remarks>
        public int Winner { get; set; }

    }

    /// <summary>
    /// The status for a game session
    /// </summary>
    public enum GameSessionStatus
    {
        /// <summary>
        /// The players are connecting to the session
        /// </summary>
        Connecting,
        /// <summary>
        /// The players are connected, the game is not started
        /// </summary>
        Connected,
        /// <summary>
        /// The game is running
        /// </summary>
        Playing,
        /// <summary>
        /// The game is finished
        /// </summary>
        Finished,
        /// <summary>
        /// One of the player is aborted or disconnected after a too long non response
        /// </summary>
        Aborted
    }

}
