namespace TestC.Model
{
    /// <summary>
    /// Используется для вывода краткой информации о пользователе
    /// </summary>
    public class UserRead
    {
        public string? Name { get; set; }
        public string? Gender { get; set; }
        public DateTime? Birthday { get; set; }
        public string? IsActive { get; set; }
    }
}
