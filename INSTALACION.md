# Instalación y requisitos — UOC Gamers (Android APK)

Este documento explica cómo **instalar y ejecutar** el proyecto **UOC Gamers** en un dispositivo Android mediante APK.

El proyecto es un **trabajo académico del ciclo DAM en la UOC**, desarrollado por el equipo **UOC Gamers**.

---

## Índice

- [Requisitos](#requisitos)
- [Descarga de la APK](#descarga-de-la-apk)
- [Instalación en Android](#instalación-en-android)
- [Permisos y recomendaciones](#permisos-y-recomendaciones)
- [Notas sobre VR](#notas-sobre-vr)
- [Problemas frecuentes](#problemas-frecuentes)
- [Repositorio relacionado: AR (Tio AR)](#repositorio-relacionado-ar-tio-ar)

---

## Requisitos

### Dispositivo
- **Android** (recomendado Android 10+).
- Espacio libre recomendado: **300 MB** (depende del build final y assets).
- Conexión a internet: **no obligatoria** para jugar (solo para descargar la APK).

### Para jugar al modo VR
- El prototipo VR requiere un dispositivo compatible con el modo VR implementado en el proyecto.
- Se recomienda un teléfono/tablet con buen rendimiento (hardware reciente) para garantizar fluidez.

> Nota: si se ejecuta solo la parte 2D, no es necesario hardware específico.

---

## Descarga de la APK

La APK oficial se encuentra publicada en el repositorio:

- **Releases**: [Releases](../../releases)

Se recomienda descargar la versión más reciente (latest).

---

## Instalación en Android

### Paso 1 — Descargar la APK
1. Accede al apartado **Releases** del repositorio.
2. Descarga el archivo `.apk` en el dispositivo Android.

---

### Paso 2 — Permitir “orígenes desconocidos”
Android puede bloquear la instalación si la APK no proviene de Google Play.

1. Intenta abrir la APK descargada.
2. Android mostrará un aviso de seguridad.
3. Pulsa:
   - **Configuración**
   - **Permitir instalación de esta fuente**  
   (o “Permitir aplicaciones desconocidas”)

> En algunas versiones:
> Ajustes → Seguridad → Instalar apps desconocidas

---

### Paso 3 — Instalar
1. Abre el archivo `.apk`.
2. Pulsa **Instalar**.
3. Espera a que termine el proceso.

---

### Paso 4 — Ejecutar
1. Abre la app desde el icono en el menú del dispositivo.
2. Selecciona el modo de juego (historia o libre).
3. Inicia el mini-juego deseado.

---

## Permisos y recomendaciones

- Si Android solicita permisos (por ejemplo almacenamiento), concédelos solo si es necesario.
- Se recomienda:
  - cerrar otras apps pesadas
  - bajar brillo si el dispositivo se calienta
  - activar modo rendimiento si existe (según el modelo)

---

## Notas sobre VR

El repositorio incluye un **prototipo VR** (“Running of the Bulls”), inspirado en San Fermín.

Recomendaciones:
- Asegúrate de tener buena iluminación y espacio.
- Evita movimientos bruscos.
- Mantén el dispositivo bien sujeto / ajustado en su soporte VR.
- Si notas mareo:
  - pausa inmediatamente
  - descansa unos minutos

---

## Problemas frecuentes

### 1) La APK no se instala
- Verifica que has permitido **apps desconocidas**.
- Comprueba que la descarga está completa.
- Reinicia el dispositivo e inténtalo de nuevo.

---

### 2) La app se sale o se queda en negro
- Puede ser falta de recursos o incompatibilidad puntual.
- Solución recomendada:
  - cerrar apps en segundo plano
  - reiniciar el dispositivo
  - abrir de nuevo

---

### 3) Controles táctiles no responden
- Asegúrate de que el dispositivo no tiene un modo de “gestos” que interfiera.
- Reintenta dentro del menú o reinicia el juego.

---

### 4) La VR va con lag / tirones
- Es normal en hardware limitado.
- Recomendaciones:
  - reducir apps abiertas
  - probar en un dispositivo más potente
  - evitar ejecución prolongada (por temperatura)

---

## Repositorio relacionado: AR (Tio AR)

Este proyecto se complementa con un mini-juego en **Realidad Aumentada (AR)** que se entrega como repositorio independiente por limitaciones técnicas de integración.

- Repositorio AR: [Tio AR](https://github.com/RaulEstevezA/tioar)

---

[**Volver al README principal**](README.md)