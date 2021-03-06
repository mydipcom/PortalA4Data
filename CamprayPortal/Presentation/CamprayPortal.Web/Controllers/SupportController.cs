﻿using System;
using System.Web.Mvc;
using CamprayPortal.Core;
using CamprayPortal.Core.Domain.Customers;
using CamprayPortal.Core.Domain.Localization;
using CamprayPortal.Services.Authentication;
using CamprayPortal.Services.Authentication.External;
using CamprayPortal.Services.Customers;
using CamprayPortal.Services.Localization;
using CamprayPortal.Services.Messages;
using CamprayPortal.Web.Models.Support;
using CamprayPortal.Services.Common;

namespace CamprayPortal.Web.Controllers
{
    public class SupportController : BasePublicController
    {
        #region Fields

        private readonly IAuthenticationService _authenticationService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly IWorkContext _workContext;
        private readonly CustomerSettings _customerSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkflowMessageService _workflowMessageService;
        #endregion

        #region Ctor

        public SupportController(IAuthenticationService authenticationService,
            ILocalizationService localizationService,
            ICustomerService customerService,
            ICustomerRegistrationService customerRegistrationService, IWorkContext workContext, CustomerSettings customerSettings,
             IGenericAttributeService genericAttributeService, IWorkflowMessageService workflowMessageService)
        {
            this._authenticationService = authenticationService;
            this._customerService = customerService;
            this._customerRegistrationService = customerRegistrationService;
            _workContext = workContext;
            _customerSettings = customerSettings;
            this._localizationService = localizationService;
            this._genericAttributeService = genericAttributeService;
            _workflowMessageService = workflowMessageService;
        }

        #endregion

        // GET: Support
        public ActionResult Benefits()
        {
            return TopicModelView();
        }

        public ActionResult CustomerPortal(string returnUrl = null)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }


