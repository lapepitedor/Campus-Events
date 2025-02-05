﻿using Campus_Events.Misc;
using Campus_Events.Models;
using Campus_Events.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Campus_Events.Repositories
{
    public class UserEfCoreRepository : IUserRepository
    {
        private readonly ApplicationDbContext context;
        private PasswordHelper passwordHelper;

        public UserEfCoreRepository(ApplicationDbContext context, PasswordHelper passwordHelper)
        {
            this.context = context;
            context.Migrate();

            this.passwordHelper = passwordHelper;

            if (!context.Users.Any())
            {
                var user = new User()
                {
                    Firstname = "Jack",
                    Lastname = "Bauer",
                    EMail = "Jack.bauer@hshl.de",
                    PasswordHash = passwordHelper.ComputeSha256Hash("secret"),
                    IsAdmin = true
                };

                context.Users.Add(user);
                context.SaveChanges();
            }
        }
        public User Add(User entity)
        {
            context.Users.Add(entity);
            context.SaveChanges();
            return entity;
        }

        public void Delete(Guid id)
        {
            var entity = context.Users.Find(id);
            if (entity is null)
                return;

            context.Users.Remove(entity);
            context.SaveChanges();
        }

        public User FindByEmail(string email)
        {
            return context.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.EMail.ToLower().Equals(email.ToLower()));
        }

        public User FindByLogin(string email, string password)
        {
            var user = FindByEmail(email);
            if (user is null)
                return null;

            var passwordHash = passwordHelper.ComputeSha256Hash(password);
            if (user.PasswordHash.Equals(passwordHash))
                return user;

            return null;
        }

        public User FindByPasswordResetToken(string token)
        {
            return context.Users
                .FirstOrDefault(u => u.PasswordResetToken == token);
        }

        public IEnumerable<User> GetAll()
        {
            return context.Users.AsNoTracking().ToList();
        }

        public User GetSingle(Guid id)
        {
            return context.Users
               .AsNoTracking()
               .FirstOrDefault(p => p.ID == id);
        }

        //public User Update(User entity)
        //{
        //    context.Users.Update(entity);
        //    context.SaveChanges();
        //    return entity;
        //}
        public User Update(User entity)
        {
            // Si l'entité est déjà suivie par le contexte, pas besoin de l'ajouter à nouveau
            var existingUser = context.Users.Find(entity.ID); 
            if (existingUser != null)
            {
                // Mettez à jour les propriétés nécessaires
                existingUser.PasswordResetToken = entity.PasswordResetToken; 

                context.SaveChanges(); // Enregistrez les modifications
            }

            return existingUser; // Retourne l'utilisateur existant
        }

    }
}
