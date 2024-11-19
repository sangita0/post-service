using Google.Cloud.Firestore;

namespace post_microservice.Model
{
    [FirestoreData]
    public class Post
    {
        [FirestoreProperty]
        public string PostId { get; set; }

        [FirestoreProperty]
        public string UserId { get; set; }

        [FirestoreProperty]
        public string Content { get; set; }

        [FirestoreProperty]
        public List<string> MediaUrls { get; set; } = new();

        [FirestoreProperty]
        public DateTime CreatedAt { get; set; }

        [FirestoreProperty]
        public DateTime UpdatedAt { get; set; }
    }
}
