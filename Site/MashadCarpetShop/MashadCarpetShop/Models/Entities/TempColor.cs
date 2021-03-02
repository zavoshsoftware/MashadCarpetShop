using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class TempColor
    {
        [Key]
        public int ColorID { get; set; }

        public string ColorName { get; set; }
        public string ColorTitle { get; set; }
        public string ColorEN_Title { get; set; }
        public bool IsDelete { get; set; }
        public DateTime? DeleteDate { get; set; }
        public string ColorNo { get; set; }
        public string Rus_ColorTitle { get; set; }
        public string China_ColorTitle { get; set; }
    }
}