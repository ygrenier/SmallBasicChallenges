using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallBasicChallenges
{

    /// <summary>
    /// Service proving data
    /// </summary>
    public interface IDataService
    {

        /// <summary>
        /// Find an active player session by the player ID
        /// </summary>
        SessionPlayer FindActiveSessionPlayerByPlayerID(string playerID);

        /// <summary>
        /// Get the game session of a player
        /// </summary>
        GameSession GetGameSessionFromPlayer(SessionPlayer player);

        /// <summary>
        /// Search a waiting player for start a new session as an opponent with <paramref name="playerID"/>
        /// </summary>
        /// <param name="game">Game type</param>
        /// <param name="playerID">Player ID of the opponent</param>
        /// <returns>A player waiting or null</returns>
        SessionPlayer GetWaitingPlayer(String game, String playerID);

        /// <summary>
        /// Create a new game session from players
        /// </summary>
        /// <param name="player1">First player</param>
        /// <param name="player2">Second player</param>
        /// <returns>The new session</returns>
        GameSession CreateGameSession(string game, SessionPlayer player1, SessionPlayer player2);

        /// <summary>
        /// Register a player in the player list
        /// </summary>
        /// <param name="game">Game</param>
        /// <param name="playerID">ID of the player</param>
        /// <param name="playerName">The name of the player</param>
        /// <param name="ipAddress">The IP Address for security</param>
        /// <returns>The new session player</returns>
        SessionPlayer RegisterInWaitingList(string game, string playerID, string playerName, string ipAddress);
    }

}
