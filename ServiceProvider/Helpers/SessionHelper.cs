using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using ServiceProvider.Models;

namespace ServiceProvider.Helpers
{
    public static class SessionHelper
    {
		private const string UserInfoKey = "User";

		public static void SetObjectAsJson<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonConvert.SerializeObject(value, Formatting.Indented, new JsonSerializerSettings()
			{
				ReferenceLoopHandling = ReferenceLoopHandling.Ignore
			}));
        }

        public static T? GetObjectAsJson<T>(this ISession session, string key)
        {
            string? str = session.GetString(key);
			return str != null ? JsonConvert.DeserializeObject<T>(str) : default(T);
        }

		public static void SetSession(Controller controller, IUser user)
		{
			controller.HttpContext.Session.SetObjectAsJson(UserInfoKey, user);
		}

		public static IUser? GetSession(Controller controller)
		{
			return controller.HttpContext.Session.GetObjectAsJson<IUser>(UserInfoKey);
		}

		public static void DeleteSession(Controller controller)
		{
			controller.HttpContext.Session.Remove(UserInfoKey);
		}

		public static bool IsLoggedIn(Controller controller, bool reqadmin = false)
		{
			return IsLoggedIn(GetSession(controller), reqadmin);
		}

		public static bool IsLoggedIn(IUser? user, bool reqadmin = false)
		{
			if (user != null)
			{
				if (!reqadmin || user.IsAdmin)
					return true;
			}
			return false;
		}

		public static IUser? GetUser(IHttpContextAccessor accessor)
        {
            return accessor.HttpContext != null ? accessor.HttpContext.Session.GetObjectAsJson<IUser>(UserInfoKey) : null;
        }
    }
}
