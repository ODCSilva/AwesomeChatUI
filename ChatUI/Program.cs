using ChatLogger;
//using LogLib;
using Microsoft.Practices.Unity;
using Ninject;
using System;
using System.Windows.Forms;

namespace ChatUI {

    internal static class Program {

        #region Methods

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //IoC Injection with Unity
            UnityContainer container = new UnityContainer();
            //container.RegisterType<ILoggingService, Logger>();

            //IoC Injection with Ninject
            IKernel kernel = new StandardKernel();
            //kernel.Bind<ILoggingService>().To<Logger>();         // Text Logger
            //kernel.Bind<ILoggingService>().To<Omar_Logger>();    // Logging using NLog platform
            kernel.Bind<ILoggingService>().To<Omar_Logger>();

            //Application.Run(container.Resolve<ChatUI>()); // Unity
            Application.Run(kernel.Get<ChatUI>());        // Ninject
            
        }

        #endregion Methods
    }
}