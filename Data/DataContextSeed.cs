using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DogInfoAPI.Models;
using Microsoft.Data.SqlClient;
using Newtonsoft.Json;

namespace DogInfoAPI.Data
{
    public class DataContextSeed
    {
        public static void SeedData()
        {
            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appSettings.json")
                .Build();

            DataContextDapper context = new DataContextDapper(config);

            string tableCreateSql = File.ReadAllText("Data/SeedData/DogInfo.sql");
            context.ExecuteSQL(tableCreateSql);

            string dogInfoJson = File.ReadAllText("Data/SeedData/Dogs.json");

            IEnumerable<Dog> dogs = JsonConvert.DeserializeObject<IEnumerable<Dog>>(dogInfoJson); 

            if(dogs != null)
            {
                using (IDbConnection dbConnection = new SqlConnection(config.GetConnectionString("DefaultConnection")))
                {
                    string sql = @"
                            INSERT INTO DogInfoSchema.DogInfo( 
                                [Name],
                                [LifeSpan],
                                [Weight],
                                [PictureUrl],
                                [Size],
                                [CountryId]
                            ) VALUES ";
                    foreach(Dog dog in dogs)
                    {
                        string sqlToAdd = @"(" +
                            "'" + dog.Name +
                            "', '" + dog.LifeSpan +
                            "', '" + dog.Weight +
                            "', '" + dog.PictureUrl +
                            "', '" + dog.Size +
                            "', " + dog.CountryId.ToString() +
                        "),";
                        
                        
                        sql += sqlToAdd; 
                    }
                    sql = sql.Substring(0,sql.Length-1);
                    System.Console.WriteLine(sql);
                    context.ExecuteSQLWithRowCount(sql);
                }

            } 

            string countryInfoJson = File.ReadAllText("Data/SeedData/Countries.json");

            IEnumerable<Country> countries = JsonConvert.DeserializeObject<IEnumerable<Country>>(countryInfoJson);
        
            if(countries != null)
            {
                using (IDbConnection dbConnection = new SqlConnection(config.GetConnectionString("DefaultConnection")))
                {
                    string sql = @"
                            INSERT INTO DogInfoSchema.Countries(
                                [CountryId],
                                [Name]
                            ) Values ";
                    foreach(Country country in countries)
                    {
                        string sqlToAdd = @"(" +
                            country.Id.ToString() +
                            ",'" + country.Name + "'),";
                        
                        sql += sqlToAdd;
                    }

                    sql = sql.Substring(0,sql.Length-1);
                    System.Console.WriteLine(sql);
                    context.ExecuteSQLWithRowCount(sql);
                }
            }
        }
    }
}