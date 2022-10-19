﻿using AutoMapper;

using Microsoft.CodeAnalysis.FlowAnalysis;

using Thoughts.DAL;
using Thoughts.UI.MVC.Infrastructure.AutoMapper;

namespace Thoughts.UI.MVC.Controllers;

public class BlogController : Controller 
{
    private readonly IBlogPostManager _postManager;
    private readonly IConfiguration _configuration;
    private IMapper _mapper;
    private readonly int _lengthText;
    private readonly ThoughtsDB _context;
    public BlogController(IBlogPostManager postManager, IConfiguration configuration, ThoughtsDB context, IMapper mapper)
    {
        _postManager = postManager;
        _configuration = configuration;
        _context = context;
        _mapper = mapper;
        _lengthText = _configuration.GetValue<int>("LengthTextOnHomeView");
    }

    /// <summary>
    /// Запрос контроллера на получение всех постов
    /// </summary>
    /// <returns> Возврат всех постов в ViewResult </returns>
    public async Task<IActionResult> Index()
    {
        var posts = (await _postManager.GetAllPostsAsync()).ToArray();
        StringTools.CutBodyTextInPosts(posts, _lengthText);
        var model = new BlogIndexWebModel
        {
            Posts = posts,
        };
        return View(model);
    }

    /// <summary>
    /// Данные детальные по одному элемпенту
    /// </summary>
    /// <param name="id">Идентификатор элемента</param>
    /// <returns>Вьюха с детальными данными по посту</returns>
    public async Task<IActionResult> Details(int id)
    {
        var post = await _postManager.GetPostAsync(id);
        var model = new BlogDetailsWebModel
        {
            Post = post,
        };
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var post = await _postManager.GetPostAsync(id);
        var model = new BlogDetailsWebModel
        {
            Post = post,
        };
        var viewModel = _mapper.Map(post, model);
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(BlogDetailsWebModel model, CancellationToken cancellation = default)
    {
        if (ModelState.IsValid)
        {
            await _postManager.ChangePostTitleAsync(model.PostId, model.Title, cancellation);
            await _postManager.ChangePostBodyAsync(model.PostId, model.Body, cancellation);
            await _postManager.ChangePostCategoryAsync(model.PostId, model.CategoryName, cancellation);
        }

        return RedirectToAction("Details", "Blog", new { Id = model.PostId });
    }

    public async Task<IActionResult> TypeaheadQuery(string query)
    {
        var categories = _context.Categories.Where(item => item.Name.StartsWith(query)).ToList();
        return Json(categories.Select(item => new
        {
            item.Name
        }));
    }
}
