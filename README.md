# Проект Группы ПО-92б
## JPEG-READER CS

---
## Настройка git

Для начала работы:

	git clone https://github.com/alex-chaplygin/jpeg-cs-92_1


Настройка удалённого репозитория

	git remote set-url origin https://github.com/alex-chaplygin/jpeg-cs-92_1


***
Чтобы проверить правильность параметров предыдущего пункта:

	git remote -v

Там должны быть строчки
	origin  https://github.com/alex-chaplygin/jpeg-cs-92_1 (fetch)
	origin  https://github.com/alex-chaplygin/jpeg-cs-92_1 (push)
***

Настройка профиля:
	
	git config --global user.name "**ВАШЕ ИМЯ НА GITHUB**"
	git config --global user.email "**ВАША ПОЧТА**"
	
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


