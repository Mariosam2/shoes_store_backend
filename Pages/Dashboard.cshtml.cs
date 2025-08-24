using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShoesStore.Entities;
using ShoesStore.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ShoesStore.Pages
{
    [Authorize(Roles = "admin")]
    public class DashboardModel : PageModel
    {
        private readonly StoreDBContext _context;
        public DashboardModel(StoreDBContext context)
        {
            _context = context;
        }



    }
}