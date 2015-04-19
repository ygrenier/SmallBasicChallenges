using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmallBasicChallenges.WebSite.Context
{

    /// <summary>
    /// Context Small Basic Challenges for Web
    /// </summary>
    public class WebContext : SbcContext
    {

        /// <summary>
        /// Create a new Web Context
        /// </summary>
        public WebContext()
            : base(new WebDataService(), new WebGameService())
        {
        }

    }

}
