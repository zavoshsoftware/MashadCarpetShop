using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class TempSize
    {
        [Key]
        public int SizeID { get; set; }

        public string SizeTitle { get; set; }
      
        public bool IsDelete { get; set; }
        public DateTime? DeleteDate { get; set; }
       
    }
}