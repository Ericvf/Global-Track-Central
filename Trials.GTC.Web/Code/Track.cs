using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Trials.GTC.Web.Code
{
    public class Track
    {
        public int ID { get; set; }
        public int Trials { get; set; }
        public int Type { get; set; }

        public string Creator { get; set; }
        public int Difficulty { get; set; }
        public List<Tag> Tags { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Released { get; set; }
        public string Link { get; set; }
        public double Rating { get; set; }

        public Track()
        {
            this.Tags = new List<Tag>();
        }
    }

    public class Tag
    {
        public int ID { get; set; }
        public string Description { get; set; }
    }
}