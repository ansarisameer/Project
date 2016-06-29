using AngularJSWebApi.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace AngularJSWebApi.Controllers
{
    public class LoginController : Controller
    {
        static int count = 0;
        static int counter = 0;
        static int check1;
        private DatabaseEntity7 db = new DatabaseEntity7();
        public bool isValid(string name, string password)
        {
            string paswd = Encryptdata(password);
            var HasUser = db.User_details.Where(u => u.name == name).Where(u => u.password == paswd).Select(p => p).ToList();
            if (HasUser.Count == 0)
            {
                return false;
            }
            return true;
        }

        private static string Encryptdata(string password)
        {
            byte[] encode = Encoding.UTF8.GetBytes(password);
            return Convert.ToBase64String(encode);
        }

        [HttpGet]
        public ActionResult Login()
        {
            
            return View();
        }
      
        [HttpPost]
        public ActionResult Login(User_details user, string returnUrl)
        {
            var u = db.User_details.ToList();
            User_details u2 = new User_details();
            foreach(User_details u1 in u)
            {
                if (u1.name == user.name)
                {
                    u2.name = u1.name;
                    u2.password = u1.password;
                    u2.user_id = u1.user_id;
                }
            }
            int id = u2.user_id;
            Session["id"] = id;
            Session["name"] = u2.name;
            int lid = 1;
            if(Session["lid"]!=null)
            {
                lid = Convert.ToInt32(Session["lid"]);
            }
            if (ModelState.IsValid)
            {
                if (isValid(user.name, user.password))
                {
                    if (FormsAuthentication.IsEnabled)
                    {
                        FormsAuthentication.SetAuthCookie(user.name, false);
                    }
                    if (returnUrl != null)
                    {
                        if (Url.IsLocalUrl(returnUrl))
                        {
                            return Redirect(returnUrl);
                        }
                    }
                    else
                    {
                        if(lid==1)
                        {
                            return RedirectToAction("Cancel", "Login");
                        }
                       
                        else
                        {
                            return RedirectToAction("Index", "Login");
                        }
                    }
                }
                return RedirectToAction("Error", "Login");
            }
            else
            {
                return View("Error");
            }

        }

        public ActionResult Error()
        {
            
            return View();
        }

         public ActionResult Error1()
        {
            
            return View();
        }

         [Authorize]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Cancel", "Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register([Bind(Include = "name,date_of_birth,email_id,mobile_no,password,address")] User_details model)
        {
            if (ModelState.IsValid)
            {
                model.password = Encryptdata(model.password);
                db.User_details.Add(model);
                db.SaveChanges();
                FormsAuthentication.SetAuthCookie(model.name, false);
                return RedirectToAction("Login", "Login");
            }
            else
            {
                ModelState.AddModelError("", "Invalid Values");
            }
            return View(model);
        }
         [Authorize]
        public ActionResult Index()
        {

            string name = Convert.ToString(Session["name"]);
            var p = db.Passengers.Where(u => u.User_details.name == name).Select(p1 => p1).ToList();
            return View(p);        
          
        }
         [Authorize]
        [HttpGet]
        public ActionResult Details()
        {
             if(count<5)
             {
                 return View();
             }
             else
             {
                 ViewBag.count = count;
                 ViewBag.message = "Sorry,You cannot add more than 5";
                 return View();
             }
           
        }
        [Authorize]
        [HttpPost]
        public ActionResult Details([Bind(Include="name,age,gender")]Passenger p)
        {
         
            int uid=Convert.ToInt32(Session["id"]);
            Passenger p1 = new Passenger();
            p1.name = p.name;
            p1.age = p.age;
            p1.gender = p.gender;
            p1.user_id = uid;
            Session["Passenger"] = p1;
            if(ModelState.IsValid)
            {
                
                    db.Passengers.Add(p1);
                    db.SaveChanges();
                    count++;
                    return RedirectToAction("Index", "Login");
               
                //else
                //{
                //    ViewBag.message = "Sorry,You cannot add more than 5";
                //    return View();
                //}
               
            }
            return View();
        }
         [Authorize]
        public ActionResult Edit(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Passenger p = db.Passengers.Find(id);
            TempData["uid"] = p.user_id;
            if (p == null)
            {
                return HttpNotFound();
            }
         
            return View(p);
        }

         [Authorize]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "passenger_id,name,age,gender,user_id")]Passenger p)
        {
            p.user_id = Convert.ToInt32(TempData["uid"]);
            if (ModelState.IsValid)
            {
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
           
            return View(p);
        }
         [Authorize]
        public ActionResult Delete(int? id)
        {
           
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Passenger p = db.Passengers.Find(id);
            if (p == null)
            {
                return HttpNotFound();
            }
            return View(p);
        }

         [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete1(int id)
        {
            Passenger book = db.Passengers.Find(id);
            db.Passengers.Remove(book);
            db.SaveChanges();
            count--;
            return RedirectToAction("Index");
        }

         [Authorize]
         public ActionResult Confirm()
         {
             int uid = Convert.ToInt32(Session["id"]);
             double totalfare=0.0;
             List<Passenger> list = new List<Passenger>();
             foreach (Passenger p in db.Passengers.ToList())
             {
                 if (p.user_id == uid)
                 {
                   
                     list.Add(p);
                 }
             }
             //var list = db.Passengers.Where(p => p.user_id == uid).Select(b => b).ToList();
             Bus bus = Session["Obj"] as Bus;
             ViewBag.bus_no = bus.bus_no;
             ViewBag.bus_name = bus.bus_name;
             ViewBag.bus_src = bus.Route.source;
             ViewBag.bus_dest = bus.Route.destination;
             ViewBag.bus_time = bus.departure_time;
             ViewBag.bus_distance = bus.Route.distance;
             ViewBag.bus_jdate = bus.start_date;
             ViewBag.bus_type = bus.Category.type;
             double distance = Convert.ToDouble(bus.Route.distance);
             if(bus.Category.type=="AC")
             {
                 foreach(var passenger in list)
                 {
                     if(passenger.age>16)
                     {
                         totalfare += 2 * distance;
                     }
                     else if (passenger.age < 16 && passenger.age > 5)
                     {
                         totalfare += 1.75 * distance;
                     }
                     else 
                     {
                         totalfare += 0;
                     }
                 }
             }
             else if (bus.Category.type == "NON-AC")
             {
                 foreach (var passenger in list)
                 {
                     if (passenger.age > 16)
                     {
                         totalfare += 1.2 * distance;
                     }
                     else if (passenger.age < 16 && passenger.age>5)
                     {
                         totalfare += 1.05 * distance;
                     }
                     else
                     {
                         totalfare += 0;
                     }
                 }
             }
             else if (bus.Category.type == "SLEEPER")
             {
                 foreach (var passenger in list)
                 {
                     if (passenger.age > 16)
                     {
                         totalfare += 1.5* distance;
                     }
                     else if (passenger.age < 16 && passenger.age > 5)
                     {
                         totalfare += 1.25 * distance;
                     }
                     else 
                     {
                         totalfare += 0;
                     }
                 }
             }
             else
             {
                 foreach (var passenger in list)
                 {
                     if (passenger.age > 16)
                     {
                         totalfare += 1.35 * distance;
                     }
                     else if (passenger.age < 16 && passenger.age > 5)
                     {
                         totalfare += 1.25 * distance;
                     }
                     else 
                     {
                         totalfare += 0;
                     }
                 }
             }
             Reservation r = new Reservation();
             r.date_of_journey = bus.start_date;
             r.bus_id = bus.bus_id;
             r.user_id = Convert.ToInt32(Session["id"]);
             r.total_fare = Convert.ToInt32(totalfare);
             db.Reservations.Add(r);
             db.SaveChanges();
             ViewBag.fare = totalfare;
             Session["fare"] = totalfare;
             ViewBag.rid = r.reservation_id;
             Bus b1 = new Bus();
             b1.bus_id = bus.bus_id;
             b1.bus_name = bus.bus_name;
             b1.bus_no = bus.bus_no;
             b1.category_id = bus.category_id;
             b1.route_id = bus.route_id;
             b1.start_date = bus.start_date;
             b1.end_date = bus.end_date;
             b1.departure_time = bus.departure_time;
             b1.capacity = bus.capacity - counter;
             db.Entry(b1).State = EntityState.Modified;
             db.SaveChanges();
             return View(list);
         }

        [Authorize]
        public ActionResult Cancel()
        {
            db.Database.ExecuteSqlCommand("delete from Passenger");
            count = 0;
            return RedirectToAction("Index", "Search");
        }
        [Authorize]
        [HttpGet]
        public ActionResult Seat()
        {
                if(count==0)
                {
                    return RedirectToAction("Error1", "Login");
                }
                else
                {
                    return View(db.Bus_Seat.ToList());
                }
               
           
        }
        [Authorize]
        [HttpPost]
        public ActionResult Seat(string ida1, string ida2, string ida3, string ida4, string idb1, string idb2, string idb3, string idb4)
        {
            Bus b = Session["Obj"] as Bus;
            Passenger p = Session["Passenger"] as Passenger;
            List<Bus_Seat> list = db.Bus_Seat.ToList();
            
            var bus1 = db.Buses.Find(b.bus_id);
           
            if(ida1!="" && ida1!=null)
            {
                Bus_Seat seat = new Bus_Seat();
                seat.seat_no= ida1;
                seat.bus_id = b.bus_id;
                seat.status = "Filled";
                foreach(var bs1 in db.Bus_Seat.ToList())
                {
                    if(bs1.bus_id==b.bus_id)
                    {
                        if (list.Count == 0)
                        {
                            db.Bus_Seat.Add(seat);
                            db.SaveChanges();
                            counter++;

                        }
                        else
                        {
                            if (count == counter)
                            {
                                foreach (var bs in db.Bus_Seat.ToList())
                                {
                                    if (bs.seat_no != ida1 && bs.bus_id == b.bus_id)
                                    {
                                        db.Bus_Seat.Add(seat);
                                        db.SaveChanges();
                                        counter++;
                                        break;
                                    }
                                    else
                                    {
                                        ViewBag.dup = "Sorry,Seat Already Booked";
                                        return View();
                                    }
                                }

                            }
                            else
                            {
                                ViewBag.err = "Passengers selected are less when compared to seats";
                                return View();
                            }
                            
                        }
                
                    }
                    else
                    {
                        foreach (var bs in db.Bus_Seat.ToList())
                        {
                            
                            if (bs.bus_id != b.bus_id && check1==0 ) 
                            {
                                db.Bus_Seat.Add(seat);
                                db.SaveChanges();
                                counter++;
                                check1++;
                                break;
                            }
                            
                        }
                       
                    }
                }
               
               
               
            }
            if (ida2 != "" && ida2 != null)
            {
                Bus_Seat seat = new Bus_Seat();
                seat.seat_no = ida2;
                seat.bus_id = b.bus_id;
                seat.status = "Filled";
                 foreach(var bs1 in db.Bus_Seat.ToList())
                 {
                    if(bs1.bus_id==b.bus_id)
                    {
                        if (list.Count == 0)
                        {
                            db.Bus_Seat.Add(seat);
                            db.SaveChanges();
                            counter++;

                        }
                        else
                        {
                            if(count==counter)
                            {
                                foreach (var bs in db.Bus_Seat.ToList())
                                {
                                    if (bs.seat_no != ida2 && bs.bus_id == b.bus_id)
                                    {
                                        db.Bus_Seat.Add(seat);
                                        db.SaveChanges();
                                        counter++;
                                        break;
                                    }
                                    else
                                    {
                                        ViewBag.dup = "Sorry,Seat Already Booked";
                                        return View();
                                    }
                                }
                                
                            }
                            else
                            {
                                ViewBag.err = "Passengers selected are less when compared to seats";
                                return View();
                            }
                            
                        }
                
                    }
                    else
                    {
                        foreach (var bs in db.Bus_Seat.ToList())
                        {

                            if (bs.bus_id != b.bus_id && check1 == 0) 
                            {
                                db.Bus_Seat.Add(seat);
                                db.SaveChanges();
                                counter++;
                                check1++;
                                break;
                            }
                            
                        }
                       
                    }
                
               
               
                }
                
            }
            if (ida3 != "" && ida3 != null)
            {
                Bus_Seat seat = new Bus_Seat();
                seat.seat_no = ida3;
                seat.bus_id = b.bus_id;
                seat.status = "Filled";
                foreach (var bs1 in db.Bus_Seat.ToList())
                {
                    if (bs1.bus_id == b.bus_id)
                    {
                        if (list.Count == 0)
                        {
                            db.Bus_Seat.Add(seat);
                            db.SaveChanges();
                            counter++;

                        }
                        else
                        {
                            if (count == counter)
                            {
                                foreach (var bs in db.Bus_Seat.ToList())
                                {
                                    if (bs.seat_no != ida3 && bs.bus_id == b.bus_id)
                                    {
                                        db.Bus_Seat.Add(seat);
                                        db.SaveChanges();
                                        counter++;
                                        break;
                                    }
                                    else
                                    {
                                        ViewBag.dup = "Sorry,Seat Already Booked";
                                        return View();
                                    }
                                }

                            }
                            else
                            {
                                ViewBag.err = "Passengers selected are less when compared to seats";
                                return View();
                            }
                            
                        }

                    }
                    else
                    {
                        foreach (var bs in db.Bus_Seat.ToList())
                        {

                            if (bs.bus_id != b.bus_id && check1 == 0 )
                            {
                                db.Bus_Seat.Add(seat);
                                db.SaveChanges();
                                counter++;
                                check1++;
                                break;
                            }

                        }

                    }
                }
                
            }
            if (ida4 != "" && ida4 != null)
            {
                Bus_Seat seat = new Bus_Seat();
                seat.seat_no = ida4;
                seat.bus_id = b.bus_id;
                seat.status = "Filled";
                foreach (var bs1 in db.Bus_Seat.ToList())
                {
                    if (bs1.bus_id == b.bus_id)
                    {
                        if (list.Count == 0)
                        {
                            db.Bus_Seat.Add(seat);
                            db.SaveChanges();
                            counter++;

                        }
                        else
                        {
                            if (count == counter)
                            {
                                foreach (var bs in db.Bus_Seat.ToList())
                                {
                                    if (bs.seat_no != ida4 && bs.bus_id == b.bus_id)
                                    {
                                        db.Bus_Seat.Add(seat);
                                        db.SaveChanges();
                                        counter++;
                                        break;
                                    }
                                    else
                                    {
                                        ViewBag.dup = "Sorry,Seat Already Booked";
                                        return View();
                                    }
                                }

                            }
                            else
                            {
                                ViewBag.err = "Passengers selected are less when compared to seats";
                                return View();
                            }
                            
                        }

                    }
                    else
                    {
                        foreach (var bs in db.Bus_Seat.ToList())
                        {

                            if (bs.bus_id != b.bus_id && check1 == 0 )
                            {
                                db.Bus_Seat.Add(seat);
                                db.SaveChanges();
                                counter++;
                                check1++;
                                break;
                            }

                        }

                    }
                }
            }
         
            if (idb1 != "" && idb1 != null)
            {
                Bus_Seat seat = new Bus_Seat();
                seat.seat_no = idb1;
                seat.bus_id = b.bus_id;
                seat.status = "Filled";
                foreach (var bs1 in db.Bus_Seat.ToList())
                {
                    if (bs1.bus_id == b.bus_id)
                    {
                        if (list.Count == 0)
                        {
                            db.Bus_Seat.Add(seat);
                            db.SaveChanges();
                            counter++;

                        }
                        else
                        {
                            if (count == counter)
                            {
                                foreach (var bs in db.Bus_Seat.ToList())
                                {
                                    if (bs.seat_no != idb1 && bs.bus_id == b.bus_id)
                                    {
                                        db.Bus_Seat.Add(seat);
                                        db.SaveChanges();
                                        counter++;
                                        break;
                                    }
                                    else
                                    {
                                        ViewBag.dup = "Sorry,Seat Already Booked";
                                        return View();
                                    }
                                }

                            }
                            else
                            {
                                ViewBag.err = "Passengers selected are less when compared to seats";
                                return View();
                            }
                            
                        }

                    }
                    else
                    {
                        foreach (var bs in db.Bus_Seat.ToList())
                        {

                            if (bs.bus_id != b.bus_id && check1 == 0 )
                            {
                                db.Bus_Seat.Add(seat);
                                db.SaveChanges();
                                counter++;
                                check1++;
                                break;
                            }

                        }

                    }
                }
            }
            if (idb2 != "" && idb2 != null)
            {
                Bus_Seat seat = new Bus_Seat();
                seat.seat_no =idb2;
                seat.bus_id = b.bus_id;
                seat.status = "Filled";
                foreach (var bs1 in db.Bus_Seat.ToList())
                {
                    if (bs1.bus_id == b.bus_id)
                    {
                        if (list.Count == 0)
                        {
                            db.Bus_Seat.Add(seat);
                            db.SaveChanges();
                            counter++;

                        }
                        else
                        {
                            if (count == counter)
                            {
                                foreach (var bs in db.Bus_Seat.ToList())
                                {
                                    if (bs.seat_no != idb2 && bs.bus_id == b.bus_id)
                                    {
                                        db.Bus_Seat.Add(seat);
                                        db.SaveChanges();
                                        counter++;
                                        break;
                                    }
                                    else
                                    {
                                        ViewBag.dup = "Sorry,Seat Already Booked";
                                        return View();
                                    }
                                }

                            }
                            else
                            {
                                ViewBag.err = "Passengers selected are less when compared to seats";
                                return View();
                            }
                            
                        }

                    }
                    else
                    {
                        foreach (var bs in db.Bus_Seat.ToList())
                        {

                            if (bs.bus_id != b.bus_id && check1 == 0)
                            {
                                db.Bus_Seat.Add(seat);
                                db.SaveChanges();
                                counter++;
                                check1++;
                                break;
                            }

                        }

                    }
                }
                
            }
            if (idb3 != "" && idb3 != null)
            {
                Bus_Seat seat = new Bus_Seat();
                seat.seat_no = idb3;
                seat.bus_id = b.bus_id;
                seat.status = "Filled";
                foreach (var bs1 in db.Bus_Seat.ToList())
                {
                    if (bs1.bus_id == b.bus_id)
                    {
                        if (list.Count == 0)
                        {
                            db.Bus_Seat.Add(seat);
                            db.SaveChanges();
                            counter++;

                        }
                        else
                        {
                            if (count == counter)
                            {
                                foreach (var bs in db.Bus_Seat.ToList())
                                {
                                    if (bs.seat_no != idb3 && bs.bus_id == b.bus_id)
                                    {
                                        db.Bus_Seat.Add(seat);
                                        db.SaveChanges();
                                        counter++;
                                        break;
                                    }
                                    else
                                    {
                                        ViewBag.dup = "Sorry,Seat Already Booked";
                                        return View();
                                    }
                                }

                            }
                            else
                            {
                                ViewBag.err = "Passengers selected are less when compared to seats";
                                return View();
                            }
                            
                        }

                    }
                    else
                    {
                        foreach (var bs in db.Bus_Seat.ToList())
                        {

                            if (bs.bus_id != b.bus_id && check1 == 0)
                            {
                                db.Bus_Seat.Add(seat);
                                db.SaveChanges();
                                counter++;
                                check1++;
                                break;
                            }

                        }

                    }
                }
                
            }
            if (idb4 != "" && idb4 != null)
            {
                Bus_Seat seat = new Bus_Seat();
                seat.seat_no = idb4;
                seat.bus_id = b.bus_id;
                seat.status = "Filled";
                foreach (var bs1 in db.Bus_Seat.ToList())
                {
                    if (bs1.bus_id == b.bus_id)
                    {
                        if (list.Count == 0)
                        {
                            db.Bus_Seat.Add(seat);
                            db.SaveChanges();
                            counter++;

                        }
                        else
                        {
                            if (count == counter)
                            {
                                foreach (var bs in db.Bus_Seat.ToList())
                                {
                                    if (bs.seat_no != idb4 && bs.bus_id == b.bus_id)
                                    {
                                        db.Bus_Seat.Add(seat);
                                        db.SaveChanges();
                                        counter++;
                                        break;
                                    }
                                    else
                                    {
                                        ViewBag.dup = "Sorry,Seat Already Booked";
                                        return View();
                                    }
                                }

                            }
                            else
                            {
                                ViewBag.err = "Passengers selected are less when compared to seats";
                                return View();
                            }
                            
                        }

                    }
                    else
                    {
                        foreach (var bs in db.Bus_Seat.ToList())
                        {

                            if (bs.bus_id != b.bus_id && check1 == 0 )
                            {
                                db.Bus_Seat.Add(seat);
                                db.SaveChanges();
                                counter++;
                                check1++;
                                break;
                            }

                        }

                    }
                }
                
            }

            List<Bus_Seat> list2 = db.Bus_Seat.ToList();
            
            ViewBag.bid = b.bus_id;
            ViewBag.name = b.bus_name;
            foreach (var bs in list2)
            {
                if(bs.bus_id==bus1.bus_id)
                {
                    return View(list2);
                }
            }
            return View();
        }
      
	}
}