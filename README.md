[![Unity 6.3 LTS](https://img.shields.io/badge/Unity-6.3_LTS-000000?style=flat&logo=unity&logoColor=white)](https://unity.com/releases/unity-6)
[![URP 17+](https://img.shields.io/badge/URP-17+-7B2CBF?style=flat&logo=unity&logoColor=white)](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@17.0)
[![Platform](https://img.shields.io/badge/Platform-Android-009688?style=flat&logo=android&logoColor=white)](https://github.com/твой-username/твой-репо)
[![Made in](https://img.shields.io/badge/Made_in-6_hours-orange?style=flat)](https://github.com/твой-username/твой-репо)

# Playnera Dress Up – Макияж

Интерактивный **dress-up / make-up** для мобильных устройств:  
нанесение крема, теней, помады, румян, сброс губкой, листание книги с палитрами.  
Плавный тач-ввод, приятные анимации и звуки.  

Сделано **за 6 часов** чистого времени.

## Геймплей

<p align="center">
  <img src="https://github.com/user-attachments/assets/d70c7027-1a12-488f-ab79-2ebe054ba798" 
       width="48%" alt="Нанесение макияжа + сброс" />
  <img src="https://github.com/user-attachments/assets/ccff63e2-b2eb-450a-9e74-ad28ae9b7cd3" 
       width="48%" alt="Листание Toolbook" />
</p>

<p align="center">
  <b> Слева</b> — нанесение (крем, тени, помада, румяна) + сброс губкой<br>
  <b> Справа</b> — листание книги палитр + мгновенная смена оттенка/инструмента
</p>

**Ключевые технические моменты:**
- Плавные анимации и твины — **DOTween**
- Асинхронная логика без фризов — **UniTask**
- Точный тач-ввод (drag/tap) — **Input System (Enhanced Touch)**
- Чистые переходы между инструментами без артефактов

## Что сделано за 6 часов

- Полный тач-контрол: нанесение макияжа (крем, тени, помада, румяна), смешивание, сброс губкой
- Анимированная книга палитр с листанием стрелками и мгновенной сменой цвета
- Инструменты через интерфейсы `ITool` и `IColorableTool` — удобно добавлять новые
- Звуки при действиях + анимации появления/переходов UI
- Всё оптимизировано под мобильные устройства (Android) на Unity 6 с URP

## Стек

- **Unity 6.3 LTS** (URP 17+)
- **DOTween** — анимации
- **UniTask** — асинхронность
- **Unity Input System** (Enhanced Touch) — мобильный ввод

## Запуск

1. Откройте проект в **Unity Hub** (рекомендуется Unity 6.3+)
2. Загрузите стартовую сцену: `Assets/Scenes/SampleScene.unity`
3. При необходимости соберите под **Android** или **iOS**  
   (Player Settings → Switching Platform)

## Структура кода

Основная логика: `Assets/Core/Scripts/`

| Папка       | Назначение                                      |
|-------------|-------------------------------------------------|
| `Makeup/`   | Контроллер сцены, зоны лица, обработка ввода    |
| `Tools/`    | Инструменты + интерфейсы `ITool` / `IColorableTool` |
| `UI/`       | Кнопки цветов, книга палитр, анимации UI       |
| `Audio/`    | Звуки и события                                 |
| `Utils/`    | Расширения, хелперы (UniTask + DOTween)        |

## UML-диаграмма классов (упрощённая)

```mermaid
classDiagram
    direction TB
    class ITool {
        <<interface>>
        +Activate()
        +Deactivate()
        OnReady
    }
    class IColorableTool {
        <<interface>>
        +SetSprite()
        +SetPalette()
    }
    class MakeupController {
        +SelectTool(ITool)
        +ClearTool()
        +FaceZone
        +FaceVisuals
        +DragController
    }
    class DragController {
        +FaceZone
        +EnableDrag()
        +DisableDrag()
        OnDragStart / OnDrag / OnRelease / OnTap
    }
    class FaceZone {
        +Contains()
    }
    class FaceVisuals {
        +Apply()
        +ResetAll()
    }
    class BaseToolBehaviour {
        <<abstract>>
        #PickUp()
        #WaitForPlayerDrag()
        #ReturnTool()
    }
    class CreamTool
    class LipstickTool
    class EyeshadowTool
    class BlushTool
    class SpongeTool
    class ColorButton
    class ToolbookController
    class AppearanceAnimation
    class AudioManager
    class AudioEvents {
        <<static>>
    }

    ITool <|.. BaseToolBehaviour
    ITool <|.. SpongeTool
    IColorableTool <|.. LipstickTool
    IColorableTool <|.. EyeshadowTool
    IColorableTool <|.. BlushTool
    BaseToolBehaviour <|-- CreamTool
    BaseToolBehaviour <|-- LipstickTool
    BaseToolBehaviour <|-- EyeshadowTool
    BaseToolBehaviour <|-- BlushTool

    MakeupController o-- ITool : currentTool
    MakeupController --> DragController
    MakeupController --> FaceVisuals
    DragController --> FaceZone
    BaseToolBehaviour --> MakeupController
    ColorButton --> MakeupController
    ColorButton ..> ITool
    ColorButton ..> IColorableTool
    AudioManager ..> AudioEvents
```

> Если Mermaid на GitHub не отрисовывается, откройте `README` в браузере на github.com - диаграмма рендерится там.
---
*Тестовое задание, Playnera.*
