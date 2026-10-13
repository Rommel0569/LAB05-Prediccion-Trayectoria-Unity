# Tuto_DrawTrajectory - proyecto para Unity 6

Este proyecto usa la escena, scripts, sprites, prefabs y material fisico originales de herbou/Tuto_DrawTrajectory: https://github.com/herbou/Tuto_DrawTrajectory. La fuente original tiene licencia MIT (ver `licence`).

## Abrir en otra PC

1. Instalar Unity Hub y Unity Editor 6000.3.17f1.
2. Clonar este repositorio con `git clone https://github.com/Rommel0569/LAB05-Prediccion-Trayectoria-Unity.git`.
3. En Unity Hub, seleccionar **Add project from disk** y abrir la carpeta clonada.
4. Esperar la importacion y abrir `Assets/Scenes/level1.unity`. Unity agrega y guarda automaticamente tres objetos `Cub`, cuatro plataformas moradas y el Canvas `Marcador`. Se pueden seleccionar y mover antes de pulsar Play. Si la escena ya estaba abierta, salir de Play y usar el menu `Laboratorio > Preparar escena y puntuacion` una vez.
5. Para el ejercicio de Unity Learn, abrir `Assets/Scenes/CalculatingTrajectories.unity` y pulsar Play. Si no aparece, usar `Laboratorio > Crear demo Calculating Trajectories`.

El repositorio original publica solo `Assets`; este proyecto agrega `Packages` y `ProjectSettings` para abrirlo tras clonarlo. La carpeta `Library` se reconstruye al abrirlo.

## Controles

- Arrastrar la bola con el mouse y soltar para lanzar.
- La escena y los controles son los del tutorial original.
- Los cubos rojo, turquesa y amarillo valen 10, 20 y 30 puntos. El jabon vale 50 solo si el centro de la bola entra en su zona interior; tocarlo por fuera no puntua.
- Tras un acierto la bola se reinicia. Si se queda quieta, sale del area o pasan 12 segundos, se reinicia automaticamente. `R` devuelve la bola y el jabon a sus posiciones iniciales sin borrar el puntaje.

`Ball.cs`, `GameManager.cs`, `Trajectory.cs` y `Cup.cs` parten de los scripts originales. Los ejercicios adicionales (objetivo y puntuacion) se implementan en esos scripts y `ScoreTarget.cs`. La explicacion y el cuestionario estan en `INFORME_LAB05.md`.
