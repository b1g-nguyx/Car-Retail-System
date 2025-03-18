using System.Linq.Expressions;
using Car_Rental_System.Models;
using Car_Rental_System.Utils;
using Car_Rental_System.ViewModels;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace Car_Rental_System.Repositories;

public class CategoryRepository : ICategoryRepository
{

    private readonly UnitOfWork _unitOfWork;
    private readonly IImageRepository _iimageRepository;

    public CategoryRepository(UnitOfWork unitOfWork, IImageRepository imageRepository)
    {
        _iimageRepository = imageRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task AddAsync(CategoryViewModel categoryViewModel)
    {
        Category category = await ConvertToModel(categoryViewModel);
        await _unitOfWork._categoryRepository.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();
        await _iimageRepository.AddAsync(categoryViewModel.Files, category.Id, Constant.category);
    }


    public async Task DeleteAsync(int id)
    {
        var item = await _unitOfWork._categoryRepository.GetByIdAsync(id);

        if (item != null)
        {
            _unitOfWork._categoryRepository.Remove(item);
        }
        await _unitOfWork.SaveChangesAsync();
        await _iimageRepository.DeleteAsync(id, Constant.category);
    }


    public async Task<IPagedList<CategoryViewModel>> GetAllAsync(string searchString, int pageSize, int pageNumber)
    {
        Expression<Func<Category, bool>> filter = PredicateBuilder.New<Category>(true);

        if (!String.IsNullOrEmpty(searchString))
        {
            filter = filter.And(c => c.Name!.ToUpper().Contains(searchString.ToUpper()));
        }

        var totalCount = await _unitOfWork._categoryRepository.GetQueryable().Where(filter).CountAsync();
        var query = _unitOfWork._categoryRepository.GetQueryable(filter, allowTracking: true)
                         .OrderBy(c => c.Id)
                         .Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize);

        var result = await query.ToListAsync();
        List<CategoryViewModel> categoryViewModel = new List<CategoryViewModel>();

        foreach (var item in result)
        {
            var category = await ConvertToViewModel(item);
            categoryViewModel.Add(category);
        }
        return new StaticPagedList<CategoryViewModel>(categoryViewModel, pageNumber, pageSize, totalCount);
    }


    public async Task<IEnumerable<CategoryViewModel>> GetAllAsync()
    {
        var result = await _unitOfWork._categoryRepository.GetAllAsync();
        var categoryViewModel = new List<CategoryViewModel>();
        foreach (var item in result)
        {
            var category = await ConvertToViewModel(item);
            categoryViewModel.Add(category);
        }
        return categoryViewModel;
    }


    public async Task<CategoryViewModel> GetByIdAsync(int id)
    {
        var category = await _unitOfWork._categoryRepository.GetByIdAsync(id);
        var categoryViewModel = await ConvertToViewModel(category!);
        return categoryViewModel;
    }

    public async Task UpdateAsync(CategoryViewModel categoryViewModel)
    {
        Category category = await ConvertToModel(categoryViewModel);
        _unitOfWork._categoryRepository.Update(category);
        await _unitOfWork.SaveChangesAsync();
        await _iimageRepository.AddAsync(categoryViewModel.Files, category.Id, Constant.category);
    }

    public async Task<Category> ConvertToModel(CategoryViewModel categoryViewModel)
    {
        var category = categoryViewModel.Category;
        return category;
    }

    public async Task<CategoryViewModel> ConvertToViewModel(Category category)
    {
        var categoryViewModel = new CategoryViewModel
        {
            Category = category,
            Images = await _iimageRepository.GetAllByIdRelationId(category.Id, Constant.category)
        };

        return categoryViewModel;
    }

    public Task<IEnumerable<Category>> GetAllForListAsync(bool IsActive)
    {
        return _unitOfWork._categoryRepository.GetManyAsync(c => c.IsActive == IsActive);
    }
}