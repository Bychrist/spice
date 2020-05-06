using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using Spice.Data;
using Spice.Models;
using Spice.ViewModel;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SubCategoryController : Controller
    {
        private readonly ApplicationDbContext _db;

        [TempData]
        public string StatusMessage { get; set; }

        public SubCategoryController(ApplicationDbContext db)
        {
            _db = db;

        }


        public async Task<ActionResult> Index(int page=1)
        {
    
       
            var Subcat =  _db.Subcategory.AsNoTracking().Include(c=>c.Category).OrderBy(p => p.Id);
            var model = await PagingList.CreateAsync(Subcat, 10, page);
            return View(model);
        }


        public async Task<IActionResult> Create()
        {
            SubCatandCatVm subcatVm = new SubCatandCatVm()
            {
                Categories = await _db.Category.ToListAsync(),
                SubCategory = new Models.SubCategory(),
                SubCategoryList = await _db.Subcategory.OrderBy(p => p.Name).Select(n => n.Name).Distinct().ToListAsync(),
                StatusMessage = null

            };
            return View(subcatVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubCatandCatVm CatandCat)
        {
            if (ModelState.IsValid)
            {
                var doesSubCatExist = await _db.Subcategory.Include(s => s.Category)
                    .Where(s => s.Name == CatandCat.SubCategory.Name && s.Category.Id == CatandCat.SubCategory.CategoryId)
                    .ToListAsync();
                if (doesSubCatExist.Count() > 0)
                {
                    StatusMessage = "Sub Category exist under " + doesSubCatExist.First().Category.Name;
                }
                else
                {
                    await _db.Subcategory.AddAsync(CatandCat.SubCategory);
                    await _db.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }


            }

            SubCatandCatVm model = new SubCatandCatVm()
            {
                Categories = await _db.Category.ToListAsync(),
                SubCategory = CatandCat.SubCategory,
                SubCategoryList = await _db.Subcategory.OrderBy(p => p.Name).Select(p => p.Name).ToListAsync(),
                StatusMessage = StatusMessage

            };

            return View(model);

            
        }

        [ActionName("GetSubCategory")]
        public async Task<IActionResult> GetSubCategory(int id)
        {
            List<SubCategory> subCategory = new List<SubCategory>();
            subCategory = await _db.Subcategory.Where(m => m.CategoryId == id).ToListAsync();
            return Json(subCategory);  
        }



        public async Task<IActionResult> Edit(int id)
        {
            var SubCategory = await _db.Subcategory.Where(c => c.Id == id).FirstOrDefaultAsync();
            SubCatandCatVm subcatVm = new SubCatandCatVm()
            {
                Categories = await _db.Category.ToListAsync(),
                SubCategory = SubCategory,
                SubCategoryList = await _db.Subcategory.OrderBy(p => p.Name).Select(n => n.Name).Distinct().ToListAsync(),
                StatusMessage = null

            };
            return View(subcatVm);

        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Update(SubCatandCatVm subCat)
        {
            if(ModelState.IsValid)
            {
                var subcategory = await _db.Subcategory.Where(s => s.Id == subCat.SubCategory.Id).FirstOrDefaultAsync();

                subcategory.Name = subCat.SubCategory.Name;
                subcategory.CategoryId = subCat.SubCategory.CategoryId;

                await _db.SaveChangesAsync();

                TempData["success"] = "The update was successful";
                return Redirect(nameof(Edit) + "/" + subCat.SubCategory.Id);
                




            }

            SubCatandCatVm modelone = new SubCatandCatVm()
            {
                
                StatusMessage = "Error: An error has occured"


            };

            return RedirectToAction(nameof(Edit));


        }

        public async Task<IActionResult>Details(int id)
        {
            try
            {
                var subcat = await _db.Subcategory.Where(s => s.Id == id).Include(s => s.Category).FirstAsync();

                  return View(subcat);
            }
            catch (Exception e)
            {
                TempData["Error"] = "Error: " + e.Message + " or record was not found in database";
                return RedirectToAction(nameof(Index));
            }

        }


        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var subcat = await _db.Subcategory.Where(s => s.Id == id).Include(s => s.Category).FirstAsync();

                return View(subcat);
            }
            catch (Exception e)
            {
                TempData["Error"] = "Error: " + e.Message + " or record was not found in database";
                return RedirectToAction(nameof(Index));
            }

        }


        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> Delete(SubCategory sub)
        {
            try
            {
                var subcat = await _db.Subcategory.Where(s => s.Id == sub.Id).FirstAsync();
               _db.Subcategory.Remove(subcat);
              await  _db.SaveChangesAsync();
                TempData["success"] = "Success: " + subcat.Name + " was deleted succefully";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                TempData["Error"] = "Error: " + e.Message + " or record was not found in database";
                return RedirectToAction(nameof(Index));
            }

        }








    }

}