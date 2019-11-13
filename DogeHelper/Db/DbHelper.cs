using DogeHelper.Entities;
using LiteDB;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DogeHelper.Db
{
    public class DbHelper : ILinkSingleton
    {
        private LiteDatabase db;
        private LiteCollection<Link> links;
        private DbHelper()
        {
            Globals.PrintMessage("Initalising DBHelper Singleton Object...");

            db = new LiteDatabase("helper.db");
            links = db.GetCollection<Link>("links");
        }

        private static Lazy<DbHelper> instance = new Lazy<DbHelper>(() => new DbHelper());

        public static DbHelper Instance => instance.Value;

        public async Task<bool> AddMessage(Link link)
        {
            // Check for existing Link by Key
            if (await KeyExists(link.Key))
            {
                return false;
            }
            else
            {
                links.Insert(link);
                return true;
            }
        }

        public List<Link> GetAllMessages()
        {
            throw new NotImplementedException();
        }

        public async Task<Link> GetMessage(string key)
        {
            return await FindMessageByKey(key);
        }

        public void RemoveAllMessages()
        {
            Task.Run(() =>
            {
                db.DropCollection("links");
                links = db.GetCollection<Link>("links");
                Globals.PrintMessage("Recreated links collection.");
            });
        }

        public async Task<int> RemoveMessage(string key)
        {
            return await Task.Run(() => links.Delete(Query.EQ("Key", key)));
        }

        private async Task<Link> FindMessageByKey(string key)
        {
            return await Task.Run(() => FindFirstByKey(key));
        }

        private Link FindFirstByKey(string key)
        {
            return links.FindOne(x => x.Key.Equals(key));
        }

        private async Task<bool> KeyExists(string key)
        {
            return await Task.Run(() =>
            {
                return links.Exists(Query.EQ("Key", key));
            });
        }

        public async Task<bool> UpdateMessage(string key, string value, string username)
        {
            return await Task.Run(() =>
            {
                var l = FindFirstByKey(key);
                if (l != null)
                {
                    l.Value = value;
                    l.LastChanged = DateTime.Now;
                    l.LastChangedBy = username;
                    return true;
                }

                return false;
            });
        }
    }
}
