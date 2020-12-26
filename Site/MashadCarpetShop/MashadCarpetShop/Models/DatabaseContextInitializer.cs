using System;

namespace Models
{
    internal static class DatabaseContextInitializer
    {
        static DatabaseContextInitializer()
        {

        }

        internal static void Seed(DatabaseContext databaseContext)
        {
            Guid roleId = Guid.NewGuid();
            Guid roleId2 = Guid.NewGuid();

            InsertBaseRole(databaseContext, roleId, roleId2);
            InsertBaseUser(databaseContext, roleId);

            databaseContext.SaveChanges();
        }

        internal static void InsertBaseRole(DatabaseContext databaseContext, Guid roleId, Guid roleId2)
        {
            Role role = new Role()
            {
                Id = roleId,
                Title = "مدیر وب سایت",
                Name = "Administrator",
                CreationDate = DateTime.Now,
                IsDeleted = false
            };

            databaseContext.Roles.Add(role);

            Role role2 = new Role()
            {
                Id = roleId2,
                Title = "مشتری",
                Name = "Customer",
                CreationDate = DateTime.Now,
                IsDeleted = false
            };

            databaseContext.Roles.Add(role2);
        }

        internal static void InsertBaseUser(DatabaseContext databaseContext, Guid roleId)
        {
            User user = new User()
            {
                Id = Guid.NewGuid(),
                RoleId = roleId,
                CellNum = "09124806404",
                Password = "123qwe456",
                FullName = "baseuser",
                CreationDate = DateTime.Now,
                IsDeleted = false,
                Code = 01
            };

            databaseContext.Users.Add(user);
        }



    }
}
