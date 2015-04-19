using SmallBasicChallenges.WebSite.Context;
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
                var context = new WebContext();

                // Get the player id
                String playerID = context.CalculatePlayerID(player, this.Request.UserHostAddress, game);

                // Try to connect the player
                var session = context.ConnectPlayer(player, this.Request.UserHostAddress, game);
                
                // If we get a session we returns his status
                if (session != null)
                {
                    return GameResult(context.GetStatusResult(session, playerID));
                }

                // So waiting
                return GameResult(new { status = "waiting" });
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
                // Create the context
                var context = new WebContext();

                // Find the game session
                var session = context.FindSessionFromPlayer(token);
                if (session == null)
                    throw new InvalidOperationException("Unknown token.");
                
                // Get the player
                var player = session.GetPlayer(token);
                if (!String.Equals(player.IpAddress, this.Request.UserHostAddress, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Your are not authorized.");

                // Get the data
                var data = context.GetGameData(session);

                // Get the result
                return GameResult(context.GetStatusResult(session, token));
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
                // Create the context
                var context = new WebContext();

                // Find the game session
                var session = context.FindSessionFromPlayer(token);
                if (session == null)
                    throw new InvalidOperationException("Unknown token.");

                // Get the player
                var player = session.GetPlayer(token);
                if (!String.Equals(player.IpAddress, this.Request.UserHostAddress, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Your are not authorized.");

                // Execute the command
                return GameResult(context.GamePlay(session, token, command));
            }
            catch (Exception ex)
            {
                return GameFailed(ex.GetBaseException().Message);
            }
        }

    }
}