﻿using System.Collections.Generic;
using System.Web.Mvc;
using FluentValidation.Attributes;
using CamprayPortal.Admin.Validators.Stores;
using CamprayPortal.Web.Framework;
using CamprayPortal.Web.Framework.Localization;
using CamprayPortal.Web.Framework.Mvc;

namespace CamprayPortal.Admin.Models.Stores
{
    [Validator(typeof(StoreValidator))]
    public partial class StoreModel : BaseNopEntityModel, ILocalizedModel<StoreLocalizedModel>
    {
        public StoreModel()
        {
            Locales = new List<StoreLocalizedModel>();
        }

        [NopResourceDisplayName("Admin.Configuration.Stores.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Stores.Fields.Url")]
        [AllowHtml]
        public string Url { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Stores.Fields.SslEnabled")]
        public virtual bool SslEnabled { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Stores.Fields.SecureUrl")]
        [AllowHtml]
        public virtual string SecureUrl { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Stores.Fields.Hosts")]
        [AllowHtml]
        public string Hosts { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Stores.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }


        public IList<StoreLocalizedModel> Locales { get; set; }
    }

    public partial class StoreLocalizedModel : ILocalizedModelLocal
    {
        public int LanguageId { get; set; }

        [NopResourceDisplayName("Admin.Configuration.Stores.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }
    }
}