using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace PDFBox.Api.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public DateTime RegistrationDate { get; set; }

        public virtual List< Document > Documents { get; set; }

        // <summary>
        //  Method to create a new encryption hash from a given password.
        //  Used when a new user is registered or an existing user changes their password.
        // </summary>
        public void CreatePasswordHash(string password)
        {
            // Argument verification
            if (password == null || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty!");

            // Generate new hash
            using (var hmac = new HMACSHA512())
            {
                PasswordSalt = hmac.Key;
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        // <summary>
        //  Method that compares the hash of a given password to that of a stored password hash.
        //  Used for password verification and user authentication.
        // </summary>
        public bool VerifyPasswordHash(string password)
        {
            // Argument verification
            if (password == null || string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty!");
            if (PasswordHash.Length != 64)
                throw new ArgumentException("Invalid password hash length!");
            if (PasswordSalt.Length != 128)
                throw new ArgumentException("Invalid password salt length!");
            
            // Hash verification
            using (var hmac = new HMACSHA512(PasswordSalt))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < hash.Length; i++)
                {
                    if (hash[i] != PasswordHash[i])
                        return false;
                }
            }

            return true;
        }
    }
}