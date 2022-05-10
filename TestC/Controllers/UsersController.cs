#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestC.Model;
using TestC.Validate;
using TestDB.Context;
using TestDB.Entities;

namespace TestC.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserContext _context;

        public UsersController(UserContext context) =>
            _context = context;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();

            return users;
        }

        #region Create

        /// <summary>
        /// Создание нового пользователя, используя модель UserCreate
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPost("{login}/{password}")]
        public async Task<ActionResult<User>> CreateUser([FromRoute] string login, [FromRoute] string password, [FromForm] UserCreate user)
        {
            // Аутентификация пользователя
            User admin = await SearchUser(login, password);

            // Проверяет наличие прав администратора
            string error = IsAdmin(admin);
            if (error != string.Empty)
                return BadRequest(error);

            // Проверяет уникальность и корректность нового логина
            var errors = UserValidate.LoginValidator(user.Login, _context.Users.ToList());
            if (errors != null)
                return BadRequest(errors);

            // Проверяет корректность пароля
            if (!UserValidate.PasswordValidator(user.Password))
                return BadRequest("Для пароля разрешены только латинские буквы и цифры");

            // Проверяет корректность имени
            if (!UserValidate.NameValidator(user.Name))
                return BadRequest("Для имени разрешены только латинские и русские буквы");

            // Создание пользователя, используя модель UserCreate
            User newUser = new User {
                Guid = System.Guid.NewGuid().ToString(),
                Login = user.Login,
                Password = user.Password,
                Name = user.Name,
                Gender = user.Gender,
                Birthday = user.Birthday,
                Admin = user.Admin,
                CreatedOn = DateTime.Now,
                CreatedBy = admin.Login
            };

            // Добавление нового пользователя
            await _context.AddAsync(newUser);

            try
            {
                // Сохранение изменений в базу данных
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(newUser);
        }

        #endregion

        #region Update-1

        /// <summary>
        /// Изменение личных данных пользователя, используя модель UserUpdate
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <param name="guid"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpPut("{login}/{password}/{guid}")]
        public async Task<IActionResult> UpdateUser(string login, string password, string guid, [FromForm] UserUpdate user)
        {
            // Аутентификация пользователя
            User currentUser = await SearchUser(login, password);

            // Поиск пользователя, чьи данные пытаемся изменить
            User updateUser = _context.Users.Find(guid);

            // Проверяет наличие обеих учетных записей
            if (currentUser == null || updateUser == null)
                return NotFound("Один из пользователей не найден");

            // Проверяет доступность основного пользователя
            //     для изменения данных выбранного пользователя
            if (!currentUser.Admin && updateUser.Guid != currentUser.Guid)
                return BadRequest("Вы не можете изменить чужую учетную запись");

            // Проверяет, активена ли учетная запись двух пользователей
            if (currentUser.RevokedOn != null || updateUser.RevokedOn != null)
                return BadRequest("Одна из учетных записей не активна");

            // Проверяет корректность имени
            if (!UserValidate.NameValidator(user.Name))
                return BadRequest("Для имени разрешены только латинские и русские буквы");

            // Обновляет поля пользователя
            updateUser.Name = user.Name;
            updateUser.Gender = user.Gender;
            updateUser.Birthday = user.Birthday;
            updateUser.ModifiedOn = DateTime.Now;
            updateUser.ModifiedBy = currentUser.Login;

            // Внесение изменений
            _context.Update(updateUser);

            try
            {
                // Сохранение изменений в базу данных
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(updateUser);
        }

        /// <summary>
        /// Изменение пароля пользователя
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <param name="guid"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        [HttpPut("{login}/{password}/{guid}/{newPassword}")]
        public async Task<IActionResult> UpdatePassword(string login, string password, string guid, string newPassword)
        {
            User currentUser = await SearchUser(login, password);

            User updateUser = _context.Users.Find(guid);

            if (currentUser == null || updateUser == null)
                return NotFound("Один из пользователей не найден");

            if (!currentUser.Admin && updateUser.Guid != currentUser.Guid)
                return BadRequest("Вы не можете изменить чужую учетную запись");

            if (currentUser.RevokedOn != null || updateUser.RevokedOn != null)
                return BadRequest("Одна из учетных записей не активна");

            // Проверяет корректность пароля
            if (!UserValidate.PasswordValidator(newPassword))
                return BadRequest("Для пароля разрешены только латинские буквы и цифры");

            // Обновляет пароль пользователя
            updateUser.Password = newPassword;
            updateUser.ModifiedOn = DateTime.Now;
            updateUser.ModifiedBy = currentUser.Login;

            // Внесение изменений
            _context.Update(updateUser);

            try
            {
                // Сохранение изменений в базу данных
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(updateUser);
        }


        /// <summary>
        /// Изменение логина пользователя
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <param name="guid"></param>
        /// <param name="newLogin"></param>
        /// <returns></returns>
        [HttpPut("{login}/{password}/{guid}/{newLogin}")]
        public async Task<IActionResult> UpdateLogin (string login, string password, string guid, string newLogin)
        {
            User currentUser = await SearchUser(login, password);

            User updateUser = await _context.Users.FindAsync(guid);

            if (currentUser == null || updateUser == null)
                return NotFound("Один из пользователей не найден");

            if (!currentUser.Admin && updateUser.Guid != currentUser.Guid)
                return BadRequest("Вы не можете изменить чужую учетную запись");

            if (currentUser.RevokedOn != null || updateUser.RevokedOn != null)
                return BadRequest("Одна из учетных записей не активна");

            // Проверяет уникальность и корректность нового логина
            var errors = UserValidate.LoginValidator(newLogin, await _context.Users.ToListAsync());
            if (errors != null)
                return BadRequest(errors);

            // Обновляет логин пользователя
            updateUser.Login = newLogin;
            updateUser.ModifiedOn = DateTime.Now;
            updateUser.ModifiedBy = currentUser.Login;

            // Внесение изменений
            _context.Update(updateUser);

            try
            {
                // Сохранение изменений в базу данных
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(updateUser);
        }

        #endregion

        #region Read

        /// <summary>
        /// Запрос списка всех активных пользователей,
        /// отсортированный по дате создания пользователя
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet("{login}/{password}")]
        public async Task<ActionResult<IEnumerable<User>>> GetActiveUsers(string login, string password)
        {
            // Проверяет наличие прав администратора
            string error = IsAdmin(await SearchUser(login, password));

            if (error != string.Empty)
                return BadRequest(error);

            // Запрос LINQ
            var users = await _context.Users
                .Where(u => u.RevokedOn == null)
                .OrderBy(u => u.CreatedOn)
                .ToListAsync();

            return users;
        }

        /// <summary>
        /// Запрос пользователя по логину, отправляющий модель UserRead
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <param name="searchLogin"></param>
        /// <returns></returns>
        [HttpGet("{login}/{password}/{searchLogin}")]
        public async Task<ActionResult<UserRead>> GetSearchLoginUser(string login, string password, string searchLogin)
        {
            // Проверяет наличие прав администратора
            string error = IsAdmin(await SearchUser(login, password));

            if (error != string.Empty)
                return BadRequest(error);

            // Поиск пользователя по логину
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == searchLogin);

            if (user == null)
                return NotFound("Искомый пользователь не найден");

            // Создание модели для отображения данных пользователя
            UserRead userRead = new UserRead { 
                Name = user.Name, 
                Gender = user.Gender switch
                {
                    0 => "Женщина",
                    1 => "Мужчина",
                    _ => "Неизвестно"
                },
                Birthday = user.Birthday, 
                IsActive = user.RevokedOn == null ? "Активен" : "Не активен"
            };

            return userRead;
        }

        /// <summary>
        /// Запрос пользователя по логину и паролю
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpGet("{login}/{password}")]
        public async Task<ActionResult<User>> GetUser(string login, string password)
        {
            var user = await SearchUser(login, password);

            if (user == null)
                return NotFound("Пользователь не найден");

            // Проверяет, активен ли учетная запись изменяемого пользователя
            if (user.RevokedOn != null)
                return BadRequest("Учетная запись не активена");

            return user; 
        }

        /// <summary>
        /// Запрос всех пользователей старше определенного возраста
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <param name="birthday"></param>
        /// <returns></returns>
        [HttpGet("{login}/{password}/{birthday}")]
        public async Task<ActionResult<IEnumerable<User>>> GetOlderUser(string login, string password, DateTime birthday)
        {
            // Проверяет наличие прав администратора
            string error = IsAdmin(await SearchUser(login, password));

            if (error != string.Empty)
                return BadRequest(error);

            // Запрос LINQ
            var users = await _context.Users
                .Where(u => u.Birthday <= birthday)
                .ToListAsync();

            return users;
        }

        #endregion

        #region Delete

        /// <summary>
        /// Удаление пользователя по логину полное или мягкое
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <param name="userLogin"></param>
        /// <param name="fullRemove"></param>
        /// <returns></returns>
        [HttpDelete("{login}/{password}/{userLogin}/{fullRemove}")]
        public async Task<IActionResult> DeleteUser(string login, string password, string userLogin, bool fullRemove)
        {
            // Аутентификация пользователя
            User admin = await SearchUser(login, password);

            // Проверяет наличие прав администратора
            string error = IsAdmin(admin);
            if (error != string.Empty)
                return BadRequest(error);

            // Поиск пользователя по логину
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == userLogin);

            if (user == null)
                return NotFound("Пользователь для удаления не найден");

            if (fullRemove)
            {
                // Полное удаление пользователя
                _context.Users.Remove(user);
            }
            else
            {
                // Мягкое удаление пользователя
                user.RevokedOn = DateTime.Now;
                user.RevokedBy = admin.Login;
                _context.Users.Update(user);
            }

            try
            {
                // Сохранение изменений в базе данных
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(user);
        }

        #endregion

        #region Update-2

        /// <summary>
        /// Восстановление пользователя
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpPut("{login}/{password}/{guid}")]
        public async Task<IActionResult> RecoveryUser(string login, string password, string guid)
        {
            User admin = await SearchUser(login, password);

            // Проверяет наличие прав администратора
            string error = IsAdmin(admin);
            if (error != string.Empty)
                return BadRequest(error);

            // Поиск пользователя по идентификатору
            var user = await _context.Users.FindAsync(guid);

            if (user == null)
                return NotFound("Пользователь для восстановления не найден");

            // Очищает поля, сообщающие о мягком удалении пользователя
            user.RevokedOn = null;
            user.RevokedBy = null;
            user.ModifiedOn = DateTime.Now;
            user.ModifiedBy = admin.Login;

            // Внесение изменений
            _context.Users.Update(user);

            try
            {
                // Сохранение изменений в базе данных
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(user);
        }

        #endregion

        /// <summary>
        /// Проверяет у пользователя наличие прав администратора и других факторов
        /// </summary>
        /// <param name="admin"></param>
        /// <returns></returns>
        private string IsAdmin(User admin)
        {
            if (admin == null)
                return "Пользователь не найден";

            if (!admin.Admin)
                return "Пользователь не обладает правами администратора";

            if (admin.RevokedOn != null)
                return "Учетная запись администратора не активна";

            return string.Empty;
        }

        /// <summary>
        /// Возвращает пользователя, найденного с помощью его логина и пароля
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private async Task<User> SearchUser(string login, string password) =>
            await _context.Users.FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
    }
}
