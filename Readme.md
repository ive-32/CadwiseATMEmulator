Эмулятор банкомата для Cadwise
==============================

c# 7
.Net Framework 4.7.3

Первое задание (обработчик текста) было сделано под .Net 6.0, это для разнообразия 
под .Net FrameWork 4.7

В описании вакансии указано WFP, значит пишем на WFP.
Это первый мой опыт с WFP, и тестовое задание может и стоило делать на том на чем умею: 
классическое WinForms или Unity/Mono.

Выбрал этот вариант, чтобы показать, что мотивирован изучать что-то
новое, что могу быстро вникать и начинать выдавать результат.

Эмулятор банкомата
------------------

- Основное окно
- Окно селектора банкнот 
- Окна с результатом операций 

Для реализации селектора валют сделаем UserControl, 
который позволит вводить с клавиатуры или кнопками количество банкнот
или сумму. 

Весь интерфейс разделим на три части - шапка, подвал и основное поле с контентом.

Основное окно: две кнопки "Внести деньги", "Получить деньги"

Окно внесения/получения денег - селектор валют, сумма и кнопка подтверждения. 

Кнопка отмены/возврата на главный экран - наверху в шапке справа.

Операции внесения и получения делаем асинхронными - в реальном банкомате
в процессе операции будем ждать ответа оборудования.

Пока идет процесс, отображаем окно ожидания.

Результат отображаем текстом и кнопкой "Готово", которая возвращает на главный экран.
В коде есть закомментированные участки в которых результат отображался в виде 
того же селектора валют: типа вот сейчас в купюроприемнике такие купюры - забирайте.
Показалось неочевидно - переделал на окно с текстом, эмулятор же.

Старый код сохранил в комментариях, потому что задание шлю в виде архива, 
обычно мусор в комментах не сохраняю, делаю ссылку на ветку в Git.

Типы банкнот описаны в [ATM.cs](https://github.com/ive-32/CadwiseATMEmulator/blob/f82973a89f5a43a8d74deeed55cbb8613e019a2d/CadwiseATMEmulator/ATMClasses/ATM.cs) 
в виде массива

    public static readonly int[] BanknotesTypes = { 10, 50, 100, 200, 500, 1000, 2000, 5000 };

Можно пересобрать под любые номиналы, расположение селектов изменится. 
Не стал делать внешним ресурсом, с возможностью пользователя задавать номиналы 
т.к. задание тестовое, оставил в технический долг. 

Состояние банкомата 
-------------------

В задании указано, что нужно отображать состояние банкомата 
для этого справа пририсуем панель в которую будем выводить список контейнеров 
с купюрами, количество купюр, номиналы и емкость контейнера.

Пририсовал. XAML рулит, интерфейс резиновый, удобно редактировать и дополнять.

Повешаем на класс банкомата event, который будет возникать во время операций получения/внесения денег.
По event обновляем отображение содержимого ящиков.

На команды добавим валидацию, чтобы не выдавать деньги если их нет, или не принимать, если контейнеры полные.
Также на селекторы купюр повесим ограничения по количеству при выдаче.
При внесении, также повесил ограничение, но закомментировал: 
эмулятор не распознает фальшивые, мятые и иностранные купюры, 
поэтому, чтобы тестировать возврат денег при внесении просто даем больше, чем может принять.     

Что можно улучшить/доделать
---------------------------

1. Улучшенную имитацию забора денег из купюроприемника.
Таймер ожидания, возврат купюр, которые забыли/не забрали по контейнерам
или в ящик для сбора нераспознанных лишних, если таковой в банкомате имеется.
2. Больше информации о наличии купюр в ящиках. Не стал - сомнительно с точки зрения безопсности
3. Добавить возможность ввести общую сумму при получении денег и распределение ее по купюрам
Не стал делать, чтобы уложиться в срок, но как сделать - понятно. 
TextBox с ограничениями в get/set - по аналогии с тем, что используется в селекторе купюр
Алгоритм распределения суммы по банкнотам следующий:


    1. Ищем самый большой номинал, которым можно выдавать сумму
    2. Набираем им максимально возможную сумму
    3. Если купюр данного номинала для набора суммы не хватает берем что есть.
    4. Для остатка повторяем с 1 шага
    5. Если по достижении самого мелкого номинала не хватило купюр до суммы 
        то корректируем возможную сумму.
    
    в шаг 2. добавиляем логику не только максимального возможного набора суммы 
    а еще и сбалансированного. Если остаток "сумма / номинал" < номинал * 0.5 
    То берем "сумма / номинал - 1" купюру.
    Тогда при запросе 10500 рублей, будет не две купюры по 5 тыс и 500 руб, 
    а одна на 5000 и 4500 мелкими
    
    в шаг 5 добавиляем логику: в случае неудачи повторить с шага 1 
    отключив сбаласированный подбор 

Что не сделал
-------------

- Можно выделить базовый класс окна и окна результатов наследовать, будет лучше DRY, не факт, что будет лучше читаемость, 
На проектах встречал подход, что лучше повторить небольшой участок кода, чем сломать прозрачность.  
- Нет стилей, руками задаю Margin, BackGroud etc...
- Интерфейсы выделил только под основные классы. Но с Rider или Visual Studio это быстро делается. 
В случае развития архитектуры можно безболезненно отрефакторить, запишем в технический долг.
- MVVM наверняка отвратная, но тут вечный холивар. 
Есть мнение (не мое), что идеально чистая модель будет стремиться к бесконечному количеству кода,
поэтому компромиссы всегда есть, считаю что для данной, локальной задачи приемлемо.
- Ну и в целом WFP часть конечно хромает.