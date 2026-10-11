# Tuto_DrawTrajectory - proyecto para Unity 6

Este proyecto usa la escena, scripts, sprites, prefabs y material fisico originales de herbou/Tuto_DrawTrajectory: https://github.com/herbou/Tuto_DrawTrajectory. La fuente original tiene licencia MIT (ver `licence`).

## Abrir en otra PC

1. Instalar Unity Hub y Unity Editor 6000.3.17f1.
2. Clonar este repositorio con `git clone <URL-del-repositorio>`.
3. En Unity Hub, seleccionar **Add project from disk** y abrir la carpeta clonada.
4. Esperar la importacion, abrir `Assets/Scenes/level1.unity` y pulsar Play.

El repositorio original publica solo `Assets`; este proyecto agrega `Packages` y `ProjectSettings` para abrirlo tras clonarlo. La carpeta `Library` se reconstruye al abrirlo.

## Controles

- Arrastrar la bola con el mouse y soltar para lanzar.
- La escena y los controles son los del tutorial original.

`Ball.cs`, `GameManager.cs`, `Trajectory.cs` y `Cup.cs` parten de los scripts originales. Los ejercicios adicionales (objetivo y puntuacion) se implementan en esos scripts y `ScoreTarget.cs`. La explicacion y el cuestionario estan en `INFORME_LAB05.md`.
