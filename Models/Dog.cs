using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogInfoAPI.Models
{
    public partial class Dog
    {
        public int DogId {get;set;}
        public string Name {get;set;}
        public string LifeSpan {get;set;}
        public string Weight {get;set;}
        public string PictureUrl {get;set;}
        public string Size {get;set;}
        public int CountryId {get;set;}
    }
}