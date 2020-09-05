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

## Порядок загрузки на удаленный репозиторий

git add *

git commit

git push origin master
