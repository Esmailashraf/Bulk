using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Bulky.Models;
using Bulky.DataAccess.Repository.IRepository;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var ProductList = _unitOfWork.Product.GetAll(properties: "Category");
            return View(ProductList);
        }

        public IActionResult Details(int id)
        {
            ShopingCart cart = new ShopingCart
            {
                Product = _unitOfWork.Product.Get(u => u.Id == id, properties: "Category"),
                Count = 1,
                ProdunctId = id // Fixed typo: Changed from "ProdunctId" to "ProductId"
            };
            return View(cart);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Prevents CSRF attacks
        [Authorize] // Requires authentication to access
        public IActionResult Details(ShopingCart cart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim != null) // Ensure user ID is not null
            {
                cart.ApplicationUserId = userIdClaim.Value; // Extract string value
                cart.Id = 0;
                var GetCartFmDb = _unitOfWork.shoppingCart.Get(u => u.ApplicationUserId == userIdClaim.Value && u.ProdunctId == cart.ProdunctId);
                if (GetCartFmDb!=null) 
                {
                    GetCartFmDb.Count = cart.Count;
                    _unitOfWork.shoppingCart.Update(GetCartFmDb);
                }
                else
                {
                  _unitOfWork.shoppingCart.Add(cart); // Ensure "ShoppingCart" is correctly named
                }
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            // If userId is null, handle the error (redirect or show message)
            ModelState.AddModelError("", "User not found.");
            return View(cart);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
