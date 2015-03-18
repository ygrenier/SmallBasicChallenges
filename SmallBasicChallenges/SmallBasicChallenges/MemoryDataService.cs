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

        /// <summary>
        /// Find a player session by the player ID
        /// </summary>
        public SessionPlayer FindSessionPlayerByPlayerID(string playerID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get the game session of a player
        /// </summary>
        public GameSession GetGameSessionFromPlayer(SessionPlayer player)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Search a waiting player for start a new session as an opponent with <paramref name="playerID"/>
        /// </summary>
        /// <param name="game">Game type</param>
        /// <param name="playerID">Player ID of the opponent</param>
        /// <returns>A player waiting or null</returns>
        public SessionPlayer GetWaitingPlayer(String game, String playerID)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Create a new game session from players
        /// </summary>
        /// <param name="player1">First player</param>
        /// <param name="player2">Second player</param>
        /// <returns>The new session</returns>
        public GameSession CreateGameSession(string game, SessionPlayer player1, SessionPlayer player2)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

    }

}