        [HttpPost]
        public ActionResult CustomerPortal(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var loginResult = _customerRegistrationService.ValidateCustomer(model.Username, model.Password);
                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                    {
                        var customer = _customerService.GetCustomerByUsername(model.Username);

                        //sign in new customer
                        _authenticationService.SignIn(customer, model.RememberMe);

                        if (customer.IsAdmin())
                            return Redirect("/admin");
                        
                        if (!String.IsNullOrEmpty(returnUrl) && returnUrl.IndexOf("resultId")!=-1)
                            return Redirect("HomePage");

                         if (!String.IsNullOrEmpty(returnUrl))
                            return Redirect(returnUrl);

                        return RedirectToRoute("HomePage");
                    }
                    case CustomerLoginResults.CustomerNotExist:
                        ModelState.AddModelError("",
                            _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist"));
                        break;
                    case CustomerLoginResults.Deleted:
                        ModelState.AddModelError("",
                            _localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
                        break;
                    case CustomerLoginResults.NotActive:
                        ModelState.AddModelError("",
                            _localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
                        break;
                    case CustomerLoginResults.NotRegistered:
                        ModelState.AddModelError("",
                            _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
                        break;
                    case CustomerLoginResults.WrongPassword:
                    default:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
                        break;
                }
            }
            return View(model);
        }



        public ActionResult Logout(string returnUrl)
        {
            //external authentication
            ExternalAuthorizerHelper.RemoveParameters();
            _authenticationService.SignOut();
            if (!String.IsNullOrEmpty(returnUrl))
                return Redirect(returnUrl);
            return RedirectToRoute("HomePage");
        }
 
        public ActionResult Register()
        {
            var model = new RegisterModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model, string returnUrl)
        {
            if (_workContext.CurrentCustomer.IsRegistered())
            {
                //Already registered customer. 
                _authenticationService.SignOut();

                //Save a new record
                _workContext.CurrentCustomer = _customerService.InsertGuestCustomer();
            }
            var customer = _workContext.CurrentCustomer;

            if (ModelState.IsValid)
            {
                if (model.Email.IndexOf("@hotmail.com") != -1 || model.Email.IndexOf("gmail.com") != -1 ||
                    model.Email.IndexOf("@126.com") != -1)
                {
                    ModelState.AddModelError("",
                        _localizationService.GetResource("Account.Register.Result.PleaseUseCompanyEmail"));
                    return View(model);
                }

                var userregister =
                    (UserRegistrationType)
                        (int.Parse(System.Configuration.ConfigurationManager.AppSettings["UserRegistrationType"]));
                var isApproved = userregister == UserRegistrationType.Standard;
                var registrationRequest = new CustomerRegistrationRequest(customer, model.Email,
                    model.Email, model.Password, _customerSettings.DefaultPasswordFormat, isApproved);
                var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
                if (registrationResult.Success)
                {

                    if (!string.IsNullOrEmpty(model.FirstName))
                    {
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName,
                            model.FirstName);
                    }
                    if (!string.IsNullOrEmpty(model.FirstName))
                    {
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName,
                            model.LastName);
                    }
                    if (!string.IsNullOrEmpty(model.FirstName))
                    {
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.Company,
                            model.Company);
                    }
                    if (!string.IsNullOrEmpty(model.FirstName))
                    {
                        _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.StreetAddress,
                            model.StreetAddress);
                    }

                    //login customer now
                    //if (isApproved)
                    //    _authenticationService.SignIn(customer, true);
                    // return Redirect("/");
                    //return RedirectToRoute("RegisterResult");
                    if (userregister == UserRegistrationType.EmailValidation)
                    {
                        _genericAttributeService.SaveAttribute(customer,
                            SystemCustomerAttributeNames.AccountActivationToken, Guid.NewGuid().ToString());
                        _workflowMessageService.SendCustomerEmailValidationMessage(customer,
                            _workContext.WorkingLanguage.Id);
                        return RedirectToRoute("RegisterResult",
                            new {resultId = (int) UserRegistrationType.EmailValidation});
                    }

                    if (userregister == UserRegistrationType.AdminApproval)
                    {
                        _workflowMessageService.SendCustomerEmailValidationMessage(customer,
                            _workContext.WorkingLanguage.Id);
                        return RedirectToRoute("RegisterResult",
                            new {resultId = (int) UserRegistrationType.AdminApproval});
                    }

                    return RedirectToRoute("RegisterResult", new {resultId = (int) UserRegistrationType.Standard});
                }
                else
                {
                    foreach (var error in registrationResult.Errors)
                        ModelState.AddModelError("", error);
                }
            }
            return View(model);
        }



        public ActionResult AccountActivation(string token, string email)
        {
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                return RedirectToRoute("HomePage");

            var cToken = customer.GetAttribute<string>(SystemCustomerAttributeNames.AccountActivationToken);
            if (String.IsNullOrEmpty(cToken))
                return RedirectToRoute("HomePage");

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("HomePage");

            //activate user account
            customer.Active = true;
            _customerService.UpdateCustomer(customer);
            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.AccountActivationToken, "");
            //send welcome message
            _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);

            return RedirectToAction("RegisterResult", new {resultId = 999});
        }


        //public ActionResult RegisterResult()
        //{
        //    var model = new RegisterResultModel();
        //    return View(model);
        //}

        public ActionResult RegisterResult(int resultId)
        {
            var resultText = "";
            switch ((UserRegistrationType) resultId)
            {
                case UserRegistrationType.Disabled:
                    resultText = _localizationService.GetResource("Account.Register.Result.Disabled");
                    break;
                case UserRegistrationType.Standard:
                    resultText = _localizationService.GetResource("Account.Register.Result.Standard");
                    break;
                case UserRegistrationType.AdminApproval:
                    resultText = _localizationService.GetResource("Account.Register.Result.AdminApproval");
                    break;
                case UserRegistrationType.EmailValidation:
                    resultText = _localizationService.GetResource("Account.Register.Result.EmailValidation");
                    break;
                default:
                    break;
            }
            if (resultId == 999)
                resultText = _localizationService.GetResource("Account.AccountActivation.Activated");

            var model = new RegisterResultModel()
            {
                Result = resultText
            };
            return View(model);
        }





        public ActionResult Documentation()
        {
            return TopicModelView();
        }

        public ActionResult Homepage()
        {
            return TopicModelView();
        }

        public ActionResult SoftwareVersion()
        {
            return TopicModelView();
        }
    }
}