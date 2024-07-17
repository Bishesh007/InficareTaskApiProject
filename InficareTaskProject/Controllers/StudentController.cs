using InficareTaskProject.Data;
using InficareTaskProject.Entities;
using InficareTaskProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InficareTaskProject.Interfaces;
using System.Buffers.Text;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using InficareTaskProject.Core.Permission;

namespace InficareTaskProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StudentController : ControllerBase
    {
        private readonly UserManager<Student> _userManager;
        private readonly SignInManager<Student> _signInManager;
        private readonly IJwtTokenManager _jwtTokenManager;
        private readonly ApplicationDbContext _context;
        public StudentController(
                                    UserManager<Student> userManger,
                                    SignInManager<Student> signInManager,
                                    ApplicationDbContext context,
                                    IJwtTokenManager jwtTokenManager)
        {
            _userManager = userManger;
            _signInManager = signInManager;
            _context = context;
            _jwtTokenManager = jwtTokenManager;
        }

        [HttpPost("/api/students/createStudent")]
        [AllowAnonymous]
        public async Task<ResponseModel> CreateStudent(StudentsViewModel user)
        {
            var response = new ResponseModel();
            try
            {
                var student = new Student
                {
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email,
                    Class = user.Class,
                    RollNo = user.RollNo,
                    Section = user.Section,
                    Age = user.Age,
                    Address = user.Address

                };

                IdentityResult result = await _userManager.CreateAsync(student, user.Password);
                await _userManager.AddToRoleAsync(student, "User");


                if (!result.Succeeded)
                {
                    response.errorMessage = result.Errors.Select(x => x.Description).First();
                    response.isSuccess = false;
                }
                else
                {
                    response.errorMessage = "";
                    response.isSuccess = true;
                    response.message = "User created Successfully";
                }
                return response;

            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                response.isSuccess = false;
                return response;
            }

        }

        [HttpPost("/api/students/authenticateStudent")]
        [AllowAnonymous]
        public async Task<ResponseModel> Authenticate([FromBody] AuthenticateUser request)
        {
            var response = new ResponseModel();

            var identityUser = await _userManager.FindByNameAsync(request.UserName);

            if (identityUser == null)
            {
                response.errorMessage = "User not Found!!!";
                response.isSuccess = false;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(identityUser, request.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                response.errorMessage = "Wrong Password !!!";
                response.isSuccess = false;
                return response;

            }
            var token = _jwtTokenManager.GenerateToken(identityUser);
            response.errorMessage = "";
            response.message = "Login Successfully";
            response.isSuccess = true;
            response.token = token;

            return response;
        }

        [HttpGet]
        [Route("/api/students/getStudents")]
        [Authorize(PermissionTypes.GetStudent)]
        public async Task<ActionResult<List<StudentsViewModel>>> GetAllStudents()
        {
            var getCustomerQuery = await _context.Students.Where(x => x.UserName != "Admin")
                                                .Select(x => new StudentsViewModel
                                                {
                                                    Id = x.Id,
                                                    UserName = x.UserName,
                                                    Email = x.Email,
                                                    PhoneNumber = x.PhoneNumber,
                                                    Class = x.Class,
                                                    RollNo = x.RollNo,
                                                    Section = x.Section,
                                                    Age = x.Age,
                                                    Address = x.Address,

                                                }).ToListAsync();
            return Ok(getCustomerQuery);
        }

        [HttpPost("/api/students/addStudent")]
        [Authorize(PermissionTypes.CreateStudent)]
        public async Task<ResponseModel> AddStudent(StudentsViewModel user)
        {
            var response = new ResponseModel();
            var student = new Student
            {
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                Class = user.Class,
                RollNo = user.RollNo,
                Section = user.Section,
                Age = user.Age,
                Address = user.Address

            };

            IdentityResult result = await _userManager.CreateAsync(student);
            await _userManager.AddToRoleAsync(student, user.Role);


            if (!result.Succeeded)
            {
                response.errorMessage = result.Errors.Select(x => x.Description).First();
                response.isSuccess = false;
            }
            else
            {
                response.errorMessage = "";
                response.isSuccess = true;
                response.message = "User created Successfully";

            }

            return response;
        }

        [HttpPost("/api/students/updateStudent")]
        [Authorize(PermissionTypes.UpdateStudent)]
        public async Task<ResponseModel> UpdateStudent(StudentsViewModel updatedUser)
        {
            var response = new ResponseModel();

            var student = await _userManager.FindByIdAsync(updatedUser.Id);

            if (student == null)
            {
                response.errorMessage = "Student not found";
                response.isSuccess = false;
                return response;
            }

            student.UserName = updatedUser.UserName;
            student.PhoneNumber = updatedUser.PhoneNumber;
            student.Email = updatedUser.Email;
            student.Class = updatedUser.Class;
            student.RollNo = updatedUser.RollNo;
            student.Section = updatedUser.Section;
            student.Age = updatedUser.Age;
            student.Address = updatedUser.Address;

            IdentityResult result = await _userManager.UpdateAsync(student);

            if (!result.Succeeded)
            {
                response.errorMessage = result.Errors.Select(x => x.Description).First();
                response.isSuccess = false;
            }
            else
            {
                response.errorMessage = "";
                response.isSuccess = true;
                response.message = "User updated successfully";
            }

            return response;
        }

        [HttpDelete("/api/students/deleteStudent/{id}")]
        [Authorize(PermissionTypes.DeleteStudent)]
        public async Task<ResponseModel> DeleteStudent(string id)
        {
            var response = new ResponseModel();

            var student = await _userManager.FindByIdAsync(id);

            if (student == null)
            {
                response.errorMessage = "Student not found";
                response.isSuccess = false;
                return response;
            }

            IdentityResult result = await _userManager.DeleteAsync(student);

            if (!result.Succeeded)
            {
                response.errorMessage = result.Errors.Select(x => x.Description).First();
                response.isSuccess = false;
            }
            else
            {
                response.errorMessage = "";
                response.isSuccess = true;
                response.message = "User deleted successfully";
            }

            return response;
        }

        [HttpGet]
        [Route("/api/students/getDetailOnSearch")]
        public async Task<ActionResult> GetDetailsWithPara(string userName)
        {
            var details = await _context.Students
                                    .Where(x => x.UserName == userName)
                                    .Select(x => new StudentsViewModel
                                    {
                                        UserName = x.UserName,
                                        Email = x.Email,
                                        PhoneNumber = x.PhoneNumber,
                                        Password = x.PasswordHash,

                                    }).ToListAsync();
            return Ok(details);
        }

        [HttpGet]
        [Route("/api/students/getAllSubject")]
        public async Task<ActionResult> GetAllSubject()
        {
            var subjectList = await _context.Subjects.ToListAsync();

            return Ok(subjectList);
        }

        [HttpPost]
        [Route("/api/students/addStudentSubject")]
        [AllowAnonymous]
        public async Task<ResponseModel> AddStudentSubject(StudentSubjectViewModel studentSubject)
        {
            var response = new ResponseModel();
            try
            {
                foreach (var subjectList in studentSubject.SubjectList)
                {
                    // Check if the entry already exists in the database
                    var existingEntry = await _context.StudentSubjects
                        .FirstOrDefaultAsync(ss => ss.StudentId == studentSubject.StudentId && ss.SubjectId == subjectList.Id);

                    if (existingEntry == null)
                    {
                        // Entry doesn't exist, add it to the database
                        var subject = new StudentSubject
                        {
                            StudentId = studentSubject.StudentId,
                            SubjectId = subjectList.Id,
                        };
                        await _context.StudentSubjects.AddAsync(subject);
                        await _context.SaveChangesAsync();
                    }
                }
                response.errorMessage = "";
                response.isSuccess = true;
                response.message = "User Assign Subject Successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.errorMessage = ex.Message;
                response.isSuccess = false;
                return response;
            }
        }

        [HttpGet]
        [Route("/api/students/getSubjectAssociatedWithUser/{id}")]
        public async Task<ActionResult> GetSubjectAs(string id)
        {
            var subjectList = await (from s in _context.Subjects
                                     join studentSubject in _context.StudentSubjects.Where(x => x.StudentId == id) on s.Id equals studentSubject.SubjectId into temp
                                     from t in temp.DefaultIfEmpty()
                                     select new SubjectViewModel
                                     {
                                         Name = s.Name,
                                         Id = s.Id,
                                         IsSelected = t == null ? false : true
                                     }).ToListAsync();

            return Ok(subjectList);
        }
    }
}


