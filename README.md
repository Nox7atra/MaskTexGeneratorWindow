# MaskTex Generator Window

Простой ассет генератор текстуры для https://docs.unity3d.com/Packages/com.unity.render-pipelines.high-definition@7.1/manual/Mask-Map-and-Detail-Map.html в Unity.

Чтобы воспользоваться нужно открыть окно в верхней панели по _MaskTexGenerator->Show_

Генератор имеет два режима:

_Label_ - В нём выбирается папка в которой лежат текстуры (текстуры для одной модели) и пишется, какие подстроки содержаться в этих текстурах, чтобы не кидать их по одной. Удобно для стандартизированного, надо доделать рекурсивный режим

_Object_ - текстуры кидаются в соответствующие поля для генерации MaskTex текстуры

_Переменная isRoughness_ - инвертирует Roughness карту, так как Unity использует Smoothness карту для PBR материалов

Сгенерированная текстура кладётся в папку с первой (снизу вверх) назначенной текстурой.

Пока не поддерживаются: разные разрешения текстур и пакетная обработка
