using System.ComponentModel.DataAnnotations;

namespace TestC.Model
{
    /// <summary>
    /// Используется для создания нового пользователя
    /// </summary>
    public class UserCreate
    {
        [Required]
        [StringLength(50, ErrorMessage = "Логин должен содержать максимум 50 символов")]
        [Display(Name = "Логин")]
        public string? Login { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Пароль должен содержать максимум 50 символов")]
        [Display(Name = "Пароль")]
        public string? Password { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Имя должен содержать максимум 50 символов")]
        [Display(Name = "Имя")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Поле Пол пустое")]
        [Display(Name = "Пол")]
        public int Gender { get; set; }

        [DataType(DataType.DateTime)]
        [Display(Name = "День рождение")]
        public DateTime? Birthday { get; set; }

        [Required]
        [Display(Name = "Администратор?")]
        public bool Admin { get; set; }
    }
}
