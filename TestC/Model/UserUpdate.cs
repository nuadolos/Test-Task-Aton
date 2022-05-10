using System.ComponentModel.DataAnnotations;

namespace TestC.Model
{
    /// <summary>
    /// Используется для обновления личных данных пользователя
    /// </summary>
    public class UserUpdate
    {
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
    }
}
