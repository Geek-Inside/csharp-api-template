using CSharpAPITemplate.BusinessLayer.Models;
using CSharpAPITemplate.Infrastructure.Results.Base;
using Microsoft.AspNetCore.JsonPatch;

namespace CSharpAPITemplate.BusinessLayer.Services;

public interface IBaseService<TDto> where TDto : BaseEntityDto
{
	Task<Result<TDto>> CreateAsync(TDto dto, long userId, CancellationToken cancellationToken);
	Task<BaseResult> UpdateAsync(long id, JsonPatchDocument? patch, CancellationToken cancellationToken);
	Task<BaseResult> DeleteAsync(long id, CancellationToken cancellationToken);
}