using AdminNotificator.Core.Domain;
using AdminNotificator.Core.DTOs;
using AdminNotificator.Core.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public ActionResult<IEnumerable<EmailType>> GetAll()
    {
        var allEmails = emailRepository.GetAll().AsNoTracking().ToArray();
        return Ok(allEmails);
    }
    
    [HttpGet("{id}", Name = nameof(GetById))]
    [Produces("application/json")]
    public async Task<ActionResult<EmailType>> GetById(string id)
    {
        ValidateId(id);
        if (!ModelState.IsValid)
            return UnprocessableEntity(ModelState);

        EmailType? email;
        try
        {
            email = await emailRepository.GetAll()
                .AsNoTracking()
                .Where(emailType => emailType.Id == id)
                .FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            logger.LogError($"Get notifications/{id} failed: {ex}");
            return Conflict();
        }

        if (email == null)
        {
            return NotFound();
        }

        return Ok(email);
    }

    [HttpPost]
    [Produces("application/json")]
    public async Task<ActionResult<EmailType>> Post(EmailTypeDTO emailDTO)
    {
        EmailType email;
        try
        {
            email = mapper.Map<EmailType>(emailDTO);
            ValidateEmailType(email);
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
            await emailRepository.AddAsync(email);
        }
        catch (Exception ex)
        {
            logger.LogError($"Post notifications/ failed: {ex}");
            return Conflict();
        }
        
        return CreatedAtRoute(nameof(GetById), new { id = email.Id }, email);
    }

    [HttpPut("{id}")]
    [Produces("application/json")]
    public async Task<ActionResult<EmailType>> Put(string id, EmailTypeDTO emailDto)
    {
        EmailType email;
        try
        {
            ValidateId(id);

            email = mapper.Map<EmailType>(emailDto);
            email.Id = id;
            
            ValidateEmailType(email);
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);

            var existing = await emailRepository.GetAll()
                .AsNoTracking()
                .Where(o => o.Id == id)
                .FirstOrDefaultAsync();
            
            if (existing == null)
            {
                await emailRepository.AddAsync(email);
                return CreatedAtRoute(nameof(GetById), new { id = email.Id }, email);
            }

            await emailRepository.UpdateAsync(email);
        }
        catch (Exception ex)
        {
            logger.LogError($"Put notifications/{id} failed: {ex}");
            return Conflict();
        }

        return Ok(email);
    }

    private void ValidateEmailType(EmailType email)
    {
        if (email.ExperianceDays != null && email.ExperianceDays < 0)
            ModelState.AddModelError(nameof(email.ExperianceDays), "experience days must be >= 0");
        if (email.IntersectDepartmentIds != null && email.ExceptDepartmentIds != null)
            ModelState.AddModelError("DepartmentIds",
                "can not use except and intersect at the same time");
        if (email.IntersectTowns != null && email.ExceptTowns != null)
            ModelState.AddModelError("Towns",
                "can not use except and intersect at the same time");
        if (email.MaternityDays != null && email.MaternityDays < 0)
            ModelState.AddModelError(nameof(email.MaternityDays),
                "maternity days must be >= 0");
        if (email.ForGenders != null)
        {
            if (email.ForGenders.Length > 2)
            {
                ModelState.AddModelError(nameof(email.ForGenders),
                    "gender list must have length <= 2");
            }
            
            for (var i = 0; i < email.ForGenders.Length; i++)
            {
                if (email.ForGenders[i] != nameof(UserGender.Female)
                    && email.ForGenders[i] != nameof(UserGender.Male))
                    ModelState.AddModelError(nameof(email.ForGenders),
                        $"invalid gender at index {i} (must be {nameof(UserGender.Male)} or {nameof(UserGender.Female)})");
            }
        }

        if (email.DayCountsForResend != null && email.DayCountsForResend < 0)
            ModelState.AddModelError(nameof(email.DayCountsForResend),
                "DayCountsForResend must be >= 0");
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAsync(string id)
    {
        try
        {
            await emailRepository.DeleteAsync(id);
            return NoContent();
        }
        catch (Exception e)
        {
            logger.LogError($"Delete id failed: {e}");
            return Conflict();
        }
    }
    
    [HttpPost("deleteMany")]
    public async Task<ActionResult> DeleteByIdsAsync([FromBody] IList<string> ids)
    {
        try
        {
            if (ids is null)
            {
                return UnprocessableEntity("No IDs provided");
            }

            await emailRepository.DeleteAllAsync(ids);

            return NoContent();

        }
        catch (Exception e)
        {
            logger.LogError($"Post notifications/deleteMany failed: {e}");
            return Conflict();
        }
    }

    private void ValidateId(string id)
    {
        if (!Guid.TryParse(id, out _))
            ModelState.AddModelError("id", "id must be a valid GUID");
    }
}