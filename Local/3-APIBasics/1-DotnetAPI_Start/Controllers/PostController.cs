using System.Net.Mime;
using DotnetAPI.Dtos;
using DotnetAPI.Models;
using HelloWorld.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("posts")]
    public class PostController(IConfiguration config) : ControllerBase
    {
        private readonly DataContextDapper _dapperContext = new(config);

        [HttpGet("")]
        public IEnumerable<Post> GetPosts()
        {
            string sql = @"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM TutorialAppSchema.Posts";

            return _dapperContext.LoadData<Post>(sql);
        }

        [HttpGet("{postId}")]
        public IActionResult GetPostSingle(int postId)
        {
            string sql = @"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM TutorialAppSchema.Posts
                    WHERE PostId = @postId";

            try
            {
                return Ok(_dapperContext.LoadSingle<Post>(sql, new { postId }));
            }
            catch (Exception exception)
            {
                Console.WriteLine("Caught error");
                if (exception.Message == "Sequence contains no elements")
                {
                    return NotFound(new { message = "Post not found" });
                }

                return StatusCode(500, "Error Occured");
            }


        }

        [HttpGet("user")]
        public IEnumerable<Post> GetPostsByUser(int userId)
        {
            Console.WriteLine("ID => " + userId);
            string sql = @"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM TutorialAppSchema.Posts
                    WHERE UserId = @userId";

            return _dapperContext.LoadData<Post>(sql, new { userId });
        }

        [HttpGet("user/me")]
        public IEnumerable<Post> GetMyPosts()
        {
            string? userId = this.User.FindFirst("userId")?.Value;
            string sql = @"SELECT [PostId],
                    [UserId],
                    [PostTitle],
                    [PostContent],
                    [PostCreated],
                    [PostUpdated] 
                FROM TutorialAppSchema.Posts
                    WHERE UserId = @userId";

            return _dapperContext.LoadData<Post>(sql, new { userId });
        }

        [ProducesResponseType<object>(StatusCodes.Status400BadRequest, MediaTypeNames.Application.Json)]
        [ProducesResponseType<IEnumerable<Post>>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)]
        [HttpGet("search")]
        public IActionResult SearchPosts(string? title, string? content)
        {
            if (title == null && content == null)
            {
                return BadRequest(new { message = "Please search by title or content" });
            }
            string sql =
            @"SELECT [PostId],
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated]
            FROM TutorialAppSchema.Posts
            WHERE PostTitle LIKE '%' + @title + '%'
                OR PostContent LIKE '%' + @content + '%'
            ";

            return Ok(_dapperContext.LoadData<Post>(sql, new { title, content }));
        }

        [HttpPost("add")]
        public IActionResult AddPost(PostToAddDto postToAdd)
        {
            string? userId = this.User.FindFirst("userId")?.Value;
            string sql = @"
            INSERT INTO TutorialAppSchema.Posts
            (
                [UserId],
                [PostTitle],
                [PostContent],
                [PostCreated],
                [PostUpdated]) VALUES (
                @userId,
                @PostTitle,
                @PostContent,
                GETDATE(),
                GETDATE()
            )";
            if (_dapperContext.ExecuteSql(sql, new { userId, postToAdd.PostContent, postToAdd.PostTitle }))
            {
                return Ok(new { message = "Successfully added new post" });
            }

            throw new Exception("Failed to create new post!");
        }

        [HttpPut("update")]
        public IActionResult EditPost(PostToEditDto postToEdit)
        {
            string? userId = this.User.FindFirst("userId")?.Value;
            string sql = @"
            UPDATE TutorialAppSchema.Posts 
                SET PostContent = @PostContent,
                PostTitle = @PostTitle,
                PostUpdated = GETDATE()
                    WHERE PostId = @PostId AND UserId = @userId";

            if (_dapperContext.ExecuteSql(sql, new { userId, postToEdit.PostContent, postToEdit.PostTitle, postToEdit.PostId }))
            {
                return Ok(new { message = "Post updated successfully" });
            }

            throw new Exception("Failed to edit post!");
        }

        [HttpDelete("{postId}")]
        public IActionResult DeletePost(int postId)
        {
            string? userId = this.User.FindFirst("userId")?.Value;
            string sql = @"DELETE FROM TutorialAppSchema.Posts 
                WHERE PostId = @postId
                    AND UserId = @userId";


            if (_dapperContext.ExecuteSql(sql, new { postId, userId }))
            {
                return NoContent();
            }

            throw new Exception("Failed to delete post!");
        }
    }
}