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
        /// Build a status result
        /// </summary>
        protected override GameStatusResult InternalBuildStatusResult(SbcContext context, GameSession session, GameData data, String forPlayer)
        {
            var player = session.GetPlayer(forPlayer);
            return new DefaultPlayingStatusResult {
                Turn = data.CurrentTurn,
                Player = data.CurrentPlayer,
                Status = data.CurrentPlayer == player.PlayerNum ? "play" : "waiting-opponent"
            };
        }

        /// <summary>
        /// Playing
        /// </summary>
        protected override GameStatusResult InternalPlay(SbcContext context, GameSession session, GameData data, string player, string command)
        {
            var ps = session.GetPlayer(player);

            // If invalid player return an error
            if (ps.PlayerNum != data.CurrentPlayer)
                throw new InvalidOperationException("Not your turn.");

            // Next turn
            data.CurrentTurn++;
            data.CurrentPlayer = ((data.CurrentPlayer + 1) % 2) + 1;

            // Save data
            context.DataService.Save(data);

            // Get the num
            var num = int.Parse(command);
            var r = num.CompareTo((int)data.Data["NumberToFind"]);
            if (r < 0)
            {
                return new GameStatusResult {
                    Status = "before"
                };
            }
            else if (r > 0)
            {
                return new GameStatusResult {
                    Status = "after"
                };
            }
            else
            {
                // Wins then end the game
                session.Winner = ps.PlayerNum;
                context.SetGameSessionStatus(session, GameSessionStatus.Finished, true);
                // Return null to end the game
                return null;
            }
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
