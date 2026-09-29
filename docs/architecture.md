# Architecture

MovieQuotes is a **.NET desktop application built with C# and Avalonia UI**.

The project follows a layered architecture designed to keep business rules independent from infrastructure and presentation concerns.

## Architecture Overview

```text
                         ┌─────────────────────┐
                         │         UI          │
                         │     Avalonia UI     │
                         └──────────┬──────────┘
                                    │
                                    ▼
                         ┌─────────────────────┐
                         │    Application      │
                         │   Use Cases / CQRS  │
                         │      MediatR        │
                         └──────────┬──────────┘
                                    │
                                    ▼
                         ┌─────────────────────┐
                         │       Domain        │
                         │ Business Rules &    │
                         │      Entities       │
                         └─────────────────────┘
                                    ▲
                                    │
                         ┌──────────┴──────────┐
                         │   Infrastructure    │
                         │ EF Core / MySQL /    │
                         │ External Services    │
                         └─────────────────────┘


                         ┌─────────────────────┐
                         │   SubtitleParser    │
                         │    SRT Parsing      │
                         └─────────────────────┘
```

The main application layers are:

- **Domain** — business concepts and rules
- **Application** — use cases and application logic
- **Infrastructure** — persistence and external services
- **UI** — Avalonia desktop interface
- **SubtitleParser** — subtitle parsing functionality isolated into its own project

# Domain Layer

The Domain layer contains the core concepts of MovieQuotes.

## Main Domain Concepts

### Movie
Represents a movie available to the application.
Movie-related information can include metadata obtained from TMDB as well as information about the local movie file.

### Study Material
`StudyMaterial` represents vocabulary or language content that the user wants to learn.

A study material can represent different types of language content, such as:

- Words
- Idioms
- Phrasal verbs
- Other expressions

Study material is not limited to a single occurrence in a movie.

A material can be associated with **multiple movie phrases**, allowing the same vocabulary item to be studied through different contexts.


### Card Progress

`CardProgress` tracks the scheduling state of an individual learning card.

It includes concepts Related to spaced repetaiton algorithm such as:

- Repetitions
- Interval
- Ease factor
- Review count
- Lapse count
- Last review
- Next review

The scheduling information is used by the spaced-repetition system.


# Application Layer

The Application layer contains the application's **use cases**.

It coordinates domain objects and infrastructure without containing UI-specific implementation details.

MovieQuotes uses **CQRS** with **MediatR** to organize application operations.

## Commands

Commands represent operations that change application state.

Examples include:

```text
Create Study Material
Edit Study Material
Delete Study Material
Add Study Material to Learning 
```

A command typically follows this flow:

```text
UI
 │
 ▼
Command
 │
 ▼
Command Handler
 │
 ▼
Domain / Infrastructure
 │
 ▼
Result
```

## Queries

Queries retrieve information without changing application state.

Examples include:

```text 
Get Study Material
Get Study Overview
Get Movies With Study Materials 
Search Movie Phrases
```

A typical query flow is:

```text
UI
 │
 ▼
Query
 │
 ▼
Query Handler
 │
 ▼
Repository / DbContext
 │
 ▼
Result
```

## Operation Results

Application operations use a result-oriented approach to communicate success and failure.

This allows handlers to return meaningful application errors instead of exposing infrastructure exceptions directly to the UI.

For example:

```text
OperationResult<T>
    ├── Success
    └── Error
          └── ErrorCode
```

The UI can then translate application errors into appropriate user-facing messages.

---

# Infrastructure Layer

## Database

MovieQuotes currently uses **MySQL** for persistence.

Entity Framework Core is used as the application's ORM.

The infrastructure layer contains the database context and persistence-related configuration.

```text
Application
     │
     ▼
Infrastructure
     │
     ▼
EF Core
     │
     ▼
MySQL
```

The Application and Domain layers should not need to know how MySQL stores the data.



# External Services


## TMDB

MovieQuotes integrates with **The Movie Database (TMDB)** to obtain movie metadata.

The integration can provide information such as:

- Movie title
- Poster
- Backdrop
- Genres
- TMDB ID


## AI
MovieQuotes optionally uses AI to help generate study-material information.

The current implementation uses a local **Ollama** service through the .NET AI ecosystem.

The AI layer can generate information for `StudyMaterial` such as:

```text
Translation
Definition
Examples
Synonyms
Pronunciation
Language Level
```

AI-generated information is treated as suggested learning content rather than replacing the user's ability to edit and review the material.


# UI Layer

The UI layer is implemented using **Avalonia UI**.

Its responsibility is to:

- Display application state
- Collect user input
- Execute application commands and queries
- Display validation and errors
- Provide navigation
- Manage presentation-specific state
 

## MVVM

MovieQuotes UI uses the **MVVM** pattern.

The general flow is:

```text
View
 │
 ▼
ViewModel
 │
 ▼
Application Command / Query
 │
 ▼
Domain / Infrastructure
```

