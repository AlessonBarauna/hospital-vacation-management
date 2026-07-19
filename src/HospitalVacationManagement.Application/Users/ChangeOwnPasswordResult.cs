namespace HospitalVacationManagement.Application.Users;

public enum ChangeOwnPasswordResult
{
    Success = 1,
    UserNotFound = 2,
    InvalidCurrentPassword = 3
}