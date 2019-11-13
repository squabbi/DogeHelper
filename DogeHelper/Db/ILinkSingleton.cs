using DogeHelper.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DogeHelper.Db
{
    public interface ILinkSingleton
    {
        Task<Link> GetMessage(string key);
        Task<int> RemoveMessage(string key);
        void RemoveAllMessages();
        Task<bool> AddMessage(Link link);
        List<Link> GetAllMessages();

        Task<bool> UpdateMessage(string key, string value, string username);
    }
}
