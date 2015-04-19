using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallBasicChallenges.WebSite.Context
{
    /// <summary>
    /// Game Service for the Web
    /// </summary>
    public class WebGameService : DefaultGameService
    {
        /// <summary>
        /// Create a new Web Game Service
        /// </summary>
        public WebGameService()
        {
            this.RegisterEnginesFromAssemblies(new System.Reflection.Assembly[] { this.GetType().Assembly }); 
        }
    }
}
