using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Http;
using post_microservice.Model;    
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace post_microservice.Services
{
    public class PostService
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly string _mediaFolderPath;

        public PostService(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
            _mediaFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Media");

            // Ensure the Media directory exists
            if (!Directory.Exists(_mediaFolderPath))
            {
                Directory.CreateDirectory(_mediaFolderPath);
            }
        }

        public async Task<Post> CreatePostAsync(Post post, List<IFormFile> mediaFiles)
        {
            post.PostId = Guid.NewGuid().ToString();
            post.CreatedAt = DateTime.UtcNow;
            post.UpdatedAt = DateTime.UtcNow;
            post.MediaUrls = new List<string>();

            // Save each media file to the Media directory
            foreach (var file in mediaFiles)
            {
                if (file.Length > 0)
                {
                    // Create a unique file name for each uploaded file
                    var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
                    var filePath = Path.Combine(_mediaFolderPath, uniqueFileName);

                    // Save the file to the local media folder
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // Add the relative path to mediaUrls
                    post.MediaUrls.Add(Path.Combine("Media", uniqueFileName));
                }
            }

            // Save the post data to Firestore
            DocumentReference docRef = _firestoreDb.Collection("posts").Document(post.PostId);
            await docRef.SetAsync(post);

            return post;
        }
        public async Task<Post?> GetPostByIdAsync(string postId)
        {
            DocumentReference docRef = _firestoreDb.Collection("posts").Document(postId);
            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

            if (!snapshot.Exists) return null;
            return snapshot.ConvertTo<Post>();
        }

        public async Task<Post?> UpdatePostAsync(string postId, string content)
        {
            DocumentReference docRef = _firestoreDb.Collection("posts").Document(postId);
            await docRef.UpdateAsync(new Dictionary<string, object>
            {
                { "Content", content },
                { "UpdatedAt", DateTime.UtcNow }
            });

            DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();
            return snapshot.ConvertTo<Post>();
        }

        public async Task DeletePostAsync(string postId)
        {
            DocumentReference docRef = _firestoreDb.Collection("posts").Document(postId);
            await docRef.DeleteAsync();
        }

        public async Task<List<Post>> GetPostsByUserIdAsync(string userId)
        {
            Query query = _firestoreDb.Collection("posts").WhereEqualTo("UserId", userId);
            QuerySnapshot snapshot = await query.GetSnapshotAsync();

            return snapshot.Documents.Select(doc => doc.ConvertTo<Post>()).ToList();
        }
        // Other methods...
    }
}

