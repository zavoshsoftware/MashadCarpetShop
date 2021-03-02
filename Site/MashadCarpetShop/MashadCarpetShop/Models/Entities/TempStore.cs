using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Models
{
    public class TempStore
    {
        [Key]
        public Guid StoreID { get; set; }

        public string StoreName { get; set; }
        public string StorePhone { get; set; }
        public string StoreAddress { get; set; }
        public string StoreCity { get; set; }
        public bool IsStore { get; set; }
        public string Prov { get; set; }
        public string StoreDesc { get; set; }
      
        public bool IsDelete { get; set; }
        public DateTime? DeleteDate { get; set; }
       
    }
}