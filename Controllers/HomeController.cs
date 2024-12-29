using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using mentor.Models;
using Microsoft.AspNetCore.Authorization;

namespace mentor.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [Authorize(Roles = "Administrator")]
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Khoahoc()
    {
        return View();
    }
    public IActionResult Lienhe()
    {
        return View();
    }
    public IActionResult Thanhvien()
    {
        return View();
    }
    public IActionResult SphamTin()
    {
        return View();
    }
    public IActionResult CNTT()
    {
        return View();
    }
    public IActionResult TinTucChung()
    {
        return View();
    }
    public IActionResult HoatDongSV()
    {
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
