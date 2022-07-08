using CSharpAPITemplate.BusinessLayer.Models;
using CSharpAPITemplate.Infrastructure.Results.Base;

namespace CSharpAPITemplate.BusinessLayer.Services;

public interface IBaseGetService<TDto> where TDto : BaseEntityDto
{
	Task<Result<TDto>> GetAsync(long id, long userId = default, CancellationToken cancellationToken = default);
	Task<Result<List<TDto>>> GetAllAsync(long userId = default, CancellationToken cancellationToken = default);
}