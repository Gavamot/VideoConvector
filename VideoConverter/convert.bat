﻿chcp 1251
rem // Утилита переконвертирует видеофайлы в mp4 формат воспринимаемый браузерами
rem // Параметры :
rem //      1  -  каталог с видео файлами (обходит папки рекурсивно)
rem //      2  -  форматы файлов через запятую ( .mp4,.ogg)
rem //      3  -  -d если установлен удалить все файлы сконвертированные с ошибкой
rem // Пример использования
cd vc
VideoConverter.exe ../ .mp4 -d

