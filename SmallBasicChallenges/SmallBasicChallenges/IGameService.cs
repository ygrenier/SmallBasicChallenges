using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallBasicChallenges
{

    /// <summary>
    /// Service providing games
    /// </summary>
    public interface IGameService
    {

        /// <summary>
        /// Find a game by is name
        /// </summary>
        GameEngine FindGame(String name);

        /// <summary>
        /// Last all games available
        /// </summary>
        IEnumerable<GameEngine> GetGames();

    }

}
