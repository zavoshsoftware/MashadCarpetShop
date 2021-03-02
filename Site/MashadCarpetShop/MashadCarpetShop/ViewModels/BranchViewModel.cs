using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Models;

namespace ViewModels
{
    public class BranchViewModel : _BaseViewModel
    {
        public List<Branch> Branches { get; set; }
    }
}