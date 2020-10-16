 # Проект Группы ПО-92б подгруппа 1
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

### private List<byte[,]> Разбиение(byte[,] матрица, int ширина, int высота)

Разбивает исходную матрицу на блоки заданного размера (ширина и высота) и возвращает список из этих блоков.

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
## Порядок работы

1. Смотри задание в разделе Issues

2. Синхронизировать удаленное хранилище и локальное

git pull origin master

3. Создаем ветку

git branch iss<номер>

git checkout iss<номер>

4. Работаем

5. Фиксация изменений

если добавились файлы, то делаем

git add <имя файла>

затем

git commit -am "Сообщение"

6. Загрузить ветку на удаленный репозиторий

git push origin iss<номер>

7. После завершения тестирования и слияния удалить ветку

git branch -d iss<номер>

git push origin --delete iss<номер>

---

## Представление матриц

Все матрицы имеют первое измерение по ширине и второе измерение - по высоте

matrix[x, y]

## Класс Channel

Представляет собой часть изображения, содержит один канал исходного изображения.

## Поля

### byte[,] matrix

Массив, который хранит данные канала

### int h

Фактор разбиения по ширине

### int v

Фактор разбиения по высоте

## Методы

### Channel(byte[,] матрица, int h, int v)

Создает канал на основе заданной матрицы, её фактора разбиения по ширине h и высоте v

### byte[,] GetMatrix()

Возвращает текущую матрицу

### int GetH()

Возвращает фактор разбиения по ширине h

### int GetV()

Возвращает фактор разбиения по высоте v

### void Sample(int Hmax, int Vmax)

Преобразует исходную матрицу в новую, изменяя ширину и высоту. Значения матрицы прореживаются.
Новая ширина матрицы равна текущая ширина умножить на h/Hmax.
Новая высота матрицы равна текущая высота умножить на v/Vmax.
Например, если h = 4, Hmax=4, то ширина не изменяется.
h = 2, Hmax = 4, ширина уменьшится в 2 раза.

### void Resample(int Hmax, int Vmax)

Преобразует исходную матрицу в новую, изменяя ширину и высоту. Значения матрицы линейно интерполируются.
Новая ширина матрицы равна текущая ширина умножить на Hmax/h.
Новая высота матрицы равна текущая высота умножить на Vmax/v.

## Класс DCT

Осуществляет DCT преобразование (прямое и обратное)

## Методы

### static short[,] СдвигУровней(byte[,] матрица)

Возвращает матрицу, каждый компонент которой приведен из диапазона 0..255 в диапазон -128..127	

### static byte[,] ОбратныйСдвигУровней(short[,] матрица)

Возвращает матрицу, каждый компонент которой приведен из диапазона -128..127 в диапазон 0..255	

### static short[,] FDCT(short[,] матрица)

Выполняет прямое DCT преобразование

### static short[,] IDCT(short[,] матрица)

Выполняет обратное DCT преобразование

## Класс Квантование

Реализует прямое и обратное квантование марицы

### static short[,] Квантование(short[,] матрицаКоэффициентов, short[,] матрицаКвантования)

Реализует квантование матрицы коэффициентов с помощью заданной матрицы квантования

### static short[,] ОбратноеКвантование(short[,] матрицаКоэффициентов, short[,] матрицаКвантования)

Реализует обратное квантование матрицы коэффициентов с помощью заданной матрицы квантования
