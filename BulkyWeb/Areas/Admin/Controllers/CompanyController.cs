
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var Companys = _unitOfWork.Company.GetAll().ToList();
            return View(Companys);
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


         
            if(id==null||id==0)
            {
            return View(new Company()); //create
            }
            else
            {
                Company CompanyObj = _unitOfWork.Company.Get(u => u.Id == id);
                return View(CompanyObj); //update
            }
        }
        [HttpPost]
        public IActionResult Upsert(Company obj)
        {
            if (obj == null)
            {
                ModelState.AddModelError("", "Invalid Company data.");
                return View(obj);
            }

            if (ModelState.IsValid)
            {
              
                if (obj.Id == 0)
                {
                    _unitOfWork.Company.Add(obj);
                }
                else
                {
                    _unitOfWork.Company.Update(obj);
                }

                _unitOfWork.Save();
                TempData["success"] = "Company is created";
                return RedirectToAction("Index", "Company");
            }

            return View(obj);
        }


        

        #region API CALLS
        [HttpGet]
        public IActionResult Getall() 
        {
            var Companys = _unitOfWork.Company.GetAll().ToList();
            return Json(new { data = Companys });
        }

        [HttpDelete]
        public IActionResult Delete(int ?id)
        {
            var CompanyDelete = _unitOfWork.Company.Get(u => u.Id == id);
            if(CompanyDelete==null)
            {
                return Json(new { success = false ,message = "error while deleting" });
            }
           
            _unitOfWork.Company.Remove(CompanyDelete);
            _unitOfWork.Save();
            return Json(new { success = true, message = "Company deleting" });
        }
        #endregion
    }
}
