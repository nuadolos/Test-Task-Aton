using System.Text.RegularExpressions;
using TestDB.Entities;

namespace TestC.Validate
{
    /// <summary>
    /// Проверяет уникальные поля сущности User
    /// </summary>
    static public class UserValidate
    {
        /// <summary>
        /// Проверяет поле "Логин" и 
        /// отправляет список ошибок в случае их возникновения 
        /// </summary>
        /// <param name="login"></param>
        /// <param name="users"></param>
        /// <returns></returns>
        public static IEnumerable<string>? LoginValidator(string login, IList<User> users)
        {
            List<string> errors = new List<string>();

            //Проверяет поле при помощи регулярного выражения
            if (!Regex.IsMatch(login, @"^\w[A-z0-9]\w+$"))
                errors.Add("Для логина разрешены только латинские буквы и цифры");
            
            //Проверяет каждого пользователя на наличие того же логина
            foreach (var user in users)
            {
                if (login == user.Login)
                {
                    errors.Add("Пользователь с данным логином уже существует");
                    break;
                }
            }

            //В случае безошибочной проверки отправляет null
            return errors.Count == 0 ? null : errors;
        }

        /// <summary>
        /// Проверяет поле "Пароль" при помощи регулярного выражения
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool PasswordValidator(string password) =>
            Regex.IsMatch(password, @"^\w[A-z0-9]\w+$") ? true : false;

        /// <summary>
        /// Проверяет поле "Имя" при помощи регулярного выражения
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool NameValidator(string name) =>
            Regex.IsMatch(name, @"^\w[A-zА-я]\w+$") ? true : false;
    }
}
