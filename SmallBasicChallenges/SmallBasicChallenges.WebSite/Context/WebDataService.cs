using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallBasicChallenges.WebSite.Context
{
    /// <summary>
    /// Web Data Service for the Web
    /// </summary>
    public class WebDataService : IDataService
    {
        static MemoryDataService dataService = new MemoryDataService();

        public SessionPlayer FindActiveSessionPlayer(string idOrToken)
        {
            return dataService.FindActiveSessionPlayer(idOrToken);
        }

        public GameSession GetGameSessionFromPlayer(SessionPlayer player)
        {
            return dataService.GetGameSessionFromPlayer(player);
        }

        public GameData GetGameData(string sessionID)
        {
            return dataService.GetGameData(sessionID);
        }

        public SessionPlayer GetWaitingPlayer(string game, string playerID)
        {
            return dataService.GetWaitingPlayer(game, playerID);
        }

        public GameSession CreateGameSession(string game, SessionPlayer player1, SessionPlayer player2)
        {
            return dataService.CreateGameSession(game, player1, player2);
        }

        public SessionPlayer RegisterInWaitingList(string game, string playerID, string playerName, string ipAddress)
        {
            return dataService.RegisterInWaitingList(game, playerID, playerName, ipAddress);
        }

        public void AbortSession(GameSession session)
        {
            dataService.AbortSession(session);
        }

        public void Save(SessionPlayer player)
        {
            dataService.Save(player);
        }

        public void Save(GameSession game)
        {
            dataService.Save(game);
        }

        public void Save(GameData data)
        {
            dataService.Save(data);
        }

        public int GetActiveSessionsCount()
        {
            return dataService.GetActiveSessionsCount();
        }

        public int GetWaitingCount()
        {
            return dataService.GetWaitingCount();
        }

        public GameHistory SaveHistory(GameSession session, GameData gameData)
        {
            return dataService.SaveHistory(session, gameData);
        }

    }
}
