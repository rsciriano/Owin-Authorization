﻿using System;
using Microsoft.Owin.Logging;
using Owin;

namespace Microsoft.Owin.Security.Authorization.Infrastructure
{
    public static class AppBuilderExtensions
    {
        public static IAppBuilder UseAuthorization(this IAppBuilder app, 
            AuthorizationOptions options, 
            IAuthorizationDependenciesFactory dependenciesFactory)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            if (dependenciesFactory == null)
            {
                throw new ArgumentNullException(nameof(dependenciesFactory));
            }

            return app.Use(typeof (ResourceAuthorizationMiddleware), options, dependenciesFactory);
        }

        public static IAppBuilder UseAuthorization(this IAppBuilder app,
            AuthorizationOptions options,
            params IAuthorizationHandler[] handlers)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var dependenciesFactory = CreateDefaultDependencyFactory(app, handlers);
            return UseAuthorization(app, options, dependenciesFactory);
        }

        public static IAppBuilder UseAuthorization(this IAppBuilder app, 
            Action<AuthorizationOptions> configure, 
            IAuthorizationDependenciesFactory dependenciesFactory)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }
            if (dependenciesFactory == null)
            {
                throw new ArgumentNullException(nameof(dependenciesFactory));
            }

            var options = new AuthorizationOptions();
            configure.Invoke(options);
            return UseAuthorization(app, options, dependenciesFactory);
        }

        public static IAppBuilder UseAuthorization(this IAppBuilder app, 
            Action<AuthorizationOptions> configure, 
            params IAuthorizationHandler[] handlers)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var dependencyFactory = CreateDefaultDependencyFactory(app, handlers);
            return UseAuthorization(app, configure, dependencyFactory);
        }

        public static IAppBuilder UseAuthorization(this IAppBuilder app, IAuthorizationDependenciesFactory dependenciesFactory)
        {
            return UseAuthorization(app, new AuthorizationOptions(), dependenciesFactory);
        }

        public static IAppBuilder UseAuthorization(this IAppBuilder app, params IAuthorizationHandler[] handlers)
        {
            var dependencyFactory = CreateDefaultDependencyFactory(app, handlers);
            return UseAuthorization(app, new AuthorizationOptions(), dependencyFactory);
        }

        private static IAuthorizationDependenciesFactory CreateDefaultDependencyFactory(
            IAppBuilder app,
            params IAuthorizationHandler[] handlers)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (handlers == null)
            {
                throw new ArgumentNullException(nameof(handlers));
            }

            return new DefaultAuthorizationDependenciesFactory(app.GetLoggerFactory(), handlers);
        }
    }
}
