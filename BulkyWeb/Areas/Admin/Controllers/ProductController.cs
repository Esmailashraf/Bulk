
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            var Products = _unitOfWork.Product.GetAll(properties:"Category").ToList();
            return View(Products);
        }
        public IActionResult Upsert(int ? id)
        {
            #region ViewBag
            //ViewBag.CategoryList = CategoryList;
            //CategoryList like Key 
            #endregion

            #region ViewData
            // ViewData["CategoryList"] = CategoryList; 
            #endregion

            ProductVM productVM = new()
            {
                //Navigation
                CategoryList = _unitOfWork.Category.GetAll()
                    .ToList().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.ID.ToString()
                    }),
                Product = new Product()
            };
            if(id==null||id==0)
            {
            return View(productVM); //create
            }
            else
            {
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM); //update
            }
        }
        [HttpPost]
        public IActionResult Upsert(ProductVM obj, IFormFile? file)
        {
            if (obj.Product == null)
            {
                ModelState.AddModelError("", "Invalid product data.");
                return View(obj);
            }

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, "images", "product");

                    // Delete old image if it exists
                    if (!string.IsNullOrEmpty(obj.Product.ImageUrl))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, obj.Product.ImageUrl.TrimStart('\\', '/'));

                        try
                        {
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }
                        catch (IOException ex)
                        {
                            Console.WriteLine($"IO Error deleting file: {ex.Message}");
                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            Console.WriteLine($"Permission error deleting file: {ex.Message}");
                        }
                    }

                    // Ensure the directory exists
                    if (!Directory.Exists(productPath))
                    {
                        Directory.CreateDirectory(productPath);
                    }

                    string filePath = Path.Combine(productPath, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    obj.Product.ImageUrl = "/images/product/" + fileName;
                }

                if (obj.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(obj.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(obj.Product);
                }

                _unitOfWork.Save();
                TempData["success"] = "Product is created";
                return RedirectToAction("Index", "Product");
            }

            // Ensure CategoryList is populated if ModelState is invalid
            obj.CategoryList = _unitOfWork.Category.GetAll()
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.ID.ToString()
                }).ToList();

            return View(obj);
        }


        

        #region API CALLS
        [HttpGet]
        public IActionResult Getall() 
        {
            var Products = _unitOfWork.Product.GetAll(properties: "Category").ToList();
            return Json(new { data = Products });
        }

        [HttpDelete]
        public IActionResult Delete(int ?id)
        {
            var productDelete = _unitOfWork.Product.Get(u => u.Id == id);
            if(productDelete==null)
            {
                return Json(new { success = false ,message = "error while deleting" });
            }
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productDelete.ImageUrl.TrimStart('\\', '/'));

            try
            {
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IO Error deleting file: {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"Permission error deleting file: {ex.Message}");
            }
            _unitOfWork.Product.Remove(productDelete);
            _unitOfWork.Save();
            return Json(new { success = true, message = "product deleting" });
        }
        #endregion
    }
}
