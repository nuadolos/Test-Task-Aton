# TestC
Web API сервис на .NET 6, реализующий API методы CRUD над сущностью User и доступный через интерфейс Swagger.
## Установка
### Without git (Windows, CMD)
1. Откройте командную строку **(CMD.exe)**;
2. Перейдите в папку, в которую хотите скачать программу *(переход - cd ([подробнее](https://docs.microsoft.com/ru-ru/windows-server/administration/windows-commands/cd)))*;
3. Скопируйте код, приведенный ниже и вставьте его в командную строку:
```
powershell -command "(New-Object Net.WebClient).DownloadFile('https://github.com//nuadolos/TestTask/archive/refs/heads/master.zip', 'TestC.zip')" 
powershell -command "Expand-Archive -Force 'TestC.zip' 'TestC'"
powershell -command "Remove-Item 'TestC.zip'"
set testСOpen=%cd%\TestC\TestTask-master\TestC\bin\Debug\net6.0\TestC.exe
echo Для старта программы напишите в строку ^%testOpen^% и нажмите Enter.
```
4. Для запуска программы в текущей сессии напишите в терминале:
```
%testСOpen%
```
5. После запуска пройдите по данной [ссылке](http://localhost:5000/swagger/index.html).

Будьте внимательны, **после того, как вы закроете текущее окно командной строки, вы не сможете воспользоваться этой командой.**</br>
Для того, **чтобы запускать программу без указания пути**, 
**добавьте путь**, по которому расположена данная программа, **в переменную окружения Path**.
Подробнее в [официальной документации Microsoft](https://docs.microsoft.com/ru-ru/previous-versions/office/developer/sharepoint-2010/ee537574(v=office.14)).
## Предупреждение
### Наличие SSMS
Необходимо установить *систему управления базами данных Microsoft SQL Server* перед установкой самой программы.
