using AutoMapper;
using CSharpAPITemplate.BusinessLayer.Extensions;
using CSharpAPITemplate.BusinessLayer.Models;
using CSharpAPITemplate.Data;
using CSharpAPITemplate.Domain.Common;
using CSharpAPITemplate.Infrastructure.Results;
using CSharpAPITemplate.Infrastructure.Results.Base;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CSharpAPITemplate.BusinessLayer.Services;

public class BaseService<TEntity, TDto> : BaseGetService<TEntity, TDto>, IBaseService<TDto> 
	where TEntity : BaseEntity
	where TDto : BaseEntityDto 
{
	public BaseService(
		IApplicationDbContext database,
		IMapper mapper,
		ILogger logger) : base(database, mapper, logger)
	{
	}

	public async Task<Result<TDto>> CreateAsync(TDto dto, long userId, CancellationToken cancellationToken)
	{
		try
		{
			var entity = Mapper.Map<TEntity>(dto);
	
			await Database.Set<TEntity>().AddAsync(entity, cancellationToken);
			await Database.SaveChangesAsync(cancellationToken);
            
			return BlResult<TDto>.Ok(Mapper.Map<TDto>(entity));
		}
		catch (Exception e)
		{
			Logger.LogError(e.Message);
			return BlResult<TDto>.InternalError("An unexpected error has occurred, please try again later.");
		}
	}

	public async Task<Result<TDto>> UpdateAsync(long id, JsonPatchDocument? patch, CancellationToken cancellationToken)
	{
		try
		{
			if (patch == null)
				return BlResult<TDto>.BadRequest("Patch is null");;

			var entity = await Database.Set<TEntity>().AsNoTracking().OnlyActive().FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
			if (entity == null)
				return BlResult<TDto>.NotFound($"Entity with id {id.ToString()} not found.");
			
			var dto = Mapper.Map<TDto>(entity);
			try
			{
				patch.ApplyTo(dto);
			}
			catch (Exception e)
			{
				Logger.LogError(e.Message);
				return BlResult<TDto>.BadRequest(e.Message);
			}

			Database.Set<TEntity>().Update( Mapper.Map<TEntity>(dto));
			await Database.SaveChangesAsync(cancellationToken);
			
			return BlResult<TDto>.NoContent();
		}
		catch (Exception e)
		{
			Logger.LogError(e.Message);
			return BlResult<TDto>.InternalError("An unexpected error has occurred, please try again later.");
		}
	}

	public async Task<Result<TDto>> DeleteAsync(long id, CancellationToken cancellationToken)
	{
		try
		{
			var entity = await Database.Set<TEntity>().FirstOrDefaultAsync(i => i.Id.Equals(id), cancellationToken);
			if (entity == null)
				return BlResult<TDto>.NotFound($"Entity with id {id.ToString()} not found.");
			if (entity.IsDeleted)
				return BlResult<TDto>.BadRequest("Entity already deleted.");

			entity.IsDeleted = true;
			entity.DeleteDateTime = DateTime.UtcNow;

			await Database.SaveChangesAsync(cancellationToken);
			return BlResult<TDto>.NoContent();
		}
		catch (Exception e)
		{
			Logger.LogError(e.Message);
			return BlResult<TDto>.InternalError("An unexpected error has occurred, please try again later.");
		}
	}
}