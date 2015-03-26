using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallBasicChallenges.WebSite.Games
{

    /// <summary>
    /// A basic game for test
    /// </summary>
    class TestGame : GameEngine
    {

        /// <summary>
        /// Called when a new game session starting
        /// </summary>
        public override void InitializeSession(GameSession session)
        {
        }

        /// <summary>
        /// List of names
        /// </summary>
        public override IEnumerable<string> GetNames()
        {
            return new String[] { Name };
        }

        /// <summary>
        /// Name
        /// </summary>
        public override string Name
        {
            get { return "Test"; }
        }

    }

}
