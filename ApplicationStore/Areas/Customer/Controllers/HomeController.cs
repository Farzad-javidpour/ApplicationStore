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
using ApplicationStore.Utility;
using Microsoft.AspNetCore.Routing;

namespace ApplicationStore.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public ApplicationPublishViewModel ApplicationPublishVM { get; set; }

        [BindProperty]
        public CommentViewModel CommentVM { get; set; }

        public HomeController(ApplicationDbContext context)
        {
            _context = context;

            ApplicationPublishVM = new ApplicationPublishViewModel()
            {
                ApplicationPublish = new ApplicationPublish(),
                Applications = _context.Applications.ToList(),
                Platforms = _context.Platforms.ToList()
            };

            CommentVM = new CommentViewModel()
            {
                Comment = new Comment(),
                ApplicationPublish = new ApplicationPublish(),
                ApplicationStoreUser = new ApplicationStoreUser()
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
            ApplicationPublishVM.Comments = _context.Comments
                .Where(c => c.ApplicationPublishId == ApplicationPublishVM.ApplicationPublish.Id)
                .Include(c=>c.ApplicationStoreUser);

            ApplicationPublishVM.RegisterDateShamsi = Persia.Calendar.ConvertToPersian(applicationPublish.RegisterDate).ToString();
            ApplicationPublishVM.PublishDateShamsi = Persia.Calendar.ConvertToPersian(applicationPublish.PublishDate).ToString();
            ApplicationPublishVM.PictureUrl = Tools.GetImageUrlFromByteArray(_context.ApplicationPictures.FirstOrDefault(p => p.ApplicationPublishId == applicationPublish.Id).Data);

            return View(ApplicationPublishVM);
        }

        //________________________________________________________________________________
        public async Task<IActionResult> AddComment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.UserId = Tools.GetCurrenyUserId(User);

            CommentVM.ApplicationPublish = await _context.ApplicationPublishs
               .Include(m => m.Application)
               .Include(m => m.ApplicationStoreUser)
               .Include(m => m.Platform)
               .SingleOrDefaultAsync(m => m.Id == id);

            CommentVM.ApplicationStoreUser = await _context.ApplicationStoreUsers
               .SingleOrDefaultAsync(m => m.Id == Tools.GetCurrenyUserId(User));

            if (CommentVM.ApplicationPublish == null)
            {
                return NotFound();
            }
            return View(CommentVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int id, [Bind] CommentViewModel commentViewModel)
        {
                try
                {
                    var comment = new Comment()
                    {
                        Text = commentViewModel.Comment.Text,
                        RegisterDate = DateTime.Now,
                        ApplicationPublishId = commentViewModel.ApplicationPublish.Id,
                        ApplicationStoreUserId = commentViewModel.ApplicationStoreUser.Id
                    };

                    _context.Comments.Add(comment);

                    await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Details), new RouteValueDictionary(
                                        new { controller = "Home", action = nameof(Details), Id = comment.ApplicationPublishId }));
                }
                catch (DbUpdateConcurrencyException)
                {
                    return NotFound();
                }
        }
        //________________________________________________________________________________

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
