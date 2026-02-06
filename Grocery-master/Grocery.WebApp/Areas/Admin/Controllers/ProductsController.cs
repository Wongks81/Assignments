using Grocery.WebApp.Areas.Admin.ViewModels;
using Grocery.WebApp.Data;
using Grocery.WebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Grocery.WebApp.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ProductsController> _logger;
        private readonly UserManager<MyIdentityUser> _userManager;

        public ProductsController(ApplicationDbContext context, ILogger<ProductsController> logger,
                                    UserManager<MyIdentityUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
        }
        // GET: ProductsController
        public ActionResult Index()
        {
            //Lambda version to extract all products 
            //var products = _context.Products
            //    .Include(p => p.CreatedByUser)
            //    .Include(p => p.UpdatedByUser)
            //    .ToList();

            var productViewModels = (from p in _context.Products
                                        .Include(p => p.CreatedByUser)
                                        .Include(p => p.UpdatedByUser)
                            select new ProductViewModel
                            {
                                ProductID = p.ProductID,
                                ProductName = p.ProductName,
                                SellingPricePerUnit = p.SellingPricePerUnit,
                                Quantity = p.Quantity,
                                Image = p.Image,
                                LastUpdateOn = p.LastUpdateOn,

                                CreatedByUser = p.CreatedByUser,
                                CreatedByUserId = p.CreatedByUserId,
                                UpdatedByUser = p.UpdatedByUser,
                                UpdatedByUserId = p.UpdatedByUserId
                            }).ToList();

            return View(productViewModels);
        }

        // GET: ProductsController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ProductsController/Create
        public ActionResult Create()
        {
            ProductViewModel productViewModel = new ProductViewModel();
            return View(productViewModel);
        }

        // POST: ProductsController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind("ProductName,Quantity,SellingPricePerUnit")] ProductViewModel productViewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if(user == null)
            {
                ModelState.AddModelError("Create", "User not found. Please login again!");
            }

            //if model object is not valid
            if (!ModelState.IsValid)
            {
                return View(productViewModel);
            }

            Product newProduct = new Product()
            {
                ProductID = new Guid(),
                ProductName = productViewModel.ProductName,
                SellingPricePerUnit = productViewModel.SellingPricePerUnit,
                Quantity = productViewModel.Quantity,

                LastUpdateOn = DateTime.Now,
                CreatedByUserId = user.Id,
            };

            //check if file has been attached
            if(Request.Form.Files.Count >= 1)
            {

                // Might not work everywhere
                //IFormFile file = productViewModel.ImageFile;

                IFormFile file = Request.Form.Files.FirstOrDefault();

                // copy the file uploaded using the MemoryStream into the Product.Image
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);

                    // convert to bytearray for uploading pics
                    newProduct.Image = dataStream.ToArray();
                }
            }

            try
            {
                // update the database
                _context.Products.Add(newProduct);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));

            }
            catch(System.Exception ex)
            {
                ModelState.AddModelError("Create", "Unable to update the Database. Please inform IT!");
                _logger.LogError($"Create Product failed: {ex.Message}");
                return View(productViewModel);
            } 
        }

        // GET: ProductsController/Edit/5
        public async Task<ActionResult> Edit(Guid? id)
        {


            if (id == null)
            {
                return NotFound();
            }

            Product productToEdit = await _context.Products.FindAsync(id);

            ProductViewModel productViewModel = new ProductViewModel()
            {
                ProductID = productToEdit.ProductID,
                ProductName = productToEdit.ProductName,
                SellingPricePerUnit = productToEdit.SellingPricePerUnit,
                Quantity = productToEdit.Quantity,
                Image = productToEdit.Image,

                LastUpdateOn = productToEdit.LastUpdateOn,
                CreatedByUserId = productToEdit.CreatedByUserId,
                CreatedByUser = productToEdit.CreatedByUser,
                UpdatedByUserId = productToEdit.UpdatedByUserId,
                UpdatedByUser = productToEdit.UpdatedByUser     
            };

            return View(productViewModel);
        }

        // POST: ProductsController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, 
            [Bind("ProductID,ProductName,Quantity,SellingPricePerUnit,Image,CreatedByUserId,UpdatedByUserId,LastUpdateOn")] 
            ProductViewModel productViewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError("Create", "User not found. Please login again!");
            }

            //if model object is not valid
            if (!ModelState.IsValid)
            {
                return View(productViewModel);
            }

            // find the data in the database
            Product editProduct = await _context.Products.FindAsync(productViewModel.ProductID);
            if(editProduct == null)
            {
                return NotFound();
            }

            // update the properties of the model 
            editProduct.ProductName = productViewModel.ProductName;
            editProduct.SellingPricePerUnit = productViewModel.SellingPricePerUnit;
            editProduct.Quantity = productViewModel.Quantity;

            editProduct.LastUpdateOn = DateTime.Now;
            editProduct.CreatedByUserId = user.Id;

            //check if file has been attached
            if (Request.Form.Files.Count >= 1)
            {

                // Might not work everywhere
                //IFormFile file = productViewModel.ImageFile;

                IFormFile file = Request.Form.Files.FirstOrDefault();

                // copy the file uploaded using the MemoryStream into the Product.Image
                using (var dataStream = new MemoryStream())
                {
                    await file.CopyToAsync(dataStream);

                    // convert to bytearray for uploading pics
                    editProduct.Image = dataStream.ToArray();
                }
            }

            try
            {
                // update the database
                _context.Products.Update(editProduct);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));

            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("Edit", "Unable to update the Database. Please inform IT!");
                _logger.LogError($"Edit Product failed: {ex.Message}");
                return View(productViewModel);
            }
        }

        // GET: ProductsController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ProductsController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
