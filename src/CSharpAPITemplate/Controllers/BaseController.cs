using CSharpAPITemplate.BusinessLayer.Models;
using CSharpAPITemplate.BusinessLayer.Services;
using CSharpAPITemplate.Infrastructure.Attributes;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CSharpAPITemplate.Controllers
{
    public class BaseController<TDto> : ControllerBase where TDto : BaseEntityDto
    {
        protected readonly IBaseService<TDto> Service;
        protected readonly ILogger Logger;
        
        protected long UserId => long.Parse((string?)HttpContext.Items["UserId"] ?? "0");

        public BaseController(
            IBaseService<TDto> service,
            ILogger logger)
        {
            Service = service;
            Logger = logger;
        }
        
        [HttpGet("{id:long}")]
        [JwtAuthorize]
        public virtual async Task<IActionResult> GetById(long id, CancellationToken cancellationToken)
        {
            var result = await Service.GetAsync(id, UserId, cancellationToken);
            return result.ToActionResult();
        }
        
        [HttpGet]
        [JwtAuthorize]
        public virtual async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var result = await Service.GetAllAsync(UserId, cancellationToken);
            return result.ToActionResult();
        }

        [HttpPost]
        [JwtAuthorize]
        public virtual async Task<IActionResult> Create([FromBody] TDto model, CancellationToken cancellationToken)
        {
            var result = await Service.CreateAsync(model, UserId, cancellationToken);
            return result.ToActionResult();
        }
        
        [HttpPatch("{id:long}")]
        [JwtAuthorize]
        public virtual async Task<IActionResult> Update(long id, [FromBody] JsonPatchDocument patch, CancellationToken cancellationToken)
        {
            var result = await Service.UpdateAsync(id, patch, cancellationToken);
            return result.ToActionResult();
        }
        
        [HttpDelete("{id:long}")]
        [JwtAuthorize("Admin")]
        public virtual async Task<IActionResult> Delete(long id, CancellationToken cancellationToken)
        {
            var result = await Service.DeleteAsync(id, cancellationToken);
            return result.ToActionResult();
        }
    }
}