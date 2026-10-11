# Informe: Laboratorio 05 - Prediccion de trayectoria

## Objetivos y base

Se partio de la escena `Assets/Scenes/level1.unity` y los recursos del repositorio `herbou/Tuto_DrawTrajectory`. La escena incluye bola, taza, muros, puntos de trayectoria, `Rigidbody2D`, colliders y material fisico de rebote. Se agrego un segundo objetivo visible llamado `Cub - objetivo adicional` y un contador de puntos.

## Ejercicio 1: estructura de clases

- `Ball.Awake`: obtiene `Rigidbody2D` y `CircleCollider2D`.
- `Ball.Push`: aplica un impulso al cuerpo fisico.
- `Ball.ActivateRb` y `DesactivateRb`: activan o detienen la simulacion durante el arrastre.
- `GameManager.Awake`: registra la instancia compartida.
- `GameManager.Start`: obtiene la camara, guarda la posicion inicial, detiene la bola y actualiza el marcador.
- `GameManager.Update`: detecta presionar, arrastrar y soltar el mouse.
- `OnDragStart`, `OnDrag`, `OnDragEnd`: calculan la fuerza, muestran la prediccion y lanzan la bola.
- `Trajectory.PrepareDots`: crea los puntos visibles de la curva.
- `Trajectory.UpdateDots`: calcula sus posiciones futuras.
- `Trajectory.Show` y `Hide`: muestran u ocultan la vista previa.
- `ScoreTarget.OnTriggerEnter2D`: suma un punto cuando la bola entra al objetivo adicional.
- `Cup.OnCollisionEnter2D`: suma un punto cuando la bola golpea la taza, con una pausa corta para evitar contar varias veces un mismo contacto.

## Ejercicios 2 y 3: objetivo y puntuacion

`ConfigureLabScene` agrega al archivo de escena un sprite cuadrado rojo con `BoxCollider2D` de tipo trigger y un Canvas con texto de puntuacion. Ambos objetos son editables en el Inspector antes de Play. `ScoreTarget` detecta la entrada de la bola. `GameManager.AddPoint` incrementa el marcador. La taza tambien aporta puntos.

Cada disparo solo puede sumar una vez. Tras un acierto, la bola regresa a la posicion inicial despues de medio segundo. Tambien se reinicia si queda casi inmovil durante 1.5 segundos, cae fuera del area o pasan 12 segundos. La tecla `R` permite reiniciarla en cualquier momento sin borrar la puntuacion.

## Ejercicio 4: Calculating Trajectories

Se aplico la ecuacion de movimiento con aceleracion constante descrita en Unity Learn: `p(t) = p0 + v0*t + 0.5*g*t^2`. El impulso `J` produce velocidad inicial `v0 = J / m`, donde `m` es la masa del `Rigidbody2D`. En `Trajectory.UpdateDots` cada punto usa esa velocidad inicial y `Physics2D.gravity`. Esto predice la trayectoria libre previa al primer rebote o choque; despues de una colision, la trayectoria real cambia por los colliders y el material de rebote.

Referencia: https://learn.unity.com/course/the-physics-of-ai (seccion Calculating Trajectories).

## Cuestionario

1. **Como funciona la prediccion?** Parte de la posicion inicial, calcula la velocidad inicial del impulso y evalua la ecuacion del movimiento para varios tiempos futuros. Los puntos obtenidos dibujan una parabola bajo gravedad constante. La prediccion no incluye rebotes futuros si no se simulan las colisiones.
2. **Que aporta Unity?** `Rigidbody2D` aplica fuerzas, masa y gravedad; los colliders 2D detectan superficies y objetivos; `PhysicsMaterial2D` controla friccion y rebote; `Physics2D.gravity` proporciona la aceleracion; los sprites y GameObjects permiten representar la bola, la taza y los puntos.

## Estado de comprobacion

La escena original fue creada con Unity 5.6 y se importa en Unity 6000.3.17f1. El marcador se muestra como `SCORE: 0` en texto negro sobre el fondo blanco. Se requiere comprobar en Play el lanzamiento, el acierto y el reinicio en el editor instalado.

## Capturas para entregar

Inserte cada captura justo debajo del apartado indicado. Antes de capturar, cierre la consola de errores y procure que el objeto o componente relevante sea legible.

1. **Despues de “Objetivos y base”**: vista de la escena en modo edicion y jerarquia con `Ball`, `Cup`, `Cub - objetivo adicional`, `GameManager` y `Marcador`. Pie: *Figura 1. Objetos de la escena de prediccion de trayectoria.*
2. **Despues de “Ejercicio 1: estructura de clases”**: Inspector de `Ball` mostrando `Rigidbody2D`, `CircleCollider2D` y el material de rebote. Pie: *Figura 2. Componentes fisicos de la bola.*
3. **Despues de “Ejercicios 2 y 3”**: Inspector del `Cub - objetivo adicional` mostrando `SpriteRenderer`, `BoxCollider2D` como trigger y `ScoreTarget`. Pie: *Figura 3. Objetivo adicional configurable en la escena.*
4. **Despues de “Ejercicios 2 y 3”**: ventana Game tras un acierto con `SCORE: 1` visible. Pie: *Figura 4. Incremento de puntuacion al acertar.*
5. **Despues de “Ejercicio 4”**: ventana Game durante el arrastre, con los puntos de la trayectoria visibles. Pie: *Figura 5. Prediccion parabolica antes del lanzamiento.*
6. **Despues de “Ejercicios 2 y 3”**: ventana Game con la bola devuelta al inicio tras fallar o pulsar `R`, conservando el puntaje. Pie: *Figura 6. Reinicio de la bola sin borrar la puntuacion.*
