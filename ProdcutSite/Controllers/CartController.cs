using System.Linq;
using System.Web.Mvc;
using ProdcutSite.Models;
using ProductSite.Domain.Abstract;
using ProductSite.Domain.Entities;

namespace ProdcutSite.Controllers
{
    public class CartController : Controller
    {
        //
        // GET: /Cart/
        private IProductsRepository repository;

        public ViewResult Index(Cart cart ,string returnUrl)
        {
            return View(new CartIndexViewModel {Cart = cart, ReturnUrl = returnUrl});
        } 

        public  CartController(IProductsRepository repo)
        {
            repository = repo;
        }
        
        public RedirectToRouteResult AddToCart(Cart cart ,int productId, string returnUrl)
        {
            Product product = repository.Products.FirstOrDefault(p => p.ProductID == productId); 
            if (product != null)
            { 
                cart.AddItem(product, 1);
            } return RedirectToAction("Index", new { returnUrl });
        }
        public RedirectToRouteResult RemoveFromCart(Cart cart ,int productId, string returnUrl)
        {
            Product product = repository.Products.FirstOrDefault(p => p.ProductID == productId);
            if (product != null)
            {
                cart.RemoveLine(product);
            } 
            return RedirectToAction("Index", new { returnUrl });
        } 
        private Cart GetCart()
        {
            Cart cart = (Cart)Session["Cart"];
            if (cart == null) 
            { cart = new Cart();
                Session["Cart"] = cart; 
            }
            return cart;
        } 
    }
}
