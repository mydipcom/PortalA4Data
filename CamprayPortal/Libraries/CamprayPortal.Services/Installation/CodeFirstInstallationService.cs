using CamprayPortal.Core;
using CamprayPortal.Core.Data;
using CamprayPortal.Core.Domain;
using CamprayPortal.Core.Domain.Cms;
using CamprayPortal.Core.Domain.Common;
using CamprayPortal.Core.Domain.Customers;
using CamprayPortal.Core.Domain.Directory;
using CamprayPortal.Core.Domain.Localization;
using CamprayPortal.Core.Domain.Logging;
using CamprayPortal.Core.Domain.Media;
using CamprayPortal.Core.Domain.Messages;
using CamprayPortal.Core.Domain.News;
using CamprayPortal.Core.Domain.Security;
using CamprayPortal.Core.Domain.Seo;
using CamprayPortal.Core.Domain.Stores;
using CamprayPortal.Core.Domain.Tasks;
using CamprayPortal.Core.Domain.Topics;
using CamprayPortal.Core.Infrastructure;
using CamprayPortal.Services.Common;
using CamprayPortal.Services.Configuration;
using CamprayPortal.Services.Customers;
using CamprayPortal.Services.Helpers;
using CamprayPortal.Services.Localization;
using CamprayPortal.Services.Seo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CamprayPortal.Services.Installation
{
    public partial class CodeFirstInstallationService : IInstallationService
    {
        #region Fields

        private readonly IRepository<Store> _storeRepository;
        private readonly IRepository<MeasureDimension> _measureDimensionRepository;
        private readonly IRepository<MeasureWeight> _measureWeightRepository;
    
        private readonly IRepository<Language> _languageRepository;
        private readonly IRepository<Currency> _currencyRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerRole> _customerRoleRepository;
       
        private readonly IRepository<UrlRecord> _urlRecordRepository;
        
        private readonly IRepository<EmailAccount> _emailAccountRepository;
        private readonly IRepository<MessageTemplate> _messageTemplateRepository;
      
        private readonly IRepository<Country> _countryRepository;
        private readonly IRepository<StateProvince> _stateProvinceRepository;
         
        private readonly IRepository<Topic> _topicRepository;
        private readonly IRepository<NewsItem> _newsItemRepository;
       
        private readonly IRepository<ActivityLogType> _activityLogTypeRepository;
    
        private readonly IRepository<ScheduleTask> _scheduleTaskRepository;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public CodeFirstInstallationService(IRepository<Store> storeRepository,
            IRepository<MeasureDimension> measureDimensionRepository,
            IRepository<MeasureWeight> measureWeightRepository,
         
            IRepository<Language> languageRepository,
            IRepository<Currency> currencyRepository,
            IRepository<Customer> customerRepository,
            IRepository<CustomerRole> customerRoleRepository,
           
            IRepository<UrlRecord> urlRecordRepository,
           
            IRepository<EmailAccount> emailAccountRepository,
            IRepository<MessageTemplate> messageTemplateRepository,
           

            IRepository<Country> countryRepository,
            IRepository<StateProvince> stateProvinceRepository,
            
            IRepository<Topic> topicRepository,
            IRepository<NewsItem> newsItemRepository,
         
            IRepository<ActivityLogType> activityLogTypeRepository,
         
            IRepository<ScheduleTask> scheduleTaskRepository,
            IGenericAttributeService genericAttributeService,
            IWebHelper webHelper)
        {
            this._storeRepository = storeRepository;
            this._measureDimensionRepository = measureDimensionRepository;
            this._measureWeightRepository = measureWeightRepository;
          
            this._languageRepository = languageRepository;
            this._currencyRepository = currencyRepository;
            this._customerRepository = customerRepository;
            this._customerRoleRepository = customerRoleRepository;
          
            this._urlRecordRepository = urlRecordRepository;
            
            this._emailAccountRepository = emailAccountRepository;
            this._messageTemplateRepository = messageTemplateRepository;
       
            this._countryRepository = countryRepository;
            this._stateProvinceRepository = stateProvinceRepository;
        
            this._topicRepository = topicRepository;
            this._newsItemRepository = newsItemRepository;
          
            this._activityLogTypeRepository = activityLogTypeRepository;
           
            this._scheduleTaskRepository = scheduleTaskRepository;
            this._genericAttributeService = genericAttributeService;
            this._webHelper = webHelper;
        }

        #endregion
        
        #region Utilities

        protected virtual void InstallStores()
        {
            //var storeUrl = "http://www.yourStore.com/";
            var storeUrl = _webHelper.GetStoreLocation(false);
            var stores = new List<Store>()
            {
                new Store()
                {
                    Name = "Your store name",
                    Url = storeUrl,
                    SslEnabled = false,
                    Hosts = "yourstore.com,www.yourstore.com",
                    DisplayOrder = 1,
                },
            };

            stores.ForEach(x => _storeRepository.Insert(x));
        }

     
    
        protected virtual void InstallLanguages()
        {
            var language = new Language
            {
                Name = "English",
                LanguageCulture = "en-US",
                UniqueSeoCode = "en",
                FlagImageFileName = "us.png",
                Published = true,
                DisplayOrder = 1
            };
            _languageRepository.Insert(language);
        }

        protected virtual void InstallLocaleResources()
        {
            //'English' language
            var language = _languageRepository.Table.Single(l => l.Name == "English");

            //save resources
            foreach (var filePath in System.IO.Directory.EnumerateFiles(_webHelper.MapPath("~/App_Data/Localization/"), "*.nopres.xml", SearchOption.TopDirectoryOnly))
            {
                var localesXml = File.ReadAllText(filePath);
                var localizationService = EngineContext.Current.Resolve<ILocalizationService>();
                localizationService.ImportResourcesFromXml(language, localesXml);
            }

        }

        protected virtual void InstallCurrencies()
        {
            var currencies = new List<Currency>()
            {
                new Currency
                {
                    Name = "US Dollar",
                    CurrencyCode = "USD",
                    Rate = 1,
                    DisplayLocale = "en-US",
                    CustomFormatting = "",
                    Published = true,
                    DisplayOrder = 1,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                new Currency
                {
                    Name = "Australian Dollar",
                    CurrencyCode = "AUD",
                    Rate = 0.94M,
                    DisplayLocale = "en-AU",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 2,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                new Currency
                {
                    Name = "British Pound",
                    CurrencyCode = "GBP",
                    Rate = 0.61M,
                    DisplayLocale = "en-GB",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 3,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                new Currency
                {
                    Name = "Canadian Dollar",
                    CurrencyCode = "CAD",
                    Rate = 0.98M,
                    DisplayLocale = "en-CA",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 4,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                new Currency
                {
                    Name = "Chinese Yuan Renminbi",
                    CurrencyCode = "CNY",
                    Rate = 6.48M,
                    DisplayLocale = "zh-CN",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 5,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                new Currency
                {
                    Name = "Euro",
                    CurrencyCode = "EUR",
                    Rate = 0.79M,
                    DisplayLocale = "",
                    //CustomFormatting = "?.00",
                    CustomFormatting = string.Format("{0}0.00", "\u20ac"),
                    Published = true,
                    DisplayOrder = 6,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                new Currency
                {
                    Name = "Hong Kong Dollar",
                    CurrencyCode = "HKD",
                    Rate = 7.75M,
                    DisplayLocale = "zh-HK",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 7,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                new Currency
                {
                    Name = "Japanese Yen",
                    CurrencyCode = "JPY",
                    Rate = 80.07M,
                    DisplayLocale = "ja-JP",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 8,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                new Currency
                {
                    Name = "Russian Rouble",
                    CurrencyCode = "RUB",
                    Rate = 27.7M,
                    DisplayLocale = "ru-RU",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 9,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                new Currency
                {
                    Name = "Swedish Krona",
                    CurrencyCode = "SEK",
                    Rate = 6.19M,
                    DisplayLocale = "sv-SE",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 10,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
                new Currency
                {
                    Name = "Romanian Leu",
                    CurrencyCode = "RON",
                    Rate = 2.85M,
                    DisplayLocale = "ro-RO",
                    CustomFormatting = "",
                    Published = false,
                    DisplayOrder = 11,
                    CreatedOnUtc = DateTime.UtcNow,
                    UpdatedOnUtc = DateTime.UtcNow,
                },
            };
            currencies.ForEach(c => _currencyRepository.Insert(c));
        }

        protected virtual void InstallCountriesAndStates()
        {
            var cUsa = new Country
            {
                Name = "United States",
                AllowsBilling = true,
                AllowsShipping = true,
                TwoLetterIsoCode = "US",
                ThreeLetterIsoCode = "USA",
                NumericIsoCode = 840,
                SubjectToVat = false,
                DisplayOrder = 1,
                Published = true,
            };
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "AA (Armed Forces Americas)",
                Abbreviation = "AA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "AE (Armed Forces Europe)",
                Abbreviation = "AE",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Alabama",
                Abbreviation = "AL",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Alaska",
                Abbreviation = "AK",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "American Samoa",
                Abbreviation = "AS",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "AP (Armed Forces Pacific)",
                Abbreviation = "AP",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Arizona",
                Abbreviation = "AZ",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Arkansas",
                Abbreviation = "AR",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "California",
                Abbreviation = "CA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Colorado",
                Abbreviation = "CO",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Connecticut",
                Abbreviation = "CT",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Delaware",
                Abbreviation = "DE",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "District of Columbia",
                Abbreviation = "DC",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Federated States of Micronesia",
                Abbreviation = "FM",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Florida",
                Abbreviation = "FL",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Georgia",
                Abbreviation = "GA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Guam",
                Abbreviation = "GU",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Hawaii",
                Abbreviation = "HI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Idaho",
                Abbreviation = "ID",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Illinois",
                Abbreviation = "IL",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Indiana",
                Abbreviation = "IN",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Iowa",
                Abbreviation = "IA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Kansas",
                Abbreviation = "KS",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Kentucky",
                Abbreviation = "KY",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Louisiana",
                Abbreviation = "LA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Maine",
                Abbreviation = "ME",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Marshall Islands",
                Abbreviation = "MH",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Maryland",
                Abbreviation = "MD",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Massachusetts",
                Abbreviation = "MA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Michigan",
                Abbreviation = "MI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Minnesota",
                Abbreviation = "MN",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Mississippi",
                Abbreviation = "MS",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Missouri",
                Abbreviation = "MO",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Montana",
                Abbreviation = "MT",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Nebraska",
                Abbreviation = "NE",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Nevada",
                Abbreviation = "NV",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "New Hampshire",
                Abbreviation = "NH",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "New Jersey",
                Abbreviation = "NJ",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "New Mexico",
                Abbreviation = "NM",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "New York",
                Abbreviation = "NY",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "North Carolina",
                Abbreviation = "NC",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "North Dakota",
                Abbreviation = "ND",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Northern Mariana Islands",
                Abbreviation = "MP",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Ohio",
                Abbreviation = "OH",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Oklahoma",
                Abbreviation = "OK",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Oregon",
                Abbreviation = "OR",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Palau",
                Abbreviation = "PW",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Pennsylvania",
                Abbreviation = "PA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Puerto Rico",
                Abbreviation = "PR",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Rhode Island",
                Abbreviation = "RI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "South Carolina",
                Abbreviation = "SC",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "South Dakota",
                Abbreviation = "SD",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Tennessee",
                Abbreviation = "TN",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Texas",
                Abbreviation = "TX",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Utah",
                Abbreviation = "UT",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Vermont",
                Abbreviation = "VT",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Virgin Islands",
                Abbreviation = "VI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Virginia",
                Abbreviation = "VA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Washington",
                Abbreviation = "WA",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "West Virginia",
                Abbreviation = "WV",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Wisconsin",
                Abbreviation = "WI",
                Published = true,
                DisplayOrder = 1,
            });
            cUsa.StateProvinces.Add(new StateProvince()
            {
                Name = "Wyoming",
                Abbreviation = "WY",
                Published = true,
                DisplayOrder = 1,
            });
            var cCanada = new Country
            {
                Name = "Canada",
                AllowsBilling = true,
                AllowsShipping = true,
                TwoLetterIsoCode = "CA",
                ThreeLetterIsoCode = "CAN",
                NumericIsoCode = 124,
                SubjectToVat = false,
                DisplayOrder = 2,
                Published = true,
            };
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Alberta",
                Abbreviation = "AB",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "British Columbia",
                Abbreviation = "BC",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Manitoba",
                Abbreviation = "MB",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "New Brunswick",
                Abbreviation = "NB",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Newfoundland and Labrador",
                Abbreviation = "NL",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Northwest Territories",
                Abbreviation = "NT",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Nova Scotia",
                Abbreviation = "NS",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Nunavut",
                Abbreviation = "NU",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Ontario",
                Abbreviation = "ON",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Prince Edward Island",
                Abbreviation = "PE",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Quebec",
                Abbreviation = "QC",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Saskatchewan",
                Abbreviation = "SK",
                Published = true,
                DisplayOrder = 1,
            });
            cCanada.StateProvinces.Add(new StateProvince()
            {
                Name = "Yukon Territory",
                Abbreviation = "YU",
                Published = true,
                DisplayOrder = 1,
            });
            var countries = new List<Country>
                                {
                                    cUsa,
                                    cCanada,
                                    //other countries
                                    new Country
                                    {
	                                    Name = "Argentina",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "AR",
	                                    ThreeLetterIsoCode = "ARG",
	                                    NumericIsoCode = 32,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Armenia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "AM",
	                                    ThreeLetterIsoCode = "ARM",
	                                    NumericIsoCode = 51,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Aruba",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "AW",
	                                    ThreeLetterIsoCode = "ABW",
	                                    NumericIsoCode = 533,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Australia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "AU",
	                                    ThreeLetterIsoCode = "AUS",
	                                    NumericIsoCode = 36,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Austria",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "AT",
	                                    ThreeLetterIsoCode = "AUT",
	                                    NumericIsoCode = 40,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Azerbaijan",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "AZ",
	                                    ThreeLetterIsoCode = "AZE",
	                                    NumericIsoCode = 31,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Bahamas",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "BS",
	                                    ThreeLetterIsoCode = "BHS",
	                                    NumericIsoCode = 44,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Bangladesh",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "BD",
	                                    ThreeLetterIsoCode = "BGD",
	                                    NumericIsoCode = 50,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Belarus",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "BY",
	                                    ThreeLetterIsoCode = "BLR",
	                                    NumericIsoCode = 112,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Belgium",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "BE",
	                                    ThreeLetterIsoCode = "BEL",
	                                    NumericIsoCode = 56,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Belize",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "BZ",
	                                    ThreeLetterIsoCode = "BLZ",
	                                    NumericIsoCode = 84,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Bermuda",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "BM",
	                                    ThreeLetterIsoCode = "BMU",
	                                    NumericIsoCode = 60,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Bolivia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "BO",
	                                    ThreeLetterIsoCode = "BOL",
	                                    NumericIsoCode = 68,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Bosnia and Herzegowina",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "BA",
	                                    ThreeLetterIsoCode = "BIH",
	                                    NumericIsoCode = 70,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Brazil",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "BR",
	                                    ThreeLetterIsoCode = "BRA",
	                                    NumericIsoCode = 76,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Bulgaria",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "BG",
	                                    ThreeLetterIsoCode = "BGR",
	                                    NumericIsoCode = 100,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Cayman Islands",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "KY",
	                                    ThreeLetterIsoCode = "CYM",
	                                    NumericIsoCode = 136,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Chile",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "CL",
	                                    ThreeLetterIsoCode = "CHL",
	                                    NumericIsoCode = 152,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "China",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "CN",
	                                    ThreeLetterIsoCode = "CHN",
	                                    NumericIsoCode = 156,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Colombia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "CO",
	                                    ThreeLetterIsoCode = "COL",
	                                    NumericIsoCode = 170,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Costa Rica",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "CR",
	                                    ThreeLetterIsoCode = "CRI",
	                                    NumericIsoCode = 188,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Croatia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "HR",
	                                    ThreeLetterIsoCode = "HRV",
	                                    NumericIsoCode = 191,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Cuba",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "CU",
	                                    ThreeLetterIsoCode = "CUB",
	                                    NumericIsoCode = 192,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Cyprus",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "CY",
	                                    ThreeLetterIsoCode = "CYP",
	                                    NumericIsoCode = 196,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Czech Republic",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "CZ",
	                                    ThreeLetterIsoCode = "CZE",
	                                    NumericIsoCode = 203,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Denmark",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "DK",
	                                    ThreeLetterIsoCode = "DNK",
	                                    NumericIsoCode = 208,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Dominican Republic",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "DO",
	                                    ThreeLetterIsoCode = "DOM",
	                                    NumericIsoCode = 214,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Ecuador",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "EC",
	                                    ThreeLetterIsoCode = "ECU",
	                                    NumericIsoCode = 218,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Egypt",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "EG",
	                                    ThreeLetterIsoCode = "EGY",
	                                    NumericIsoCode = 818,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Finland",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "FI",
	                                    ThreeLetterIsoCode = "FIN",
	                                    NumericIsoCode = 246,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "France",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "FR",
	                                    ThreeLetterIsoCode = "FRA",
	                                    NumericIsoCode = 250,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Georgia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "GE",
	                                    ThreeLetterIsoCode = "GEO",
	                                    NumericIsoCode = 268,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Germany",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "DE",
	                                    ThreeLetterIsoCode = "DEU",
	                                    NumericIsoCode = 276,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Gibraltar",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "GI",
	                                    ThreeLetterIsoCode = "GIB",
	                                    NumericIsoCode = 292,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Greece",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "GR",
	                                    ThreeLetterIsoCode = "GRC",
	                                    NumericIsoCode = 300,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Guatemala",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "GT",
	                                    ThreeLetterIsoCode = "GTM",
	                                    NumericIsoCode = 320,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Hong Kong",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "HK",
	                                    ThreeLetterIsoCode = "HKG",
	                                    NumericIsoCode = 344,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Hungary",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "HU",
	                                    ThreeLetterIsoCode = "HUN",
	                                    NumericIsoCode = 348,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "India",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "IN",
	                                    ThreeLetterIsoCode = "IND",
	                                    NumericIsoCode = 356,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Indonesia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "ID",
	                                    ThreeLetterIsoCode = "IDN",
	                                    NumericIsoCode = 360,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Ireland",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "IE",
	                                    ThreeLetterIsoCode = "IRL",
	                                    NumericIsoCode = 372,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Israel",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "IL",
	                                    ThreeLetterIsoCode = "ISR",
	                                    NumericIsoCode = 376,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Italy",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "IT",
	                                    ThreeLetterIsoCode = "ITA",
	                                    NumericIsoCode = 380,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Jamaica",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "JM",
	                                    ThreeLetterIsoCode = "JAM",
	                                    NumericIsoCode = 388,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Japan",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "JP",
	                                    ThreeLetterIsoCode = "JPN",
	                                    NumericIsoCode = 392,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Jordan",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "JO",
	                                    ThreeLetterIsoCode = "JOR",
	                                    NumericIsoCode = 400,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Kazakhstan",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "KZ",
	                                    ThreeLetterIsoCode = "KAZ",
	                                    NumericIsoCode = 398,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Korea, Democratic People's Republic of",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "KP",
	                                    ThreeLetterIsoCode = "PRK",
	                                    NumericIsoCode = 408,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Kuwait",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "KW",
	                                    ThreeLetterIsoCode = "KWT",
	                                    NumericIsoCode = 414,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Malaysia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "MY",
	                                    ThreeLetterIsoCode = "MYS",
	                                    NumericIsoCode = 458,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Mexico",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "MX",
	                                    ThreeLetterIsoCode = "MEX",
	                                    NumericIsoCode = 484,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Netherlands",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "NL",
	                                    ThreeLetterIsoCode = "NLD",
	                                    NumericIsoCode = 528,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "New Zealand",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "NZ",
	                                    ThreeLetterIsoCode = "NZL",
	                                    NumericIsoCode = 554,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Norway",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "NO",
	                                    ThreeLetterIsoCode = "NOR",
	                                    NumericIsoCode = 578,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Pakistan",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "PK",
	                                    ThreeLetterIsoCode = "PAK",
	                                    NumericIsoCode = 586,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Paraguay",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "PY",
	                                    ThreeLetterIsoCode = "PRY",
	                                    NumericIsoCode = 600,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Peru",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "PE",
	                                    ThreeLetterIsoCode = "PER",
	                                    NumericIsoCode = 604,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Philippines",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "PH",
	                                    ThreeLetterIsoCode = "PHL",
	                                    NumericIsoCode = 608,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Poland",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "PL",
	                                    ThreeLetterIsoCode = "POL",
	                                    NumericIsoCode = 616,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Portugal",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "PT",
	                                    ThreeLetterIsoCode = "PRT",
	                                    NumericIsoCode = 620,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Puerto Rico",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "PR",
	                                    ThreeLetterIsoCode = "PRI",
	                                    NumericIsoCode = 630,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Qatar",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "QA",
	                                    ThreeLetterIsoCode = "QAT",
	                                    NumericIsoCode = 634,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Romania",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "RO",
	                                    ThreeLetterIsoCode = "ROM",
	                                    NumericIsoCode = 642,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Russia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "RU",
	                                    ThreeLetterIsoCode = "RUS",
	                                    NumericIsoCode = 643,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Saudi Arabia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "SA",
	                                    ThreeLetterIsoCode = "SAU",
	                                    NumericIsoCode = 682,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Singapore",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "SG",
	                                    ThreeLetterIsoCode = "SGP",
	                                    NumericIsoCode = 702,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Slovakia (Slovak Republic)",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "SK",
	                                    ThreeLetterIsoCode = "SVK",
	                                    NumericIsoCode = 703,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Slovenia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "SI",
	                                    ThreeLetterIsoCode = "SVN",
	                                    NumericIsoCode = 705,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "South Africa",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "ZA",
	                                    ThreeLetterIsoCode = "ZAF",
	                                    NumericIsoCode = 710,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Spain",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "ES",
	                                    ThreeLetterIsoCode = "ESP",
	                                    NumericIsoCode = 724,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Sweden",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "SE",
	                                    ThreeLetterIsoCode = "SWE",
	                                    NumericIsoCode = 752,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Switzerland",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "CH",
	                                    ThreeLetterIsoCode = "CHE",
	                                    NumericIsoCode = 756,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Taiwan",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "TW",
	                                    ThreeLetterIsoCode = "TWN",
	                                    NumericIsoCode = 158,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Thailand",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "TH",
	                                    ThreeLetterIsoCode = "THA",
	                                    NumericIsoCode = 764,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Turkey",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "TR",
	                                    ThreeLetterIsoCode = "TUR",
	                                    NumericIsoCode = 792,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Ukraine",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "UA",
	                                    ThreeLetterIsoCode = "UKR",
	                                    NumericIsoCode = 804,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "United Arab Emirates",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "AE",
	                                    ThreeLetterIsoCode = "ARE",
	                                    NumericIsoCode = 784,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "United Kingdom",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "GB",
	                                    ThreeLetterIsoCode = "GBR",
	                                    NumericIsoCode = 826,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "United States minor outlying islands",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "UM",
	                                    ThreeLetterIsoCode = "UMI",
	                                    NumericIsoCode = 581,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Uruguay",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "UY",
	                                    ThreeLetterIsoCode = "URY",
	                                    NumericIsoCode = 858,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Uzbekistan",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "UZ",
	                                    ThreeLetterIsoCode = "UZB",
	                                    NumericIsoCode = 860,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Venezuela",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "VE",
	                                    ThreeLetterIsoCode = "VEN",
	                                    NumericIsoCode = 862,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Serbia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "RS",
	                                    ThreeLetterIsoCode = "SRB",
	                                    NumericIsoCode = 688,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Afghanistan",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "AF",
	                                    ThreeLetterIsoCode = "AFG",
	                                    NumericIsoCode = 4,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Albania",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "AL",
	                                    ThreeLetterIsoCode = "ALB",
	                                    NumericIsoCode = 8,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Algeria",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "DZ",
	                                    ThreeLetterIsoCode = "DZA",
	                                    NumericIsoCode = 12,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "American Samoa",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "AS",
	                                    ThreeLetterIsoCode = "ASM",
	                                    NumericIsoCode = 16,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Andorra",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "AD",
	                                    ThreeLetterIsoCode = "AND",
	                                    NumericIsoCode = 20,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Angola",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "AO",
	                                    ThreeLetterIsoCode = "AGO",
	                                    NumericIsoCode = 24,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Anguilla",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "AI",
	                                    ThreeLetterIsoCode = "AIA",
	                                    NumericIsoCode = 660,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Antarctica",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "AQ",
	                                    ThreeLetterIsoCode = "ATA",
	                                    NumericIsoCode = 10,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Antigua and Barbuda",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "AG",
	                                    ThreeLetterIsoCode = "ATG",
	                                    NumericIsoCode = 28,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Bahrain",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "BH",
	                                    ThreeLetterIsoCode = "BHR",
	                                    NumericIsoCode = 48,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Barbados",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "BB",
	                                    ThreeLetterIsoCode = "BRB",
	                                    NumericIsoCode = 52,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Benin",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "BJ",
	                                    ThreeLetterIsoCode = "BEN",
	                                    NumericIsoCode = 204,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Bhutan",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "BT",
	                                    ThreeLetterIsoCode = "BTN",
	                                    NumericIsoCode = 64,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Botswana",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "BW",
	                                    ThreeLetterIsoCode = "BWA",
	                                    NumericIsoCode = 72,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Bouvet Island",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "BV",
	                                    ThreeLetterIsoCode = "BVT",
	                                    NumericIsoCode = 74,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "British Indian Ocean Territory",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "IO",
	                                    ThreeLetterIsoCode = "IOT",
	                                    NumericIsoCode = 86,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Brunei Darussalam",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "BN",
	                                    ThreeLetterIsoCode = "BRN",
	                                    NumericIsoCode = 96,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Burkina Faso",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "BF",
	                                    ThreeLetterIsoCode = "BFA",
	                                    NumericIsoCode = 854,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Burundi",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "BI",
	                                    ThreeLetterIsoCode = "BDI",
	                                    NumericIsoCode = 108,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Cambodia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "KH",
	                                    ThreeLetterIsoCode = "KHM",
	                                    NumericIsoCode = 116,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Cameroon",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "CM",
	                                    ThreeLetterIsoCode = "CMR",
	                                    NumericIsoCode = 120,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Cape Verde",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "CV",
	                                    ThreeLetterIsoCode = "CPV",
	                                    NumericIsoCode = 132,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Central African Republic",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "CF",
	                                    ThreeLetterIsoCode = "CAF",
	                                    NumericIsoCode = 140,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Chad",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "TD",
	                                    ThreeLetterIsoCode = "TCD",
	                                    NumericIsoCode = 148,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Christmas Island",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "CX",
	                                    ThreeLetterIsoCode = "CXR",
	                                    NumericIsoCode = 162,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Cocos (Keeling) Islands",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "CC",
	                                    ThreeLetterIsoCode = "CCK",
	                                    NumericIsoCode = 166,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Comoros",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "KM",
	                                    ThreeLetterIsoCode = "COM",
	                                    NumericIsoCode = 174,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Congo",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "CG",
	                                    ThreeLetterIsoCode = "COG",
	                                    NumericIsoCode = 178,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Cook Islands",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "CK",
	                                    ThreeLetterIsoCode = "COK",
	                                    NumericIsoCode = 184,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Cote D'Ivoire",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "CI",
	                                    ThreeLetterIsoCode = "CIV",
	                                    NumericIsoCode = 384,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Djibouti",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "DJ",
	                                    ThreeLetterIsoCode = "DJI",
	                                    NumericIsoCode = 262,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Dominica",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "DM",
	                                    ThreeLetterIsoCode = "DMA",
	                                    NumericIsoCode = 212,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "El Salvador",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "SV",
	                                    ThreeLetterIsoCode = "SLV",
	                                    NumericIsoCode = 222,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Equatorial Guinea",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "GQ",
	                                    ThreeLetterIsoCode = "GNQ",
	                                    NumericIsoCode = 226,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Eritrea",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "ER",
	                                    ThreeLetterIsoCode = "ERI",
	                                    NumericIsoCode = 232,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Estonia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "EE",
	                                    ThreeLetterIsoCode = "EST",
	                                    NumericIsoCode = 233,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Ethiopia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "ET",
	                                    ThreeLetterIsoCode = "ETH",
	                                    NumericIsoCode = 231,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Falkland Islands (Malvinas)",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "FK",
	                                    ThreeLetterIsoCode = "FLK",
	                                    NumericIsoCode = 238,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Faroe Islands",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "FO",
	                                    ThreeLetterIsoCode = "FRO",
	                                    NumericIsoCode = 234,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Fiji",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "FJ",
	                                    ThreeLetterIsoCode = "FJI",
	                                    NumericIsoCode = 242,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "French Guiana",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "GF",
	                                    ThreeLetterIsoCode = "GUF",
	                                    NumericIsoCode = 254,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "French Polynesia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "PF",
	                                    ThreeLetterIsoCode = "PYF",
	                                    NumericIsoCode = 258,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "French Southern Territories",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "TF",
	                                    ThreeLetterIsoCode = "ATF",
	                                    NumericIsoCode = 260,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Gabon",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "GA",
	                                    ThreeLetterIsoCode = "GAB",
	                                    NumericIsoCode = 266,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Gambia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "GM",
	                                    ThreeLetterIsoCode = "GMB",
	                                    NumericIsoCode = 270,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Ghana",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "GH",
	                                    ThreeLetterIsoCode = "GHA",
	                                    NumericIsoCode = 288,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Greenland",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "GL",
	                                    ThreeLetterIsoCode = "GRL",
	                                    NumericIsoCode = 304,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Grenada",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "GD",
	                                    ThreeLetterIsoCode = "GRD",
	                                    NumericIsoCode = 308,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Guadeloupe",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "GP",
	                                    ThreeLetterIsoCode = "GLP",
	                                    NumericIsoCode = 312,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Guam",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "GU",
	                                    ThreeLetterIsoCode = "GUM",
	                                    NumericIsoCode = 316,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Guinea",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "GN",
	                                    ThreeLetterIsoCode = "GIN",
	                                    NumericIsoCode = 324,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Guinea-bissau",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "GW",
	                                    ThreeLetterIsoCode = "GNB",
	                                    NumericIsoCode = 624,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Guyana",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "GY",
	                                    ThreeLetterIsoCode = "GUY",
	                                    NumericIsoCode = 328,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Haiti",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "HT",
	                                    ThreeLetterIsoCode = "HTI",
	                                    NumericIsoCode = 332,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Heard and Mc Donald Islands",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "HM",
	                                    ThreeLetterIsoCode = "HMD",
	                                    NumericIsoCode = 334,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Honduras",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "HN",
	                                    ThreeLetterIsoCode = "HND",
	                                    NumericIsoCode = 340,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Iceland",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "IS",
	                                    ThreeLetterIsoCode = "ISL",
	                                    NumericIsoCode = 352,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Iran (Islamic Republic of)",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "IR",
	                                    ThreeLetterIsoCode = "IRN",
	                                    NumericIsoCode = 364,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Iraq",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "IQ",
	                                    ThreeLetterIsoCode = "IRQ",
	                                    NumericIsoCode = 368,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Kenya",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "KE",
	                                    ThreeLetterIsoCode = "KEN",
	                                    NumericIsoCode = 404,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Kiribati",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "KI",
	                                    ThreeLetterIsoCode = "KIR",
	                                    NumericIsoCode = 296,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Korea",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "KR",
	                                    ThreeLetterIsoCode = "KOR",
	                                    NumericIsoCode = 410,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Kyrgyzstan",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "KG",
	                                    ThreeLetterIsoCode = "KGZ",
	                                    NumericIsoCode = 417,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Lao People's Democratic Republic",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "LA",
	                                    ThreeLetterIsoCode = "LAO",
	                                    NumericIsoCode = 418,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Latvia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "LV",
	                                    ThreeLetterIsoCode = "LVA",
	                                    NumericIsoCode = 428,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Lebanon",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "LB",
	                                    ThreeLetterIsoCode = "LBN",
	                                    NumericIsoCode = 422,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Lesotho",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "LS",
	                                    ThreeLetterIsoCode = "LSO",
	                                    NumericIsoCode = 426,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Liberia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "LR",
	                                    ThreeLetterIsoCode = "LBR",
	                                    NumericIsoCode = 430,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Libyan Arab Jamahiriya",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "LY",
	                                    ThreeLetterIsoCode = "LBY",
	                                    NumericIsoCode = 434,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Liechtenstein",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "LI",
	                                    ThreeLetterIsoCode = "LIE",
	                                    NumericIsoCode = 438,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Lithuania",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "LT",
	                                    ThreeLetterIsoCode = "LTU",
	                                    NumericIsoCode = 440,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Luxembourg",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "LU",
	                                    ThreeLetterIsoCode = "LUX",
	                                    NumericIsoCode = 442,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Macau",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "MO",
	                                    ThreeLetterIsoCode = "MAC",
	                                    NumericIsoCode = 446,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Macedonia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "MK",
	                                    ThreeLetterIsoCode = "MKD",
	                                    NumericIsoCode = 807,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Madagascar",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "MG",
	                                    ThreeLetterIsoCode = "MDG",
	                                    NumericIsoCode = 450,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Malawi",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "MW",
	                                    ThreeLetterIsoCode = "MWI",
	                                    NumericIsoCode = 454,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Maldives",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "MV",
	                                    ThreeLetterIsoCode = "MDV",
	                                    NumericIsoCode = 462,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Mali",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "ML",
	                                    ThreeLetterIsoCode = "MLI",
	                                    NumericIsoCode = 466,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Malta",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "MT",
	                                    ThreeLetterIsoCode = "MLT",
	                                    NumericIsoCode = 470,
	                                    SubjectToVat = true,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Marshall Islands",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "MH",
	                                    ThreeLetterIsoCode = "MHL",
	                                    NumericIsoCode = 584,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Martinique",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "MQ",
	                                    ThreeLetterIsoCode = "MTQ",
	                                    NumericIsoCode = 474,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Mauritania",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "MR",
	                                    ThreeLetterIsoCode = "MRT",
	                                    NumericIsoCode = 478,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Mauritius",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "MU",
	                                    ThreeLetterIsoCode = "MUS",
	                                    NumericIsoCode = 480,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Mayotte",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "YT",
	                                    ThreeLetterIsoCode = "MYT",
	                                    NumericIsoCode = 175,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Micronesia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "FM",
	                                    ThreeLetterIsoCode = "FSM",
	                                    NumericIsoCode = 583,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Moldova",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "MD",
	                                    ThreeLetterIsoCode = "MDA",
	                                    NumericIsoCode = 498,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Monaco",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "MC",
	                                    ThreeLetterIsoCode = "MCO",
	                                    NumericIsoCode = 492,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Mongolia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "MN",
	                                    ThreeLetterIsoCode = "MNG",
	                                    NumericIsoCode = 496,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Montenegro",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "ME",
	                                    ThreeLetterIsoCode = "MNE",
	                                    NumericIsoCode = 499,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Montserrat",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "MS",
	                                    ThreeLetterIsoCode = "MSR",
	                                    NumericIsoCode = 500,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Morocco",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "MA",
	                                    ThreeLetterIsoCode = "MAR",
	                                    NumericIsoCode = 504,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Mozambique",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "MZ",
	                                    ThreeLetterIsoCode = "MOZ",
	                                    NumericIsoCode = 508,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Myanmar",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "MM",
	                                    ThreeLetterIsoCode = "MMR",
	                                    NumericIsoCode = 104,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Namibia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "NA",
	                                    ThreeLetterIsoCode = "NAM",
	                                    NumericIsoCode = 516,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Nauru",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "NR",
	                                    ThreeLetterIsoCode = "NRU",
	                                    NumericIsoCode = 520,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Nepal",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "NP",
	                                    ThreeLetterIsoCode = "NPL",
	                                    NumericIsoCode = 524,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Netherlands Antilles",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "AN",
	                                    ThreeLetterIsoCode = "ANT",
	                                    NumericIsoCode = 530,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "New Caledonia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "NC",
	                                    ThreeLetterIsoCode = "NCL",
	                                    NumericIsoCode = 540,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Nicaragua",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "NI",
	                                    ThreeLetterIsoCode = "NIC",
	                                    NumericIsoCode = 558,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Niger",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "NE",
	                                    ThreeLetterIsoCode = "NER",
	                                    NumericIsoCode = 562,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Nigeria",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "NG",
	                                    ThreeLetterIsoCode = "NGA",
	                                    NumericIsoCode = 566,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Niue",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "NU",
	                                    ThreeLetterIsoCode = "NIU",
	                                    NumericIsoCode = 570,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Norfolk Island",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "NF",
	                                    ThreeLetterIsoCode = "NFK",
	                                    NumericIsoCode = 574,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Northern Mariana Islands",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "MP",
	                                    ThreeLetterIsoCode = "MNP",
	                                    NumericIsoCode = 580,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Oman",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "OM",
	                                    ThreeLetterIsoCode = "OMN",
	                                    NumericIsoCode = 512,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Palau",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "PW",
	                                    ThreeLetterIsoCode = "PLW",
	                                    NumericIsoCode = 585,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Panama",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "PA",
	                                    ThreeLetterIsoCode = "PAN",
	                                    NumericIsoCode = 591,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Papua New Guinea",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "PG",
	                                    ThreeLetterIsoCode = "PNG",
	                                    NumericIsoCode = 598,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Pitcairn",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "PN",
	                                    ThreeLetterIsoCode = "PCN",
	                                    NumericIsoCode = 612,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Reunion",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "RE",
	                                    ThreeLetterIsoCode = "REU",
	                                    NumericIsoCode = 638,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Rwanda",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "RW",
	                                    ThreeLetterIsoCode = "RWA",
	                                    NumericIsoCode = 646,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Saint Kitts and Nevis",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "KN",
	                                    ThreeLetterIsoCode = "KNA",
	                                    NumericIsoCode = 659,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Saint Lucia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "LC",
	                                    ThreeLetterIsoCode = "LCA",
	                                    NumericIsoCode = 662,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Saint Vincent and the Grenadines",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "VC",
	                                    ThreeLetterIsoCode = "VCT",
	                                    NumericIsoCode = 670,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Samoa",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "WS",
	                                    ThreeLetterIsoCode = "WSM",
	                                    NumericIsoCode = 882,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "San Marino",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "SM",
	                                    ThreeLetterIsoCode = "SMR",
	                                    NumericIsoCode = 674,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Sao Tome and Principe",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "ST",
	                                    ThreeLetterIsoCode = "STP",
	                                    NumericIsoCode = 678,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Senegal",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "SN",
	                                    ThreeLetterIsoCode = "SEN",
	                                    NumericIsoCode = 686,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Seychelles",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "SC",
	                                    ThreeLetterIsoCode = "SYC",
	                                    NumericIsoCode = 690,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Sierra Leone",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "SL",
	                                    ThreeLetterIsoCode = "SLE",
	                                    NumericIsoCode = 694,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Solomon Islands",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "SB",
	                                    ThreeLetterIsoCode = "SLB",
	                                    NumericIsoCode = 90,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Somalia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "SO",
	                                    ThreeLetterIsoCode = "SOM",
	                                    NumericIsoCode = 706,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "South Georgia & South Sandwich Islands",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "GS",
	                                    ThreeLetterIsoCode = "SGS",
	                                    NumericIsoCode = 239,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Sri Lanka",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "LK",
	                                    ThreeLetterIsoCode = "LKA",
	                                    NumericIsoCode = 144,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "St. Helena",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "SH",
	                                    ThreeLetterIsoCode = "SHN",
	                                    NumericIsoCode = 654,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "St. Pierre and Miquelon",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "PM",
	                                    ThreeLetterIsoCode = "SPM",
	                                    NumericIsoCode = 666,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Sudan",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "SD",
	                                    ThreeLetterIsoCode = "SDN",
	                                    NumericIsoCode = 736,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Suriname",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "SR",
	                                    ThreeLetterIsoCode = "SUR",
	                                    NumericIsoCode = 740,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Svalbard and Jan Mayen Islands",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "SJ",
	                                    ThreeLetterIsoCode = "SJM",
	                                    NumericIsoCode = 744,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Swaziland",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "SZ",
	                                    ThreeLetterIsoCode = "SWZ",
	                                    NumericIsoCode = 748,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Syrian Arab Republic",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "SY",
	                                    ThreeLetterIsoCode = "SYR",
	                                    NumericIsoCode = 760,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Tajikistan",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "TJ",
	                                    ThreeLetterIsoCode = "TJK",
	                                    NumericIsoCode = 762,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Tanzania",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "TZ",
	                                    ThreeLetterIsoCode = "TZA",
	                                    NumericIsoCode = 834,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Togo",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "TG",
	                                    ThreeLetterIsoCode = "TGO",
	                                    NumericIsoCode = 768,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Tokelau",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "TK",
	                                    ThreeLetterIsoCode = "TKL",
	                                    NumericIsoCode = 772,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Tonga",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "TO",
	                                    ThreeLetterIsoCode = "TON",
	                                    NumericIsoCode = 776,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Trinidad and Tobago",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "TT",
	                                    ThreeLetterIsoCode = "TTO",
	                                    NumericIsoCode = 780,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Tunisia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "TN",
	                                    ThreeLetterIsoCode = "TUN",
	                                    NumericIsoCode = 788,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Turkmenistan",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "TM",
	                                    ThreeLetterIsoCode = "TKM",
	                                    NumericIsoCode = 795,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Turks and Caicos Islands",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "TC",
	                                    ThreeLetterIsoCode = "TCA",
	                                    NumericIsoCode = 796,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Tuvalu",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "TV",
	                                    ThreeLetterIsoCode = "TUV",
	                                    NumericIsoCode = 798,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Uganda",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "UG",
	                                    ThreeLetterIsoCode = "UGA",
	                                    NumericIsoCode = 800,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Vanuatu",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "VU",
	                                    ThreeLetterIsoCode = "VUT",
	                                    NumericIsoCode = 548,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Vatican City State (Holy See)",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "VA",
	                                    ThreeLetterIsoCode = "VAT",
	                                    NumericIsoCode = 336,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Viet Nam",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "VN",
	                                    ThreeLetterIsoCode = "VNM",
	                                    NumericIsoCode = 704,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Virgin Islands (British)",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "VG",
	                                    ThreeLetterIsoCode = "VGB",
	                                    NumericIsoCode = 92,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Virgin Islands (U.S.)",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "VI",
	                                    ThreeLetterIsoCode = "VIR",
	                                    NumericIsoCode = 850,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Wallis and Futuna Islands",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "WF",
	                                    ThreeLetterIsoCode = "WLF",
	                                    NumericIsoCode = 876,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Western Sahara",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "EH",
	                                    ThreeLetterIsoCode = "ESH",
	                                    NumericIsoCode = 732,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Yemen",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "YE",
	                                    ThreeLetterIsoCode = "YEM",
	                                    NumericIsoCode = 887,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Zambia",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "ZM",
	                                    ThreeLetterIsoCode = "ZMB",
	                                    NumericIsoCode = 894,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                    new Country
                                    {
	                                    Name = "Zimbabwe",
	                                    AllowsBilling = true,
	                                    AllowsShipping = true,
	                                    TwoLetterIsoCode = "ZW",
	                                    ThreeLetterIsoCode = "ZWE",
	                                    NumericIsoCode = 716,
	                                    SubjectToVat = false,
	                                    DisplayOrder = 100,
	                                    Published = true
                                    },
                                };
            countries.ForEach(c => _countryRepository.Insert(c));
        }
         

        protected virtual void InstallCustomersAndUsers(string defaultUserEmail, string defaultUserPassword)
        {
            var crAdministrators = new CustomerRole
            {
                Name = "Administrators",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Administrators,
            };
            var crForumModerators = new CustomerRole
            {
                Name = "Forum Moderators",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.ForumModerators,
            };
            var crRegistered = new CustomerRole
            {
                Name = "Registered",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Registered,
            };
            var crGuests = new CustomerRole
            {
                Name = "Guests",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Guests,
            };
            var crVendors = new CustomerRole
            {
                Name = "Vendors",
                Active = true,
                IsSystemRole = true,
                SystemName = SystemCustomerRoleNames.Vendors,
            };
            var customerRoles = new List<CustomerRole>
                                {
                                    crAdministrators,
                                    crForumModerators,
                                    crRegistered,
                                    crGuests,
                                    crVendors
                                };
            customerRoles.ForEach(cr => _customerRoleRepository.Insert(cr));

            //admin user
            var adminUser = new Customer()
            {
                CustomerGuid = Guid.NewGuid(),
                Email = defaultUserEmail,
                Username = defaultUserEmail,
                Password = defaultUserPassword,
                PasswordFormat = PasswordFormat.Clear,
                PasswordSalt = "",
                Active = true,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc= DateTime.UtcNow,
            };
            var defaultAdminUserAddress = new Address()
            {
                FirstName = "John",
                LastName = "Smith",
                PhoneNumber = "12345678",
                Email = "admin@yourStore.com",
                FaxNumber = "",
                Company = "Nop Solutions",
                Address1 = "21 West 52nd Street",
                Address2 = "",
                City = "New York",
                StateProvince = _stateProvinceRepository.Table.FirstOrDefault(sp => sp.Name == "New York"),
                Country = _countryRepository.Table.FirstOrDefault(c => c.ThreeLetterIsoCode == "USA"),
                ZipPostalCode = "10021",
                CreatedOnUtc = DateTime.UtcNow,
            };
            adminUser.Addresses.Add(defaultAdminUserAddress);
            adminUser.BillingAddress = defaultAdminUserAddress;
            adminUser.ShippingAddress = defaultAdminUserAddress;
            adminUser.CustomerRoles.Add(crAdministrators);
            adminUser.CustomerRoles.Add(crForumModerators);
            adminUser.CustomerRoles.Add(crRegistered);
            _customerRepository.Insert(adminUser);
            //set default customer name
            _genericAttributeService.SaveAttribute(adminUser, SystemCustomerAttributeNames.FirstName, "John");
            _genericAttributeService.SaveAttribute(adminUser, SystemCustomerAttributeNames.LastName, "Smith");


            //search engine (crawler) built-in user
            var searchEngineUser = new Customer()
            {
                Email = "builtin@search_engine_record.com",
                CustomerGuid = Guid.NewGuid(),
                PasswordFormat = PasswordFormat.Clear,
                AdminComment = "Built-in system guest record used for requests from search engines.",
                Active = true,
                IsSystemAccount = true,
                SystemName = SystemCustomerNames.SearchEngine,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
            };
            searchEngineUser.CustomerRoles.Add(crGuests);
            _customerRepository.Insert(searchEngineUser);


            //built-in user for background tasks
            var backgroundTaskUser = new Customer()
            {
                Email = "builtin@background-task-record.com",
                CustomerGuid = Guid.NewGuid(),
                PasswordFormat = PasswordFormat.Clear,
                AdminComment = "Built-in system record used for background tasks.",
                Active = true,
                IsSystemAccount = true,
                SystemName = SystemCustomerNames.BackgroundTask,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
            };
            backgroundTaskUser.CustomerRoles.Add(crGuests);
            _customerRepository.Insert(backgroundTaskUser);
        }

        protected virtual void HashDefaultCustomerPassword(string defaultUserEmail, string defaultUserPassword)
        {
            var customerRegistrationService = EngineContext.Current.Resolve<ICustomerRegistrationService>();
            customerRegistrationService.ChangePassword(new ChangePasswordRequest(defaultUserEmail, false,
                 PasswordFormat.Hashed, defaultUserPassword));
        }

        protected virtual void InstallEmailAccounts()
        {
            var emailAccounts = new List<EmailAccount>
                               {
                                   new EmailAccount
                                       {
                                           Email = "test@mail.com",
                                           DisplayName = "Store name",
                                           Host = "smtp.mail.com",
                                           Port = 25,
                                           Username = "123",
                                           Password = "123",
                                           EnableSsl = false,
                                           UseDefaultCredentials = false
                                       },
                               };
            emailAccounts.ForEach(ea => _emailAccountRepository.Insert(ea));

        }

        protected virtual void InstallMessageTemplates()
        {
            var eaGeneral = _emailAccountRepository.Table.FirstOrDefault();
            if (eaGeneral == null)
                throw new Exception("Default email account cannot be loaded");
            var messageTemplates = new List<MessageTemplate>
                               {
                                   new MessageTemplate
                                       {
                                           Name = "Blog.BlogComment",
                                           Subject = "%Store.Name%. New blog comment.",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />A new blog comment has been created for blog post \"%BlogComment.BlogPostTitle%\".</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Customer.BackInStock",
                                           Subject = "%Store.Name%. Back in stock notification",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />Hello %Customer.FullName%, <br />Product <a target=\"_blank\" href=\"%BackInStockSubscription.ProductUrl%\">%BackInStockSubscription.ProductName%</a> is in stock.</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Customer.EmailValidationMessage",
                                           Subject = "%Store.Name%. Email validation",
                                           Body = "<a href=\"%Store.URL%\">%Store.Name%</a>  <br />  <br />  To activate your account <a href=\"%Customer.AccountActivationURL%\">click here</a>.     <br />  <br />  %Store.Name%",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Customer.NewPM",
                                           Subject = "%Store.Name%. You have received a new private message",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />You have received a new private message.</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Customer.PasswordRecovery",
                                           Subject = "%Store.Name%. Password recovery",
                                           Body = "<a href=\"%Store.URL%\">%Store.Name%</a>  <br />  <br />  To change your password <a href=\"%Customer.PasswordRecoveryURL%\">click here</a>.     <br />  <br />  %Store.Name%",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Customer.WelcomeMessage",
                                           Subject = "Welcome to %Store.Name%",
                                           Body = "We welcome you to <a href=\"%Store.URL%\"> %Store.Name%</a>.<br /><br />You can now take part in the various services we have to offer you. Some of these services include:<br /><br />Permanent Cart - Any products added to your online cart remain there until you remove them, or check them out.<br />Address Book - We can now deliver your products to another address other than yours! This is perfect to send birthday gifts direct to the birthday-person themselves.<br />Order History - View your history of purchases that you have made with us.<br />Products Reviews - Share your opinions on products with our other customers.<br /><br />For help with any of our online services, please email the store-owner: <a href=\"mailto:%Store.Email%\">%Store.Email%</a>.<br /><br />Note: This email address was provided on our registration page. If you own the email and did not register on our site, please send an email to <a href=\"mailto:%Store.Email%\">%Store.Email%</a>.",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Forums.NewForumPost",
                                           Subject = "%Store.Name%. New Post Notification.",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />A new post has been created in the topic <a href=\"%Forums.TopicURL%\">\"%Forums.TopicName%\"</a> at <a href=\"%Forums.ForumURL%\">\"%Forums.ForumName%\"</a> forum.<br /><br />Click <a href=\"%Forums.TopicURL%\">here</a> for more info.<br /><br />Post author: %Forums.PostAuthor%<br />Post body: %Forums.PostBody%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Forums.NewForumTopic",
                                           Subject = "%Store.Name%. New Topic Notification.",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />A new topic <a href=\"%Forums.TopicURL%\">\"%Forums.TopicName%\"</a> has been created at <a href=\"%Forums.ForumURL%\">\"%Forums.ForumName%\"</a> forum.<br /><br />Click <a href=\"%Forums.TopicURL%\">here</a> for more info.</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "GiftCard.Notification",
                                           Subject = "%GiftCard.SenderName% has sent you a gift card for %Store.Name%",
                                           Body = "<p>You have received a gift card for %Store.Name%</p><p>Dear %GiftCard.RecipientName%, <br /><br />%GiftCard.SenderName% (%GiftCard.SenderEmail%) has sent you a %GiftCard.Amount% gift cart for <a href=\"%Store.URL%\"> %Store.Name%</a></p><p>You gift card code is %GiftCard.CouponCode%</p><p>%GiftCard.Message%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "NewCustomer.Notification",
                                           Subject = "%Store.Name%. New customer registration",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />A new customer registered with your store. Below are the customer's details:<br />Full name: %Customer.FullName%<br />Email: %Customer.Email%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "NewReturnRequest.StoreOwnerNotification",
                                           Subject = "%Store.Name%. New return request.",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />%Customer.FullName% has just submitted a new return request. Details are below:<br />Request ID: %ReturnRequest.ID%<br />Product: %ReturnRequest.Product.Quantity% x Product: %ReturnRequest.Product.Name%<br />Reason for return: %ReturnRequest.Reason%<br />Requested action: %ReturnRequest.RequestedAction%<br />Customer comments:<br />%ReturnRequest.CustomerComment%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "News.NewsComment",
                                           Subject = "%Store.Name%. New news comment.",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />A new news comment has been created for news \"%NewsComment.NewsTitle%\".</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "NewsLetterSubscription.ActivationMessage",
                                           Subject = "%Store.Name%. Subscription activation message.",
                                           Body = "<p><a href=\"%NewsLetterSubscription.ActivationUrl%\">Click here to confirm your subscription to our list.</a></p><p>If you received this email by mistake, simply delete it.</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "NewVATSubmitted.StoreOwnerNotification",
                                           Subject = "%Store.Name%. New VAT number is submitted.",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />%Customer.FullName% (%Customer.Email%) has just submitted a new VAT number. Details are below:<br />VAT number: %Customer.VatNumber%<br />VAT number status: %Customer.VatNumberStatus%<br />Received name: %VatValidationResult.Name%<br />Received address: %VatValidationResult.Address%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "OrderCancelled.CustomerNotification",
                                           Subject = "%Store.Name%. Your order cancelled",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />Hello %Order.CustomerFullName%, <br />Your order has been cancelled. Below is the summary of the order. <br /><br />Order Number: %Order.OrderNumber%<br />Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForCustomer%\">%Order.OrderURLForCustomer%</a><br />Date Ordered: %Order.CreatedOn%<br /><br /><br /><br />Billing Address<br />%Order.BillingFirstName% %Order.BillingLastName%<br />%Order.BillingAddress1%<br />%Order.BillingCity% %Order.BillingZipPostalCode%<br />%Order.BillingStateProvince% %Order.BillingCountry%<br /><br /><br /><br />Shipping Address<br />%Order.ShippingFirstName% %Order.ShippingLastName%<br />%Order.ShippingAddress1%<br />%Order.ShippingCity% %Order.ShippingZipPostalCode%<br />%Order.ShippingStateProvince% %Order.ShippingCountry%<br /><br />Shipping Method: %Order.ShippingMethod%<br /><br />%Order.Product(s)%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "OrderCompleted.CustomerNotification",
                                           Subject = "%Store.Name%. Your order completed",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />Hello %Order.CustomerFullName%, <br />Your order has been completed. Below is the summary of the order. <br /><br />Order Number: %Order.OrderNumber%<br />Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForCustomer%\">%Order.OrderURLForCustomer%</a><br />Date Ordered: %Order.CreatedOn%<br /><br /><br /><br />Billing Address<br />%Order.BillingFirstName% %Order.BillingLastName%<br />%Order.BillingAddress1%<br />%Order.BillingCity% %Order.BillingZipPostalCode%<br />%Order.BillingStateProvince% %Order.BillingCountry%<br /><br /><br /><br />Shipping Address<br />%Order.ShippingFirstName% %Order.ShippingLastName%<br />%Order.ShippingAddress1%<br />%Order.ShippingCity% %Order.ShippingZipPostalCode%<br />%Order.ShippingStateProvince% %Order.ShippingCountry%<br /><br />Shipping Method: %Order.ShippingMethod%<br /><br />%Order.Product(s)%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "ShipmentDelivered.CustomerNotification",
                                           Subject = "Your order from %Store.Name% has been delivered.",
                                           Body = "<p><a href=\"%Store.URL%\"> %Store.Name%</a> <br /> <br /> Hello %Order.CustomerFullName%, <br /> Good news! You order has been delivered. <br /> Order Number: %Order.OrderNumber%<br /> Order Details: <a href=\"%Order.OrderURLForCustomer%\" target=\"_blank\">%Order.OrderURLForCustomer%</a><br /> Date Ordered: %Order.CreatedOn%<br /> <br /> <br /> <br /> Billing Address<br /> %Order.BillingFirstName% %Order.BillingLastName%<br /> %Order.BillingAddress1%<br /> %Order.BillingCity% %Order.BillingZipPostalCode%<br /> %Order.BillingStateProvince% %Order.BillingCountry%<br /> <br /> <br /> <br /> Shipping Address<br /> %Order.ShippingFirstName% %Order.ShippingLastName%<br /> %Order.ShippingAddress1%<br /> %Order.ShippingCity% %Order.ShippingZipPostalCode%<br /> %Order.ShippingStateProvince% %Order.ShippingCountry%<br /> <br /> Shipping Method: %Order.ShippingMethod% <br /> <br /> Delivered Products: <br /> <br /> %Shipment.Product(s)%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "OrderPlaced.CustomerNotification",
                                           Subject = "Order receipt from %Store.Name%.",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />Hello %Order.CustomerFullName%, <br />Thanks for buying from <a href=\"%Store.URL%\">%Store.Name%</a>. Below is the summary of the order. <br /><br />Order Number: %Order.OrderNumber%<br />Order Details: <a target=\"_blank\" href=\"%Order.OrderURLForCustomer%\">%Order.OrderURLForCustomer%</a><br />Date Ordered: %Order.CreatedOn%<br /><br /><br /><br />Billing Address<br />%Order.BillingFirstName% %Order.BillingLastName%<br />%Order.BillingAddress1%<br />%Order.BillingCity% %Order.BillingZipPostalCode%<br />%Order.BillingStateProvince% %Order.BillingCountry%<br /><br /><br /><br />Shipping Address<br />%Order.ShippingFirstName% %Order.ShippingLastName%<br />%Order.ShippingAddress1%<br />%Order.ShippingCity% %Order.ShippingZipPostalCode%<br />%Order.ShippingStateProvince% %Order.ShippingCountry%<br /><br />Shipping Method: %Order.ShippingMethod%<br /><br />%Order.Product(s)%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "OrderPlaced.StoreOwnerNotification",
                                           Subject = "%Store.Name%. Purchase Receipt for Order #%Order.OrderNumber%",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />%Order.CustomerFullName% (%Order.CustomerEmail%) has just placed an order from your store. Below is the summary of the order. <br /><br />Order Number: %Order.OrderNumber%<br />Date Ordered: %Order.CreatedOn%<br /><br /><br /><br />Billing Address<br />%Order.BillingFirstName% %Order.BillingLastName%<br />%Order.BillingAddress1%<br />%Order.BillingCity% %Order.BillingZipPostalCode%<br />%Order.BillingStateProvince% %Order.BillingCountry%<br /><br /><br /><br />Shipping Address<br />%Order.ShippingFirstName% %Order.ShippingLastName%<br />%Order.ShippingAddress1%<br />%Order.ShippingCity% %Order.ShippingZipPostalCode%<br />%Order.ShippingStateProvince% %Order.ShippingCountry%<br /><br />Shipping Method: %Order.ShippingMethod%<br /><br />%Order.Product(s)%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "ShipmentSent.CustomerNotification",
                                           Subject = "Your order from %Store.Name% has been shipped.",
                                           Body = "<p><a href=\"%Store.URL%\"> %Store.Name%</a> <br /><br />Hello %Order.CustomerFullName%!, <br />Good news! You order has been shipped. <br />Order Number: %Order.OrderNumber%<br />Order Details: <a href=\"%Order.OrderURLForCustomer%\" target=\"_blank\">%Order.OrderURLForCustomer%</a><br />Date Ordered: %Order.CreatedOn%<br /><br /><br /><br />Billing Address<br />%Order.BillingFirstName% %Order.BillingLastName%<br />%Order.BillingAddress1%<br />%Order.BillingCity% %Order.BillingZipPostalCode%<br />%Order.BillingStateProvince% %Order.BillingCountry%<br /><br /><br /><br />Shipping Address<br />%Order.ShippingFirstName% %Order.ShippingLastName%<br />%Order.ShippingAddress1%<br />%Order.ShippingCity% %Order.ShippingZipPostalCode%<br />%Order.ShippingStateProvince% %Order.ShippingCountry%<br /><br />Shipping Method: %Order.ShippingMethod% <br /> <br /> Shipped Products: <br /> <br /> %Shipment.Product(s)%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Product.ProductReview",
                                           Subject = "%Store.Name%. New product review.",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />A new product review has been written for product \"%ProductReview.ProductName%\".</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "QuantityBelow.StoreOwnerNotification",
                                           Subject = "%Store.Name%. Quantity below notification. %Product.Name%",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />%Product.Name% (ID: %Product.ID%) low quantity. <br /><br />Quantity: %Product.StockQuantity%<br /></p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "ReturnRequestStatusChanged.CustomerNotification",
                                           Subject = "%Store.Name%. Return request status was changed.",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />Hello %Customer.FullName%,<br />Your return request #%ReturnRequest.ID% status has been changed.</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Service.EmailAFriend",
                                           Subject = "%Store.Name%. Referred Item",
                                           Body = "<p><a href=\"%Store.URL%\"> %Store.Name%</a> <br /><br />%EmailAFriend.Email% was shopping on %Store.Name% and wanted to share the following item with you. <br /><br /><b><a target=\"_blank\" href=\"%Product.ProductURLForCustomer%\">%Product.Name%</a></b> <br />%Product.ShortDescription% <br /><br />For more info click <a target=\"_blank\" href=\"%Product.ProductURLForCustomer%\">here</a> <br /><br /><br />%EmailAFriend.PersonalMessage%<br /><br />%Store.Name%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Wishlist.EmailAFriend",
                                           Subject = "%Store.Name%. Wishlist",
                                           Body = "<p><a href=\"%Store.URL%\"> %Store.Name%</a> <br /><br />%Wishlist.Email% was shopping on %Store.Name% and wanted to share a wishlist with you. <br /><br /><br />For more info click <a target=\"_blank\" href=\"%Wishlist.URLForCustomer%\">here</a> <br /><br /><br />%Wishlist.PersonalMessage%<br /><br />%Store.Name%</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "Customer.NewOrderNote",
                                           Subject = "%Store.Name%. New order note has been added",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />Hello %Customer.FullName%, <br />New order note has been added to your account:<br />\"%Order.NewNoteText%\".<br /><a target=\"_blank\" href=\"%Order.OrderURLForCustomer%\">%Order.OrderURLForCustomer%</a></p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "RecurringPaymentCancelled.StoreOwnerNotification",
                                           Subject = "%Store.Name%. Recurring payment cancelled",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />%Customer.FullName% (%Customer.Email%) has just cancelled a recurring payment ID=%RecurringPayment.ID%.</p>",
                                           IsActive = true,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "OrderPlaced.VendorNotification",
                                           Subject = "%Store.Name%. Order placed",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />%Customer.FullName% (%Customer.Email%) has just placed an order. <br /><br />Order Number: %Order.OrderNumber%<br />Date Ordered: %Order.CreatedOn%</p>",
                                           //this template is disabled by default
                                           IsActive = false,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                                   new MessageTemplate
                                       {
                                           Name = "OrderPaid.StoreOwnerNotification",
                                           Subject = "%Store.Name%. Order #%Order.OrderNumber% paid",
                                           Body = "<p><a href=\"%Store.URL%\">%Store.Name%</a> <br /><br />Order #%Order.OrderNumber% has been just paid<br />Date Ordered: %Order.CreatedOn%</p>",
                                           //this template is disabled by default
                                           IsActive = false,
                                           EmailAccountId = eaGeneral.Id,
                                       },
                               };
            messageTemplates.ForEach(mt => _messageTemplateRepository.Insert(mt));

        }

        protected virtual void InstallTopics()
        {
            var topics = new List<Topic>
                               {
                                   new Topic
                                       {
                                           SystemName = "AboutUs",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           Title = "About Us",
                                           Body = "<p>Put your &quot;About Us&quot; information here. You can edit this in the admin site.</p>"
                                       },
                                   new Topic
                                       {
                                           SystemName = "CheckoutAsGuestOrRegister",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           Title = "",
                                           Body = "<p><strong>Register and save time!</strong><br />Register with us for future convenience:</p><ul><li>Fast and easy check out</li><li>Easy access to your order history and status</li></ul>"
                                       },
                                   new Topic
                                       {
                                           SystemName = "ConditionsOfUse",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           Title = "Conditions of use",
                                           Body = "<p>Put your conditions of use information here. You can edit this in the admin site.</p>"
                                       },
                                   new Topic
                                       {
                                           SystemName = "ContactUs",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           Title = "",
                                           Body = "<p>Put your contact information here. You can edit this in the admin site.</p>"
                                       },
                                   new Topic
                                       {
                                           SystemName = "ForumWelcomeMessage",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           Title = "Forums",
                                           Body = "<p>Put your welcome message here. You can edit this in the admin site.</p>"
                                       },
                                   new Topic
                                       {
                                           SystemName = "HomePageText",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           Title = "Welcome to our store",
                                           Body = "<p>Online shopping is the process consumers go through to purchase products or services over the Internet. You can edit this in the admin site.</p><p>If you have questions, see the <a href=\"http://www.CamprayPortal.com/documentation.aspx\">Documentation</a>, or post in the <a href=\"http://www.CamprayPortal.com/boards/\">Forums</a> at <a href=\"http://www.CamprayPortal.com\">CamprayPortal.com</a></p>"
                                       },
                                   new Topic
                                       {
                                           SystemName = "LoginRegistrationInfo",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           Title = "About login / registration",
                                           Body = "<p>Put your login / registration information here. You can edit this in the admin site.</p>"
                                       },
                                   new Topic
                                       {
                                           SystemName = "PrivacyInfo",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           Title = "Privacy policy",
                                           Body = "<p>Put your privacy policy information here. You can edit this in the admin site.</p>"
                                       },
                                   new Topic
                                       {
                                           SystemName = "PageNotFound",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           Title = "",
                                           Body = "<p><strong>The page you requested was not found, and we have a fine guess why.</strong></p><ul><li>If you typed the URL directly, please make sure the spelling is correct.</li><li>The page no longer exists. In this case, we profusely apologize for the inconvenience and for any damage this may cause.</li></ul>"
                                       },
                                   new Topic
                                       {
                                           SystemName = "ShippingInfo",
                                           IncludeInSitemap = false,
                                           IsPasswordProtected = false,
                                           Title = "Shipping & Returns",
                                           Body = "<p>Put your shipping &amp; returns information here. You can edit this in the admin site.</p>"
                                       },
                               };
            topics.ForEach(t => _topicRepository.Insert(t));


            //search engine names
            foreach (var topic in topics)
            {
                _urlRecordRepository.Insert(new UrlRecord()
                {
                    EntityId = topic.Id,
                    EntityName = "Topic",
                    LanguageId = 0,
                    IsActive = true,
                    Slug = topic.ValidateSeName("", !String.IsNullOrEmpty(topic.Title) ? topic.Title : topic.SystemName, true)
                });
            }

        }

        protected virtual void InstallSettings()
        {
            var settingService = EngineContext.Current.Resolve<ISettingService>();
            settingService.SaveSetting(new PdfSettings()
                {
                    LogoPictureId = 0,
                    LetterPageSizeEnabled = false,
                    RenderOrderNotes = true,
                    FontFileName = "FreeSerif.ttf",
                    InvoiceFooterTextColumn1 = null,
                    InvoiceFooterTextColumn2 = null,
                });

            settingService.SaveSetting(new CommonSettings()
                {
                    UseSystemEmailForContactUsForm = true,
                    UseStoredProceduresIfSupported = true,
                    SitemapEnabled = true,
                    SitemapIncludeCategories = true,
                    SitemapIncludeManufacturers = true,
                    SitemapIncludeProducts = false,
                    SitemapIncludeTopics = true,
                    DisplayJavaScriptDisabledWarning = false,
                    UseFullTextSearch = false,
                    FullTextMode = FulltextSearchMode.ExactMatch,
                    Log404Errors = true,
                    BreadcrumbDelimiter = "/",
                    RenderXuaCompatible = false,
                    XuaCompatibleValue = "IE=edge"
                });

            settingService.SaveSetting(new SeoSettings()
                {
                    PageTitleSeparator = ". ",
                    PageTitleSeoAdjustment = PageTitleSeoAdjustment.PagenameAfterStorename,
                    DefaultTitle = "Your store",
                    DefaultMetaKeywords = "",
                    DefaultMetaDescription = "",
                    GenerateProductMetaDescription = true,
                    ConvertNonWesternChars = false,
                    AllowUnicodeCharsInUrls = true,
                    CanonicalUrlsEnabled = false,
                    WwwRequirement = WwwRequirement.NoMatter,
                    //we disable bundling out of the box because it requires a lot of server resources
                    EnableJsBundling = false,
                    EnableCssBundling = false,
                    TwitterMetaTags = true,
                    OpenGraphMetaTags = true,
                    ReservedUrlRecordSlugs = new List<string>()
                    {
                        "admin", 
                        "install", 
                        "recentlyviewedproducts", 
                        "newproducts",
                        "compareproducts", 
                        "clearcomparelist",
                        "setproductreviewhelpfulness",
                        "login", 
                        "register", 
                        "logout", 
                        "cart",
                        "wishlist", 
                        "emailwishlist", 
                        "checkout", 
                        "onepagecheckout", 
                        "contactus", 
                        "passwordrecovery", 
                        "subscribenewsletter",
                        "blog", 
                        "boards", 
                        "inboxupdate",
                        "sentupdate", 
                        "news", 
                        "sitemap", 
                        "search",
                        "config", 
                        "eucookielawaccept", 
                        "page-not-found"
                    },
                });

            settingService.SaveSetting(new AdminAreaSettings()
                {
                    DefaultGridPageSize = 15,
                    GridPageSizes = "10, 15, 20, 50, 100",
                    DisplayProductPictures = true,
                });

           

            settingService.SaveSetting(new LocalizationSettings()
                {
                    DefaultAdminLanguageId = _languageRepository.Table.Single(l => l.Name == "English").Id,
                    UseImagesForLanguageSelection = false,
                    SeoFriendlyUrlsForLanguagesEnabled = false,
                    AutomaticallyDetectLanguage = false,
                    LoadAllLocaleRecordsOnStartup = true,
                    LoadAllLocalizedPropertiesOnStartup = true,
                    LoadAllUrlRecordsOnStartup = false,
                    IgnoreRtlPropertyForAdminArea = false,
                });

            settingService.SaveSetting(new CustomerSettings()
                {
                    UsernamesEnabled = false,
                    CheckUsernameAvailabilityEnabled = false,
                    AllowUsersToChangeUsernames = false,
                    DefaultPasswordFormat = PasswordFormat.Hashed,
                    HashedPasswordFormat = "SHA1",
                    PasswordMinLength = 6,
                    UserRegistrationType = UserRegistrationType.Standard,
                    AllowCustomersToUploadAvatars = false,
                    AvatarMaximumSizeBytes = 20000,
                    DefaultAvatarEnabled = true,
                    ShowCustomersLocation = false,
                    ShowCustomersJoinDate = false,
                    AllowViewingProfiles = false,
                    NotifyNewCustomerRegistration = false,
                    HideDownloadableProductsTab = false,
                    HideBackInStockSubscriptionsTab = false,
                    DownloadableProductsValidateUser = false,
                    CustomerNameFormat = CustomerNameFormat.ShowFirstName,
                    GenderEnabled = true,
                    DateOfBirthEnabled = true,
                    CompanyEnabled = true,
                    StreetAddressEnabled = false,
                    StreetAddress2Enabled = false,
                    ZipPostalCodeEnabled = false,
                    CityEnabled = false,
                    CountryEnabled = false,
                    StateProvinceEnabled = false,
                    PhoneEnabled = false,
                    FaxEnabled = false,
                    AcceptPrivacyPolicyEnabled = false,
                    NewsletterEnabled = true,
                    NewsletterTickedByDefault = true,
                    HideNewsletterBlock = false,
                    OnlineCustomerMinutes = 20,
                    StoreLastVisitedPage = false,
                    SuffixDeletedCustomers = false,
                });

            settingService.SaveSetting(new AddressSettings()
                {
                    CompanyEnabled = true,
                    StreetAddressEnabled = true,
                    StreetAddressRequired = true,
                    StreetAddress2Enabled = true,
                    ZipPostalCodeEnabled = true,
                    ZipPostalCodeRequired = true,
                    CityEnabled = true,
                    CityRequired = true,
                    CountryEnabled = true,
                    StateProvinceEnabled = true,
                    PhoneEnabled = true,
                    PhoneRequired = true,
                    FaxEnabled = true,
                });

            settingService.SaveSetting(new MediaSettings()
                {
                    AvatarPictureSize = 85,
                    ProductThumbPictureSize = 125,
                    ProductDetailsPictureSize = 300,
                    ProductThumbPictureSizeOnProductDetailsPage = 70,
                    ProductThumbPerRowOnProductDetailsPage = 4,
                    AssociatedProductPictureSize = 125,
                    CategoryThumbPictureSize = 125,
                    ManufacturerThumbPictureSize = 125,
                    CartThumbPictureSize = 80,
                    MiniCartThumbPictureSize = 47,
                    AutoCompleteSearchThumbPictureSize = 20,
                    MaximumImageSize = 1280,
                    DefaultPictureZoomEnabled = false,
                    DefaultImageQuality = 80,
                    MultipleThumbDirectories = false
                });

            settingService.SaveSetting(new StoreInformationSettings()
                {
                    StoreClosed = false,
                    StoreClosedAllowForAdmins = false,
                    DefaultStoreTheme = "DefaultClean",
                    AllowCustomerToSelectTheme = true,
                    ResponsiveDesignSupported = true,
                    DisplayMiniProfilerInPublicStore = false,
                    DisplayEuCookieLawWarning = false,
                    FacebookLink = "http://www.facebook.com/CamprayPortal",
                    TwitterLink = "https://twitter.com/CamprayPortal",
                    YoutubeLink = "http://www.youtube.com/user/CamprayPortal",
                    GooglePlusLink = "https://plus.google.com/+CamprayPortal",
                });

            settingService.SaveSetting(new ExternalAuthenticationSettings()
                {
                    AutoRegisterEnabled = true,
                });

           

            settingService.SaveSetting(new CurrencySettings()
                {
                    DisplayCurrencyLabel = false,
                    PrimaryStoreCurrencyId = _currencyRepository.Table.Single(c => c.CurrencyCode == "USD").Id,
                    PrimaryExchangeRateCurrencyId = _currencyRepository.Table.Single(c => c.CurrencyCode == "USD").Id,
                    ActiveExchangeRateProviderSystemName = "CurrencyExchange.MoneyConverter",
                    AutoUpdateEnabled = false,
                    LastUpdateTime = 0
                });

            

            settingService.SaveSetting(new MessageTemplatesSettings()
                {
                    CaseInvariantReplacement = false,
                    Color1 = "#b9babe",
                    Color2 = "#ebecee",
                    Color3 = "#dde2e6",
                });
 

            settingService.SaveSetting(new SecuritySettings()
                {
                    ForceSslForAllPages = false,
                    EncryptionKey = CommonHelper.GenerateRandomDigitCode(16),
                    AdminAreaAllowedIpAddresses = null,
                });
             
            settingService.SaveSetting(new DateTimeSettings()
                {
                    DefaultStoreTimeZoneId = "",
                    AllowCustomersToSetTimeZone = false
                });
             
            settingService.SaveSetting(new NewsSettings()
                {
                    Enabled = true,
                    AllowNotRegisteredUsersToLeaveComments = true,
                    NotifyAboutNewNewsComments = false,
                    ShowNewsOnMainPage = false,
                    MainPageNewsCount = 3,
                    NewsArchivePageSize = 10,
                    ShowHeaderRssUrl = false,
                }); 
             

            var eaGeneral = _emailAccountRepository.Table.FirstOrDefault();
            if (eaGeneral == null)
                throw new Exception("Default email account cannot be loaded");
            settingService.SaveSetting(new EmailAccountSettings()
                {
                    DefaultEmailAccountId = eaGeneral.Id
                });

            settingService.SaveSetting(new WidgetSettings()
                {
                    ActiveWidgetSystemNames = new List<string>() { "Widgets.NivoSlider" },
                });
        }
     
        protected virtual void InstallNews()
        {
            var defaultLanguage = _languageRepository.Table.FirstOrDefault();
            var news = new List<NewsItem>
                                {
                                    new NewsItem
                                        {
                                             AllowComments = true,
                                             Language = defaultLanguage,
                                             Title = "CamprayPortal new release!",
                                             Short = "CamprayPortal includes everything you need to begin your e-commerce online store. We have thought of everything and it's all included!<br /><br />CamprayPortal is a fully customizable shopping cart. It's stable and highly usable. From downloads to documentation, www.CamprayPortal.com offers a comprehensive base of information, resources, and support to the CamprayPortal community.",
                                             Full = "<p>CamprayPortal includes everything you need to begin your e-commerce online store. We have thought of everything and it's all included!</p><p>For full feature list go to <a href=\"http://www.CamprayPortal.com\">CamprayPortal.com</a></p><p>Providing outstanding custom search engine optimization, web development services and e-commerce development solutions to our clients at a fair price in a professional manner.</p>",
                                             Published  = true,
                                             CreatedOnUtc = DateTime.UtcNow,
                                        },
                                    new NewsItem
                                        {
                                             AllowComments = true,
                                             Language = defaultLanguage,
                                             Title = "New online store is open!",
                                             Short = "The new CamprayPortal store is open now! We are very excited to offer our new range of products. We will be constantly adding to our range so please register on our site, this will enable you to keep up to date with any new products.",
                                             Full = "<p>Our online store is officially up and running. Stock up for the holiday season! We have a great selection of items. We will be constantly adding to our range so please register on our site, this will enable you to keep up to date with any new products.</p><p>All shipping is worldwide and will leave the same day an order is placed! Happy Shopping and spread the word!!</p>",
                                             Published  = true,
                                             CreatedOnUtc = DateTime.UtcNow.AddSeconds(1),
                                        },
                                };
            news.ForEach(n => _newsItemRepository.Insert(n));

            //search engine names
            foreach (var newsItem in news)
            {
                _urlRecordRepository.Insert(new UrlRecord()
                {
                    EntityId = newsItem.Id,
                    EntityName = "NewsItem",
                    LanguageId = newsItem.LanguageId,
                    IsActive = true,
                    Slug = newsItem.ValidateSeName("", newsItem.Title, true)
                });
            }
        }
 

        protected virtual void InstallActivityLogTypes()
        {
            var activityLogTypes = new List<ActivityLogType>()
                                      {
                                          //admin area activities
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "AddNewCategory",
                                                  Enabled = true,
                                                  Name = "Add a new category"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "AddNewCheckoutAttribute",
                                                  Enabled = true,
                                                  Name = "Add a new checkout attribute"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "AddNewCustomer",
                                                  Enabled = true,
                                                  Name = "Add a new customer"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "AddNewCustomerRole",
                                                  Enabled = true,
                                                  Name = "Add a new customer role"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "AddNewDiscount",
                                                  Enabled = true,
                                                  Name = "Add a new discount"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "AddNewGiftCard",
                                                  Enabled = true,
                                                  Name = "Add a new gift card"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "AddNewManufacturer",
                                                  Enabled = true,
                                                  Name = "Add a new manufacturer"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "AddNewProduct",
                                                  Enabled = true,
                                                  Name = "Add a new product"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "AddNewProductAttribute",
                                                  Enabled = true,
                                                  Name = "Add a new product attribute"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "AddNewSetting",
                                                  Enabled = true,
                                                  Name = "Add a new setting"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "AddNewSpecAttribute",
                                                  Enabled = true,
                                                  Name = "Add a new specification attribute"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "AddNewWidget",
                                                  Enabled = true,
                                                  Name = "Add a new widget"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "DeleteCategory",
                                                  Enabled = true,
                                                  Name = "Delete category"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "DeleteCheckoutAttribute",
                                                  Enabled = true,
                                                  Name = "Delete a checkout attribute"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "DeleteCustomer",
                                                  Enabled = true,
                                                  Name = "Delete a customer"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "DeleteCustomerRole",
                                                  Enabled = true,
                                                  Name = "Delete a customer role"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "DeleteDiscount",
                                                  Enabled = true,
                                                  Name = "Delete a discount"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "DeleteGiftCard",
                                                  Enabled = true,
                                                  Name = "Delete a gift card"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "DeleteManufacturer",
                                                  Enabled = true,
                                                  Name = "Delete a manufacturer"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "DeleteProduct",
                                                  Enabled = true,
                                                  Name = "Delete a product"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "DeleteProductAttribute",
                                                  Enabled = true,
                                                  Name = "Delete a product attribute"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "DeleteReturnRequest",
                                                  Enabled = true,
                                                  Name = "Delete a return request"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "DeleteSetting",
                                                  Enabled = true,
                                                  Name = "Delete a setting"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "DeleteSpecAttribute",
                                                  Enabled = true,
                                                  Name = "Delete a specification attribute"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "DeleteWidget",
                                                  Enabled = true,
                                                  Name = "Delete a widget"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "EditCategory",
                                                  Enabled = true,
                                                  Name = "Edit category"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "EditCheckoutAttribute",
                                                  Enabled = true,
                                                  Name = "Edit a checkout attribute"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "EditCustomer",
                                                  Enabled = true,
                                                  Name = "Edit a customer"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "EditCustomerRole",
                                                  Enabled = true,
                                                  Name = "Edit a customer role"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "EditDiscount",
                                                  Enabled = true,
                                                  Name = "Edit a discount"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "EditGiftCard",
                                                  Enabled = true,
                                                  Name = "Edit a gift card"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "EditManufacturer",
                                                  Enabled = true,
                                                  Name = "Edit a manufacturer"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "EditProduct",
                                                  Enabled = true,
                                                  Name = "Edit a product"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "EditProductAttribute",
                                                  Enabled = true,
                                                  Name = "Edit a product attribute"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "EditPromotionProviders",
                                                  Enabled = true,
                                                  Name = "Edit promotion providers"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "EditReturnRequest",
                                                  Enabled = true,
                                                  Name = "Edit a return request"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "EditSettings",
                                                  Enabled = true,
                                                  Name = "Edit setting(s)"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "EditSpecAttribute",
                                                  Enabled = true,
                                                  Name = "Edit a specification attribute"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "EditWidget",
                                                  Enabled = true,
                                                  Name = "Edit a widget"
                                              },
                                              //public store activities
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.ViewCategory",
                                                  Enabled = false,
                                                  Name = "Public store. View a category"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.ViewManufacturer",
                                                  Enabled = false,
                                                  Name = "Public store. View a manufacturer"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.ViewProduct",
                                                  Enabled = false,
                                                  Name = "Public store. View a product"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.PlaceOrder",
                                                  Enabled = false,
                                                  Name = "Public store. Place an order"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.SendPM",
                                                  Enabled = false,
                                                  Name = "Public store. Send PM"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.ContactUs",
                                                  Enabled = false,
                                                  Name = "Public store. Use contact us form"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.AddToCompareList",
                                                  Enabled = false,
                                                  Name = "Public store. Add to compare list"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.AddToShoppingCart",
                                                  Enabled = false,
                                                  Name = "Public store. Add to shopping cart"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.AddToWishlist",
                                                  Enabled = false,
                                                  Name = "Public store. Add to wishlist"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.Login",
                                                  Enabled = false,
                                                  Name = "Public store. Login"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.Logout",
                                                  Enabled = false,
                                                  Name = "Public store. Logout"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.AddProductReview",
                                                  Enabled = false,
                                                  Name = "Public store. Add product review"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.AddNewsComment",
                                                  Enabled = false,
                                                  Name = "Public store. Add news comment"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.AddBlogComment",
                                                  Enabled = false,
                                                  Name = "Public store. Add blog comment"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.AddForumTopic",
                                                  Enabled = false,
                                                  Name = "Public store. Add forum topic"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.EditForumTopic",
                                                  Enabled = false,
                                                  Name = "Public store. Edit forum topic"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.DeleteForumTopic",
                                                  Enabled = false,
                                                  Name = "Public store. Delete forum topic"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.AddForumPost",
                                                  Enabled = false,
                                                  Name = "Public store. Add forum post"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.EditForumPost",
                                                  Enabled = false,
                                                  Name = "Public store. Edit forum post"
                                              },
                                          new ActivityLogType
                                              {
                                                  SystemKeyword = "PublicStore.DeleteForumPost",
                                                  Enabled = false,
                                                  Name = "Public store. Delete forum post"
                                              },
                                      };
            activityLogTypes.ForEach(alt => _activityLogTypeRepository.Insert(alt));
        }
   
        protected virtual void InstallScheduleTasks()
        {
            var tasks = new List<ScheduleTask>()
            {
                new ScheduleTask()
                {
                    Name = "Send emails",
                    Seconds = 60,
                    Type = "CamprayPortal.Services.Messages.QueuedMessagesSendTask, CamprayPortal.Services",
                    Enabled = true,
                    StopOnError = false,
                },
                new ScheduleTask()
                {
                    Name = "Keep alive",
                    Seconds = 300,
                    Type = "CamprayPortal.Services.Common.KeepAliveTask, CamprayPortal.Services",
                    Enabled = true,
                    StopOnError = false,
                },
                new ScheduleTask()
                {
                    Name = "Delete guests",
                    Seconds = 600,
                    Type = "CamprayPortal.Services.Customers.DeleteGuestsTask, CamprayPortal.Services",
                    Enabled = true,
                    StopOnError = false,
                },
                new ScheduleTask()
                {
                    Name = "Clear cache",
                    Seconds = 600,
                    Type = "CamprayPortal.Services.Caching.ClearCacheTask, CamprayPortal.Services",
                    Enabled = false,
                    StopOnError = false,
                },
                new ScheduleTask()
                {
                    Name = "Clear log",
                    //60 minutes
                    Seconds = 3600,
                    Type = "CamprayPortal.Services.Logging.ClearLogTask, CamprayPortal.Services",
                    Enabled = false,
                    StopOnError = false,
                },
                new ScheduleTask()
                {
                    Name = "Update currency exchange rates",
                    Seconds = 900,
                    Type = "CamprayPortal.Services.Directory.UpdateExchangeRateTask, CamprayPortal.Services",
                    Enabled = true,
                    StopOnError = false,
                },
            };

            tasks.ForEach(x => _scheduleTaskRepository.Insert(x));
        }

  
        #endregion

        #region Methods

        public virtual void InstallData(string defaultUserEmail,
            string defaultUserPassword, bool installSampleData = true)
        {
            InstallStores();
          
            InstallLanguages();
            InstallCurrencies();
            InstallCountriesAndStates();
          
            InstallCustomersAndUsers(defaultUserEmail, defaultUserPassword);
            InstallEmailAccounts();
            InstallMessageTemplates();
            InstallSettings();
            InstallTopics();
            InstallLocaleResources();
            InstallActivityLogTypes();
            HashDefaultCustomerPassword(defaultUserEmail, defaultUserPassword);
            
            InstallScheduleTasks();

            if (installSampleData)
            {
               
                InstallNews();
                
            }
        }

        #endregion
    }
}