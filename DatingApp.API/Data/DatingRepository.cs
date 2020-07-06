using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DatingRepository : IDatingRepository
    {   private readonly DataContext _context;
        public DatingRepository(DataContext context)
        {
            _context = context;
        }
        public void Add<T>(T entity) where T : class
        {
            //we use async only in save, cause at this time this is going to be saved at local memory, not in db
            _context.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.isMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

            return photo;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);

            return user;
        }

        // Prin to pagination
        // public async  Task<IEnumerable<User>> GetUsers()
        // {
        //     var users = await _context.Users.Include(p => p.Photos).ToListAsync();

        //     return users;
        // }

        // me to pagination
        public async  Task<PagedList<User>> GetUsers(UserParams userParams)
        {
            // to asqueryable einai gia to filtering. alliws de xreiazetai
            // var users = _context.Users.Include(p => p.Photos).AsQueryable();
            //sorting area
            var users = _context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();
            // end of sorting area

            // filtering area 
            users = users.Where(u => u.Id != userParams.UserId);

            users = users.Where(u => u.Gender == userParams.Gender);
            // end of filtering area 

            // additional filtering area 
            if (userParams.MinAge != 18 || userParams.MaxAge != 99){
                var minDoB = DateTime.Today.AddYears(-userParams.MaxAge - 1);// minDateOfBirth
                var maxDoB = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDoB && u.DateOfBirth <= maxDoB);
            }
            // end of additional filtering area

            // sorting area
            if (!string.IsNullOrEmpty(userParams.OrderBy)){
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;

                }
            }
            // end of sorting area

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }


        public async Task<bool> SaveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}