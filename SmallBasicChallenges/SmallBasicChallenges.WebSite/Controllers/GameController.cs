using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SmallBasicChallenges.WebSite.Controllers
{
    /// <summary>
    /// Game controller
    /// </summary>
    [RoutePrefix("Game")]
    public class GameController : Controller
    {
        #region Nested types
        class Game
        {
            public String GameID { get; set; }
            public String GameType { get; set; }
            public String GameStatus { get; set; }
            public String Player1 { get; set; }
            public String Player2 { get; set; }
        }
        class GamePlayer
        {
            public String PlayerToken { get; set; }
            public String PlayerID { get; set; }
            public String PlayerName { get; set; }
            public int PlayerNum { get; set; }
            public String GameID { get; set; }
            public String PlayerStatus { get; set; }
        }
        class GameTest
        {
            public String GameID { get; set; }
            public int NumberToFind { get; set; }
            public int TurnCount { get; set; }
            public int TurnPlayer { get; set; }
            public int Winner { get; set; }
        }
        #endregion
        #region Game management
        static Dictionary<String, Game> _Games = new Dictionary<string, Game>();
        static Dictionary<String, GamePlayer> _Players = new Dictionary<string, GamePlayer>();
        static List<GamePlayer> _Waiting = new List<GamePlayer>();
        GamePlayer GetPlayer(String token)
        {
            return _Players[token];
        }
        Game FindGame(String gameType, String playerId)
        {
            var gp = _Players.Values.Where(p => p.PlayerID == playerId).FirstOrDefault();
            if (gp == null) return null;
            return _Games.Values.Where(g => g.GameType == gameType && (g.Player1 == gp.PlayerToken || g.Player2 == gp.PlayerToken)).FirstOrDefault();
        }
        GamePlayer FindOpponent(String gameType, String playerID)
        {
            return _Waiting.Where(w => w.PlayerID != playerID && w.GameID == gameType).FirstOrDefault();
        }
        void RegisterPlayerWaiting(string gameType, string playerID, string player)
        {
            var gp = _Waiting.Where(w => w.PlayerID == playerID && w.GameID == gameType).FirstOrDefault();
            if (gp == null)
            {
                _Waiting.Add(new GamePlayer {
                    PlayerID = playerID,
                    PlayerName = player,
                    PlayerStatus = "waiting",
                    GameID = gameType
                });
            }
        }
        Game CreateNewGame(string game, GamePlayer player1, GamePlayer player2)
        {

            var result = new Game {
                GameID = Guid.NewGuid().ToString(),
                GameStatus = "connecting",
                GameType = game
            };

            player1.GameID = result.GameID;
            player1.PlayerToken = Guid.NewGuid().ToString();
            player1.PlayerNum = 1;
            player1.PlayerStatus = "connecting";

            player2.GameID = result.GameID;
            player2.PlayerToken = Guid.NewGuid().ToString();
            player2.PlayerNum = 2;
            player2.PlayerStatus = "connecting";

            result.Player1 = player1.PlayerToken;
            result.Player2 = player2.PlayerToken;

            _Players[player1.PlayerToken] = player1;
            _Players[player2.PlayerToken] = player2;
            _Games[result.GameID] = result;

            return result;

        }
        GamePlayer CreateNewPlayer(string playerID, string player)
        {
            return new GamePlayer {
                PlayerID=playerID,
                PlayerToken=Guid.NewGuid().ToString(),
                PlayerName=player
            };
        }
        static Dictionary<String, GameTest> _TestGames = new Dictionary<string, GameTest>();
        GameTest GetTestGame(string gameId)
        {
            GameTest result;
            if (!_TestGames.TryGetValue(gameId, out result))
            {
                var rnd=new Random();
                result = new GameTest {
                    GameID = gameId,
                    NumberToFind = rnd.Next(100) + 1,
                    TurnCount = 0,
                    TurnPlayer = 1,
                    Winner = 0
                };
                _TestGames[result.GameID] = result;
            }
            return result;
        }
        #endregion
        #region Helpers
        /// <summary>
        /// Escape values
        /// </summary>
        static String EscapeSmallBasicArrayString(String s)
        {
            if (!String.IsNullOrWhiteSpace(s))
            {
                return s.Replace("\\", "\\\\")
                    .Replace("=", "\\=")
                    .Replace(";", "\\;")
                    .Replace("\"", "\\\"")
                    ;
            }
            return s;
        }

        /// <summary>
        /// Encode an objet to a Small Basic Array String
        /// </summary>
        static String EncodeToSmallBasicArrayString(Object obj, bool lowerCaseKey = true)
        {
            if (obj == null) return String.Empty;
            // Basic type ?
            if (!obj.GetType().IsClass || obj is String)
                return obj.ToString();
            IDictionary<String, String> dic = obj as IDictionary<String, String>;
            // Not a dictionary, build it the object properties
            if (dic == null)
            {
                dic = new Dictionary<String, String>();
                foreach (var property in obj.GetType().GetProperties())
                    dic[property.Name] = EncodeToSmallBasicArrayString(property.GetValue(obj), lowerCaseKey);
            }
            // Encode
            StringBuilder result = new StringBuilder();
            foreach (var kv in dic)
            {
                result.AppendFormat("{0}={1};", EscapeSmallBasicArrayString(lowerCaseKey ? kv.Key.ToLower() : kv.Key), EscapeSmallBasicArrayString(kv.Value));
            }
            // Return result
            return result.ToString();
        }

        /// <summary>
        /// Clean the player name for ID
        /// </summary>
        static String CleanPlayerName(String name)
        {
            if (String.IsNullOrWhiteSpace(name)) return String.Empty;
            name = name.Trim();
            StringBuilder result = new StringBuilder();
            foreach (var c in name.ToLower())
            {
                if (Char.IsLetterOrDigit(c))
                    result.Append(c);
            }
            return result.ToString();
        }
        #endregion

        static MemoryDataService dataService = new MemoryDataService();

        ActionResult GameResult(Object content)
        {
            if (Request.IsAjaxRequest())
                return Json(content, JsonRequestBehavior.AllowGet);
            return Content(EncodeToSmallBasicArrayString(content));
        }

        ActionResult GameFailed(String message)
        {
            return GameResult(new {
                result = "failed",
                message = message
            });
        }

        //// GET: Game
        //public ActionResult Index()
        //{
        //    return View();
        //}

        /// <summary>
        /// Try to connect a player
        /// </summary>
        /// <param name="game">Wich game the player want to play</param>
        /// <param name="player">Name of the player</param>
        [Route("Connect")]
        [OutputCache(NoStore = true, Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult Connect(String game, String player)
        {
            try
            {
                // Create the context
                var context = new SbcContext(dataService);

                // Try to connect the player
                var session = context.ConnectPlayer(player, this.Request.UserHostAddress, game);


                // Encode the playerId : Name + gametype + IP
                String playerID = String.Format("{0}-{1}-{2}", CleanPlayerName(player), game, this.Request.UserHostAddress);

                // Player is in a game ?
                var gameInfo = FindGame(game, playerID);
                if (gameInfo != null)
                {
                    // Connect the players
                    var p1 = GetPlayer(gameInfo.Player1);
                    var p2 = GetPlayer(gameInfo.Player2);
                    var pInfo = p1.PlayerID == playerID ? p1 : p2;
                    if (pInfo.PlayerStatus == "connecting")
                        pInfo.PlayerStatus = "connected";
                    // If the two players are connected then the game can start
                    if (p1.PlayerStatus == "connected" && p2.PlayerStatus == "connected")
                        gameInfo.GameStatus="play";
                    return GameResult(new { 
                        token = pInfo.PlayerToken,
                        playernum = pInfo.PlayerNum,
                        opponent = pInfo==p1 ? p2.PlayerName : p1.PlayerName,
                        result = pInfo.PlayerStatus
                    });
                }

                // An opponent is available ?
                var opponent = FindOpponent(game, playerID);
                if (opponent != null)
                {
                    // Create a player
                    var playerInfo = CreateNewPlayer(playerID, player);
                    // Create a new game
                    gameInfo = CreateNewGame(game, playerInfo, opponent);
                    // While opponent no confirmed where connecting
                    return GameResult(new { 
                        token = playerInfo.PlayerToken,
                        playernum = playerInfo.PlayerNum,
                        opponent = opponent.PlayerName,
                        result = playerInfo.PlayerStatus
                    });
                }

                // Register the player to be connected as opponent
                RegisterPlayerWaiting(game, playerID, player);

                // So waiting
                return GameResult(new { result = "waiting" });
            }
            catch (Exception ex)
            {
                return GameFailed(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Get the status of a game
        /// </summary>
        /// <param name="game">Player token</param>
        [Route("Status")]
        [OutputCache(NoStore = true, Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult Status(String token)
        {
            try
            {
                // Find the player
                var player = GetPlayer(token);
                // Get the game
                var game = _Games[player.GameID];
                if (game.GameStatus == "finished" || game.GameStatus == "aborted")
                    return GameResult(new { status = game.GameStatus, result = game.GameStatus });

                // Get test game
                var testGame = GetTestGame(player.GameID);

                return GameResult(new {
                    status = game.GameStatus,
                    turn = testGame.TurnCount + 1,
                    player = testGame.TurnPlayer,
                    result = "success" 
                });

            }
            catch (Exception ex)
            {
                return GameFailed(ex.GetBaseException().Message);
            }
        }

        /// <summary>
        /// Play in the game
        /// </summary>
        /// <param name="game">Player token</param>
        [Route("Play")]
        [OutputCache(NoStore = true, Location = System.Web.UI.OutputCacheLocation.None)]
        public ActionResult Play(String token, String command)
        {
            try
            {
                // Find the player
                var player = GetPlayer(token);
                // Get the game
                var game = _Games[player.GameID];
                if (game.GameStatus == "finished" || game.GameStatus == "aborted")
                    return GameResult(new { status = game.GameStatus, result = game.GameStatus });

                // Get test game
                var testGame = GetTestGame(player.GameID);

                // Is this the turn of the player ?
                if (player.PlayerNum != testGame.TurnPlayer)
                {
                    return GameResult(new {
                        status = game.GameStatus,
                        turn = testGame.TurnCount + 1,
                        player = testGame.TurnPlayer,
                        result = "failed"
                    });
                }

                // Next turn
                testGame.TurnCount++;
                testGame.TurnPlayer = testGame.TurnPlayer == 1 ? 2 : 1;

                // Get the num
                var num = int.Parse(command);
                var r = num.CompareTo(testGame.NumberToFind);
                if (r < 0)
                {
                    return GameResult(new {
                        result = "before"
                    });
                }
                else if (r > 0)
                {
                    return GameResult(new {
                        result = "after"
                    });
                }
                else
                {
                    // Wins the end the game
                    testGame.Winner = player.PlayerNum;
                    game.GameStatus = "finished";
                    return GameResult(new {
                        result = "wins"
                    });
                }

            }
            catch (Exception ex)
            {
                return GameFailed(ex.GetBaseException().Message);
            }
        }

    }
}