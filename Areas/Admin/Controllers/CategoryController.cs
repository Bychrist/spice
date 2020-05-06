using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _db;
        public CategoryController(ApplicationDbContext db)
        {
            _db = db;
        }

        //GET
        public async Task<IActionResult> Index()
        {
            var category = await _db.Category.ToListAsync();
            return View(category);
        }



        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
          

              if (ModelState.IsValid  && !checkCategoryName(category.Name) )
            {

               await  _db.Category.AddAsync(category);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));


            }
            else
            {
                return View();
            }

        }


        public async Task<IActionResult>Edit(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            var category = await _db.Category.FindAsync(id);
            if(category == null)
            {
                return NotFound();
            }

            return View(category);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Update(Category category)
        {
            if(ModelState.IsValid)
            {
                var cat =  _db.Category.Where(c => c.Id == category.Id).FirstOrDefault();
                if(cat != null)
                {
                    cat.Name = category.Name;
                    cat.Name = char.ToUpper(cat.Name[0]) + cat.Name.Substring(1);
                   await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
                else
                {
                    return NotFound();
                }

            }

            return View();
            

        }
        public bool checkCategoryName(string categoryName)
        {
            
            var category = _db.Category.Where(c => c.Name == categoryName).FirstOrDefault();
            if(category != null)
            {
                
                return true;
            }

            return false;
             
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _db.Category.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);

        }

        [HttpPost]
        public async Task<IActionResult>Delete(Category category)
        {
            _db.Category.Remove(category);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index)); 
        }


        [HttpGet]
        public async Task<IActionResult>Detail(int id)
        {
            var category = await _db.Category.FindAsync(id);
            if(category == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(category);

        }








    }
}