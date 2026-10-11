# Prediccion de trayectoria - Laboratorio 05

Proyecto Unity 2D autocontenido para el laboratorio de prediccion de trayectoria. Incluye lanzamiento por arrastre, vista previa parabolica, rebotes, taza, objetivo adicional y contador de puntos.

## Abrir en otra PC

1. Instalar Unity Hub y Unity Editor 2022.3 LTS (idealmente 2022.3.62f1).
2. Clonar este repositorio con `git clone <URL-del-repositorio>`.
3. En Unity Hub, seleccionar **Add project from disk** y abrir la carpeta clonada.
4. Esperar la importacion. La primera vez se genera `Assets/Scenes/Main.unity` automaticamente. Abrir esa escena y pulsar Play.

El proyecto no usa recursos externos. Los elementos visuales se generan en tiempo de ejecucion, por lo que basta con clonar los archivos versionados. La carpeta `Library` se reconstruye al abrirlo.

## Controles

- Arrastrar la bola con el mouse y soltar para lanzar.
- Pulsar `R` para reiniciar la bola.
- Cada impacto en el cubo o entrada a la taza suma un punto.

## Explicacion breve

`TrajectoryGame.Start` prepara la escena y sus componentes fisicos. `Update` lee el arrastre y aplica la velocidad inicial al `Rigidbody2D`. `DrawTrajectory` estima cada punto con `p(t) = p0 + v0*t + 0.5*g*t^2`. `ScoreTarget.OnTriggerEnter2D` detecta el impacto y `AwardPoint` incrementa la puntuacion. Unity aporta `Rigidbody2D`, colliders, `PhysicsMaterial2D`, gravedad y deteccion de triggers para esta implementacion.

Referencia del enunciado: https://github.com/herbou/Tuto_DrawTrajectory (MIT). Este proyecto contiene codigo original y no copia sus assets.
