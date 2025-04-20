using System.Security.Claims;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        //[BindProperties]
        public ShoppingCartVm ShoppingCartVm { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ShoppingCartVm = new ShoppingCartVm
            {
                ShoppingCartList = _unitOfWork.shoppingCart.GetAll(
                    u => u.ApplicationUserId == userIdClaim,
                    properties: "Product"
                ),
                OrderHeader = new OrderHeader()
            };
            foreach (var cart in ShoppingCartVm.ShoppingCartList)
            {
                cart.Price = GetPriceBaseOnQuantity(cart);
                ShoppingCartVm.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVm);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userIdClaim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ShoppingCartVm ShoppingCartVm = new ShoppingCartVm
            {
                ShoppingCartList = _unitOfWork.shoppingCart.GetAll(
                    u => u.ApplicationUserId == userIdClaim,
                    properties: "Product"
                ),
                OrderHeader = new OrderHeader()
            };
            ShoppingCartVm.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userIdClaim);
            ShoppingCartVm.OrderHeader.City = ShoppingCartVm.OrderHeader.ApplicationUser.City;
            ShoppingCartVm.OrderHeader.Name = ShoppingCartVm.OrderHeader.ApplicationUser.Name;
            ShoppingCartVm.OrderHeader.PhoneNumber = ShoppingCartVm.OrderHeader.ApplicationUser.PhoneNumber;
            ShoppingCartVm.OrderHeader.StreetAddress = ShoppingCartVm.OrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVm.OrderHeader.State = ShoppingCartVm.OrderHeader.ApplicationUser.State;
            ShoppingCartVm.OrderHeader.PostalCode = ShoppingCartVm.OrderHeader.ApplicationUser.PostalCode;


            foreach (var cart in ShoppingCartVm.ShoppingCartList)
            {
                cart.Price = GetPriceBaseOnQuantity(cart);
                ShoppingCartVm.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(ShoppingCartVm);
        }
        [HttpPost]
        public IActionResult Summary(ShoppingCartVm shoppingCartVm)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            shoppingCartVm.ShoppingCartList = _unitOfWork.shoppingCart.GetAll(
                u => u.ApplicationUserId == userId,
                properties: "Product"
            ).ToList();

            shoppingCartVm.OrderHeader.OrderDate = System.DateTime.Now;
            shoppingCartVm.OrderHeader.ApplicationUserId = userId;
            ApplicationUser applicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

            foreach (var cart in shoppingCartVm.ShoppingCartList)
            {
                cart.Price = GetPriceBaseOnQuantity(cart);
                shoppingCartVm.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            if (applicationUser?.CompanyId.GetValueOrDefault() == 0)
            {
                shoppingCartVm.OrderHeader.OrderStatus = SD.StatusPending;
                shoppingCartVm.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            }
            else
            {
                shoppingCartVm.OrderHeader.OrderStatus = SD.StatusApproved;
                shoppingCartVm.OrderHeader.PaymentStatus = SD.PaymentStatusDelayPayment;
            }

            _unitOfWork.OrderHeader.Add(shoppingCartVm.OrderHeader);
            _unitOfWork.Save();

            foreach (var cart in shoppingCartVm.ShoppingCartList)
            {
                var orderDetail = new OrderDetail
                {
                    ProductId = cart.Product.Id,
                    OrderHeaderId = shoppingCartVm.OrderHeader.Id,
                    Price = cart.Price,
                    Count = cart.Count
                };

                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
            if (applicationUser?.CompanyId.GetValueOrDefault() == 0)
            {
                // regular customer 
                // stripe logic
                var domain = "https://localhost:7180/";
                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    SuccessUrl = domain + $"customer/cart/OrderConfirmation?id={shoppingCartVm.OrderHeader.Id}",
                    CancelUrl = domain + "customer/cart/index",
                    Mode = "payment",
                    LineItems = new List<SessionLineItemOptions>(),
                };

                foreach (var item in shoppingCartVm.ShoppingCartList)
                {
                    var sessionLineItem = new SessionLineItemOptions()
                    {
                        PriceData = new SessionLineItemPriceDataOptions()
                        {
                            UnitAmount = (long)(item.Price * 100), // Convert to cents
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions()
                            {
                                Name = item.Product.Title
                            }
                        },
                        Quantity = item.Count
                    };
                    options.LineItems.Add(sessionLineItem);
                }


                try
                {
                    var service = new SessionService();
                    Session session = service.Create(options);
                    Console.WriteLine("ID : "+session.Id);
                    if (session == null)
                    {
                        throw new Exception("Stripe session creation failed: session is null.");
                    }

                    if (string.IsNullOrEmpty(session.Id))
                    {
                        throw new Exception("Stripe session creation failed: session.Id is null.");
                    }

                    Console.WriteLine("Stripe Session Created Successfully!");
                    Console.WriteLine("Session ID: " + session.Id);
                    Console.WriteLine("PaymentIntent ID: " + session.PaymentIntentId);

                    // Update database with session ID
                    _unitOfWork.OrderHeader.updateStripePaymentID(
                        shoppingCartVm.OrderHeader.Id,
                        session.Id,
                        session.PaymentIntentId ?? "PENDING"
                    );
                    _unitOfWork.Save();

                    // Redirect to Stripe checkout
                    Response.Headers.Add("Location", session.Url);
                    return new StatusCodeResult(303);
                }
                catch (StripeException stripeEx)
                {
                    Console.WriteLine("Stripe Error: " + stripeEx.Message);
                    return RedirectToAction("Error", "Home");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("General Error: " + ex.Message);
                    return RedirectToAction("Error", "Home");
                }


            }

            return RedirectToAction(nameof(OrderConfirmation), new { id = shoppingCartVm.OrderHeader.Id });
        }
        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }
        private double GetPriceBaseOnQuantity(ShopingCart cart)
        {
            if (cart.Count <= 50)
            {
                return cart.Product.Price;

            }
            else
            {
                if (cart.Count <= 100)
                {
                    return cart.Product.Price50;
                }
                else
                {
                    return cart.Product.Price100;

                }

            }
        }

        public IActionResult Plus(int cardId)
        {
            var CartFrmDB = _unitOfWork.shoppingCart.Get(u => u.Id == cardId);
            CartFrmDB.Count += 1;
            _unitOfWork.shoppingCart.Update(CartFrmDB);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Minus(int cardId)
        {
            var CartFrmDB = _unitOfWork.shoppingCart.Get(u => u.Id == cardId);
            if (CartFrmDB.Count <= 1)
            {
                _unitOfWork.shoppingCart.Remove(CartFrmDB);

            }
            else
            {
                CartFrmDB.Count -= 1;
                _unitOfWork.shoppingCart.Update(CartFrmDB);

            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Delete(int cardId)
        {
            var CartFrmDB = _unitOfWork.shoppingCart.Get(u => u.Id == cardId);
            _unitOfWork.shoppingCart.Remove(CartFrmDB);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
