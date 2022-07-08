using CSharpAPITemplate.BusinessLayer.Models;
using CSharpAPITemplate.Infrastructure.Results.Base;
using Microsoft.AspNetCore.JsonPatch;

namespace CSharpAPITemplate.BusinessLayer.Services;

public interface IBaseService<TDto> : IBaseGetService<TDto> where TDto : BaseEntityDto
{
	Task<Result<TDto>> CreateAsync(TDto dto, long userId, CancellationToken cancellationToken);
	Task<Result<TDto>> UpdateAsync(long id, JsonPatchDocument? patch, CancellationToken cancellationToken);
	Task<Result<TDto>> DeleteAsync(long id, CancellationToken cancellationToken);
}