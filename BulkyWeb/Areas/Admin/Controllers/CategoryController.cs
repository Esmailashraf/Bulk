using System.Collections.Generic;
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
    public class CategoryController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var Categories = _unitOfWork.Category.GetAll().ToList();
            
            return View(Categories);
        }
        public IActionResult Create()
        {
            
            return View();
        }
        [HttpPost]
        public IActionResult Create(Category obj)
        {
            #region ValidationCustom
            //if (obj.Name==obj.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("name", "name is not the same  with DisplayOrder");
            //    // this message will appear under the name
            //}
            //if (obj.Name.ToLower() == "test")
            //{
            //    ModelState.AddModelError("", "test is not valid");
            //    // this message will appear under the name
            //} 
            #endregion
            if (ModelState.IsValid) 
            {
                _unitOfWork.Category.Add(obj);
                _unitOfWork.Save();
            TempData["success"] = "Category is created";
            return RedirectToAction("Index","Category");
            }
            return View();
        }
        public IActionResult Edit(int? id )
        {
            if(id==null)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u=>u.ID==id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Category obj)
        {
            #region ValidationCustom
            //if (obj.Name==obj.DisplayOrder.ToString())
            //{
            //    ModelState.AddModelError("name", "name is not the same  with DisplayOrder");
            //    // this message will appear under the name
            //}
            //if (obj.Name.ToLower() == "test")
            //{
            //    ModelState.AddModelError("", "test is not valid");
            //    // this message will appear under the name
            //} 
            #endregion
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(obj);
                _unitOfWork.Save();
                TempData["success"] = "Category is Updated";

                return RedirectToAction("Index", "Category");
            }
            return View();
        }
        public IActionResult Delete(int? id)
        {
            if (id == null|| id==0)
            {
                return NotFound();
            }
            Category? categoryFromDb = _unitOfWork.Category.Get(u => u.ID == id);

            if (categoryFromDb == null)
            {
                return NotFound();
            }

            return View(categoryFromDb);
        }
        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePost(int ? id)
        {
            Category obj = _unitOfWork.Category.Get(u => u.ID == id);
            if (obj==null)
            {
                return NotFound();
            }

            _unitOfWork.Category.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Category is Deleted";

            return RedirectToAction("Index", "Category");
            
           
        }
    }
}
