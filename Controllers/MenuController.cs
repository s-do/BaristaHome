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
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

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
        public async Task<IActionResult> AddItem([Bind("DrinkName,Instructions,Description,DrinkImageData,DrinkVideo,StoreId,Image,DrinkTags")] Drink drink, List<string> tagList, List<string> ingredientList)
        {
            /*ALEX ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
            //Store Id
            var storeId = Convert.ToInt32(User.FindFirst("StoreId").Value);

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

            //Fix youtube link
            if ((drink.DrinkVideo != null) && drink.DrinkVideo.Contains("youtube.com"))
            {
                var temp = drink.DrinkVideo.Split("watch?v=");
                drink.DrinkVideo = temp[0] + "embed/" + temp[1];
            }
            else
            {
                drink.DrinkVideo = null;
            }

            if (ModelState.IsValid)
            {
                _context.Add(drink);
                await _context.SaveChangesAsync();

                foreach (var ing in ingredientList)
                {
                    //Check for existing ingredients
                    var existingIng = (from i in _context.Ingredient
                                       where i.IngredientName == ing
                                       select i).FirstOrDefault();
                    //Add new ingredient
                    if (existingIng == null)
                    {
                        //Add to db
                        Ingredient newIngredient = new Ingredient { IngredientName = ing };
                        _context.Add(newIngredient);
                        await _context.SaveChangesAsync();

                        //Get Ingredient Id
                        var id = (from i in _context.Ingredient
                                        where i == newIngredient
                                        select i.IngredientId).FirstOrDefault();
                        DrinkIngredient drinkIngredient = new DrinkIngredient
                        {
                            DrinkId = drink.DrinkId,
                            IngredientId = id
                        };
                        _context.Add(drinkIngredient);
                        await _context.SaveChangesAsync();
                    }
                    //If ingredient exists in db
                    else
                    {
                        DrinkIngredient drinkIngredient = new DrinkIngredient
                        {
                            DrinkId = drink.DrinkId,
                            IngredientId = existingIng.IngredientId
                        };
                        _context.Add(drinkIngredient);
                        await _context.SaveChangesAsync();
                    }
                }
            /*ALEX ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

                /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
                if(tagList != null)
                {
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
                            //Gets the DrinkTag with the drink id and tag id
                            var existingDrinkTag = (from d in _context.Drink
                                                    join dt in _context.DrinkTag on d.DrinkId equals dt.DrinkId
                                                    join t in _context.Tag on dt.TagId equals t.TagId
                                                    where drink.DrinkId == dt.DrinkId && existingTag.TagId == dt.TagId
                                                    select dt).FirstOrDefault();

                            //Checks if there is an existing DrinkTag
                            //This check is for when users enter in the same tag twice
                            //Duplicated tag is not added to the DrinkTag db
                            if (existingDrinkTag == null)
                            {
                                //If DrinkTag doesn't exist yet, add it into the DrinkTag db
                                DrinkTag drinkTag = new DrinkTag
                                {
                                    DrinkId = drink.DrinkId,
                                    TagId = existingTag.TagId
                                };
                                _context.Add(drinkTag);
                                await _context.SaveChangesAsync();
                            }

                        }
                        /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
                    }
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
            /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

            return View(drinkList);
        }

        /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        //Displays the filtered menu
        [HttpPost]
        public async Task<IActionResult> Menu(string tagLine)
        {
            if(tagLine != null)
            {
                // Converting the x,y,z,... string to an int list
                List<int> tagList = tagLine.Split(',').Select(int.Parse).ToList();

                var filteredDrinks = (from dt in _context.DrinkTag
                                     .Where(dt => tagList.Contains(dt.TagId))                 // get the drinktags that contain any of the ids in tagList
                                      join d in _context.Drink on dt.DrinkId equals d.DrinkId  // then joining with drink to return the drink obj
                                      select d).Distinct(); // ensure distinct drinks to prevent multiple same objs

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

            // If we reached here, that means we selected nothing to filter, so we must requery the drinks of the store again to redisplay onto the view
            var storeId = Convert.ToInt32(User.FindFirst("StoreId").Value);
            var drinkList = (IEnumerable<Drink>)from d in _context.Drink
                                                where d.StoreId == storeId
                                                orderby d.DrinkId descending
                                                select d;
            return View(drinkList);   
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
            
            else
            {
                /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
                //Gets a list of tags that a drink has
                List<Tag> drinkTagQuery = (from d in _context.Drink
                                      join drinkTag in _context.DrinkTag on d.DrinkId equals drinkTag.DrinkId
                                      join tag in _context.Tag on drinkTag.TagId equals tag.TagId
                                      where d.DrinkId == drink.DrinkId
                                      select tag).ToList();
                ViewBag.DrinkTagList = drinkTagQuery;
                /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

                /*ALEX ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
                //Gets ingredients of drink
                List<Ingredient> ingredientQuery = (from d in _context.Drink
                                                    join drinkIngredient in _context.DrinkIngredient on d.DrinkId equals drinkIngredient.DrinkId
                                                    join ingredient in _context.Ingredient on drinkIngredient.IngredientId equals ingredient.IngredientId
                                                    where d.DrinkId == drink.DrinkId
                                                    select ingredient).ToList();
                ViewBag.IngredientList = ingredientQuery;
                /*ALEX ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
            }
            

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

            //Gets ingredients of drink
            List<Ingredient> ingredientQuery = (from d in _context.Drink
                                  join drinkIngredient in _context.DrinkIngredient on d.DrinkId equals drinkIngredient.DrinkId
                                  join ingredient in _context.Ingredient on drinkIngredient.IngredientId equals ingredient.IngredientId
                                  where d.DrinkId == drink.DrinkId
                                  select ingredient).ToList();
            ViewBag.IngredientList = ingredientQuery;

            /*ALEX ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

            /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
            //Gets a list of tags that a drink has
            List<Tag> tagQuery = (from d in _context.Drink
                                       join drinkTag in _context.DrinkTag on d.DrinkId equals drinkTag.DrinkId
                                       join tag in _context.Tag on drinkTag.TagId equals tag.TagId
                                       where d.DrinkId == drink.DrinkId
                                       select tag).ToList();
            ViewBag.TagList = tagQuery;
            /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
            return View(drink);
        }


        //POST Edit Drink details
        [HttpPost]
        public async Task<IActionResult> EditItem([Bind("DrinkId,DrinkName,Description,Instructions,DrinkImageData,DrinkVideo,StoreId,Image")] Drink drink, List<string> tagList, List<string> ingredientList)
        {
            /*ALEX ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
            List<DrinkIngredient> drinkIngredients = (from d in _context.Drink
                                                      join di in _context.DrinkIngredient on d.DrinkId equals di.DrinkId
                                                      where di.DrinkId == drink.DrinkId
                                                      select di).ToList();


            List<Ingredient> ingredients = (from i in _context.Ingredient
                                            join di in _context.DrinkIngredient on i.IngredientId equals di.IngredientId
                                            join d in _context.Drink on di.DrinkId equals d.DrinkId
                                            where d.DrinkId == drink.DrinkId
                                            select i).ToList();

            //Removing old ingredients from drink
            foreach (var existingIngredient in ingredients)
            {
                var found = false;
                foreach (var ing in ingredientList)
                {
                    if (ing == existingIngredient.IngredientName)
                    {
                        found = true;
                    }
                }
                //Remove ingredient if not found in ingredient is not found in new list
                if (!found)
                {
                    //Find Ingredient
                    var foundIngredient = (from i in _context.Ingredient
                                            where i.IngredientId == existingIngredient.IngredientId
                                            select i).FirstOrDefault();
                    //Remove junction
                    var DI = (from di in _context.DrinkIngredient
                              where di.IngredientId == foundIngredient.IngredientId
                              select di).First();
                    _context.DrinkIngredient.Remove(DI);
                    await _context.SaveChangesAsync();
                    //Delete ingredient from db if no other drinks use ingredient
                    var count = (from di in _context.DrinkIngredient
                                 where di.IngredientId == foundIngredient.IngredientId
                                 select di).Count();
                    if (count <= 0)
                    {
                        _context.Ingredient.Remove(foundIngredient);
                    }
                               
                }
            }

            //Adding new ingredients to drink
            var newList = ingredientList;
            var allIngredients = (from i in _context.Ingredient
                                  select i).ToList();
            foreach (var ingredient in newList)
            {
                var checkIngredient = (from i in _context.Ingredient
                                     where i.IngredientName == ingredient
                                     select i).FirstOrDefault() as Ingredient;
                //If ingredient exists
                if (allIngredients.Contains(checkIngredient))
                {
                    //if drink does not have ingredient
                    if (!ingredients.Contains(checkIngredient))
                    {
                        DrinkIngredient drinkIngredient = new DrinkIngredient
                        {
                            DrinkId = drink.DrinkId,
                            IngredientId = checkIngredient.IngredientId
                        };
                        _context.Add(drinkIngredient);
                        await _context.SaveChangesAsync();
                    }
                }
                //If ingredient does not exist
                else
                {
                    //Add ingredient to db
                    Ingredient newIngredient = new Ingredient { IngredientName = ingredient };
                    _context.Add(newIngredient);
                    await _context.SaveChangesAsync();

                    //Get Ingredient Id
                    var id = (from i in _context.Ingredient
                              where i == newIngredient
                              select i.IngredientId).FirstOrDefault();

                    //Create junction
                    DrinkIngredient drinkIngredient = new DrinkIngredient
                    {
                        DrinkId = drink.DrinkId,
                        IngredientId = id
                    };
                    _context.Add(drinkIngredient);
                    await _context.SaveChangesAsync();
                }
            }


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

            //Format embed link
            if ((drink.DrinkVideo != null) && drink.DrinkVideo.Contains("youtube.com"))
            {
                var temp = drink.DrinkVideo.Split("watch?v=");
                drink.DrinkVideo = temp[0] + "embed/" + temp[1];
            }
            else
            {
                drink.DrinkVideo = null;
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(drink);
                    await _context.SaveChangesAsync();
                    /*ALEX ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

                    /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
                    /* 
                     * delete drink tags with old tags from DrinkTag db
                     * if drink tag doesnt exist: check if tag exists in tag db, 
                     *      - if tag exists, make drinktag
                     *      - if tag doesnt exist: make tag, check if drinktag alrdy exists, make drink tag
                     * check if there are any tags not associated with a drink
                     *      - if a tag is used in no drinks, delete from the tags database
                     * 
                     */
                    if (tagList != null)
                    {
                        //Gets all the tag names of the old tags of the Drink
                        var oldTags = (from d in _context.Drink
                                       join dt in _context.DrinkTag on d.DrinkId equals dt.DrinkId
                                       join t in _context.Tag on dt.TagId equals t.TagId
                                       where drink.DrinkId == dt.DrinkId
                                       select t.TagName).ToList();

                        //If drink has existing old tags
                        if (oldTags != null)
                        {
                            //Returns a list of tags that are no longer used by the drink
                            var deleteOldTags = oldTags.Except(tagList);

                            if (deleteOldTags != null)
                            {
                                foreach (var ot in deleteOldTags)
                                {
                                    //Gets the tag id of the tag that is going to be deleted
                                    var deleteTag = (from t in _context.Tag
                                                     where t.TagName == ot
                                                     select t.TagId).FirstOrDefault();

                                    //Gets DrinkTag with the tag that is no longer being used
                                    var deleteDrinkTag = (from dt in _context.DrinkTag
                                                          join d in _context.Drink on dt.DrinkId equals d.DrinkId
                                                          join t in _context.Tag on dt.TagId equals t.TagId
                                                          where drink.DrinkId == dt.DrinkId && dt.TagId == deleteTag
                                                          select dt).FirstOrDefault();

                                    //Deletes Drink Tags with old tag from DrinkTag db
                                    _context.DrinkTag.Remove(deleteDrinkTag);
                                    await _context.SaveChangesAsync();

                                }
                            }
                        }

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
                            //If tag already exists then add to DrinkTag database with the drink and associated tag ids
                            else
                            {
                                //Gets the DrinkTag with the drink id and tag id
                                var existingDrinkTag = (from d in _context.Drink
                                                        join dt in _context.DrinkTag on d.DrinkId equals dt.DrinkId
                                                        join t in _context.Tag on dt.TagId equals t.TagId
                                                        where drink.DrinkId == dt.DrinkId && existingTag.TagId == dt.TagId
                                                        select dt).FirstOrDefault();

                                //Checks if there is an existing DrinkTag
                                if (existingDrinkTag == null)
                                {
                                    //If DrinkTag doesn't exist yet, add it into the DrinkTag db
                                    //This check is for when users enter in the same tag twice
                                    //Duplicated tag is not added to the DrinkTag db
                                    DrinkTag drinkTag = new DrinkTag
                                    {
                                        DrinkId = drink.DrinkId,
                                        TagId = existingTag.TagId
                                    };
                                    _context.Add(drinkTag);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }
                    }
                   
                    //Gets all the tag IDs from the tag db
                    var allTags = (from t in _context.Tag
                                   select t.TagId).ToList();

                    //Gets all the tag IDs from the drinktag db
                    var allUsedTag = (from d in _context.Drink
                                            join dt in _context.DrinkTag on d.DrinkId equals dt.DrinkId
                                            join t in _context.Tag on dt.TagId equals t.TagId
                                            where dt.TagId == t.TagId
                                            select dt.TagId).Distinct();
                    
                    //Gets all the tags that are not associated with any drinks
                    var deleteUnusedTag = allTags.Except(allUsedTag);

                    //If a tag is used in no drinks, delete from the Tags db
                    if (deleteUnusedTag != null)
                    {
                        foreach (var t in deleteUnusedTag)
                        {
                            var tag = await _context.Tag.FindAsync(t);
                            _context.Tag.Remove(tag);
                            await _context.SaveChangesAsync();
                        }
                    }

                    

                    /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
                return RedirectToAction("Menu", "Menu");
            }
            return View(drink);
            
        }

        /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/
        //[HttpGet]
        /*public async Task<IActionResult> Delete(int? id)
        {
            var a = id;
            if (id == null)
            {
                return NotFound();
            }

            var drink = await _context.Drink.FirstOrDefaultAsync(m => m.DrinkId == id);

            if (drink == null)
            {
                return NotFound();
            }

            List<Tag> drinkTagQuery = (from d in _context.Drink
                                       join drinkTag in _context.DrinkTag on d.DrinkId equals drinkTag.DrinkId
                                       join tag in _context.Tag on drinkTag.TagId equals tag.TagId
                                       where d.DrinkId == drink.DrinkId
                                       select tag).ToList();
            ViewBag.DrinkTagList = drinkTagQuery;

            return View(drink);
        }*/

        // POST: Menu/Delete/DrinkId

        [HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int? drinkId, int? tagId)
        {
            var existingDrinkTag = (from dt in _context.DrinkTag
                                    join d in _context.Drink on dt.DrinkId equals d.DrinkId
                                    join t in _context.Tag on dt.TagId equals t.TagId
                                    where d.DrinkId == drinkId && t.TagId == tagId
                                    select dt).FirstOrDefault();

            //var z = existingDrinkTag;
            //var b = existingTag;
            /* foreach (var tag in existingTag)
             {
                 var dTag = await (from dt in _context.DrinkTag
                                   join d in _context.Drink on dt.DrinkId equals d.DrinkId
                                   join t in _context.Tag on dt.TagId equals t.TagId
                                   where d.DrinkId == id && t.TagName == tag
                                   select dt).FirstOrDefaultAsync();
                 _context.DrinkTag.Remove(dTag);
                 await _context.SaveChangesAsync();
             }*/
            //var dTag = await _context.DrinkTag.FindAsync(drinkId, tagId);
            _context.DrinkTag.Remove(existingDrinkTag);
            await _context.SaveChangesAsync();

            var drink = await _context.Drink
                .FirstOrDefaultAsync(m => m.DrinkId == drinkId);
            if (drink == null)
            {
                return NotFound();
            }

            return View(drink);


            /*var filteredDrinks = (from dt in _context.DrinkTag
                             .Where(dt => tagList.Contains(dt.TagId))                 // get the drinktags that contain any of the ids in tagList
                             join d in _context.Drink on dt.DrinkId equals d.DrinkId  // then joining with drink to return the drink obj
                             select d).Distinct();                                    // ensure distinct drinks to prevent multiple same objs
            */
            /*
                var tags = await (from s in _context.Store
                        join d in _context.Drink on s.StoreId equals d.StoreId
                        join dt in _context.DrinkTag on d.DrinkId equals dt.DrinkId
                        join t in _context.Tag on dt.TagId equals t.TagId
                        select t).ToListAsync();
                */

        }
        /*SELINA ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/

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
        //Method for returning and displaying all the store's Drink items that contain the search phrase in its name
        //If there are any filters/tags selected, it will return only drinks with matching filters/tags

        public async Task<IActionResult> ShowSearchResults(string SearchPhrase, string tagLine)
        {

            //Get current store ID
            var storeId = Convert.ToInt32(User.FindFirst("StoreId").Value);

            //Return a list of drinks that contain the search phrase in its name
            var drinkList = (List<Drink>)(from d in _context.Drink
                                                 where (d.StoreId == storeId && d.DrinkName.Contains(SearchPhrase))
                                                 orderby d.DrinkId descending
                                                 select d).ToList();

            if (tagLine == null)
            {
                return View(drinkList);
            }


            List<int> tagList = tagLine.Split(',').Select(int.Parse).ToList();
            int numOfTags = tagList.Count;
            int numOfMatchingTags = 0;
            List<Drink> resultDrinks = new List<Drink>();

            if ((tagLine != null) && (SearchPhrase == null))
            {
                Menu();
            }

            //If there are tags selected
            if (tagList.Any())
            {
                //If there are drinks that match the search phrase
                if (drinkList.Any())
                {
                    // For each drink that contains the search phrase
                    foreach (Drink d in drinkList)
                    {
                        numOfMatchingTags = 0;

                        // Get all tags belonging to the current drink
                        var drinkTagList = (IEnumerable<Tag>)(from dt in _context.DrinkTag
                                                              join t in _context.Tag on dt.TagId equals t.TagId
                                                              where d.DrinkId == dt.DrinkId
                                                              select t).ToList();
                        // Go thru each tag that the current drink has
                        foreach (Tag t in drinkTagList)
                        {
                            // Go thru each tag that the user selected 
                            foreach (int drinkTag in tagList)
                            {
                                // Check if they are the same tag
                                if (drinkTag.Equals(t.TagId))
                                {
                                    numOfMatchingTags += 1;
                                }

                                // If the drink contains all of the tags that the user selected, add the drink to the result list
                                if (numOfMatchingTags == numOfTags)
                                {
                                    resultDrinks.Add(d);
                                    numOfMatchingTags = 0;

                                }
                            }

                        }


                    }
                }
            }
            else
            {
                return View(drinkList);
            }


            //Return list of drinks to the .cshtml to be displayed
            return View(resultDrinks);
        }
        /*CINDIE ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^*/


    }

}
