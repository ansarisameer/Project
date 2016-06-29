using AngularJSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AngularJSWebApi.Controllers
{
    public class SearchController : Controller
    {
        private DatabaseEntity7 db = new DatabaseEntity7();
        [HttpGet]
        public ActionResult Index()
        {
            
            return View();
        }
        [HttpPost]
        public ActionResult Search(string source,string destination,string jdate,string cid)
        {
            db.Database.ExecuteSqlCommand("delete from Passenger");
            ViewBag.cid = cid;
            if(cid=="-1" || jdate == "")
            {
                ViewBag.src = source;
                ViewBag.dest = destination;
                var bus = db.Buses.Where(b => b.Route.source == source.ToLower()).Where(b => b.Route.destination == destination.ToLower()).Select(p => p).ToList();
                ViewBag.message = bus.Count;
                return View(bus.ToList());
            }
            else
            {
                ViewBag.src = source.ToUpper();
                ViewBag.dest = destination.ToUpper();
                DateTime d = Convert.ToDateTime(jdate);
                var routes = db.Routes.ToList();
                Route r = new Route();
                //foreach (var cat in db.Categories.ToList())
                //{
                //    if (cat.category_id == Convert.ToInt32(cid))
                //    {
                //        Session["type"] = cat.type;
                //    }
                //}

                foreach (var route in routes)
                {
                    if (route.source == source.ToLower() && route.destination == destination.ToLower())
                    {
                        r.route_id = route.route_id;
                        r.distance = route.distance;
                    }
                }
              
                int id = Convert.ToInt32(cid);
                var bus = db.Buses.Where(b => b.category_id == id).Where(b => b.route_id == r.route_id).Where(b => b.start_date == d).Select(p => p).ToList();
                ViewBag.message = bus.Count;
                return View(bus.ToList());
            }
           
        }
      
        public ActionResult Book(int? id)
        {
            var name = Session["name"] as string;
            var bus = db.Buses.Find(id);
            Session["Obj"] = bus;
            if(name!=null)
            {
               
                return RedirectToAction("Index", "Login");
            }
            else
            {
              
                Session["lid"] = 2;
                return RedirectToAction("Login", "Login");
            }
            
        }
        public ActionResult Fare(int? id)
        {
            Bus bus = db.Buses.Find(id);
            ViewBag.bus_no = bus.bus_no;
            ViewBag.bus_name = bus.bus_name;
            ViewBag.bus_src = bus.Route.source;
            ViewBag.bus_dest = bus.Route.destination;
            ViewBag.bus_time = bus.departure_time;
            ViewBag.bus_distance = bus.Route.distance;
            string type = bus.Category.type;
            int distance =Convert.ToInt32(bus.Route.distance);
            Fare f = new Fare();
            if(type=="AC")
            {
                f.Type = type;
                f.A16 = 2 * distance;
                f.B16 = 1.75 * distance;
                f.U5 = 0;
                return View(f);
            }
            else if(type=="NON-AC")
            {
                f.Type = type;
                f.A16 = 1.2 * distance;
                f.B16 = 1.05 * distance;
                f.U5 = 0;
                return View(f);
            }
            else if(type=="SLEEPER")
            {
                f.Type = type;
                f.A16 = 1.5 * distance;
                f.B16 = 1.25 * distance;
                f.U5 = 0;
                return View(f);
            }
            else
            {
                f.Type = type;
                f.A16 = 1.35 * distance;
                f.B16 = 1.25 * distance;
                f.U5 = 0;
                return View(f);
            }
           
        }
        [Authorize]
        public ActionResult History()
        {
            
            int uid = Convert.ToInt32(Session["id"]);
         
            List<Reservation> list1 = new List<Reservation>();
            int c=0;
            foreach(var rd in db.Reservations.ToList())
            {
                if(rd.user_id==uid)
                {
                    c++;
                }
            }
            if(c==0)
            {
                ViewBag.count = c;
                return View();
            }
            else
            {
                foreach (var res in db.Reservations.ToList())
                {
                    if (res.user_id == uid)
                    {
                        list1.Add(res);
                       
                    }
                   
                }
            }


            return View(list1);
        }
	}
}