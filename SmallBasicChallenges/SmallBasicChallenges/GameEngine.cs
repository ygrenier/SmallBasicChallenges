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
        /// Called when a new game session starting
        /// </summary>
        public abstract void InitializeSession(SbcContext context, GameSession session, GameData data);

        /// <summary>
        /// Called to create the build the game status result
        /// </summary>
        /// <remarks>
        /// Return null if the game is finished, the session need to have a winner defined, else the game is aborted
        /// </remarks>
        protected abstract GameStatusResult InternalBuildStatusResult(SbcContext context, GameSession session, GameData data, String forPlayer);

        /// <summary>
        /// Build the game status result for a player
        /// </summary>
        public GameStatusResult BuildStatusResult(SbcContext context, GameSession session, GameData data, String forPlayer)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (session == null) throw new ArgumentNullException("session");
            if (data == null)
                data = context.DataService.GetGameData(session.SessionID);
            var result = InternalBuildStatusResult(context, session, data, forPlayer);
            if (result == null)
            {
                // Check the status to set aborted status if no winner defined
                if (session.Status == GameSessionStatus.Finished && session.Winner == 0)
                {
                    session.Status = GameSessionStatus.Aborted;
                }
            }
            return result;
        }

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
