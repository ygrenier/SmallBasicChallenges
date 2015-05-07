using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SmallBasicChallenges.WebSite.Context
{
    /// <summary>
    /// Web Data Service for the Web
    /// </summary>
    public class WebDataService : IDataService
    {
        static MemoryDataService dataService = new MemoryDataService();
        CloudStorageAccount _Account;
        CloudQueueClient _QueueClient;
        CloudQueue _QueueWaitingList;

        /// <summary>
        /// Create a new DataService
        /// </summary>
        public WebDataService()
        {
            try
            {
                _Account = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["DataService:StorageConnectionString"]);
            }
            catch (Exception)
            {
                _Account = CloudStorageAccount.DevelopmentStorageAccount;
            }
        }

        CloudQueueClient GetQueueClient()
        {
            if (_QueueClient == null)
            {
                _QueueClient = _Account.CreateCloudQueueClient();
            }
            return _QueueClient;
        }

        CloudQueue GetWaitingListQueue()
        {
            if (_QueueWaitingList == null)
            {
                var client = GetQueueClient();
                _QueueWaitingList = client.GetQueueReference("waitingplayers");
                _QueueWaitingList.CreateIfNotExists();
            }
            return _QueueWaitingList;
        }

        /// <summary>
        /// Search a Waiting Player
        /// </summary>
        public SessionPlayer GetWaitingPlayer(string game, string playerID)
        {
            var queue = GetWaitingListQueue();
            CloudQueueMessage message;
            while ((message = queue.GetMessage(TimeSpan.FromMilliseconds(200))) != null)
            {
                SessionPlayer player = JsonConvert.DeserializeObject<SessionPlayer>(message.AsString);
                queue.DeleteMessage(message);
                if (player.PlayerID != playerID)
                    return player;
            }
            return null;
        }

        /// <summary>
        /// Register a player as waiting
        /// </summary>
        public SessionPlayer RegisterInWaitingList(string game, string playerID, string playerName, string ipAddress)
        {
            SessionPlayer player = new SessionPlayer {
                Game = game,
                PlayerID = playerID,
                PlayerName = playerName,
                IpAddress = ipAddress
            };
            var queue = GetWaitingListQueue();
            CloudQueueMessage message = new CloudQueueMessage(JsonConvert.SerializeObject(player));
            queue.AddMessage(message, TimeSpan.FromSeconds(2));
            return player;
        }

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

        public GameSession CreateGameSession(string game, SessionPlayer player1, SessionPlayer player2)
        {
            return dataService.CreateGameSession(game, player1, player2);
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
