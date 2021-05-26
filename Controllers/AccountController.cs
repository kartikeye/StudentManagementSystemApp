using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StudentManagement.BusinessLogic;
using StudentManagement.Data;
using StudentManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        

        UserAuthManagement _userAuth;
        StudentManagementLogic _studentManagement;
        
        
        
        public AccountController(IMapper mapper)
        {
            _userAuth = new UserAuthManagement();
            _studentManagement = new StudentManagementLogic(mapper);
        }


        public IActionResult Index()
        {
            
            UserModel model = new UserModel() { UserName = "Kartikeye", Password = "master" };
            {
                //get the session value 
                //tpContext.Session.GetString("SessionUser");
                return View("~/Views/Account/Index.cshtml",model);
            }
        }
        
        public IActionResult LogIn(UserModel model)
        {

            

            var validUser = _userAuth.IsValidUser(model.UserName,model.Password);

            

            if (validUser!=null)
            {
                _userAuth.setSession(validUser.UserId, HttpContext.Session);
                //HttpContext.Session.SetString("userId", model.UserId.ToString());
                return RedirectToAction("StudentList","Account");
            }
            else
            {
                
                ModelState.AddModelError("", "Invalid Username and password");
                return View("~/Views/Account/Index.cshtml");
            }
            
        }
        
        public IActionResult StudentList()
        {
            List<StudentModel> students = _studentManagement.GetStudents();
            return View(students);
        }

        

        [ActionName("new-student")]
        public IActionResult NewStudent()
        {
            string userName = _userAuth.getUserNameFromSession(HttpContext.Session);
            int userId = _userAuth.getSession(HttpContext.Session);


            StudentModel model = new StudentModel() { CreateByUser=userName,CreateBy=userId};
            //set the session
            //ttext.Session.SetString("SessionUser",model.CreateByUser="ss");

            
            return View("~/Views/Account/NewStudent.cshtml",model);
        }

        public IActionResult Edit(int studentId)
        {
            StudentModel model = _studentManagement.GetStudent(studentId);
            return View("~/Views/Account/NewStudent.cshtml",model);
        }

        

        public IActionResult DeleteAjax(int studentId)
        {
            _studentManagement.DeleteStudent(studentId);
            List<StudentModel> students = _studentManagement.GetStudents();
            return PartialView("~/Views/Account/_StudentList.cshtml",students);
        }

        [ActionName("save-student")]
        public IActionResult SaveStudent(StudentModel model)
        {
            

            int userId = _userAuth.getSession(HttpContext.Session);

            if (userId>0)
            {
                model.CreateBy = userId;
            }
            if (model.StudentId > 0)
            {
                try
                {
                    _studentManagement.EditStudent(model);
                }
                catch(Exception e)
                {
                    ViewBag.flag = "roll number are same";

                    
                    return View("~/Views/Account/NewStudent.cshtml",model);
                }
             }
            else
            {
                try
                {
                    _studentManagement.CreateStudent(model);
                }
                catch(Exception e)
                {
                    
                    ViewBag.flag = "roll number are same";
                    return View("~/Views/Account/NewStudent.cshtml", model);
                }
            }
            return RedirectToAction("StudentList");
        }
        
    }
}
