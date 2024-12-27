using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using mentor.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mentor.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuController : Controller
    {
        private readonly DataContext _context;

        public MenuController(DataContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var mnList = _context.Menus.OrderBy(m => m.MenuID).ToList();
            return View(mnList);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var mn = _context.Menus.Find(id);
            if (mn == null)
                return NotFound();

            return View(mn);
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var delMenu = _context.Menus.Find(id);
            if (delMenu == null)
                return NotFound();

            _context.Menus.Remove(delMenu);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult Create()
        {
            // Fetching the list of menus to populate the dropdown list
            var mnList = _context.Menus
                           .Select(m => new SelectListItem()
                           {
                               Text = (m.Levels == 1) ? m.MenuName : "-- " + m.MenuName,
                               Value = m.MenuID.ToString()
                           }).ToList();

            // Add a default item at the start of the list
            if (!mnList.Any())
            {
                mnList.Add(new SelectListItem
                {
                    Text = "--- No Menus Available ---",
                    Value = "0"
                });
            }
            else
            {
                mnList.Insert(0, new SelectListItem()
                {
                    Text = "--- select ---",
                    Value = "0"
                });
            }

            ViewBag.mnList = mnList; // Pass the list to the view using ViewBag
            return View();
        }

        [HttpPost]
        public IActionResult Create(tblMenu mn)
        {
            if (ModelState.IsValid)
            {
                _context.Menus.Add(mn);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            // Reload the mnList in case of validation error
            ViewBag.mnList = GetMenuSelectList();
            return View(mn);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
                return NotFound();

            var mn = _context.Menus.Find(id);
            if (mn == null)
                return NotFound();

            // Populate the dropdown list for the Edit view
            ViewBag.mnList = GetMenuSelectList();
            return View(mn);
        }

        [HttpPost]
        public IActionResult Edit(tblMenu mn)
        {
            if (ModelState.IsValid)
            {
                _context.Menus.Update(mn);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            // Reload the mnList in case of validation error
            ViewBag.mnList = GetMenuSelectList();
            return View(mn);
        }

        // Helper method to retrieve the SelectList for mnList
        private List<SelectListItem> GetMenuSelectList()
        {
            var mnList = _context.Menus
                           .Select(m => new SelectListItem()
                           {
                               Text = (m.Levels == 1) ? m.MenuName : "-- " + m.MenuName,
                               Value = m.MenuID.ToString()
                           }).ToList();

            if (!mnList.Any())
            {
                mnList.Add(new SelectListItem
                {
                    Text = "--- No Menus Available ---",
                    Value = "0"
                });
            }
            else
            {
                mnList.Insert(0, new SelectListItem()
                {
                    Text = "--- select ---",
                    Value = "0"
                });
            }
            return mnList;
        }
    }
}
