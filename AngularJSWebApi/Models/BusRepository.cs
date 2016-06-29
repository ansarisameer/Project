using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AngularJSWebApi.Models
{
    public class BusRepository:IBusRepository
    {
        private DatabaseEntity7 db = new DatabaseEntity7();

        public IEnumerable<Bus> GetAll()
        {
            return db.Buses.ToList();
        }

        public Bus GetBusById(int id)
        {
            return db.Buses.Find(id);
        }

        public Bus AddBus(Bus b)
        {
            if (b == null)
            {
                throw new NullReferenceException("Bus is null");
            }
            db.Buses.Add(b);
            db.SaveChanges();
            return b;
        }

        public bool UpdateBus(Bus b)
        {
            if (b == null)
            {
                throw new ArgumentNullException("Bus is Empty");
            }
            db.Entry(b).State = EntityState.Modified;
            db.SaveChanges();
            return true;
        }

        public bool RemoveBus(int id)
        {
            var b = db.Buses.Find(id);
            if (b == null)
            {
                return false;
            }
            db.Buses.Remove(b);
            db.SaveChanges();
            return true;
        }
    }
}