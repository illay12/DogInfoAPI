using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogInfoAPI.Models
{
    public partial class Like
    {
        public int LikeId {get;set;}
        public int UserId {get;set;}
        public int DogId {get; set;}
        public DateTime LikeCreated {get;set;} 
    }
}