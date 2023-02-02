using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoneShop_Models;
using StoneShop_Utility;
using System;
using System.Linq;

namespace StoneShop_DataAccess.Initializer
{
	public class DataBaseInitializer : IDataBaseInitializer
	{
		private readonly ApplicationDbContext _dataBase;
		private readonly UserManager<IdentityUser> _userManager;  // оба беруться из "метаданных"
		private readonly RoleManager<IdentityRole> _roleManager;  //

		public DataBaseInitializer(ApplicationDbContext dataBase, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_dataBase = dataBase;
			_userManager = userManager;
			_roleManager = roleManager;
		}

		public void Initialize()
		{
			try
			{
				if (_dataBase.Database.GetPendingMigrations().Count() > 0)
				{
					_dataBase.Database.Migrate();
				}
			}
			catch (Exception ex) { }

			if (!_roleManager.RoleExistsAsync(WebConstants.AdminRole).GetAwaiter().GetResult())
			{
				_roleManager.CreateAsync(new IdentityRole(WebConstants.AdminRole)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(WebConstants.CustomerRole)).GetAwaiter().GetResult();
			}
			else
			{
				return;
			}

			_userManager.CreateAsync(new User  // создание первого пользователя
			{
				UserName = "admin@admin.com",
				Email = "admin@admin.com",
				EmailConfirmed = true,
				FullName = "+375112223344"
			},"Admin1234*").GetAwaiter().GetResult();

			User user = _dataBase.User.FirstOrDefault(u => u.Email == "admin@admin.com");
			_userManager.AddToRoleAsync(user, WebConstants.AdminRole).GetAwaiter().GetResult();
		}
	}
}
