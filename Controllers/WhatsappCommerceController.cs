using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Messages;
using Nop.Plugin.Misc.WhatsappCommerce.Models;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.WhatsappCommerce.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class WhatsappCommerceController : BasePluginController
    {

        #region Fields
        private readonly IEmailAccountService _emailAccountService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IStoreService _storeService;
        private readonly IWorkContext _workContext;
        private readonly MessageTemplatesSettings _messageTemplatesSettings;
        #endregion

        #region Ctor

        public WhatsappCommerceController(IEmailAccountService emailAccountService,
             IGenericAttributeService genericAttributeService,
             ILocalizationService localizationService,
             ILogger logger,
             IMessageTemplateService messageTemplateService,
             IMessageTokenProvider messageTokenProvider,
             INotificationService notificationService,
             ISettingService settingService,
             IStaticCacheManager staticCacheManager,
             IStoreContext storeContext,
             IStoreMappingService storeMappingService,
             IStoreService storeService,
             IWorkContext workContext,
             MessageTemplatesSettings messageTemplatesSettings)
        {
            _emailAccountService = emailAccountService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _logger = logger;
            _messageTemplateService = messageTemplateService;
            _messageTokenProvider = messageTokenProvider;
            _notificationService = notificationService;
            _settingService = settingService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _storeService = storeService;
            _workContext = workContext;
            _messageTemplatesSettings = messageTemplatesSettings;
        }

        #endregion

        #region Utilities



        #endregion

        #region Methods

        private async Task PrepareModelAsync(ConfigurationModel model)
        {
            //load settings for active store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<WhatsappCommerceSettings>(storeId);
            //whether plugin is configured
            if (string.IsNullOrEmpty(settings.PhoneNumber ))
                return;
            model.PhoneNumber = settings.PhoneNumber;
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            var model = new ConfigurationModel();
            await PrepareModelAsync(model);
            return View("~/Plugins/Misc.WhatsappCommerce/Views/Configure.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return await Configure();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var settings = await _settingService.LoadSettingAsync<WhatsappCommerceSettings>(storeId);

            //set API key
            settings.PhoneNumber = model.PhoneNumber;
            await _settingService.SaveSettingAsync(settings, settings => settings.PhoneNumber, clearCache: false);
            await _settingService.ClearCacheAsync();

            
            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion
    }
}