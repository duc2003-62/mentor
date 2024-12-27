using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mentor.Models;
using Microsoft.AspNetCore.Mvc;
namespace mentor.Components
{
    [ViewComponent(Name = "Post")]
    public class PostComponent : ViewComponent
    {
        private readonly DataContext _context;
        public PostComponent(DataContext context)
        {
            _context = context;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var listOfPost = (from p in _context.ViewPostMenus
                              where (p.IsActive == true)
                              orderby p.PostID descending
                              select p).Take(1).ToList();
            return await Task.FromResult((IViewComponentResult)View("Default", listOfPost));
        }
        
    }
}