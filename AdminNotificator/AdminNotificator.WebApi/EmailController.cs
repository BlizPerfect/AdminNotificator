using System;
using System.Linq;
using System.Threading.Tasks;
using AdminNotificator.Core.Domain;
using AdminNotificator.Core.DTOs;
using AdminNotificator.Core.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace AdminNotificator.WebApi;

[Controller]
[Route("notifications")]
public class EmailController(
    IRepository<EmailType> emailRepository,
    IMapper mapper,
    ILogger<EmailController> logger
) : ControllerBase
{
    [HttpGet]
    [Produces("application/json")]
    public IActionResult GetAll()
    {
        var allEmails = emailRepository.GetAll().AsNoTracking().ToArray();
        return Ok(allEmails);
    }
    
    [HttpGet("{id}", Name = nameof(GetById))]
    [Produces("application/json")]
    public async Task<IActionResult> GetById(string id)
    {
        var email = await emailRepository.GetAll()
            .AsNoTracking()
            .Where(emailType => emailType.Id == id)
            .FirstOrDefaultAsync();

        if (email == null)
        {
            return NotFound();
        }

        return Ok(email);
    }

    [HttpPost]
    [Produces("application/json")]
    public async Task<IActionResult> Post(EmailTypeDTO emailDTO)
    {
        EmailType email;
        try
        {
            email = mapper.Map<EmailType>(emailDTO);
            await emailRepository.AddAsync(email);
        }
        catch (Exception ex)
        {
            logger.LogError($"Post notifications/ failed: {ex}");
            return Conflict();
        }

        return CreatedAtRoute(nameof(GetById), new { id = email.Id }, email);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(string id)
    {
        await emailRepository.DeleteAsync(id);
        return NoContent();
    }
    
    [HttpPost("delete")]
    public async Task<IActionResult> DeleteByIdsAsync([FromBody] IList<string> ids)
    {
        if (ids is null)
        {
            return UnprocessableEntity("No IDs provided");
        }
        
        await emailRepository.DeleteAllAsync(ids);
        
        return NoContent();
    }
}