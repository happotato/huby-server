using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Huby.Data.Extensions
{
    public static class Extensions
    {
        public static bool ContainsSpecialCharacter(this string str)
        {
            var regex = new Regex("[a-zA-Z0-9]+");
            return !regex.IsMatch(str);
        }

        public static Permissions ToPermissions(this IPermissions permissions)
        {
            return new Permissions
            {
                CanEdit = permissions.CanEdit,
                CanDeletePosts = permissions.CanDeletePosts,
            };
        }

        public static IEnumerable<T> Paginate<T>(this IEnumerable<T> items, int page, int limit)
        {
            return items
                .Skip(page * limit)
                .Take(limit);
        }

        public static IEnumerable<T> Paginate<T, I>(this IEnumerable<T> items, int page, int limit, I after)
            where T: IIdentifiable<I>
            where I: IEquatable<I>
        {
            var results = items;

            if (after != null)
            {
                results = results
                    .SkipWhile(item => !item.Id.Equals(after))
                    .Skip(1);
            }

            return results
                .Skip(page * limit)
                .Take(limit);
        }
    }
}
