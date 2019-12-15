using Capstone.Models;
using System.Collections.Generic;

namespace Capstone.Areas.CMS.Models
{
    public class DataInfoModel
    {
        public List<HeModel> lsthe { get; set; }
        public List<NganhModel> lstnganh { get; set; }
        public List<KhoaModel> lstkhoa { get; set; }
        public List<sc_Khoikienthuc> lstkhoi { get; set; }
        public List<sc_Khoikienthuc> lst { get; set; }
    }
}