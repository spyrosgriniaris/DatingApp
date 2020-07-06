namespace DatingApp.API.Helpers
{
    public class UserParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1; // returns 1 unless requested something else
        private int pageSize = 10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = (value > MaxPageSize) ? MaxPageSize : value; }
        }

        // filtering properties
        public int UserId { get; set; }
        public string Gender { get; set; }

        // end of filtering variables

        // additional filtering properties
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 99;

        // end of additional filtering properties

        // sorting area
        public string OrderBy { get; set; }
        // end of sorting area
        
    }
}