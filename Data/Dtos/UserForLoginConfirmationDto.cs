using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogInfoAPI.Data.Dtos
{
    public partial class UserForLoginConfirmationDto
    {
        public byte[] PasswordHash {get;set;}
        public byte[] PasswordSalt {get;set;}
    }
}