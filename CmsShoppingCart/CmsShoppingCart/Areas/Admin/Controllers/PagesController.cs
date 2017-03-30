using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.MappingViews;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CmsShoppingCart.Models.Data;
using CmsShoppingCart.Models.ViewModels.Pages;

namespace CmsShoppingCart.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
        // GET: Admin/Pages
        public ActionResult Index()
        {
            //Declaare the list of PageVM
            List<PageVM> pageList;

            
            using (Db db=new Db())
            {
                //Init the list
                pageList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();

            }

            return View(pageList);
        }

        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }

        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            //Check Model State
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db= new Db())
            {
                //Delare the Slug
                string slug;

                //Init PageDTO
                PageDTO dto=new PageDTO();

                //DTO Title
                dto.Title = model.Title;

                //Check for and Set slug if need be
                if (string.IsNullOrEmpty(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();

                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }
              

                //Make sure title and slug are unique
                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("","That title or slug already exists.");
                    return View(model);
                }
                
                //DTO the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;


                //Save DTO
                db.Pages.Add(dto);
                db.SaveChanges();
            }

            //Set the TempData Message
            TempData["SM"] = "You have add a new page!";

            //Redirect
            return RedirectToAction("AddPage");

            
        }


        // Get: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            //Delare the PageVM
            PageVM model;


            using (Db db=new Db())
            {
                //Get the Page
                PageDTO dto = db.Pages.Find(id);

                //Confirm the Page exists
                if (dto == null)
                {
                    return Content("The page does not exists");
                };
                
                //Init PageVM
                model=new PageVM(dto);

            }




            return View(model);
        }


        // Post: Admin/Pages/EditPage/id
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            //Check the Model State
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db=new Db())
            {
                //Get the page id
                int id = model.Id;

                //Init slug
                string slug="home";

                //get the page
                PageDTO dto = db.Pages.Find(id);


                //DTO the Title
                dto.Title = model.Title;


                //Check for Slug and set it if need be
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }

                }


                // Make sure the Title and slug are unique
                if (db.Pages.Where(x=> x.Id !=id).Any(x => x.Title == model.Title) || 
                    db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("","The title or slug already exists ");
                    return View(model);
                } 
                

                //DTO the rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;


                //Save the DTO
                db.SaveChanges();
            }



            //Set TempData Message
            TempData["SM"] = "You have edited the page!";

            //Reidrect
            return RedirectToAction("EditPage");
        }

        //Get: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            //Delare the PageVM
            PageVM model;

            using (Db db=new Db())
            {
                //Get the page
                PageDTO dto = db.Pages.Find(id);

                //Confirm page exists
                if (dto == null)
                {
                    return Content("The page does not exists");
                }
                
                //int PageVM
                model=new PageVM(dto);
            }

            return View(model);
        }


        //Get: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            using (Db db=new Db())
            {
                //get the Page
                PageDTO dto = db.Pages.Find(id);

                //remove the page
                db.Pages.Remove(dto);

                //save
                db.SaveChanges();
            }
            //redirect

            return RedirectToAction("Index");
        }


        //Post: Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (Db db =new Db())
            {
                //Set the Inital count
                int count = 1;

                //Declare the PageDTO
                PageDTO dto;

                //Set sorting for each page
                foreach (var PageId in id)
                {
                    dto = db.Pages.Find(PageId);
                    dto.Sorting = count;
                    db.SaveChanges();

                    count++;
                }
            }




        }


        //GET: Admin/Pages/EditSidebar
        [HttpGet]
        public ActionResult EditSidebar()
        {
            //Delcare Model
            SidebarVM model;

            using (Db db=new Db())
            {
                //get the DTO
                SidebarDTO dto = db.Sidebar.Find(1);

                //Init Model
                model = new SidebarVM(dto);


            }

            return View(model);
        }



        //POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db=new Db())
            {
                
           
            //Get the DTO
                SidebarDTO dto = db.Sidebar.Find(1);

                //DTO the Body
                dto.Body = model.Body;

                //Save
                db.SaveChanges();

            }

            //Set the Temp Data Messgae
            TempData["SM"] = "You have edited the sidebar!";
            return RedirectToAction("EditSidebar");
        }



    }
}