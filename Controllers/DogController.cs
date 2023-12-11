using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DogInfoAPI.Data;
using DogInfoAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace DogInfoAPI.Controllers
{

    [Route("[controller]")]
    public class DogController : BaseApiController
    {
        DataContextDapper _dapper;
        public DogController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
        }

        [HttpGet("TestConnection")]
        public DateTime TestConnection()
        {
            return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
        }


        [HttpGet("GetDogs")]
        public IEnumerable<Dog> GetDogs()
        {
            string sql = @"
                SELECT [DogId],
                    [Name],
                    [LifeSpan],
                    [Weight],
                    [PictureUrl],
                    [Size],
                    [CountryId] 
                FROM DogInfoSchema.DogInfo";
            
            return _dapper.LoadData<Dog>(sql);
        }

        [HttpGet("GetSingleDog/{dogId}")]
        public IEnumerable<Dog> GetSingleDog(int dogId)
        {
            string sql = @"
            SELECT [DogId],
                    [Name],
                    [LifeSpan],
                    [Weight],
                    [PictureUrl],
                    [Size],
                    [CountryId] 
            FROM DogInfoSchema.DogInfo WHERE [DogId] = " + dogId.ToString();
            
            return _dapper.LoadData<Dog>(sql);
        }

        [HttpGet("GetDogsBySearch/{searchParam}")]
        public IEnumerable<Dog> GetDogsBySearch(string searchParam)
        {

            string sql = @"SELECT [DogId],
                    [Name],
                    [LifeSpan],
                    [Weight],
                    [PictureUrl],
                    [Size],
                    [CountryId]
                FROM DogInfoSchema.DogInfo
                    WHERE Name LIKE '%" + searchParam + "%'" + 
                       " OR LifeSpan LIKE '%" + searchParam + "%'" + 
                       " OR Weight LIKE '%" + searchParam + "%'" + 
                       " OR Size LIKE '%" + searchParam + "%'";
            
            System.Console.WriteLine(sql);

            return _dapper.LoadData<Dog>(sql);
        }

    }
}