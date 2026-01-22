# UOC Gamers — Documentation (English)

This repository corresponds to an **academic project** developed by the **UOC Gamers** team as part of the **DAM program** at the **Universitat Oberta de Catalunya (UOC)**.

The project includes a collection of **three 2D mini-games** and one **VR prototype**, each with independent mechanics but connected through an arcade and thematic approach.

---

## Table of Contents

- [Project overview](#project-overview)
- [Included games](#included-games)
  - [Castanyera (2D)](#castanyera-2d)
  - [Halloween Arkanoid (2D)](#halloween-arkanoid-2d)
  - [Luz Divina (2D Puzzle)](#luz-divina-2d-puzzle)
  - [Running of the Bulls (VR)](#running-of-the-bulls-vr)
- [Story mode and Free mode](#story-mode-and-free-mode)
- [Controls](#controls)
- [Project structure](#project-structure)
- [Build and execution](#build-and-execution)
- [Related repository: AR (Tio AR)](#related-repository-ar-tio-ar)
- [Technologies used](#technologies-used)

---

## Project overview

**UOC Gamers** is a project developed in **Unity 6** that contains several short mini-games designed for quick play sessions.

Main goals of the project:

- Design and implement multiple mini-games with different mechanics.
- Implement a navigation flow between scenes.
- Provide two different ways to play:
  - **Story Mode**
  - **Free Mode**
- Generate an Android executable (APK) and prepare the project for academic delivery.

---

## Included games

### Castanyera (2D)

A 2D reflex-based mini-game inspired by the traditional Castanyera celebration.

**Gameplay:**
- **Chestnuts** and **stones** fall from the top of the screen.
- The player controls an on-screen character (the Castanyera woman).
- The objective is to **collect chestnuts** while avoiding obstacles.

**Objective:**
- Score as many points as possible before the timer ends.
- Maintain survival/progress by avoiding negative impacts.

---

### Halloween Arkanoid (2D)

An **Arkanoid / Breakout** style mini-game with a Halloween theme.

**Gameplay:**
- The player controls a **broom**, acting as a paddle.
- Instead of classic blocks, the game features static **eye-shaped targets** (horror theme).

**Objective:**
- Break as many eyes as possible within the time limit.
- Prevent the pumpkin from falling below the paddle.

---

### Luz Divina (2D Puzzle)

A logic/puzzle mini-game based on a 3x3 candle grid.

**Gameplay:**
- The game contains **3 rows of 3 candles**.
- When you press a candle:
  - that candle toggles on/off
  - the adjacent candles also toggle

**Objective:**
- Turn off all candles within 60 seconds.
- The game includes 3 rounds; each completed round adds score.

---

### Running of the Bulls (VR)

A VR prototype inspired by **San Fermín** (bull running).

**Gameplay:**
- The player moves through an environment with **three lanes**.
- Incoming elements include:
  - **bulls**
  - **obstacles**
- Players dodge by tilting their head (VR sensors / gyroscope-based movement).

**Progressive difficulty:**
- As time passes:
  - spawn intervals decrease
  - object speed increases
  - the final seconds become extremely intense (“pressure” / “chase” effect)

---

## Story mode and Free mode

The project includes two different gameplay flows:

### Story Mode
- Players progress through the story in defined steps.
- After each mini-game, the system:
  - stores the score
  - accumulates the total score
  - advances to the next step
  - shows a story / text scene between games

### Free Mode
- Players can access the mini-games directly.
- After finishing a game:
  - results are shown
  - buttons appear to return to the menu or retry

---

## Controls

Controls may vary between games. Summary:

### Castanyera
- Character horizontal movement:
  - keyboard on PC / horizontal input
  - touch input on Android

### Halloween Arkanoid
- Broom (paddle) horizontal movement:
  - keyboard / horizontal input
  - touch to move the paddle

### Luz Divina
- Interaction via click/tap on candles.

### Running of the Bulls (VR)
- Dodge movement by head tilt.
- Lane-based movement system.
- Obstacles move toward the player.

---

## Project structure

General organization:

- **Scenes/**
  - main scenes (menu, story mode)
  - individual scenes for each game
- **Scripts/**
  - UI, navigation, and game logic scripts
- **Prefabs/**
  - reusable elements
- **Images / Sprites**
  - UI resources and sprites
- **Audio/**
  - music and sound effects
- **XR / VR**
  - XR Management configuration and VR assets

---

## Build and execution

For technical requirements and detailed installation/build steps:

- 🇬🇧 [**Setup & Requirements (English)**](SETUP.md)

If the repository includes a built APK, it can be found in:

- Repository **Releases** (APK)

---

## Related repository: AR (Tio AR)

This project is complemented with an Augmented Reality mini-game developed by the team:

- **Tio AR** (AR mini-game)
- delivered as a separate repository due to technical integration constraints

Related repository:
- [Companion Repository (Tio AR)](https://github.com/RaulEstevezA/tioar)

---

## Technologies used

- Unity 6
- C#
- TextMeshPro
- Android build (APK)
- 2D Physics
- XR Management (VR)

---

## Download APK

The latest APK is available here:

- [Releases](../../releases)

---

[**Main**](README.md)

[**Installation & Requirements (Spanish)**](SETUP.md)