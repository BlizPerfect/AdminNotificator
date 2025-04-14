using AdminNotificator.Core.Domain;
using AdminNotificator.Core.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace AdminNotificator.WebApi;

[Controller]
[Route("notifications")]
public class EmailController(IRepository<EmailType> emailRepository) : ControllerBase
{
    [HttpGet]
    [Produces("application/json")]
    public IActionResult GetAll()
    {
        var allEmails = emailRepository.GetAll();
        return Ok(allEmails.ToArray());
    }
}