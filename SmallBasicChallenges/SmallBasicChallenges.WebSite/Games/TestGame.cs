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
        public override void InitializeSession(SbcContext context, GameSession session, GameData data)
        {
            var rnd = new Random();
            data.CurrentTurn = 1;
            data.CurrentPlayer = (rnd.Next(100) % 2) + 1;
            data.Data["NumberToFind"] = rnd.Next(100) + 1;
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
