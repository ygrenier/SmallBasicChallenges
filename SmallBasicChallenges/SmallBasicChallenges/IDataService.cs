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
        /// Find a player session by the player ID
        /// </summary>
        SessionPlayer FindSessionPlayerByPlayerID(string playerID);

    }

}
