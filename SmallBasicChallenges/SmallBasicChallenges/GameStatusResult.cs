using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmallBasicChallenges
{
    /// <summary>
    /// Class for the game status result
    /// </summary>
    public class GameStatusResult
    {
        /// <summary>
        /// Status
        /// </summary>
        public String Status { get; set; }
    }

    /// <summary>
    /// Class for the default result
    /// </summary>
    public class DefaultStatusResult : GameStatusResult
    {
        /// <summary>
        /// Player Token
        /// </summary>
        public String Token { get; set; }
        /// <summary>
        /// Player number
        /// </summary>
        public int PlayerNum { get; set; }
        /// <summary>
        /// Opponent name
        /// </summary>
        public String Opponent { get; set; }
    }

    /// <summary>
    /// Class for the default playing result
    /// </summary>
    public class DefaultPlayingStatusResult : GameStatusResult
    {
        /// <summary>
        /// Number of the current turn
        /// </summary>
        public int Turn { get; set; }
        /// <summary>
        /// Number of the player who need to play
        /// </summary>
        public int Player { get; set; }

    }
}
