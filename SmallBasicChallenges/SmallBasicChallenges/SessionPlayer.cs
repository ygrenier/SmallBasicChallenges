using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallBasicChallenges
{

    /// <summary>
    /// A player in a session
    /// </summary>
    public class SessionPlayer
    {
        /// <summary>
        /// The unique player session token
        /// </summary>
        public String PlayerToken { get; set; }
        /// <summary>
        /// ID player
        /// </summary>
        public String PlayerID { get; set; }
        /// <summary>
        /// The name of the player as his registered
        /// </summary>
        public String PlayerName { get; set; }
        /// <summary>
        /// IP Adress
        /// </summary>
        public String IpAddress { get; set; }
        /// <summary>
        /// The game type playing
        /// </summary>
        public String Game { get; set; }
        /// <summary>
        /// Number of the player
        /// </summary>
        public int PlayerNum { get; set; }
        /// <summary>
        /// The player status
        /// </summary>
        public SessionPlayerStatus Status { get; set; }
        /// <summary>
        /// Date of the player status changed last time
        /// </summary>
        public DateTime StatusChanged { get; set; }
    }

    /// <summary>
    /// The status of a session player
    /// </summary>
    public enum SessionPlayerStatus
    {
        /// <summary>
        /// The player is waiting a session
        /// </summary>
        Waiting,
        /// <summary>
        /// A session is opened but the player don't confirm the connection
        /// </summary>
        Connecting,
        /// <summary>
        /// The player is connected, waiting the game starting
        /// </summary>
        Connected,
        /// <summary>
        /// In play
        /// </summary>
        Playing,
        /// <summary>
        /// The player wins the game
        /// </summary>
        Winner,
        /// <summary>
        /// The player loose the game
        /// </summary>
        Looser,
        /// <summary>
        /// The player is aborted or was disconnected
        /// </summary>
        Aborted
    }

}
