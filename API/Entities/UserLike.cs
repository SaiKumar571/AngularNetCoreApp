namespace API.Entities
{
    public class UserLike
    {
        public AppUser LikedUser { get; set; }
        public AppUser SourceUser { get; set; }
        public int SourceUserId { get; set; }
        public int LikeUserId { get; set; }

    }
}