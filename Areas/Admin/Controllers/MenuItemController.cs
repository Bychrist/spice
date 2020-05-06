using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;
using Spice.Models.Utitlity;
using Spice.ViewModel;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MenuItemController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _hostingEnvironment;

        [BindProperty]
        public MenuItemViewModel MenutItemVm {get; set;}


        public MenuItemController(ApplicationDbContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment =  hostingEnvironment;
            MenutItemVm = new MenuItemViewModel()
            {
                Categories = _db.Category.ToList(),
                MenuItem = new Models.MenuItem(),
                SubCategories = new List<Models.SubCategory>() { }
            }; 

        }
  
        public async Task<IActionResult> Index()
        {
            var menuItem = await _db.MenuItem.Include(m=>m.Category).Include(m=>m.SubCategory).ToListAsync();
            return View(menuItem);
        }

        public IActionResult Create()
        {
            return View(MenutItemVm);
        }


        public async Task<IActionResult> Edit(int id)
        {
            MenutItemVm.MenuItem = await  _db.MenuItem.Where(m=>m.Id == id).Include(m => m.Category).Include(m => m.SubCategory).SingleAsync();

            return View(MenutItemVm);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>CreatePost()
        {
            if(!ModelState.IsValid)
            {
                return View(MenutItemVm);
            }

            await _db.MenuItem.AddAsync(MenutItemVm.MenuItem);
            await _db.SaveChangesAsync();


            //work on the image saving session
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var menuItemFromDb = await _db.MenuItem.FindAsync(MenutItemVm.MenuItem.Id);
            if(files.Count > 0)
            {
                //files has beeb uploaded0
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);
                using (var filesStream = new FileStream(Path.Combine(uploads, MenutItemVm.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }

                menuItemFromDb.Image = @"\images\" + MenutItemVm.MenuItem.Id + extension;
            }
            else
            {
                //no file was uploaded use default
                var uploads = Path.Combine(webRootPath, @"images\" + SD.DefaultFoodImage);
                System.IO.File.Copy(uploads, webRootPath + @"\images\" + MenutItemVm.MenuItem.Id+".png");
                menuItemFromDb.Image = @"\images\" + MenutItemVm.MenuItem.Id + ".png";
                
                
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));



        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>EditPost()
        {
            if (!ModelState.IsValid)
            {
                return View(MenutItemVm);
            }

          //  await _db.MenuItem.AddAsync(MenutItemVm.MenuItem);
         //   await _db.SaveChangesAsync();


            //work on the image saving session
            string webRootPath = _hostingEnvironment.WebRootPath;
            var files = HttpContext.Request.Form.Files;
            var menuItemFromDb = await _db.MenuItem.FindAsync(MenutItemVm.MenuItem.Id);
            if (files.Count > 0)
            {
                //files has beeb uploaded0
                var uploads = Path.Combine(webRootPath, "images");
                var extension = Path.GetExtension(files[0].FileName);

                //delete original image
                var imagePath = Path.Combine(webRootPath, menuItemFromDb.Image.TrimStart('\\'));
                if(System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                //we will upload the new file
                using (var filesStream = new FileStream(Path.Combine(uploads, MenutItemVm.MenuItem.Id + extension), FileMode.Create))
                {
                    files[0].CopyTo(filesStream);
                }

                menuItemFromDb.Image = @"\images\" + MenutItemVm.MenuItem.Id + extension;
            }

            menuItemFromDb.Name = MenutItemVm.MenuItem.Name;
            menuItemFromDb.Price = MenutItemVm.MenuItem.Price;
            menuItemFromDb.SubCategoryId = MenutItemVm.MenuItem.SubCategoryId;
            menuItemFromDb.CategoryId = MenutItemVm.MenuItem.CategoryId;
            menuItemFromDb.Description = MenutItemVm.MenuItem.Description;
            menuItemFromDb.Spicyness = MenutItemVm.MenuItem.Spicyness;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));


        }


        public IActionResult Details(int id)
        {
            try
            {
                MenuItem menuItem = _db.MenuItem.Include(m=>m.Category).Include(m=>m.SubCategory).Where(m => m.Id == id).FirstOrDefault();
                return View(menuItem);
            }
            catch (Exception)
            {

                return RedirectToAction(nameof(Index));
            }

        }


        public async Task<IActionResult>Delete(int id)
        {
            try
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                var menuItem = await _db.MenuItem.Where(m => m.Id == id).FirstAsync();


                //delete original image
                var imagePath = Path.Combine(webRootPath, menuItem.Image.TrimStart('\\'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                _db.Remove(menuItem);
                await _db.SaveChangesAsync();

                TempData["deletion"] = "Menu item was deleted successfully";

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["faileddeletion"] = "something went wrong or menu not found in database";

                return RedirectToAction(nameof(Index));

            }


        }




    }
}