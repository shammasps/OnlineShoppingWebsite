using CrudProject.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CrudProject.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public UserController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                _context.users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Login");
            }
            return View();
        }
        [HttpGet]
        public IActionResult Login() 
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(LoginViewModel viewModel) 
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.users
                    .FirstOrDefault(u => u.UserName == viewModel.UserName && u.Password == viewModel.Password);
                if(existingUser != null)
                {
                    HttpContext.Session.SetString("UserId", existingUser.Id.ToString());
                    HttpContext.Session.SetString("UserName", viewModel.UserName);
                    return RedirectToAction("Home");
                }
                else
                {
                    ViewBag.ErrorMessage = "Invalid UserName or Password";
                }
            }
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Home()
        {
            var products = _context.products.ToList(); // Fetch products from the database
            return View(products); // Pass the list to the view
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddProduct(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = HttpContext.Session.GetString("UserId");
                // Create a new product object
                var product = new Product
                {
                    PName = model.PName,
                    Description = model.Description,
                    Price = model.Price,
                    UserId = userId,
                    MobileNumber = model.MobileNumber,
                    Email = model.Email
                    
                };

                // Save the image file if provided
                if (model.PImage != null)
                {
                    // Generate a unique filename
                    var fileName = Path.GetFileName(model.PImage.FileName);
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;

                    // Define the file path
                    var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "images");
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Save the file to the server
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.PImage.CopyToAsync(fileStream);
                    }

                    // Save the file name in the product
                    product.ImageFileName = uniqueFileName;
                }

                // Add the product to the database
                _context.products.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction("Home");
            }

            return View(model);
        }

        [HttpGet]

        public IActionResult MyProducts()
        {
            var userId = HttpContext.Session.GetString("UserId");
            var userProducts = _context.products.Where(p => p.UserId == userId).ToList();
            if (userProducts == null)
            {
                // If product is not found or doesn't belong to the user, redirect or show error
                return NotFound("Product not found or access denied.");
            }
            return View(userProducts);
        }
        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var userId = HttpContext.Session.GetString("UserId");
            var product = _context.products.FirstOrDefault(p => p.Id == id && p.UserId == userId);
            return View(product);
        }

        [HttpPost]
        public IActionResult EditProduct(Product model)
        {
            if (ModelState.IsValid)
            {
                // Retrieve the logged-in user's UserId from session
                var userId = HttpContext.Session.GetString("UserId");

                // Find the product in the database and check if it belongs to the user
                var product = _context.products.FirstOrDefault(p => p.Id == model.Id && p.UserId == userId);


                // Update the product properties
                product.PName = model.PName;
                product.Description = model.Description;
                product.Price = model.Price;
                product.MobileNumber = model.MobileNumber;
                product.Email = model.Email;

                // Save the changes
                _context.SaveChanges();

                return RedirectToAction("MyProducts");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult DeleteProduct(int id)
        {
            // Retrieve the logged-in user's UserId from session
            var userId = HttpContext.Session.GetString("UserId");

            // Find the product by id and check if it belongs to the logged-in user
            var product = _context.products.FirstOrDefault(p => p.Id == id && p.UserId == userId);

            //if (product == null)
            //{
            //    return NotFound("Product not found or access denied.");
            //}

            return View(product);
        }

        // POST: User/DeleteProduct/5
        [HttpPost, ActionName("DeleteProduct")]
        public IActionResult DeleteConfirmed(int id)
        {
            // Retrieve the logged-in user's UserId from session
            var userId = HttpContext.Session.GetString("UserId");

            // Find the product by id and check if it belongs to the logged-in user
            var product = _context.products.FirstOrDefault(p => p.Id == id && p.UserId == userId);

           
            // Remove the product from the database
            _context.products.Remove(product);
            _context.SaveChanges();

            return RedirectToAction("MyProducts");
        }

        [HttpGet]
        public IActionResult ProductDetails(int id)
        {
            var item = _context.products.FirstOrDefault(i => i.Id == id);
            return View(item);
        }


    }
}
