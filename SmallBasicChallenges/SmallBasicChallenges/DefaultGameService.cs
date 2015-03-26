using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SmallBasicChallenges
{
    /// <summary>
    /// Default game service
    /// </summary>
    public class DefaultGameService : IGameService
    {
        IDictionary<String, GameEngine> _Engines = null;
        static DefaultGameService _Current = null;

        /// <summary>
        /// Build the engine list
        /// </summary>
        protected void BuildEngines()
        {
            // Force recreate the list
            _Engines = new Dictionary<String, GameEngine>(StringComparer.OrdinalIgnoreCase);
            // Search all engines in the loaded assembly 
            RegisterEnginesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies());
        }

        /// <summary>
        /// Current game service
        /// </summary>
        public static DefaultGameService Current { get { return _Current ?? (_Current = new DefaultGameService()); } }

        /// <summary>
        /// Register the engines availables in some assemblies
        /// </summary>
        public void RegisterEnginesFromAssemblies(IEnumerable<Assembly> assemblies)
        {
            if (assemblies == null) return;
            if (_Engines == null)
                _Engines = new Dictionary<String, GameEngine>(StringComparer.OrdinalIgnoreCase);
            // Search all engines in the assemblies
            var tQuery = assemblies
                .Where(a => a != null)
                .SelectMany(a => a.GetTypes())
                .Where(t => !t.IsAbstract && t.IsSubclassOf(typeof(GameEngine)));
            foreach (var tge in tQuery)
            {
                // New instance
                var ge = (GameEngine)Activator.CreateInstance(tge);
                _Engines[ge.Name] = ge;
                foreach (var gameName in ge.GetNames())
                    _Engines[gameName] = ge;
            }
        }

        /// <summary>
        /// Find a game
        /// </summary>
        public GameEngine FindGame(string name)
        {
            if (_Engines == null) BuildEngines();
            GameEngine result = null;
            if (!_Engines.TryGetValue(name, out result))
                return null;
            return result;
        }

        /// <summary>
        /// List all the games
        /// </summary>
        public IEnumerable<GameEngine> GetGames()
        {
            if (_Engines == null) BuildEngines();
            return _Engines.Values.Distinct();
        }

    }
}
