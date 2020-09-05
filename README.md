# Проект Группы ПО-92б
## JPEG-READER CS

---
## Настройка git
Пример .git/config

[core]

        repositoryformatversion = 0

        filemode = true

        bare = false

        logallrefupdates = true

[remote "origin"]

        url = https://github.com/alex-chaplygin/jpeg-cs-92_1

        fetch = +refs/heads/*:refs/remotes/origin/*

[branch "master"]

        remote = origin

        merge = refs/heads/master

Пример ~/.gitconfig

[user]

        email = alex_chaplygin@mail.ru

        name = alex-chaplygin

[core]

        editor = vi

        autocrlf = input

## Порядок загрузки на удаленный репозиторий

git commit -a

git push origin master



**тестовый текст от Максима чтобы проверить работает ли**