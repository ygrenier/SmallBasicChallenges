using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallBasicChallenges.Tests
{
    public class MockGameService : GameEngine
    {

        public override void InitializeSession(SbcContext context, GameSession session, GameData data)
        {
        }

        public override IEnumerable<string> GetNames()
        {
            return new String[] { "Test1", "Test2" };
        }

        public override string Name
        {
            get { return "Test"; }
        }

    }
}
