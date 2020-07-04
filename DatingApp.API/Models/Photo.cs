using System;

namespace DatingApp.API.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool isMain { get; set; }
        //=============================================
        // to response apo to cloudinary periexei kai ena public id, to opoio prosthetw stin klasi auti gia na to apothikeusw
        public string PublicId { get; set; }
        //=============================================
        public User User { get; set; }
        public int UserId { get; set; }
    }
}