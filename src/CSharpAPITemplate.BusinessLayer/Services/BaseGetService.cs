using AutoMapper;
using CSharpAPITemplate.BusinessLayer.Extensions;
using CSharpAPITemplate.BusinessLayer.Models;
using CSharpAPITemplate.Data;
using CSharpAPITemplate.Domain.Common;
using CSharpAPITemplate.Infrastructure.Results;
using CSharpAPITemplate.Infrastructure.Results.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CSharpAPITemplate.BusinessLayer.Services;

public class BaseGetService<TEntity, TDto> : IBaseGetService<TDto> 
	where TEntity : BaseEntity
	where TDto : BaseEntityDto 
{
	protected readonly IApplicationDbContext Database;
	protected readonly IMapper Mapper;
	protected readonly ILogger Logger;
	
	public BaseGetService(
		IApplicationDbContext database,
		IMapper mapper, 
		ILogger logger)
	{
		Database = database;
		Mapper = mapper;
		Logger = logger;
	}
	
	public async Task<Result<TDto>> GetAsync(long id, long userId = default, CancellationToken cancellationToken = default)
	{
		try
		{
			var result = await Database.Set<TEntity>().AsNoTracking().OnlyActive().FirstOrDefaultAsync(i => i.Id.Equals(id), cancellationToken);
		
			return result is null 
				? BlResult<TDto>.NotFound("Entity not found.")
				: BlResult<TDto>.Ok(Mapper.Map<TDto>(result));
		}
		catch (Exception e)
		{
			Logger.LogError(e.Message);
			return BlResult<TDto>.InternalError("An unexpected error has occurred, please try again later.");
		}
	}

	public async Task<Result<List<TDto>>> GetAllAsync(long userId = default, CancellationToken cancellationToken = default)
	{
		try
		{
			var result = await Database.Set<TEntity>().AsNoTracking().OnlyActive().ToListAsync(cancellationToken);
		
			return BlResult<List<TDto>>.Ok(Mapper.Map<List<TDto>>(result));
		}
		catch (Exception e)
		{
			Logger.LogError(e.Message);
			return BlResult<List<TDto>>.InternalError("An unexpected error has occurred, please try again later.");
		}
	}
}