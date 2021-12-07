using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Data;
using ProjectTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectTracker.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ProjectController(ApplicationDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            IEnumerable<Project> objList = _db.Projects;
            return View(objList);
        }

        // GET-Create
        public IActionResult Create()
        {
            //IEnumerable<Project> objList = _db.Projects;
            return View();
        }

        // POST-Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Project project)
        {
            if (ModelState.IsValid)
            {
                //IEnumerable<Project> objList = _db.Projects;
                _db.Projects.Add(project);
                project.Created = DateTime.Now;
                _db.SaveChanges();
                return RedirectToAction("Index");

            }
            return View(project);

        }

        public IActionResult Delete(int? id)
        {

            if (id == null || id == 0)
            {
                return NotFound();
            }

            var project = _db.Projects.Find(id);
            
            if (project == null)
            {
                return NotFound();
            }
            ViewBag.Created = project.Created;
            return View(project);

        }

        // POST-Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePost(int? id)
        {

            var project = _db.Projects.Find(id);
            if (project == null)
            {
                return NotFound();
            }


            _db.Projects.Remove(project);

            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET-Update
        public IActionResult Update(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }


            var project = _db.Projects.Find(id);
            if (project == null)
            {
                return NotFound();
            }
            ViewBag.Created = project.Created;

            return View(project);
        }

        // POST-Update
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(Project project)
        {
            if (ModelState.IsValid)
            {
                _db.Projects.Update(project);
                
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(project);
        }
    }
}
