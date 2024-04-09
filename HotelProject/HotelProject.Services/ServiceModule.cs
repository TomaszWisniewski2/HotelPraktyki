using Autofac;
using HotelProject.Services.AdminService;
using HotelProject.Services.AuthorizationService;
using HotelProject.Services.UserService;

namespace HotelProject.Services
{
    public class ServiceModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AuthorizationService.AuthorizationService>().As<IAuthorizationService>();
            builder.RegisterType<AdminService.AdminService>().As<IAdminService>();
            builder.RegisterType<UserService.UserService>().As<IUserService>();

        }
    }
}