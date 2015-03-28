using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallBasicChallenges
{
    /// <summary>
    /// Data for a game engine
    /// </summary>
    public class GameData
    {

        /// <summary>
        /// Create a new game data
        /// </summary>
        public GameData()
        {
            Data = new Dictionary<String, Object>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Session corresponding to these data
        /// </summary>
        public String SessionID { get; set; }

        /// <summary>
        /// Number of the current player turn
        /// </summary>
        public int CurrentPlayer { get; set; }

        /// <summary>
        /// Number of the current play turn
        /// </summary>
        public int CurrentTurn { get; set; }

        /// <summary>
        /// Data of the game for the engine
        /// </summary>
        public IDictionary<String, Object> Data { get; private set; }

    }
}
