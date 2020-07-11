using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
    public interface IDatingRepository
    {
         void Add<T>(T entity) where T: class;
         void Delete<T>(T entity) where T: class;
         Task<bool> SaveAll();
        //  Task<IEnumerable<User>> GetUsers(); auto einai prin to pagination
         Task<PagedList<User>> GetUsers(UserParams userParams); // auto einai me to pagination
         Task<User> GetUser(int id);
         // added with phtos controller for get method
         Task<Photo> GetPhoto(int id);
         Task <Photo> GetMainPhotoForUser(int userId);
         // send like functionality
         Task<Like> GetLike(int userId, int recipientId);

         // message functionality
         Task<Message> GetMessage(int id);
         Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams); // inbox, outbox or unread messages
         Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId); // it is the conversation in the tab panel

    }
}