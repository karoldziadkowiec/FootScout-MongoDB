using FootScout_MongoDB.WebAPI.Entities;
using FootScout_MongoDB.WebAPI.Models.Constants;
using FootScout_MongoDB.WebAPI.Models.DTOs;
using FootScout_MongoDB.WebAPI.Services.Interfaces;
using MongoDB.Driver;

namespace FootScout_MongoDB.WebAPI.DbManager
{
    public static class AppSeeder
    {
        public static async Task Seed(IServiceProvider services)
        {
            var dbContext = services.GetRequiredService<MongoDBContext>();
            var roleService = services.GetRequiredService<IRoleService>();
            var accountService = services.GetRequiredService<IAccountService>();
            var userRepository = services.GetRequiredService<IUserRepository>();
            var newIdGeneratorService = services.GetRequiredService<INewIdGeneratorService>();

            await SeedRoles(roleService);
            await SeedAdminRole(accountService, userRepository, roleService);
            await SeedOfferStatuses(dbContext, newIdGeneratorService);
            await SeedPlayerPositions(dbContext, newIdGeneratorService);
            await SeedPlayerFeet(dbContext, newIdGeneratorService);
            await SeedUnknownUser(accountService, userRepository, roleService);
        }

        private static async Task SeedRoles(IRoleService roleService)
        {
            var roles = new List<string> { RoleName.Admin, RoleName.User };

            foreach (var role in roles)
            {
                if (!await roleService.CheckRoleExists(role))
                    await roleService.CreateNewRole(role);
            }
        }

        private static async Task SeedAdminRole(IAccountService accountService, IUserRepository userRepository, IRoleService roleService)
        {
            string adminEmail = "admin@admin.com";
            string adminPassword = "Admin1!";

            var admin = await userRepository.FindUserByEmail(adminEmail);
            if (admin == null)
            {
                var adminUser = new RegisterDTO
                {
                    Email = adminEmail,
                    Password = adminPassword,
                    ConfirmPassword = adminPassword,
                    FirstName = "Admin",
                    LastName = "Admin",
                    PhoneNumber = "000000000",
                    Location = "Admin"
                };

                await accountService.Register(adminUser);

                admin = await userRepository.FindUserByEmail(adminEmail);
                if (admin != null)
                {
                    await roleService.RemoveRoleFromUser(admin.Id.ToString(), RoleName.User);
                    await roleService.AddRoleToUser(admin.Id.ToString(), RoleName.Admin);
                }
            }
        }

        private static async Task SeedOfferStatuses(MongoDBContext dbContext, INewIdGeneratorService newIdGeneratorService)
        {
            var statuses = new List<string> { OfferStatusName.Offered, OfferStatusName.Accepted, OfferStatusName.Rejected };

            foreach (var status in statuses)
            {
                var existingStatus = await dbContext.OfferStatusesCollection
                    .Find(os => os.StatusName == status)
                    .FirstOrDefaultAsync();

                if (existingStatus == null)
                {
                    var newStatus = new OfferStatus
                    {
                        Id = await newIdGeneratorService.GenerateNewOfferStatusId(),
                        StatusName = status
                    };

                    await dbContext.OfferStatusesCollection.InsertOneAsync(newStatus);
                }
            }
        }

        private static async Task SeedPlayerPositions(MongoDBContext dbContext, INewIdGeneratorService newIdGeneratorService)
        {
            var positions = new List<string>
            {
                Position.Goalkeeper, Position.RightBack, Position.CenterBack, Position.LeftBack, Position.RightWingBack, Position.LeftWingBack, Position.CentralDefensiveMidfield, Position.CentralMidfield, Position.CentralAttackingMidfield, Position.RightMidfield, Position.RightWing, Position.LeftMidfield, Position.LeftWing, Position.CentreForward, Position.Striker
            };

            foreach (var position in positions)
            {
                var existingPosition = await dbContext.PlayerPositionsCollection
                    .Find(p => p.PositionName == position)
                    .FirstOrDefaultAsync();

                if (existingPosition == null)
                {
                    var newPosition = new PlayerPosition
                    {
                        Id = await newIdGeneratorService.GenerateNewPlayerPositionId(),
                        PositionName = position
                    };

                    await dbContext.PlayerPositionsCollection.InsertOneAsync(newPosition);
                }
            }
        }

        private static async Task SeedPlayerFeet(MongoDBContext dbContext, INewIdGeneratorService newIdGeneratorService)
        {
            var feet = new List<string> { Foot.Left, Foot.Right, Foot.TwoFooted };

            foreach (var foot in feet)
            {
                var existingFoot = await dbContext.PlayerFeetCollection
                    .Find(p => p.FootName == foot)
                    .FirstOrDefaultAsync();

                if (existingFoot == null)
                {
                    var newFoot = new PlayerFoot
                    {
                        Id = await newIdGeneratorService.GenerateNewPlayerFootId(),
                        FootName = foot
                    };

                    await dbContext.PlayerFeetCollection.InsertOneAsync(newFoot);
                }
            }
        }

        private static async Task SeedUnknownUser(IAccountService accountService, IUserRepository userRepository, IRoleService roleService)
        {
            string unknownUserEmail = "unknown@unknown.com";
            string unknownUserPassword = "Unknown1!";

            var unknownUser = await userRepository.FindUserByEmail(unknownUserEmail);
            if (unknownUser == null)
            {
                var unknownUserDto = new RegisterDTO
                {
                    Email = unknownUserEmail,
                    Password = unknownUserPassword,
                    ConfirmPassword = unknownUserPassword,
                    FirstName = "Unknown",
                    LastName = "Unknown",
                    PhoneNumber = "000000000",
                    Location = "Unknown"
                };

                await accountService.Register(unknownUserDto);
            }
        }
    }
}