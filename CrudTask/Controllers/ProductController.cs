using CrudTask.Data;
using CrudTask.Models;
using CrudTask.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrudTask.Controllers
{
    public class ProductController : Controller
    {
        DbSet<Category> categories1;
        private readonly AppDbContext _appDbContext;

        public ProductController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            categories1 = _appDbContext.Categories;
        }


        public IActionResult Index()
        {
            var products = _appDbContext.Products.Include(p => p.Category).ToList();
            return View(products);
        }


        public IActionResult Add()
        {
            var categories = _appDbContext.Categories.ToList();
            var tags = _appDbContext.Tags.ToList();
            ViewData["Categories"] = categories;
            ViewData["Tags"] = tags;
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Add(AddProductViewModel product)
        {
            if (ModelState.IsValid)
            {
                //var newProduct = new Product
                //{
                //    Category = product.Category,
                //    Price = product.Price,
                //    Description = product.Description,
                //    ImageUrl = product.ImageUrl,
                //    Title = product.Title,
                //};
                //_appDbContext.Products.Add(newProduct);
                //_appDbContext.Products.Add(product);

                var tags = _appDbContext.Tags.Where(t => product.TagIds.Contains(t.Id)).ToList();

                var newProduct = new Product
                {
                    CategoryId = product.CategoryId,
                    Price = product.Price,
                    Description = product.Description,
                    ImageUrl = product.ImageUrl,
                    Title = product.Title,
                    Tags = tags
                };
                _appDbContext.Products.Add(newProduct);
                await _appDbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(product);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _appDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product is not null)
            {
                _appDbContext.Products.Remove(product);
                await _appDbContext.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var product =  _appDbContext.Products.FirstOrDefault(x => x.Id == id);

            return View(product);
        }

        public  IActionResult EditProduct(Product pr)
        {
            var oldProduct = _appDbContext.Products.FirstOrDefault(i => i.Id == pr.Id);
            _appDbContext.Entry(oldProduct).CurrentValues.SetValues(pr);
            _appDbContext.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
