using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace AspNetCore.ExtDirect.Binders
{
    /// <summary>
    /// HTTP request to model converter
    /// </summary>
    internal sealed class ExtDirectGenericModelBinder
    {
        private readonly ControllerContext _controllerContext;
        private readonly IModelBinderFactory _modelBinderFactory;
        private readonly IModelMetadataProvider _modelMetadataProvider;

        public ExtDirectGenericModelBinder(IServiceProvider serviceProvider, ControllerContext controllerContext)
        {
            if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));
            _controllerContext = controllerContext ?? throw new ArgumentNullException(nameof(controllerContext));
            _modelMetadataProvider = serviceProvider.GetService<IModelMetadataProvider>();
            _modelBinderFactory = serviceProvider.GetService<IModelBinderFactory>();

            if (_modelBinderFactory == null) throw new Exception();
            if (_modelMetadataProvider == null) throw new Exception();
        }

        /// <summary>
        /// Converts a HTTP request to an object
        /// </summary>
        /// <param name="targetType">Target object type</param>
        /// <returns></returns>
        public async Task<object> BindAsync(Type targetType)
        {
            var valueProvider = await CompositeValueProvider.CreateAsync(_controllerContext);
            var modelMetadata = _modelMetadataProvider.GetMetadataForType(targetType);
            var modelBinderFactoryContext = new ModelBinderFactoryContext
            {
                Metadata = modelMetadata,
                CacheToken = modelMetadata
            };
            var modelBinder = _modelBinderFactory.CreateBinder(modelBinderFactoryContext);
            var ctx = DefaultModelBindingContext.CreateBindingContext(_controllerContext, valueProvider, modelMetadata, new BindingInfo(), string.Empty);
            await modelBinder.BindModelAsync(ctx);
            if (!ctx.Result.IsModelSet)
            {
                return null;
            }
            else
            {
                return ctx.Model;
            }
        }

        /// <summary>
        /// Converts a HTTP request to an object
        /// </summary>
        /// <typeparam name="T">Target object type</typeparam>
        /// <returns></returns>
        public async Task<T> BindAsync<T>()
            where T : class
        {
            return await BindAsync(typeof(T)) as T;
        }
    }
}