using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Capstone.Areas.CMS.Models
{
    //public class APIMindMap
    //{
    //    public string name { get; set; }
    //    public string color { get; set; }
    //}
    public class APIMindMap1
    {
        public string name { get; set; }
        public string color { get; set; }
        public List<APIMindMap0> children { get; set; }
    }
    public class APIMindMap2
    {
        public string name { get; set; }
        public List<APIMindMap1> children { get; set; }
    }
    public class APIMindMap0
    {
        public int manganh { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public List<APIMindMap00> children { get; set; }
    }
    public class APIMindMap01
    {
        public int ma { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public string claimReason { get; set; }

    }
    public class APIMindMap00
    {
        public int? manganh { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public string color { get; set; }
        public List<APIMindMap01> children { get; set; }
    }

    public class MT00
    {
        public string name { get; set; }
        public List<MT01> children { get; set; }

    }
    public class MT01
    {
        public string name { get; set; }
        public string color { get; set; }
        public List<MT02> children { get; set; }
    }
    public class MT02
    {
        public string name { get; set; }
    }
}