ViewModels coordinate UI state with application use cases.

They should not contain database-specific logic or business rules.


# Learning System

The learning system is one of the central parts of MovieQuotes.

Its architecture separates **language content** from **learning context** and **learning progress**.

The conceptual relationship is:

```text
Movie
  │
  └── Movie Phrases
          │
          │
          ▼
    Study Material
          │
          ├── Definitions
          ├── Translations
          ├── Examples
          ├── Synonyms
          └── Pronunciation
          │
          ▼
      Study Cards
          │
          ▼
     Study Progress
          │
          ▼
   Spaced Repetition
```

This separation allows the same vocabulary item to be learned using different movie contexts.

For example:

```text
Study Material
      │
      ├── Movie Phrase A
      │      └── Movie 1
      │
      ├── Movie Phrase B
      │      └── Movie 2
      │
      └── Movie Phrase C
             └── Movie 3
```

The learner can therefore encounter the same expression in multiple real-world contexts.


# Active Recall

MovieQuotes uses active recall as the primary mechanism for practicing learned vocabulary.

Current study exercises include:

- **Recognition**
- **Context Recall**

The learning flow is approximately:

```text
Study Session
     │
     ▼
Present Question
     │
     ▼
User Recalls Answer
     │
     ▼
Evaluate Answer
     │
     ▼
Update Progress
     │
     ▼
Schedule Next Review
```

The exercise mechanism is intentionally separated from the vocabulary content so that additional exercise types can be introduced later.


# Spaced Repetition

MovieQuotes uses review scheduling to determine when learning material should be presented again.

The system tracks information such as:

```text
Repetitions
Review Count
Lapse Count
Interval
Ease Factor
Last Reviewed
Next Review
```

A review can update the scheduling state based on the learner's performance.

The goal is to present material again when additional retrieval practice is useful rather than treating every study item equally.



# SubtitleParser

Subtitle parsing is separated into its own project.

```text
SubtitleParser
      │
      └── SRT parsing
```

This separation keeps subtitle-specific parsing logic independent from the rest of the application.

The parser is responsible for converting subtitle files into structured subtitle entries that MovieQuotes can use to discover movie dialogue.

For example:

```text
SRT File
   │
   ▼
SubtitleParser
   │
   ▼
Subtitle Entries
   │
   ▼
Movie Phrases
```

Keeping this functionality separate also makes it easier to test and potentially support additional subtitle formats in the future.


# Dependency Direction

The most important architectural rule is that dependencies should point **toward the core of the application**.

```text
UI
 │
 ▼
Application
 │
 ▼
Domain

Infrastructure ───────► Application / Domain
```

The Domain layer should remain independent.

Infrastructure contains implementations of technical concerns, while Application contains the use cases that coordinate them.

This separation makes it possible to change technical details without rewriting the core business logic.

For example, changing the database technology should primarily affect Infrastructure rather than the Domain model.


# Testing

The architecture allows different concerns to be tested independently.

### Domain Tests

Test business rules without requiring:

- MySQL
- Avalonia
- TMDB
- VLC
- File system

### Application Tests

Test commands, queries, handlers, validation, and application behavior.

### SubtitleParser Tests

Test subtitle parsing independently.

### UI Tests

Test ViewModels and UI-specific behavior where required.

This separation helps keep tests fast and focused.


# Design Goals

The architecture of MovieQuotes is designed around several goals:

### Separation of Concerns

Each layer should have a clear responsibility.

### Testability

Business logic should be testable without requiring the complete application environment.

### Extensibility

The application should be able to add new:

- Study exercises
- Language features
- Subtitle formats
- AI providers
- External services
- Learning algorithms

without requiring major changes throughout the application.

### Maintainability

Infrastructure and UI details should not leak into the Domain model.

### Language Independence

Although MovieQuotes currently focuses on English, the learning model is being designed so that language-specific concepts can be extended to support additional languages in the future.

# Summary

MovieQuotes separates the application into focused components:

```text
┌───────────────────────────────────────────┐
│                    UI                     │
│              Avalonia / MVVM              │
└─────────────────────┬─────────────────────┘
                      │
┌─────────────────────▼─────────────────────┐
│               Application                 │
│       Use Cases / CQRS / MediatR          │
└─────────────────────┬─────────────────────┘
                      │
┌─────────────────────▼─────────────────────┐
│                  Domain                   │
│      Learning Model / Business Rules      │
└───────────────────────────────────────────┘

┌───────────────────────────────────────────┐
│              Infrastructure               │
│  MySQL / EF Core / TMDB / AI / File I/O   │
└───────────────────────────────────────────┘

┌───────────────────────────────────────────┐
│              SubtitleParser               │
│                 SRT Parsing               │
└───────────────────────────────────────────┘
```

The architecture reflects the core idea of MovieQuotes:

> **Movies provide the language. MovieQuotes turns that language into structured learning material, active-recall exercises, and long-term review.**