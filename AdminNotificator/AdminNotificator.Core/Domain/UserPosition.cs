namespace AdminNotificator.Core.Domain;

public class UserPosition
{
    public int Id { get; set; }
    /// <summary>
    ///    Наименование организации
    /// </summary>
    public string OrganizationShortname { get; init; }

    /// <summary>
    ///     Должность
    /// </summary>
    public string Post { get; init; }
    
    public string UserProfileId { get; set; }
    public UserProfile UserProfile { get; set; }
}