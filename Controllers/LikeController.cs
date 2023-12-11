using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DogInfoAPI.Data;
using DogInfoAPI.Data.Dtos;
using DogInfoAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DogInfoAPI.Controllers
{
    [Authorize]
    public class LikeController : BaseApiController
    {
        private readonly DataContextDapper _dapper;

        public LikeController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("MyLikes")]
        public IEnumerable<Like> GetLikesFromUser()
        {
            string sql = @"SELECT [LikeId],
                            [UserId],
                            [LikeCreated],
                            [DogId]
                        FROM DogInfoSchema.Likes
                            WHERE UserId = " + User.FindFirst("userId").Value;

            return _dapper.LoadData<Like>(sql);
        }

        [HttpPost("LikeUnlikeDog/{dogId}")]
        public IActionResult LikeUnlikeDog(int dogId)
        {
            string sqlCheckLikeExists = @"SELECT DogId FROM DogInfoSchema.Likes
                WHERE DogId = " + dogId.ToString()
                + "AND UserId = " + this.User.FindFirst("userId").Value;

            IEnumerable<int> existingLikes = _dapper.LoadData<int>(sqlCheckLikeExists);

            if(existingLikes.Count() == 0)
            {
                string sqlToAddLike = @"INSERT INTO DogInfoSchema.Likes(
                                [UserId],
                                [LikeCreated],
                                [DogId]
                            ) VALUES ("
                            + User.FindFirst("userId").Value
                            + ", GETDATE(), " 
                            + dogId.ToString() + ")";
                                

                if(_dapper.ExecuteSQL(sqlToAddLike))
                {
                    return Ok();
                }
                
                throw new Exception("Failed to Like a post");

            }

            else
            {
                string sqlToUnLike = @"DELETE FROM DogInfoSchema.Likes
                    WHERE DogId = " + dogId.ToString()
                + " AND UserId = " + this.User.FindFirst("userId").Value;
            
                if(_dapper.ExecuteSQL(sqlToUnLike))
                {
                    return Ok();
                }

                throw new Exception("Failed to Unlike a post");

            }

        }

        [AllowAnonymous]
        [HttpGet("GetSingleDogLikes/{dogId}")]
        public int GetSingleDogLikes(int dogId)
        {
            string sql = @"SELECT DogId FROM DogInfoSchema.Likes
                WHERE DogId = " + dogId.ToString();

            IEnumerable<int> dogLikes = _dapper.LoadData<int>(sql);
            
            return dogLikes.Count();
                
        }


    }
}