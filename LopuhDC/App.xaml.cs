using LopuhDC.Data.Remote.Models;
using LopuhDC.Domain.Contexts;
using LopuhDC.Domain.Services;
using LopuhDC.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;

namespace LopuhDC
{
    public partial class App : Application
    {
        protected readonly IServiceProvider _provider;

        public App()
        {
            var services = new ServiceCollection();

            RegisterServices(services);

            _provider = services.BuildServiceProvider();
        }

        protected void RegisterServices(IServiceCollection services)
        {
            // Contexts
            services.AddSingleton<MainContext>();
            services.AddSingleton<LopuhDbEntities>();

            // ViewModels
            services.AddSingleton(MainViewModelFactory);
            services.AddSingleton(MainWindowFactory);

            services.AddTransient(ProductsViewModelFactory);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var navService = ProductsNavFactory(_provider);
            navService.Push();

            MainWindow = _provider.GetRequiredService<MainWindow>();
            MainWindow.Show();

            base.OnStartup(e);
        }

        #region viewmodels factories
        protected MainViewModel MainViewModelFactory(IServiceProvider p)
        {
            return new MainViewModel(p.GetRequiredService<MainContext>());
        }
        protected MainWindow MainWindowFactory(IServiceProvider p)
        {
            return new MainWindow
            {
                DataContext = p.GetRequiredService<MainViewModel>(),
            };
        }
        protected ProductsViewModel ProductsViewModelFactory(IServiceProvider p)
        {
            return new ProductsViewModel(p.GetRequiredService<LopuhDbEntities>());
        }
        #endregion

        #region nav factories
        protected MainNavService ProductsNavFactory(IServiceProvider p)
        {
            return new MainNavService(p.GetRequiredService<MainContext>(), p.GetRequiredService<ProductsViewModel>);
        }
        #endregion
    }
}
