using System;
using System.Collections.Generic;
using System.Linq;

namespace AdminNotificator.Core.Domain;

public class UserProfile
{
    public string Id { get; private set; }

    /// <summary>
    ///     Логин в домене
    /// </summary>
    public string Login { get; set; }

    /// <summary>
    ///     Уникальный ИД в домене
    /// </summary>
    public string Sid { get; private set; }

    /// <summary>
    ///     Имя
    /// </summary>
    public string Firstname { get; set; }

    /// <summary>
    ///     Фамилия
    /// </summary>
    public string Surname { get; set; }

    /// <summary>
    ///     Отчество
    /// </summary>
    public string Patronymic { get; set; }

    /// <summary>
    ///     Электронная почта
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    ///     Дата найма
    /// </summary>
    public DateTime? Seniority { get; set; }

    /// <summary>
    ///     Кол-во дней стажа на момент остановки
    /// </summary>
    public int? SuspendedSeniorityDays { get; set; }

    /// <summary>
    ///     Отделения где работает пользователь
    /// </summary>
    public UserDepartment[] DepartmentItems { get; set; }

    /// <summary>
    ///     Коллекция организаций, в которых работает пользователь
    /// </summary>
    public UserPosition[] UserPositions { get; set; }

    /// <summary>
    ///     Время последней синхронизации (обновления) профиля
    /// </summary>
    public DateTime SyncDate { get; set; }

    /// <summary>
    ///     Идентификаторы подразделений с родительскими
    /// </summary>
    public int[] DepartmentIds { get; set; }

    /// <summary>
    /// Статус пользователя
    /// </summary>
    public UserStatus UserStatus { get; set; }

    /// <summary>
    ///     Стаж
    /// </summary>
    public int? ExperienceDays => SuspendedSeniorityDays ??
                                  (Seniority.HasValue ? (int) (DateTime.UtcNow - Seniority.Value).TotalDays : null);

    /// <summary>
    /// Офис пользователя
    /// </summary>
    public UserOffice UserOffice { get; set; }

    /// <summary>
    /// Пол пользователя
    /// </summary>
    public UserGender UserGender { get; set; }

    /// <summary>
    /// Дата декрета
    /// </summary>
    public DateTime? MaternityLeaveDate { get; set; }
    
    protected UserProfile() { }

    private UserProfile(string login, string sid, string firstname, string surname, string patronymic,
        string email, DateTime? seniorityDate, int? suspendedSeniorityDays, IEnumerable<UserDepartment> departmentItems,
        IEnumerable<UserPosition> positions, int[] departmentIds, UserStatus userStatus, UserOffice userOffice,
        UserGender userGender, DateTime? maternityLeaveDate)
    {
        Login = login;
        Sid = sid;
        Firstname = firstname;
        Surname = surname;
        Patronymic = patronymic;
        SyncDate = DateTime.UtcNow;
        Email = email;
        Seniority = seniorityDate;
        SuspendedSeniorityDays = suspendedSeniorityDays;
        DepartmentItems = departmentItems?.ToArray();
        UserPositions = positions?.ToArray();
        DepartmentIds = departmentIds;
        UserStatus = userStatus;
        UserOffice = userOffice;
        UserGender = userGender;
        MaternityLeaveDate = maternityLeaveDate;
    }

    public static UserProfile Create(string login, string sid, string firstname, string surname, string patronymic, string email,
        DateTime? seniorityDate, int? suspendedSeniorityDays, IEnumerable<UserPosition> positions,
        IEnumerable<UserDepartment> departmentItems, int[] departmentIds, UserStatus userStatus, UserOffice userOffice,
        UserGender userGender, DateTime? maternityLeaveDate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(login);
        ArgumentException.ThrowIfNullOrWhiteSpace(sid);
        ArgumentException.ThrowIfNullOrWhiteSpace(firstname);
        ArgumentException.ThrowIfNullOrWhiteSpace(surname);

        return new UserProfile(login, sid, firstname, surname, patronymic, email, seniorityDate, suspendedSeniorityDays,
            departmentItems, positions, departmentIds, userStatus, userOffice, userGender, maternityLeaveDate);
    }
    
}