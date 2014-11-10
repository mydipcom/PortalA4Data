using Autofac;
using Autofac.Core;
using CamprayPortal.Admin.Controllers;
using CamprayPortal.Core.Caching;
using CamprayPortal.Core.Infrastructure;
using CamprayPortal.Core.Infrastructure.DependencyManagement;

namespace CamprayPortal.Admin.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //we cache presentation models between requests
            builder.RegisterType<HomeController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"));
        }

        public int Order
        {
            get { return 2; }
        }
    }
}
