using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DailyStream.Models;
using DailyStream.Services;

namespace DailyStream.Controllers;

public class HomeController : Controller
{
    private readonly DailymotionApiService _apiService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, DailymotionApiService apiService)
    {
        _apiService = apiService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index() 
    {
        try
        {
            var popularVideos = await _apiService.GetPopularVideosAsync();
            return View(popularVideos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener videos populares");
            return View(new List<Video>());
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return RedirectToAction(nameof(Index));
            
        try
        {
            var searchResult = await _apiService.SearchVideosAsync(query);
            ViewBag.SearchQuery = query;
            return View(searchResult.List);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar videos");
            ViewBag.SearchQuery = query;
            ViewBag.Error = "No se pudieron encontrar resultados. Intente más tarde.";
            return View(new List<Video>());
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> Watch(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return RedirectToAction(nameof(Index));
            
        try
        {
            var video = await _apiService.GetVideoByIdAsync(id);

            var relatedVideos = (await _apiService.SearchVideosAsync(
                  video.Tags.Count > 0 
                    ? video.Tags[0]
                    : video.Title.Split(" ")[0], 7))
              .List
              .Where(v => v.Id != id)
              .Take(6)
              .ToList();

            ViewBag.RelatedVideos = relatedVideos;         
            return View(video);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener video");
            return RedirectToAction(nameof(Index));
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
