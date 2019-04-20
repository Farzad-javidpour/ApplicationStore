using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ApplicationStore.Models.ViewModels;
using ApplicationStore.Data;
using Microsoft.EntityFrameworkCore;
using ApplicationStore.Models;

namespace ApplicationStore.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public ApplicationPublishViewModel ApplicationPublishVM { get; set; }

        public HomeController(ApplicationDbContext context)
        {
            _context = context;

            ApplicationPublishVM = new ApplicationPublishViewModel()
            {
                ApplicationPublish = new ApplicationPublish(),
                Applications = _context.Applications.ToList(),
                Platforms = _context.Platforms.ToList()
            };
        }

        public async Task<IActionResult> Index()
        {
            var applicationPublishVMList = new List<ApplicationPublishViewModel>();
            foreach (var item in await _context.ApplicationPublishs.Include(m => m.Application).Include(m => m.Platform).ToListAsync())
            {
                applicationPublishVMList.Add(
                    new ApplicationPublishViewModel()
                    {
                        ApplicationPublish = item,
                        RegisterDateShamsi = Persia.Calendar.ConvertToPersian(item.RegisterDate).ToString(),
                        PublishDateShamsi = Persia.Calendar.ConvertToPersian(item.PublishDate).ToString(),
                        PictureUrl = Utility.Tools.GetImageUrlFromByteArray(_context.ApplicationPictures.FirstOrDefault(p => p.ApplicationPublishId == item.Id).Data)
                    });
            }

            return View(applicationPublishVMList);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationPublish = await _context.ApplicationPublishs
                .Include(a => a.Application)
                .Include(a => a.Platform)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationPublish == null)
            {
                return NotFound();
            }

            ApplicationPublishVM.ApplicationPublish = applicationPublish;
            ApplicationPublishVM.RegisterDateShamsi = Persia.Calendar.ConvertToPersian(applicationPublish.RegisterDate).ToString();
            ApplicationPublishVM.PublishDateShamsi = Persia.Calendar.ConvertToPersian(applicationPublish.PublishDate).ToString();
            ApplicationPublishVM.PictureUrl = Utility.Tools.GetImageUrlFromByteArray(_context.ApplicationPictures.FirstOrDefault(p => p.ApplicationPublishId == applicationPublish.Id).Data);

            return View(ApplicationPublishVM);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
