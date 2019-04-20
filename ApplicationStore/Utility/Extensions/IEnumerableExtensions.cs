using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApplicationStore.Utility.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<SelectListItem> ToSelectListItems<T>(this IEnumerable<T> items, int selectedValue)
        {
            try
            {
                if (items != null)
                {
                    return from item in items
                           select new SelectListItem
                           {
                               Text = item.GetPropertyValue("Title"),
                               Value = item.GetPropertyValue("Id"),
                               Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())
                           };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }

        }
    }
}
