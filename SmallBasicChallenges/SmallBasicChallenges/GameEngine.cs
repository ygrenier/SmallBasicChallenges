using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallBasicChallenges
{

    /// <summary>
    /// Abstract game engine
    /// </summary>
    public abstract class GameEngine
    {

        /// <summary>
        /// List all names of the engine
        /// </summary>
        public abstract IEnumerable<String> GetNames();

        /// <summary>
        /// Name
        /// </summary>
        public abstract string Name { get; }

    }

}
