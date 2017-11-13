using LoginRoles.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using Microsoft.Owin.Security;

namespace LoginRoles.Controllers
{
    public class AccountController : Controller
    {
        public RoleBaseAccessibilityEntities _context = new RoleBaseAccessibilityEntities();
      
        public AccountController()
        {
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                if (this.Request.IsAuthenticated)
                {
                    return RedirectToLocal(returnUrl);
                }
            }
            catch (Exception ex)
            {

                Console.Write(ex);
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AccountViewModel model, string returnUrl)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var loginInfo = _context.LoginByUsernamePassword(model.Username, model.Password).ToList();

                    if (loginInfo != null && loginInfo.Count() > 0)
                    {
                        var loginDetails = loginInfo.First();

                        SignInUser(loginDetails.username, loginDetails.role_id, false);
                        //setting
                        Session["role_id"] = loginDetails.role_id;

                        return RedirectToLocal(returnUrl);
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Username Invalido o passaword invalido");
                    } 
                }
            }
            catch (Exception ex)
            {

                Console.Write(ex);
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            try
            {
                // Setting.
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;

                // Sign Out.
                authenticationManager.SignOut();
            }
            catch (Exception ex)
            {
                // Info
                throw ex;
            }

            // Info.
            return this.RedirectToAction("Login", "Account");
        }

        private void SignInUser(string username, int role_id, bool isPersistent)
        {
            var claims = new List<Claim>();

            try
            {
                //Settings
                claims.Add(new Claim(ClaimTypes.Name, username));
                claims.Add(new Claim(ClaimTypes.Role, role_id.ToString()));
                var claimIdentities = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                var ctx = Request.GetOwinContext();//owin.host.system
                var authenticationManager = ctx.Authentication;

                //Sign In
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent  }, claimIdentities);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            try
            {
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
            return RedirectToAction("Index", "Home");
        }
    }
}