using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Data;
using ProjectTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectTracker.Controllers
{
    public class TicketController : Controller
    {
        private readonly ApplicationDbContext _db;
        public TicketController(ApplicationDbContext db)
        {
            _db = db;
        }

        //public IActionResult Index(int? id, string sortOrder, string searchString, int? pageNumber)
        //PAGINATION
        public async Task<IActionResult> Index(int? id, string sortOrder, string currentFilter, string searchString, int? pageNumber)
        {
            

            if (id == null) //standard view
            {
                //pagination
                ViewData["CurrentSort"] = sortOrder;                
                //p1
                ViewBag.IdSortParm = String.IsNullOrEmpty(sortOrder);
                //ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
                ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
                ViewBag.ProjectSortParm = sortOrder == "Project" ? "proj_desc" : "Project";
                ViewBag.StatusSortParm = sortOrder == "Status" ? "status_desc" : "Status";
                //var items = _db.Items.OrderBy(x => x.Id).Include(x => x.Project);

                //PAGINATION
                if (searchString != null)
                {
                    pageNumber = 1;
                }
                else
                {
                    searchString = currentFilter;
                }
                //p2
                //PAGIATION
                ViewData["CurrentFilter"] = searchString;
                //p3
                var items = from x in _db.Tickets select x;
                if (!String.IsNullOrEmpty(searchString))
                {
                    items = items.Where(x => x.Name.Contains(searchString)
                                || x.Description.Contains(searchString));
                }

                switch (sortOrder)
                {
                    case "Name":
                        items = items.OrderBy(x => x.Name).Include(x => x.Project);
                        break;
                    case "name_desc":
                        items = items.OrderByDescending(x => x.Name).Include(x => x.Project);
                        break;
                    case "Date":
                        items = items.OrderBy(x => x.Created).Include(x => x.Project);
                        break;
                    case "date_desc":
                        items = items.OrderByDescending(x => x.Created).Include(x => x.Project);
                        break;
                    case "Project":
                        items = items.OrderBy(x => x.ProjectId).Include(x => x.Project);
                        break;
                    case "proj_desc":
                        items = items.OrderByDescending(x => x.ProjectId).Include(x => x.Project);
                        break;
                    case "Status":
                        items = items.OrderBy(x => x.Status).Include(x => x.Project);
                        break;
                    case "status_desc":
                        items = items.OrderByDescending(x => x.Status).Include(x => x.Project);
                        break;
                    default:
                        items = items.OrderBy(x => x.Id).Include(x => x.Project);
                        break;

                }
                //PAGINATION
                int pageSize = 8;
                return View(await PaginatedList<Ticket>.CreateAsync(items.AsNoTracking(), pageNumber ?? 1, pageSize));
                //p4


                //return View(items.ToList());
            }

            else //if redirected from views/project/index (associated items) --filters by tickets attached to that project
            {
                //Project project = _db.Projects.Where(x => x.Id == id).FirstOrDefault();
                var items = _db.Tickets.OrderBy(x => x.Id).Include(x => x.Project).Where(x => x.ProjectId == id);

                //PAGINATION
                int pageSize = 8;
                return View(await PaginatedList<Ticket>.CreateAsync(items.AsNoTracking(), pageNumber ?? 1, pageSize));
                //p5

                //return View(items.ToList());
            }


        }



        [Authorize]
        // GET-Create
        public IActionResult Create()
        {
            ViewBag.ProjectId = new SelectList(_db.Projects.OrderBy(x => x.Id), "Id", "Name");
            //_db.Users.ToList();
            ViewBag.UserList = new SelectList(_db.Users.ToList());
            //ViewBag.UserList = _db.Users.ToList();
            //var users = _db.Users.ToList();
            
            return View();
        }

        // POST-Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Ticket ticket)
        {

            if (ModelState.IsValid)
            {
                _db.Tickets.Add(ticket);

                //ticket.Priority = User.Identity.Name;
                ticket.Submitter = User.Identity.Name;
                ticket.Created = DateTime.Now;
                ticket.Modified = DateTime.Now;
                
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ticket);
        }

        [Authorize]
        // GET - DELETE
        public IActionResult Delete(int? id)
        {

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var ticket = _db.Tickets.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewBag.Created = ticket.Created;
            return View(ticket);

        }

        // POST-Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {

            var ticket = _db.Tickets.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }


            _db.Tickets.Remove(ticket);

            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        [Authorize]
        // GET-Update
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            ViewBag.ProjectId = new SelectList(_db.Projects.OrderBy(x => x.Id), "Id", "Name");

            var ticket = _db.Tickets.Find(id);
            if (ticket == null)
            {
                return NotFound();
            }

            ViewBag.Status = ticket.Status;
            ViewBag.Priority = ticket.Priority;
            ViewBag.Type = ticket.Type;

            return View(ticket);
        }

        // POST-Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _db.Tickets.Update(ticket);
                ticket.Modified = DateTime.Now;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(ticket);
        }

        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            //ViewBag.ProjectId = new SelectList(_db.Projects.OrderBy(x => x.Id).Include(x => x.Project), "Id", "Name");

            var ticket = _db.Tickets.Find(id);
            
            if (ticket == null)
            {
                return NotFound();
            }

            //
            //ViewBag.ProjectId = ticket.ProjectId;
            var project = _db.Projects.Find(ticket.ProjectId);
            //ViewBag.ProjectName = project.Name;
            //

            return View(ticket);

        }
    }
}
