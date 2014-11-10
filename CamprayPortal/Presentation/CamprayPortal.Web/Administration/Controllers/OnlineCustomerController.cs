﻿using System;
using System.Linq;
using System.Web.Mvc;
using CamprayPortal.Admin.Models.Customers;
using CamprayPortal.Core.Domain.Customers;
using CamprayPortal.Services.Common;
using CamprayPortal.Services.Customers;
using CamprayPortal.Services.Directory;
using CamprayPortal.Services.Helpers;
using CamprayPortal.Services.Localization;
using CamprayPortal.Services.Security;
using CamprayPortal.Web.Framework.Kendoui;

namespace CamprayPortal.Admin.Controllers
{
    public partial class OnlineCustomerController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IGeoLookupService _geoLookupService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly CustomerSettings _customerSettings;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Constructors

        public OnlineCustomerController(ICustomerService customerService,
            IGeoLookupService geoLookupService, IDateTimeHelper dateTimeHelper,
            CustomerSettings customerSettings,
            IPermissionService permissionService, ILocalizationService localizationService)
        {
            this._customerService = customerService;
            this._geoLookupService = geoLookupService;
            this._dateTimeHelper = dateTimeHelper;
            this._customerSettings = customerSettings;
            this._permissionService = permissionService;
            this._localizationService = localizationService;
        }

        #endregion
        
        #region Methods

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            var customers = _customerService.GetOnlineCustomers(DateTime.UtcNow.AddMinutes(-_customerSettings.OnlineCustomerMinutes),
                null, command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = customers.Select(x =>
                {
                    return new OnlineCustomerModel()
                    {
                        Id = x.Id,
                        CustomerInfo = x.IsRegistered() ? x.Email : _localizationService.GetResource("Admin.Customers.Guest"),
                        LastIpAddress = x.LastIpAddress,
                        Location = _geoLookupService.LookupCountryName(x.LastIpAddress),
                        LastActivityDate = _dateTimeHelper.ConvertToUserTime(x.LastActivityDateUtc, DateTimeKind.Utc),
                        LastVisitedPage = _customerSettings.StoreLastVisitedPage ?
                            x.GetAttribute<string>(SystemCustomerAttributeNames.LastVisitedPage) :
                            _localizationService.GetResource("Admin.Customers.OnlineCustomers.Fields.LastVisitedPage.Disabled")
                    };
                }),
                Total = customers.TotalCount
            };

            return Json(gridModel);
        }

        #endregion
    }
}
