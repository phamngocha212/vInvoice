using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Core.Resource;
using log4net;
using EInvoice.Core;
using FX.Context;
using FX.Core;
namespace EInvoice.CAdmin
{
    public class Bootstrapper
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Bootstrapper));
        private static IWindsorContainer container;
        public static void InitializeContainer()
        {
            try
            {               
                // Initialize Windsor
                container = new WindsorContainer(new XmlInterpreter());
               
                //container = new WindsorContainer(new XmlInterpreter(new ConfigResource("castle")));

                // Inititialize the static Windsor helper class. 
                IoC.Initialize(container);

                // Add ICuyahogaContext to the container.
                container.Register(Component.For<IFXContext>()
                    .ImplementedBy<EInvoiceContext>()
                    .Named("FX.context")
                    .LifeStyle.PerWebRequest
                );
            }
            catch (Exception ex)
            {
                log.Error("Error initializing application.", ex);
                throw;
            }
        }
    }

    public class BootstrapperForThread
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Bootstrapper));
        private static IWindsorContainer container;
        public static void InitializeContainer()
        {
            try
            {
                // Initialize Windsor
                container = new WindsorContainer(new XmlInterpreter());

                //container = new WindsorContainer(new XmlInterpreter(new ConfigResource("castle")));

                // Inititialize the static Windsor helper class. 
                IoC.Initialize(container);

                // Add ICuyahogaContext to the container.
                container.Register(Component.For<IFXContext>()
                    .ImplementedBy<EInvoiceContext>()
                    .Named("FX.context")
                    .LifeStyle.PerThread
                );
            }
            catch (Exception ex)
            {
                log.Error("Error initializing application.", ex);
                throw;
            }
        }
    }
}