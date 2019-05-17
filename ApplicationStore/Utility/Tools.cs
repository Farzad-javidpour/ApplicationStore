using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ApplicationStore.Utility
{
    public static class Tools
    {
        public static string GetImageUrlFromByteArray(byte[] imageBytes)
        {
            //Convert byte array to base64string   
            string imreBase64Data = Convert.ToBase64String(imageBytes);
            string imgDataURL = string.Format("data:image/jpg;base64,{0}", imreBase64Data);
            
            return imgDataURL;
        }

        public static string GetCurrentUserId(ClaimsPrincipal claimsPrincipal)
        {
            var claimsIdentity = (ClaimsIdentity)claimsPrincipal.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            return claim.Value;
        }

        public static string GetCurrentUserEmail(ClaimsPrincipal claimsPrincipal)
        {
            var claimsIdentity = (ClaimsIdentity)claimsPrincipal.Identity;
            return  claimsIdentity.Name;
          
        }
    }
}
