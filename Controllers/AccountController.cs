using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Restaurant.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IUnitOfWork _context;
        private readonly IMapper _mapper;

        public AccountController(ILogger<AccountController> logger, IUnitOfWork context, IMapper mapper)
        {
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var vm = await GetDataAsync();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAccount(CreateAccountViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var landen = await _context.LandenRepository.GetAllAsync();
                vm.Landen = landen.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Naam
                });
                return View("Index", vm);
            }

            var user = _mapper.Map<CustomUser>(vm);

            await _context.CustomUserRepository.AddAsync(user);
            await _context.SaveChangesAsync();

            return View("Confirmation", user);
        }

        private async Task<CreateAccountViewModel> GetDataAsync()
        {
            var landen = await _context.LandenRepository.GetAllAsync();

            return new CreateAccountViewModel
            {
                Landen = landen.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Naam
                })
            };
        }
    }
}
