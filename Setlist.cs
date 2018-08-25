using System;
using System.Collections.Generic;

namespace e_sharp_minor
{
    public class Setlist
    {
        public string Date { get; set; }
        public string Venue { get; set; }
        public bool Active { get; set; }
        public bool Print { get; set; }
        public List<string> Songs { get; set; }
    }

    public class Setlists
    {
        public List<Setlist> Sets { get; set; }
    }

    public class SongName
    {
        public List<string> Names { get; set; }
        public string ShortName { get; set; }
        public string Starts { get; set; }
    }

    public class SongNames {
        public List<SongName> Songs { get; set; }
    }
}
