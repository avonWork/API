using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Coffee0417.Models
{
    public class LineUserProfile
    {
        public string userId { get; set; }
        public string displayName { get; set; }
        public string pictureUrl { get; set; }
        public string statusMessage { get; set; }
        public string email { get; set; }
    }
}