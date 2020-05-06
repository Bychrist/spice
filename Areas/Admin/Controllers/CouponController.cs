using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spice.Data;
using Spice.Models;

namespace Spice.Areas.Admin.Controllers
{
    [Area("Admin")]

  

    
    public class CouponController : Controller
    {
        private readonly ApplicationDbContext _db;

        public CouponController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /<controller>/
        public async  Task<IActionResult> Index()
        {
            var coupons = await _db.Coupon.ToListAsync();
            return View(coupons);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Create(Coupon coupon)
        {
            if(ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if(files.Count > 0)
                {
                    byte[] p1 = null;
                    using(var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }

                    }

                    coupon.Picture = p1;

                }

                _db.Coupon.Add(coupon);
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(coupon);      
         
        }


        [HttpGet]
        public async Task<IActionResult>Edit(int id)
        {
            try
            {
                var coupon = await _db.Coupon.Where(m => m.Id == id).FirstOrDefaultAsync();
                if(coupon is null)
                {
                    TempData["failedcouponedit"] = "No record was found in database";
                    return RedirectToAction(nameof(Index));
                }
                return View(coupon);
            }
            catch (Exception e)
            {

                TempData["failedcouponedit"] = e.Message;
                return View(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>Edit(Coupon coupon)
        {
            if(ModelState.IsValid)
            {
                var couponFromDb = _db.Coupon.Where(m => m.Id == coupon.Id).FirstOrDefault();
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    byte[] p1 = null;
                    using (var fs1 = files[0].OpenReadStream())
                    {
                        using (var ms1 = new MemoryStream())
                        {
                            fs1.CopyTo(ms1);
                            p1 = ms1.ToArray();
                        }

                    }

                    coupon.Picture = p1;
                    couponFromDb.Picture = coupon.Picture;

                }

                couponFromDb.Name = coupon.Name;
                couponFromDb.MinimumAmount = coupon.MinimumAmount;
                couponFromDb.IsActive = coupon.IsActive;
                couponFromDb.Discount = coupon.Discount;
                couponFromDb.CouponType = coupon.CouponType;
                await _db.SaveChangesAsync();
                TempData["couponUpdate"] = "Coupon update was successful";
                return RedirectToAction(nameof(Index));

            }

            return View(coupon);
        }



    }
}
