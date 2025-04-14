using AdminNotificator.Core.Domain;
using AdminNotificator.Core.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminNotificator.WebApi;

[Controller]
[Route("notifications")]
public class EmailController(IRepository<EmailType> emailRepository) : ControllerBase
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
    public async Task<IActionResult> Post(EmailType email)
    {
        email.Id = Guid.NewGuid().ToString();
        try
        {
            await emailRepository.AddAsync(email);
        }
        catch (Exception ex)
        {
            return Conflict();
        }

        return CreatedAtRoute(nameof(GetById), new { id = email.Id }, email);
    }
}