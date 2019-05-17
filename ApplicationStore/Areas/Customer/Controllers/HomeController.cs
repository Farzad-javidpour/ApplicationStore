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
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Hosting.Internal;

namespace ApplicationStore.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly HostingEnvironment _hostingEnvironment;

        [BindProperty]
        public ApplicationPublishViewModel ApplicationPublishVM { get; set; }

        [BindProperty]
        public CommentViewModel CommentVM { get; set; }
        //________________________________________________________________________________
        public HomeController(ApplicationDbContext context, HostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;

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
        //________________________________________________________________________________
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
                        PictureUrl = Utility.Tools.GetImageUrlFromByteArray(_context.ApplicationPictures.FirstOrDefault(p => p.ApplicationPublishId == item.Id).Data),
                        ShowIcon = !string.IsNullOrEmpty(this.User.Identity.Name),
                        IsFavorite = !string.IsNullOrEmpty(this.User.Identity.Name) ?
                                    await _context.FavorieApplications
                                     .AnyAsync(f => f.ApplicationPublishId == item.Id && f.ApplicationStoreUserId == Tools.GetCurrentUserId(User))
                                     : false,


                    });
            }
            this.ViewBag.ShowReturn = false;
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
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationPublish == null)
            {
                return NotFound();
            }

            ApplicationPublishVM.ApplicationPublish = applicationPublish;

            ApplicationPublishVM.Comments = _context.Comments
                .Where(c => c.ApplicationPublishId == ApplicationPublishVM.ApplicationPublish.Id && c.CommentState == CommentStateEnum.Confirmed)
                .Include(c => c.ApplicationStoreUser);

            ApplicationPublishVM.CommentLikes = _context.CommentLikes.ToList();

            ApplicationPublishVM.RegisterDateShamsi = Persia.Calendar.ConvertToPersian(applicationPublish.RegisterDate).ToString();
            ApplicationPublishVM.PublishDateShamsi = Persia.Calendar.ConvertToPersian(applicationPublish.PublishDate).ToString();
            ApplicationPublishVM.PictureUrl = Tools.GetImageUrlFromByteArray(_context.ApplicationPictures.FirstOrDefault(p => p.ApplicationPublishId == applicationPublish.Id).Data);
            this.ViewBag.HasUserId = !string.IsNullOrEmpty(this.User.Identity.Name);
            return View(ApplicationPublishVM);
        }

        //________________________________________________________________________________
        public async Task<IActionResult> AddComment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewBag.UserId = Tools.GetCurrentUserId(User);

            CommentVM.ApplicationPublish = await _context.ApplicationPublishs
               .Include(m => m.Application)
               .Include(m => m.ApplicationStoreUser)
               .Include(m => m.Platform)
               .SingleOrDefaultAsync(m => m.Id == id);

            CommentVM.ApplicationStoreUser = await _context.ApplicationStoreUsers
               .SingleOrDefaultAsync(m => m.Id == Tools.GetCurrentUserId(User));

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
                    CommentState = CommentStateEnum.Hidden,
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
        public async Task<IActionResult> LikeComment(int? id)
        {
            var currentUserId = Tools.GetCurrentUserId(User);
            ViewBag.UserId = currentUserId;

            var comment = _context.Comments.FirstOrDefault(c => c.Id == id);
            var commentLike = _context.CommentLikes.FirstOrDefault(cl => cl.IsLike && cl.ApplicationStoreUserId == currentUserId && cl.CommentId == id);
            var commentDislike = _context.CommentLikes.FirstOrDefault(cl => !cl.IsLike && cl.ApplicationStoreUserId == currentUserId && cl.CommentId == id);

            if (commentLike == null)
            {
                if (commentDislike != null)
                {
                    commentDislike.IsLike = true;
                }
                else
                {
                    var newCommentLike = new CommentLike()
                    {
                        IsLike = true,
                        ApplicationStoreUserId = currentUserId,
                        CommentId = (int)id
                    };
                    _context.CommentLikes.Add(newCommentLike);
                }

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new RouteValueDictionary(
                                        new { controller = "Home", action = nameof(Details), Id = comment.ApplicationPublishId }));
        }
        //________________________________________________________________________________
        public async Task<IActionResult> DisLikeComment(int? id)
        {
            var currentUserId = Tools.GetCurrentUserId(User);
            ViewBag.UserId = currentUserId;

            var comment = _context.Comments.FirstOrDefault(c => c.Id == id);
            var commentDislike = _context.CommentLikes.FirstOrDefault(cl => !cl.IsLike && cl.ApplicationStoreUserId == currentUserId && cl.CommentId == id);
            var commentLike = _context.CommentLikes.FirstOrDefault(cl => cl.IsLike && cl.ApplicationStoreUserId == currentUserId && cl.CommentId == id);

            if (commentDislike == null)
            {
                if (commentLike != null)
                {
                    commentLike.IsLike = false;
                }
                else
                {
                    var newCommentLike = new CommentLike()
                    {
                        IsLike = false,
                        ApplicationStoreUserId = currentUserId,
                        CommentId = (int)id
                    };
                    _context.CommentLikes.Add(newCommentLike);
                }

                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Details), new RouteValueDictionary(
                                        new { controller = "Home", action = nameof(Details), Id = comment.ApplicationPublishId }));
        }
        //________________________________________________________________________________

        public async Task<IActionResult> Favorite(int id)
        {
            var favorite = new FavorieApplication();
            favorite.ApplicationPublishId = id;
            favorite.ApplicationStoreUserId = Tools.GetCurrentUserId(User);
            favorite.RegisterDate = DateTime.Now.Date;
            _context.FavorieApplications.Add(favorite);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        //________________________________________________________________________________

        public async Task<IActionResult> RemoveFavorite(int id)
        {
            var favorite = await _context.FavorieApplications
                .FirstOrDefaultAsync(c => c.ApplicationPublishId == id && c.ApplicationStoreUserId == Tools.GetCurrentUserId(User));

            _context.FavorieApplications.Remove(favorite);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        //________________________________________________________________________________
        public async Task<IActionResult> FavoriteList()
        {
            var favoriteList = _context.FavorieApplications
                .Where(c => c.ApplicationStoreUserId == Tools.GetCurrentUserId(User))
                .Select(c => c.ApplicationPublishId);
            var applicationPublishVMList = new List<ApplicationPublishViewModel>();
            foreach (var item in await _context.ApplicationPublishs.Where(p => favoriteList.Contains(p.Id)).Include(m => m.Application).Include(m => m.Platform).ToListAsync())
            {
                applicationPublishVMList.Add(
                    new ApplicationPublishViewModel()
                    {
                        ApplicationPublish = item,
                        RegisterDateShamsi = Persia.Calendar.ConvertToPersian(item.RegisterDate).ToString(),
                        PublishDateShamsi = Persia.Calendar.ConvertToPersian(item.PublishDate).ToString(),
                        PictureUrl = Utility.Tools.GetImageUrlFromByteArray(_context.ApplicationPictures.FirstOrDefault(p => p.ApplicationPublishId == item.Id).Data),
                        ShowIcon = true,
                        IsFavorite = true,


                    });
            }
            this.ViewBag.ShowReturn = true;
            return this.View(@"../Home/Index", applicationPublishVMList);
        }

        //________________________________________________________________________________

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }
        //________________________________________________________________________________
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }
        //________________________________________________________________________________
        public IActionResult Privacy()
        {
            return View();
        }
        //________________________________________________________________________________
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //_______________________________________________________________________________

        [HttpPost]
        public async Task<IActionResult> Search(string searchText)
        {
            if (string.IsNullOrEmpty(searchText) || searchText == null) return RedirectToAction(nameof(Index));

            var searchList = _context.ApplicationPublishs
                .Include(a => a.Application)
                .ThenInclude(a => a.ApplicationCategory)
                .Where(
                c => c.ChangeList.Contains(searchText) ||
                c.Version.Contains(searchText) ||
                c.Application.Code.Contains(searchText) ||
                c.Application.Title.Contains(searchText) ||
                c.Application.ApplicationCategory.Title.Contains(searchText) ||
                c.Application.ApplicationCategory.Description.Contains(searchText)
                );


            var applicationPublishVMList = new List<ApplicationPublishViewModel>();
            foreach (var item in await searchList.ToListAsync())
            {
                applicationPublishVMList.Add(
                    new ApplicationPublishViewModel()
                    {
                        ApplicationPublish = item,
                        RegisterDateShamsi = Persia.Calendar.ConvertToPersian(item.RegisterDate).ToString(),
                        PublishDateShamsi = Persia.Calendar.ConvertToPersian(item.PublishDate).ToString(),
                        PictureUrl = Utility.Tools.GetImageUrlFromByteArray(_context.ApplicationPictures.FirstOrDefault(p => p.ApplicationPublishId == item.Id).Data),
                        ShowIcon = !string.IsNullOrEmpty(this.User.Identity.Name),
                        IsFavorite = !string.IsNullOrEmpty(this.User.Identity.Name) ?
                                    await _context.FavorieApplications
                                     .AnyAsync(f => f.ApplicationPublishId == item.Id && f.ApplicationStoreUserId == Tools.GetCurrentUserId(User))
                                     : false,


                    });
            }
            this.ViewBag.ShowReturn = true;
            return this.View(@"../Home/Index", applicationPublishVMList);
        }

        //________________________________________________________________________________


        public async Task<IActionResult> Download(int id)
        {
            if (id <= 0) return this.NotFound();
            var UserId = Tools.GetCurrentUserId(User);
            var applicationPublish = await _context.ApplicationPublishs
                .Include(a => a.Application)
                .FirstOrDefaultAsync(a => a.Id == id);
            if (applicationPublish == null) return this.NotFound();

            var applicationDownload = await _context.DownloadApplications
                .FirstOrDefaultAsync(u => u.ApplicationStoreUserId == Tools.GetCurrentUserId(User) && u.ApplicationPublishId == id);
            if (applicationDownload == null)
            {
                applicationDownload.ApplicationStoreUserId = UserId;
                applicationDownload.RegisterDate = DateTime.Now.Date;
                applicationDownload.ApplicationPublishId = id;
                _context.DownloadApplications.Add(applicationDownload);
                await _context.SaveChangesAsync();
            }


            var webRootPath = _hostingEnvironment.WebRootPath;
            var uploadApp = Path.Combine(webRootPath, "ApplicationFiles");
            IFileProvider provider = new PhysicalFileProvider(uploadApp);
            IFileInfo fileInfo = provider.GetFileInfo(id + applicationPublish.Extension);
            return File(fileInfo.CreateReadStream(), "application/andrew-inset", applicationPublish.Application.Title + applicationPublish.Extension);
        }

        //________________________________________________________________________________
        public async Task<IActionResult> DownloadList()
        {
            var downloadList = _context.DownloadApplications
                .Where(c => c.ApplicationStoreUserId == Tools.GetCurrentUserId(User))
                .Select(c => c.ApplicationPublishId);
            var applicationPublishVMList = new List<ApplicationPublishViewModel>();
            foreach (var item in await _context.ApplicationPublishs.Where(p => downloadList.Contains(p.Id)).Include(m => m.Application).Include(m => m.Platform).ToListAsync())
            {
                applicationPublishVMList.Add(
                    new ApplicationPublishViewModel()
                    {
                        ApplicationPublish = item,
                        RegisterDateShamsi = Persia.Calendar.ConvertToPersian(item.RegisterDate).ToString(),
                        PublishDateShamsi = Persia.Calendar.ConvertToPersian(item.PublishDate).ToString(),
                        PictureUrl = Utility.Tools.GetImageUrlFromByteArray(_context.ApplicationPictures.FirstOrDefault(p => p.ApplicationPublishId == item.Id).Data),
                        ShowIcon = true,
                        IsFavorite = !string.IsNullOrEmpty(this.User.Identity.Name) ?
                                    await _context.FavorieApplications
                                     .AnyAsync(f => f.ApplicationPublishId == item.Id && f.ApplicationStoreUserId == Tools.GetCurrentUserId(User))
                                     : false,


                    });
            }
            this.ViewBag.ShowReturn = true;
            return this.View(@"../Home/Index", applicationPublishVMList);
        }

        //__________________________________________________________________________________

        public async Task<IActionResult> SuggestList()
        {
            var SuggestViewModel = new SuggestViewModel();
            SuggestViewModel.SuggestByDownload = new List<ApplicationPublishViewModel>();
            SuggestViewModel.SuggestBySize = new List<ApplicationPublishViewModel>();

            var download = _context.DownloadApplications
             .Include(a => a.ApplicationPublish)
            .GroupBy(s => s.ApplicationPublishId, (k, g) => g
            .Select(s => new { MaxApp = g.Max(s2 => s2.ApplicationPublishId), App = s }))
            .SelectMany(s => s)
            .OrderBy(s => s.MaxApp)
            .Select(s => s.App.ApplicationPublishId)
            .Skip(0)
            .Take(5).Distinct();

            foreach (var item in await _context.ApplicationPublishs.Where(p => download.Contains(p.Id)).Include(m => m.Application).Include(m => m.Platform).ToListAsync())
            {
                SuggestViewModel.SuggestByDownload.Add(
                    new ApplicationPublishViewModel()
                    {
                        ApplicationPublish = item,
                        RegisterDateShamsi = Persia.Calendar.ConvertToPersian(item.RegisterDate).ToString(),
                        PublishDateShamsi = Persia.Calendar.ConvertToPersian(item.PublishDate).ToString(),
                        PictureUrl = Utility.Tools.GetImageUrlFromByteArray(_context.ApplicationPictures.FirstOrDefault(p => p.ApplicationPublishId == item.Id).Data),
                        ShowIcon = true,
                        IsFavorite = !string.IsNullOrEmpty(this.User.Identity.Name) ?
                                    await _context.FavorieApplications
                                     .AnyAsync(f => f.ApplicationPublishId == item.Id && f.ApplicationStoreUserId == Tools.GetCurrentUserId(User))
                                     : false,


                    });
            }

            var size = _context.ApplicationPublishs
           .GroupBy(s => s.Size, (k, g) => g
           .Select(s => new { MinSize = g.Min(s2 => s2.Size), App = s }))
           .SelectMany(s => s)
           .OrderBy(s => s.MinSize)
           .Select(s => s.App.Id)
           .Skip(0)
           .Take(5);


            foreach (var item in await _context.ApplicationPublishs.Where(p => size.Contains(p.Id)).Include(m => m.Application).Include(m => m.Platform).ToListAsync())
            {
                SuggestViewModel.SuggestBySize.Add(
                    new ApplicationPublishViewModel()
                    {
                        ApplicationPublish = item,
                        RegisterDateShamsi = Persia.Calendar.ConvertToPersian(item.RegisterDate).ToString(),
                        PublishDateShamsi = Persia.Calendar.ConvertToPersian(item.PublishDate).ToString(),
                        PictureUrl = Utility.Tools.GetImageUrlFromByteArray(_context.ApplicationPictures.FirstOrDefault(p => p.ApplicationPublishId == item.Id).Data),
                        ShowIcon = true,
                        IsFavorite = !string.IsNullOrEmpty(this.User.Identity.Name) ?
                                    await _context.FavorieApplications
                                     .AnyAsync(f => f.ApplicationPublishId == item.Id && f.ApplicationStoreUserId == Tools.GetCurrentUserId(User))
                                     : false,


                    });
            }
            this.ViewBag.ShowReturn = true;
            return this.View(SuggestViewModel);
        }
    }
}
