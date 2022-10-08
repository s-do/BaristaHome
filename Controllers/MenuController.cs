using BaristaHome.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.SqlServer;
using BaristaHome.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BaristaHome.Controllers
{
    [Authorize]
    public class MenuController : Controller
    {
        private readonly BaristaHomeContext _context;

        public MenuController(BaristaHomeContext context)
        {
            _context = context;
        }
        /*        public IActionResult Menu()
                {
                    return View();
                }*/

        /*ALEX ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        //Displays view for add drink
        [HttpGet]
        public IActionResult Additem()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        /*ALEX ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        //POST add a drink, drink tag, and tag to database
        [HttpPost]
        public async Task<IActionResult> AddItem([Bind("DrinkName,Instructions,Description,DrinkImageData,DrinkImage,StoreId,Image,DrinkTags")] Drink drink, List<string> tagList)
        {
            /*            var storeId = Convert.ToInt32(User.FindFirst("StoreId").Value);
                        drink.StoreId = storeId;*/
            //var d = tagList;

            //Store Id
            var storeId = Convert.ToInt32(User.FindFirst("StoreId").Value);

            /*ALEX ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
            if (drink.Image != null)
            {
                using (var ms = new MemoryStream())
                {
                    drink.Image.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    drink.DrinkImageData = fileBytes;
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(drink);
                await _context.SaveChangesAsync();
            /*ALEX ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

                /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
                foreach (var tag in tagList)
                {
                    //Checks Tag database to see if tag from list exists or not
                    var existingTag = (from t in _context.Tag
                                       where t.TagName == tag
                                       select t).FirstOrDefault();
                    //If tag does not exist yet, add it into the Tags database
                    if (existingTag == null)
                    {
                        Tag newTag = new Tag { TagName = tag };
                        _context.Add(newTag);
                        await _context.SaveChangesAsync();

                        var addedTag = (from t in _context.Tag
                                        where t == newTag
                                        select t.TagId).FirstOrDefault();

                        DrinkTag drinkTag = new DrinkTag
                        {
                            DrinkId = drink.DrinkId,
                            TagId = addedTag
                        };
                        _context.Add(drinkTag);
                        await _context.SaveChangesAsync();
                    }
                    //If tag already exists then add to DrinkTag database with the new drink and associated tag ids
                    else
                    {
                        DrinkTag drinkTag = new DrinkTag
                        {
                            DrinkId = drink.DrinkId,
                            TagId = existingTag.TagId
                        };
                        _context.Add(drinkTag);
                        await _context.SaveChangesAsync();
                    }
                    /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
                }
                return RedirectToAction("Menu", "Menu");
            }
            ModelState.AddModelError(string.Empty, drink.DrinkName);
            return View(drink);

        }

        //Displays view for menu
        [HttpGet]
        public async Task<IActionResult> Menu()
        {
            /*ALEX ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
            // Used to get drink list
            // Use type casting to return a IEnumerable<Model> with a LINQ query instead of doing await _context.Model.ToListAsync()
            var storeId = Convert.ToInt32(User.FindFirst("StoreId").Value);
            var drinkList = (IEnumerable<Drink>)from d in _context.Drink
                                                where d.StoreId == storeId
                                                orderby d.DrinkId descending
                                                select d;
            /*ALEX ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

            /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
            // To get tags that belong to a store from database
            var tags = (IEnumerable<Tag>)(from s in _context.Store
                              join d in _context.Drink on s.StoreId equals d.StoreId
                              join dt in _context.DrinkTag on d.DrinkId equals dt.DrinkId
                              join t in _context.Tag on dt.TagId equals t.TagId
                              where s.StoreId == storeId // forgot to filter by the user's store 
                              select t);
            ViewData["Tags"] = new SelectList(tags.Distinct(), "TagId", "TagName");
            /*List<Tag> tagQuery = (from tag in _context.Tag
                                  select new Tag
                                  {
                                      TagName = tag.TagName
                                  }).ToList();
            ViewBag.TagList = tagQuery;*/
            /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

            return View(drinkList);
        }

        /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        //Displays the filtered menu
        [HttpPost]
        public async Task<IActionResult> Menu(string tagLine)
        {

            // Converting the x,y,z,... string to an int list
            List<int> tagList = tagLine.Split(',').Select(int.Parse).ToList();

            var filteredDrinks = (from dt in _context.DrinkTag
                             .Where(dt => tagList.Contains(dt.TagId))                 // get the drinktags that contain any of the ids in tagList
                             join d in _context.Drink on dt.DrinkId equals d.DrinkId  // then joining with drink to return the drink obj
                             select d).Distinct();                                    // ensure distinct drinks to prevent multiple same objs

            // Recreating viewbag to display store's filters/tags again
            var tags = (IEnumerable<Tag>)(from s in _context.Store
                                          join d in _context.Drink on s.StoreId equals d.StoreId
                                          join dt in _context.DrinkTag on d.DrinkId equals dt.DrinkId
                                          join t in _context.Tag on dt.TagId equals t.TagId
                                          where s.StoreId == Convert.ToInt32(User.FindFirst("StoreId").Value)
                                          select t);
            ViewData["Tags"] = new SelectList(tags.Distinct(), "TagId", "TagName");       

            return View(filteredDrinks);           
        }
        /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

        [HttpGet]
        // GET: Display drink information of drink's page
        public async Task<IActionResult> Drink(int? id)
        {
            /*ALEX ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
            if (id == null)
            {
                return NotFound();
            }

            var drink = await _context.Drink
                .FirstOrDefaultAsync(m => m.DrinkId == id);
            if (drink == null)
            {
                return NotFound();
            }
            /*ALEX ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

            /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
            else
            {
                //Gets a list of tags that a drink has
                List<Tag> drinkTagQuery = (from d in _context.Drink
                                      join drinkTag in _context.DrinkTag on d.DrinkId equals drinkTag.DrinkId
                                      join tag in _context.Tag on drinkTag.TagId equals tag.TagId
                                      where d.DrinkId == drink.DrinkId
                                      select tag).ToList();
                ViewBag.DrinkTagList = drinkTagQuery;
            }
            /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

            return View(drink);
        }

        [HttpGet]
        // GET: Drink Details
        public async Task<IActionResult> EditItem(int? id)
        {
            /*ALEX ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
            if (id == null)
            {
                return NotFound();
            }

            var drink = await _context.Drink
                .FirstOrDefaultAsync(m => m.DrinkId == id);
            if (drink == null)
            {
                return NotFound();
            }
            /*ALEX ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

            /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
            else
            {
                //Gets a list of tags that a drink has
                List<Tag> drinkTagQuery = (from d in _context.Drink
                                           join drinkTag in _context.DrinkTag on d.DrinkId equals drinkTag.DrinkId
                                           join tag in _context.Tag on drinkTag.TagId equals tag.TagId
                                           where d.DrinkId == drink.DrinkId
                                           select tag).ToList();
                ViewBag.DrinkTagList = drinkTagQuery;
            }
            /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
            return View(drink);
        }


        //POST Edit Drink details
        [HttpPost]
        public async Task<IActionResult> EditItem([Bind("DrinkId,DrinkName,Description,Instructions,DrinkImageData,DrinkImage,StoreId,Image")] Drink drink, List<string> tagList)
        {
            /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

            /* SAME NUMBER OF TAGS
             * tags stay the same: {apple, juice} --> compare existing drink tags with list of tags if same, nothing happens
             * change all tags: {icy, cold} --> compare
             * change some tags: {apple, cold}
             * 
             * DIFFERENT NUMBER OF TAGS
             * tags stay the same and add new tags: {apple, juice, cold}
             * add all different tags and new amount: {icy, cold, fruit}
             * keep some previous tags and add new tags: {apple, red, cold}
             * 
             * removing all the tags and adding none
             */
            foreach (var t in tagList)
            {
                var existingDrinkTag = (from s in _context.Store
                                         join d in _context.Drink on s.StoreId equals d.StoreId
                                         join dt in _context.DrinkTag on d.DrinkId equals dt.DrinkId
                                         join tag in _context.Tag on dt.TagId equals tag.TagId
                                         where dt.DrinkId == drink.DrinkId
                                         select tag).ToList();
                existingDrinkTag.Sort();
                tagList.Sort();
                //When you edit list and have the same number of tags but the tags are different
                if ((existingDrinkTag.Count() == tagList.Count()) && (existingDrinkTag.Equals(tagList) == false))
                {
                    var existingTag = (from s in _context.Store
                                            join d in _context.Drink on s.StoreId equals d.StoreId
                                            join dt in _context.DrinkTag on d.DrinkId equals dt.DrinkId
                                            join tag in _context.Tag on dt.TagId equals tag.TagId
                                            where dt.DrinkId == drink.DrinkId
                                            select tag);
                }
            }
            /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

            /*ALEX ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
            var existingDrink = (from d in _context.Drink
                                 where d.DrinkName.Equals(drink.DrinkName) && !d.DrinkId.Equals(drink.DrinkId)
                                 select d).FirstOrDefault();

            if (existingDrink != null)
            {
                ModelState.AddModelError(string.Empty, "Drink name in use");
                return View(drink);
            }

            if (drink.Image != null)
            {
                using (var ms = new MemoryStream())
                {
                    drink.Image.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    drink.DrinkImageData = fileBytes;
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(drink);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction("Menu", "Menu");
            }
            return View(drink);
            /*ALEX ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        }

        /*ALEX ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        //Method for rendering images
        public async Task<ActionResult> RenderImage(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drink = await _context.Drink
                .FirstOrDefaultAsync(m => m.DrinkId == id);
            if (drink == null)
            {
                return NotFound();
            }
            var image = (from d in _context.Drink
                         where d.DrinkId == drink.DrinkId
                         select drink.DrinkImageData).First();


            return File(image, "image/png");
        }
        /*ALEX ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


        /*CINDIE ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        public async Task<IActionResult> ShowSearchResults(string SearchPhrase)
        {
            var storeId = Convert.ToInt32(User.FindFirst("StoreId").Value);
            var drinkList = (IEnumerable<Drink>)(from d in _context.Drink
                                                 where (d.StoreId == storeId && d.DrinkName.Contains(SearchPhrase))
                                                 orderby d.DrinkId descending
                                                 select d).ToList();
            return View(drinkList);
        }
        /*CINDIE ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
    }
}
