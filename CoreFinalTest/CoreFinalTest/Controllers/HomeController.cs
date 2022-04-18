using CoreFinalTest.Db_Context;
using CoreFinalTest.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CoreFinalTest.Controllers
{

    
    public class HomeController : Controller
    {
        chetuContext obj = new chetuContext();
        private readonly ILogger<HomeController> _logger;

        
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Authorize]
        public IActionResult Index()
        {
            HttpContext.Session.SetString("name", "manoj kumar");
            HttpContext.Session.GetString("name");
            List<ChetuStudentModel> sobj = new List<ChetuStudentModel>();
            var res = obj.Students.ToList();
            foreach (var item in res)
            {
                sobj.Add(new ChetuStudentModel
                {
                    Id = item.Id,
                    Name=item.Name,
                    Email=item.Email,
                    Mobile=item.Mobile,
                    Password=item.Password
                }) ;
            }
            return View(sobj);
        }

        [Authorize]
        public IActionResult Delete(int id)
        {
            var del = obj.Students.Where(a => a.Id == id).First();
            obj.Students.Remove(del);
            obj.SaveChanges();
            return RedirectToAction("Index","Home");
        }

        [HttpGet]
        public IActionResult AddStudent()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddStudent(ChetuStudentModel cobj)
        {
            Student tobj = new Student();
            tobj.Id = cobj.Id;
            tobj.Name = cobj.Name;
            tobj.Email = cobj.Email;
            tobj.Mobile = cobj.Mobile;
            tobj.Password = cobj.Password;
            if (cobj.Id == 0)
            {
                obj.Students.Add(tobj);
                obj.SaveChanges();
            }
            else
            {
                obj.Entry(tobj).State = EntityState.Modified;
                obj.SaveChanges();
            }
            return RedirectToAction("Index","Home");
        }

        [Authorize]
        public IActionResult Edit(int id)
        {
            ChetuStudentModel csobj = new ChetuStudentModel();
            var edititem = obj.Students.Where(a => a.Id == id).First();
            csobj.Id = edititem.Id;
            csobj.Name = edititem.Name;
            csobj.Email = edititem.Email;
            csobj.Mobile = edititem.Mobile;
            csobj.Password = edititem.Password;
            ViewBag.Id= edititem.Id;

            return View("AddStudent", csobj);
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Login(ChetuStudentModel objUser)
        {

            chetuContext DbNew = new chetuContext();

            var res = obj.Students.Where(a => a.Email == objUser.Email).FirstOrDefault();

            //var res = DbNew.TblUserInfos.FromSqlRaw<TblUserInfo>("UserSelect").t

            if (res == null)
            {

                TempData["Invalid"] = "Email is not found";
            }

            else
            {
                if (res.Email == objUser.Email && res.Password == objUser.Password)
                {

                    var claims = new[] { new Claim(ClaimTypes.Name, res.Name),
                                        new Claim(ClaimTypes.Email, res.Email) };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };
                    HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity),
                    authProperties);


                    HttpContext.Session.SetString("Name", res.Email);

                    return RedirectToAction("Index", "Home");

                }

                else
                {

                    ViewBag.Inv = "Wrong password";

                    return View("Login");
                }


            }


            return View("Login");
        }
        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return View("Login");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
