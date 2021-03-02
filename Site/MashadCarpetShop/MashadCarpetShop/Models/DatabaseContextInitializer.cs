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
            Guid roleId = new Guid("bb3c1194-272f-4f20-a853-4e7fa3365979");
            Guid roleId2 = new Guid("00616634-25df-49b4-9c7d-db81e1587e2f");

            InsertBaseRole(databaseContext, roleId, roleId2);
            InsertBaseUser(databaseContext, roleId);

            databaseContext.SaveChanges();
        }

        internal static void InsertBaseRole(DatabaseContext databaseContext, Guid roleId, Guid roleId2)
        {
            Role role = new Role()
            {
                Id = roleId,
                Title = "Administrator",
                Name = "Administrator",
                CreationDate = DateTime.Now,
                IsDeleted = false
            };

            databaseContext.Roles.Add(role);

            Role role2 = new Role()
            {
                Id = roleId2,
                Title = "Customer",
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
