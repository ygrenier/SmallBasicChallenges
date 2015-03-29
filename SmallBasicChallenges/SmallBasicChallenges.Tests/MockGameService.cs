﻿using System;
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

        protected override GameStatusResult InternalBuildStatusResult(SbcContext context, GameSession session, GameData data, String forPlayer)
        {
            return null;
        }

        protected override GameStatusResult InternalPlay(SbcContext context, GameSession session, GameData data, string player, string command)
        {
            return null;
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
