using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AngularJSWebApi.Models
{
    public class RouteRepository:IRouteRepository
    {
        private DatabaseEntity7 db = new DatabaseEntity7();

        public IEnumerable<Route> GetAll()
        {
            return db.Routes.ToList();
        }

        public Route GetRouteById(int id)
        {
            return db.Routes.Find(id);
        }

        public Route AddRoute(Route b)
        {
            if (b == null)
            {
                throw new NullReferenceException("Bus is null");
            }
            db.Routes.Add(b);
            db.SaveChanges();
            return b;
        }

        public bool UpdateRoute(Route b)
        {
            if (b == null)
            {
                throw new ArgumentNullException("Bus is Empty");
            }
            db.Entry(b).State = EntityState.Modified;
            db.SaveChanges();
            return true;
        }

        public bool RemoveRoute(int id)
        {
            var b = db.Routes.Find(id);
            if (b == null)
            {
                return false;
            }
            db.Routes.Remove(b);
            db.SaveChanges();
            return true;
        }
    }
}