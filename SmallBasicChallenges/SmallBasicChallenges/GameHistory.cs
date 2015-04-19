using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallBasicChallenges
{

    /// <summary>
    /// History data
    /// </summary>
    public class GameHistory
    {
        /// <summary>
        /// Build an history
        /// </summary>
        public static GameHistory BuildHistory(GameSession session, GameData gameData)
        {
            if (session == null) throw new ArgumentNullException("session");
            var result = new GameHistory {
                Date = DateTime.Now,
                SessionID = session.SessionID,
                Session = new GameSession {
                    Game = session.Game,
                    Player1 = session.Player1,
                    Player2 = session.Player2,
                    SessionID = session.SessionID,
                    Status = session.Status,
                    StatusChanged = session.StatusChanged,
                    Winner = session.Winner
                }
            };
            if (gameData != null)
            {
                result.GameData = new GameData {
                    CurrentPlayer=gameData.CurrentPlayer,
                    CurrentTurn=gameData.CurrentTurn,
                    SessionID=gameData.SessionID
                };
                foreach (var kvp in gameData.Data)
                    result.GameData.Data[kvp.Key] = kvp.Value;
            }
            return result;
        }

        /// <summary>
        /// Date of the history
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// ID of the game session
        /// </summary>
        public String SessionID { get; set; }

        /// <summary>
        /// Data of the session
        /// </summary>
        public GameSession Session { get; set; }

        /// <summary>
        /// Data of the game
        /// </summary>
        public GameData GameData { get; set; }

    }

}
