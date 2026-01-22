# UOC Gamers — Documentación (Español)

Este repositorio corresponde a un **proyecto académico** desarrollado por el equipo **UOC Gamers** dentro del ciclo **DAM** de la **Universitat Oberta de Catalunya (UOC)**.

El proyecto incluye una colección de **tres mini-juegos 2D** y un **prototipo VR**, cada uno con mecánicas independientes, pero unidos por un enfoque arcade y temático.

---

## Índice

- [Resumen del proyecto](#resumen-del-proyecto)
- [Juegos incluidos](#juegos-incluidos)
  - [Castanyera (2D)](#castanyera-2d)
  - [Halloween Arkanoid (2D)](#halloween-arkanoid-2d)
  - [Luz Divina (2D Puzzle)](#luz-divina-2d-puzzle)
  - [Running of the Bulls (VR)](#running-of-the-bulls-vr)
- [Modo historia y modo libre](#modo-historia-y-modo-libre)
- [Controles](#controles)
- [Estructura del proyecto](#estructura-del-proyecto)
- [Compilación y ejecución](#compilación-y-ejecución)
- [Repositorio relacionado: AR (Tio AR)](#repositorio-relacionado-ar-tio-ar)
- [Tecnologías utilizadas](#tecnologías-utilizadas)

---

## Resumen del proyecto

**UOC Gamers** es un proyecto desarrollado en **Unity 6** que contiene varios mini-juegos cortos orientados a jugar en sesiones rápidas.

Objetivos principales del proyecto:

- Diseñar y programar varios mini-juegos con mecánicas distintas.
- Implementar un flujo de navegación entre escenas.
- Diferenciar dos formas de jugar:
  - **Modo Historia**
  - **Modo Libre**
- Generar un ejecutable en Android (APK) y preparar el proyecto para su entrega.

---

## Juegos incluidos

### Castanyera (2D)

Mini-juego 2D de reflejos inspirado en la tradición de la Castanyera.

**Mecánica:**
- Desde la parte superior caen **castañas** y **piedras**.
- El jugador controla un personaje en pantalla (una mujer).
- El objetivo es **recoger las castañas** y evitar obstáculos.

**Objetivo del juego:**
- Conseguir la máxima puntuación posible antes de que finalice el tiempo.
- Mantener la supervivencia / progreso evitando impactos negativos.

---

### Halloween Arkanoid (2D)

Mini-juego tipo **Arkanoid / Breakout** con estética de Halloween.

**Mecánica:**
- El jugador controla una **escoba**, que actúa como pala/paddle.
- En lugar de bloques, se rompen **ojos** estáticos (con temática terror).

**Objetivo del juego:**
- Romper el máximo número de ojos dentro del límite de tiempo.
- Evitar que la calabaza caiga por debajo de la pala.

---

### Luz Divina (2D Puzzle)

Mini-juego 2D de lógica/puzzle basado en un tablero de velas 3x3.

**Mecánica:**
- Hay **3 filas de 3 velas**.
- Al tocar una vela:
  - se enciende/apaga esa vela
  - y también se encienden/apagan las velas adyacentes

**Objetivo del juego:**
- Apagar todas las velas resolviendo el puzzle en 60 segundos. Hay 3 rondas, cada ronda completada suma puntos.

---

### Running of the Bulls (VR)

Prototipo en VR inspirado en **San Fermín** (encierros).

**Mecánica:**
- El jugador avanza por un entorno con **3 carriles**.
- Aparecen:
  - **toros**
  - **obstáculos**
- Se esquivan inclinando la cabeza (sensores de VR / giroscopio).

**Dificultad progresiva:**
- A medida que avanza el tiempo:
  - disminuye el intervalo de aparición (spawns)
  - aumenta la velocidad de objetos
  - la presión final se vuelve muy intensa (últimos segundos “acoso”)

---

## Modo historia y modo libre

El proyecto implementa dos flujos de juego diferentes:

### Modo historia
- El jugador avanza por una historia dividida en pasos.
- Tras cada mini-juego, el sistema:
  - registra la puntuación
  - acumula el total
  - avanza al siguiente paso
  - muestra una escena de historia / texto entre juegos

### Modo libre
- El jugador puede acceder directamente a los mini-juegos.
- Al finalizar un juego:
  - se muestra el resultado
  - se presentan botones para volver al menú o reintentar

---

## Controles

Los controles pueden variar entre juegos, pero se resumen así:

### Castanyera
- Movimiento lateral del personaje:
  - teclado en PC / input horizontal
  - touch / input táctil en Android

### Halloween Arkanoid
- Movimiento lateral de la escoba:
  - teclado o input horizontal
  - touch para mover la pala

### Luz Divina
- Interacción mediante toque/click sobre velas.

### Running of the Bulls (VR)
- Esquiva mediante inclinación del casco/cabeza.
- Movimiento guiado por carriles.
- Obstáculos en movimiento hacia el jugador.

---

## Estructura del proyecto

Organización general:

- **Scenes/**
  - escenas principales (menú, modo historia)
  - escenas individuales de cada juego
- **Scripts/**
  - scripts de UI, navegación y lógica de juego
- **Prefabs/**
  - elementos reutilizables
- **Images / Sprites**
  - recursos de UI y sprites
- **Audio/**
  - música y efectos
- **XR / VR**
  - configuraciones de XR Management y assets VR

---

## Compilación y ejecución

Para requisitos técnicos y pasos detallados de instalación/compilación:

- 🇪🇸 [Instalación y requisitos (Español)](INSTALACION.md)

Si el repositorio incluye APK compilada, se encuentra en:

- **Releases** del repositorio (APK)

---

## Repositorio relacionado: AR (Tio AR)

Este proyecto se complementa con un mini-juego en realidad aumentada desarrollado por el equipo:

- **Tio AR** (AR mini-game)
- repositorio independiente por limitaciones técnicas de integración

Repositorio relacionado:
- [Companion Repository (Tio AR)](https://github.com/RaulEstevezA/tioar)

---

## Tecnologías utilizadas

- Unity 6
- C#
- TextMeshPro
- Android build (APK)
- 2D Physics
- XR Management (VR)

---

## Descargar APK

La APK más reciente está disponible en:

- [Releases](../../releases)

---

[**Main**](README.md)

[**Instalación y requisitos (Español)**](SETUP.md)