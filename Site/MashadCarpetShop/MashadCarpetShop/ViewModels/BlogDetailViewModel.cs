
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class BlogDetailViewModel : _BaseViewModel
    {
        public Blog Blog { get; set; }
        public List<BlogGroupItemViewModel> SidebarBlogGroups { get; set; }
        public List<BlogItemViewModel> SidebarBlogs { get; set; }
        public List<BlogItemViewModel> RelatedBlog { get; set; }
        public List<BlogComment> BlogComments { get; set; }
    }

    public class BlogGroupItemViewModel
    {
        public string Title { get; set; }
        public string UrlParam { get; set; }
        public int BlogCount { get; set; }
    }

    public class BlogItemViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string UrlParam { get; set; }
        public string CreationDateStr { get; set; }
        public int CommentCount { get; set; }
        public string ImageUrl { get; set; }
        public string Summery { get; set; }
    }
}