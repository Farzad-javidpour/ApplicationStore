using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ApplicationStore.Data;
using ApplicationStore.Models;
using ApplicationStore.Models.ViewModels;
using System.IO;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using ApplicationStore.Utility;
using System.Net.Mail;
using System.Net;

namespace ApplicationStore.Areas.Admin.Controllers
{
    [Authorize(Roles = "Super Admin")]
    [Area("Admin")]
    public class ApplicationPublishesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HostingEnvironment _hostingEnvironment;

        [BindProperty]
        public ApplicationPublishViewModel ApplicationPublishVM { get; set; }

        public ApplicationPublishesController(ApplicationDbContext context, HostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;

            ApplicationPublishVM = new ApplicationPublishViewModel()
            {
                ApplicationPublish = new ApplicationPublish(),
                Applications = _context.Applications.ToList(),
                Platforms = _context.Platforms.ToList()
            };
        }
        //________________________________________________________________________________
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ApplicationPublishs
                .Include(a => a.Application)
                .Include(a => a.Platform)
                .Include(a=>a.ApplicationStoreUser);

            var applicationPublishVMList = new List<ApplicationPublishViewModel>();
            foreach (var item in await applicationDbContext.ToListAsync())
            {
                applicationPublishVMList.Add(
                    new ApplicationPublishViewModel()
                    {
                        ApplicationPublish = item,
                        RegisterDateShamsi = Persia.Calendar.ConvertToPersian(item.RegisterDate).ToString(),
                        PublishDateShamsi = Persia.Calendar.ConvertToPersian(item.PublishDate).ToString(),
                    });
            }

