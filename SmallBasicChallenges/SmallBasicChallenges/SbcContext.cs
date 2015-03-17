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

            // Search a session for the player
            var player = DataService.FindSessionPlayerByPlayerID(playerID);

            // If we have a player, get the session game
            if (player != null)
            {
                //var session = DataService.GetGameFromToken(player.PlayerToken);

            }

            throw new NotImplementedException();

        }

        /// <summary>
        /// Current dataservice
        /// </summary>
        public IDataService DataService { get; private set; }
    }

}
