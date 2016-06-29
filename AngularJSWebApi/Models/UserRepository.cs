using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AngularJSWebApi.Models
{
    public class UserRepository:IUserRepository
    {
        private DatabaseEntity7 db = new DatabaseEntity7();

        public IEnumerable<User_details> GetAll()
        {
            return db.User_details.ToList();
        }

        public User_details GetUserById(int id)
        {
            return db.User_details.Find(id);
        }

        public User_details AddUser(User_details b)
        {
            if (b == null)
            {
                throw new NullReferenceException("User is null");
            }
            db.User_details.Add(b);
            db.SaveChanges();
            return b;
        }

        public bool UpdateUser(User_details b)
        {
            if (b == null)
            {
                throw new ArgumentNullException("User is Empty");
            }
            db.Entry(b).State = EntityState.Modified;
            db.SaveChanges();
            return true;
        }

        public bool RemoveUser(int id)
        {
            var b = db.User_details.Find(id);
            if (b == null)
            {
                return false;
            }
            db.User_details.Remove(b);
            db.SaveChanges();
            return true;
        }
    }
}