            return View(applicationPublishVMList);
        }
        //________________________________________________________________________________
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationPublish = await _context.ApplicationPublishs
                .Include(a => a.Application)
                .Include(a => a.Platform)
                .Include(a => a.ApplicationStoreUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationPublish == null)
            {
                return NotFound();
            }

            ApplicationPublishVM.ApplicationPublish = applicationPublish;
            ApplicationPublishVM.RegisterDateShamsi = Persia.Calendar.ConvertToPersian(applicationPublish.RegisterDate).ToString();
            ApplicationPublishVM.PublishDateShamsi = Persia.Calendar.ConvertToPersian(applicationPublish.PublishDate).ToString();

            return View(ApplicationPublishVM);
        }
        //________________________________________________________________________________
        public IActionResult Create()
        {
            ViewBag.UserId = Tools.GetCurrentUserId(User);
            ApplicationPublishVM.ApplicationPublish.PublishDate = DateTime.Now.Date;

            return View(ApplicationPublishVM);
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost()
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;

                var objApplicationPublish = new ApplicationPublish();
                objApplicationPublish.Version = ApplicationPublishVM.ApplicationPublish.Version.Trim();
                objApplicationPublish.ChangeList = ApplicationPublishVM.ApplicationPublish.ChangeList.Trim();
                objApplicationPublish.PublishDate = ApplicationPublishVM.ApplicationPublish.PublishDate;
                objApplicationPublish.RegisterDate = DateTime.Now.Date;
                objApplicationPublish.Price = ApplicationPublishVM.ApplicationPublish.Price;
                objApplicationPublish.Size = files[0].Length / 1024 / 1024;
                objApplicationPublish.Extension = Path.GetExtension(files[0].FileName);
                objApplicationPublish.PlatformId = ApplicationPublishVM.ApplicationPublish.PlatformId;
                objApplicationPublish.ApplicationId = ApplicationPublishVM.ApplicationPublish.ApplicationId;
                objApplicationPublish.ApplicationStoreUserId = ApplicationPublishVM.ApplicationPublish.ApplicationStoreUserId;

                _context.Add(objApplicationPublish);

                // Register Pictures----------------------------
                foreach (var file in files)
                {
                    if (file.Length <= 0) continue;

                    if (file?.Length > 1 * 1024 * 1024)
                    {
                        //this.ModelState.AddModelError("", Error_Warninig_SusMessage.Message.LongPicture);
                    }
                    var imageExtension = Path.GetExtension(file?.FileName);

                    if (imageExtension.ToLower().Equals(".jpg") ||
                        imageExtension.ToLower().Equals(".jpeg"))
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            var fileBytes = ms.ToArray();
                            var picture = new ApplicationPicture();
                            picture.ApplicationPublish = objApplicationPublish;
                            picture.Title = file.FileName;
                            picture.Size = file.Length;
                            picture.Data = fileBytes;
                            picture.RegisterDate = DateTime.Now.Date;
                            picture.IsDefaultIcon = false;
                            _context.Add(picture);
                        }
                    }
                }
                //----------------------------------------------

                await _context.SaveChangesAsync();

                // Upload The Application File----------------------------
                var webRootPath = _hostingEnvironment.WebRootPath;
                var uploadApp = Path.Combine(webRootPath, "ApplicationFiles");
                var extension = Path.GetExtension(files[0].FileName);
                var filePath = Path.Combine(uploadApp, objApplicationPublish.Id + extension);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    files[0].CopyTo(fileStream);
                }
                //--------------------------------------------------------

                //Send Email To User Download This Application

                var downlodedApplication = _context.DownloadApplications
             .Include(d => d.ApplicationPublish)
             .ThenInclude(a => a.Application)
             .Where(d => d.ApplicationPublish.Application.Id == ApplicationPublishVM.ApplicationPublish.ApplicationId);
                foreach (var item in downlodedApplication)
                {
                    var EmailAddress = Tools.GetCurrentUserEmail(User);
                    var smtpClient = new SmtpClient
                    {
                        Host = "smtp.gmail.com", // set your SMTP server name here
                        Port = 587, // Port 
                        EnableSsl = true,
                        Credentials = new NetworkCredential("yelpapplicationserver@gmail.com", "96242369iust")
                    };

                    using (var message = new MailMessage("yelpapplicationserver@gmail.com", EmailAddress)
                    {
                        Subject = "نسخه جدید نرم افزار",
                        Body = string.Format("Hello dear user ,add new version of {0} to application list", item.ApplicationPublish.Application.Title)
                    })
                    {
                        await smtpClient.SendMailAsync(message);
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ApplicationPublishVM.Applications = _context.Applications.ToList();
            ApplicationPublishVM.Platforms = _context.Platforms.ToList();

         
            return View(ApplicationPublishVM);
        }
        //________________________________________________________________________________
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.UserId = Tools.GetCurrentUserId(User);
            ApplicationPublishVM.ApplicationPublish = await _context.ApplicationPublishs.Include(m => m.Application)
                                                                                        .Include(m => m.Platform)
                                                                                        .Include(m => m.ApplicationStoreUser)
                                                                                        .SingleOrDefaultAsync(m => m.Id == id);

            if (ApplicationPublishVM.ApplicationPublish == null)
            {
                return NotFound();
            }

            return View(ApplicationPublishVM);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id)
        {
            if (id != ApplicationPublishVM.ApplicationPublish.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ApplicationPublishVM.ApplicationPublish);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationPublishExists(ApplicationPublishVM.ApplicationPublish.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            ApplicationPublishVM.Applications = _context.Applications.ToList();
            ApplicationPublishVM.Platforms = _context.Platforms.ToList();
            return View(ApplicationPublishVM);
        }
        //________________________________________________________________________________
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationPublish = await _context.ApplicationPublishs
                .Include(a => a.Application)
                .Include(a => a.Platform)
                .Include(a => a.ApplicationStoreUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationPublish == null)
            {
                return NotFound();
            }

            ApplicationPublishVM.ApplicationPublish = applicationPublish;
            ApplicationPublishVM.RegisterDateShamsi = Persia.Calendar.ConvertToPersian(applicationPublish.RegisterDate).ToString();
            ApplicationPublishVM.PublishDateShamsi = Persia.Calendar.ConvertToPersian(applicationPublish.PublishDate).ToString();

            return View(ApplicationPublishVM);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var applicationPublish = await _context.ApplicationPublishs.FindAsync(id);
            _context.ApplicationPublishs.Remove(applicationPublish);
            await _context.SaveChangesAsync();

            // Delete The Application----------------------------
            var webRootPath = _hostingEnvironment.WebRootPath;
            var uploadApp = Path.Combine(webRootPath, "ApplicationFiles");
            var filePath = Path.Combine(uploadApp, applicationPublish.Id + applicationPublish.Extension);
            System.IO.File.Delete(filePath);
            //--------------------------------------------------------

            return RedirectToAction(nameof(Index));
        }
        //________________________________________________________________________________
        private bool ApplicationPublishExists(int id)
        {
            return _context.ApplicationPublishs.Any(e => e.Id == id);
        }
    }
}
