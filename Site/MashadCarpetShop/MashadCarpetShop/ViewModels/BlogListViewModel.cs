using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class BlogListViewModel:_BaseViewModel
    {
        public List<Blog> Blogs { get; set; }
        public BlogGroup BlogGroup { get; set; }
    }
}