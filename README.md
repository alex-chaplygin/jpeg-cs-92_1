# Проект Группы ПО-92б
## Библиотека JPEG-CS

Библиотека предназначена для сжатия и распаковки изображений в формате JPEG

## Класс JPEG-Cs

### Конструктор JPEG-Cs(Stream name)
Создает объект класса из потока name

### Структура Точка
struct Точка {
       byte r; // красный
       byte g; // зеленый
       byte b; // синий
}

### Метод Точка[,] Распаковать()
Распаковывает содержимое JPEG и возвращает изображение

### Метод void Сжать(Точка[,] изображение)
Сжимает изображение и записывает его в поток

### int Ширина()

Возвращает ширину изображения

### int Высота()

Возвращает высоту изображения

### void ЗадатьПараметры(int параметры)

Устанавливает параметры сжатия JPEG

### Параметры

enum Параметры {
     ВЫСОКОЕ_КАЧЕСТВО = 1 << 0,
     СРЕДНЕЕ_КАЧЕСТВО = 1 << 1,
     НИЗКОЕ_КАЧЕСТВО = 1 << 2,
     ЭНТРОПИЙНОЕ_КОДИРОВАНИЕ = 1 << 3, 
}

## Пример использования

// пример распаковки
JPEG-Cs j = new JPEG-Cs(File.Open("test.jpg"));
Точка[,] изображение = j.Распаковать();
изображение[0, 0].r = 100;

---

// пример сжатия
JPEG-Cs j = new JPEG-Cs(File.Open("new.jpg", FileMode.Create));

Точка[,] изображение = new Точка[8, 8];
// создать изображение
for (int i = 0; i < 8; i++)
    for (int j = 0; j < 8; j++) {
    	изображение[i, j].r = 128;
    	изображение[i, j].g = 255;
    	изображение[i, j].b = 128;
	}
j.ЗадатьПараметры(Параметры.СРЕДНЕЕ_КАЧЕСТВО);
j.ЗадатьПараметры(Параметры.ЭНТРОПИЙНОЕ_КОДИРОВАНИЕ);
j.Сжать(изображение);

---
## Настройка git

Для начала работы:

	git clone https://github.com/alex-chaplygin/jpeg-cs-92_1


Настройка удалённого репозитория

	git remote set-url origin https://github.com/alex-chaplygin/jpeg-cs-92_1




Чтобы проверить правильность параметров предыдущего пункта:

	git remote -v

Там должны быть строчки:

>origin  https://github.com/alex-chaplygin/jpeg-cs-92_1 (fetch)
>
>origin  https://github.com/alex-chaplygin/jpeg-cs-92_1 (push)
	



Настройка профиля:
	
	git config --global user.name "ВАШЕ ИМЯ НА GITHUB"
	git config --global user.email "ВАША ПОЧТА"
	
Настройка ядра git:
	
	git config --system core.autocrlf input
	git config core.repositoryformatversion 0
	git config core.filemode true
	git config core.bare false
	git config core.logallrefupdates true
	

***
## Порядок загрузки на удаленный репозиторий

git commit -a

git push origin master


