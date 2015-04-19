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
            try
            {
                // If the game is finished we return null
                if (session.Status == GameSessionStatus.Finished || session.Status == GameSessionStatus.Aborted)
                    return null;
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
            catch (Exception ex)
            {
                return new FailedPlayingStatusResult {
                    Player = data.CurrentPlayer,
                    Turn = data.CurrentTurn,
                    Message = ex.GetBaseException().Message,
                    Status = "failed"
                };
            }
        }

        /// <summary>
        /// Internal play
        /// </summary>
        /// <remarks>
        /// Return null if the game is finished, the session need to have a winner defined, else the game is aborted
        /// </remarks>
        protected abstract GameStatusResult InternalPlay(SbcContext context, GameSession session, GameData data, string player, string command);

        /// <summary>
        /// Execute a play command
        /// </summary>
        public GameStatusResult Play(SbcContext context, GameSession session, GameData data, string player, string command)
        {
            if (context == null) throw new ArgumentNullException("context");
            if (session == null) throw new ArgumentNullException("session");

            // If the game is not playing, invalidoperation
            if (session.Status != GameSessionStatus.Playing)
                throw new InvalidOperationException("The game is not playing.");

            if (data == null)
                data = context.DataService.GetGameData(session.SessionID);

            var result = InternalPlay(context, session, data, player, command);
            if (result == null)
            {
                // Check the status to set aborted status if no winner defined
                if (session.Status == GameSessionStatus.Finished && session.Winner == 0)
                {
                    session.Status = GameSessionStatus.Aborted;
                }
            }

            // Save the datas
            context.DataService.Save(session);
            context.DataService.Save(data);
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
