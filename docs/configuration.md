# Configuration

MovieQuotes uses application settings to configure the database, movie library, cache, external services, and application behavior.

> **Important:** API keys, passwords, and connection strings are sensitive information. Do not commit them to source control.

## Configuration Overview

MovieQuotes configuration can be divided into two categories:

- **Application settings** — preferences and paths used by the application.
- **Service configuration** — credentials and connection information required by external services.

The main configuration areas are:

| Setting | Purpose |
|---|---|
| Database Connection String | Connects MovieQuotes to MySQL |
| TMDB API Key | Retrieves movie metadata |
| Movies Library Path | Location of the local movie library |
| Cache Folder Path | Location for generated and cached files |
| Subtitle Font Size | Controls subtitle display size |
| Dark Mode | Controls the application's appearance |
| AI Configuration | Configures optional AI-assisted study material generation |


## Database

MovieQuotes currently uses **MySQL** as its database.

A typical connection string looks like:

```text
Server=localhost;Port=3306;Database=MovieQuotesDb;User=root;Password=your_password;
```


## TMDB

MovieQuotes uses **The Movie Database (TMDB)** to retrieve movie information such as:

- Movie titles
- Posters
- Backdrops
- Genres
- TMDB IDs

A **TMDB API key** is required for these features.

Configure the API key in the application's configuration/settings.

```text
TMDB API Key: your_api_key
```

Keep the API key private and do not commit it to the repository.


## Movie Library

MovieQuotes needs access to the directory containing your movie files.

Configure the **Movies Library Path** to point to the root directory of your movie collection.

Example:

```text
/home/user/Movies
```

or on Windows:

```text
D:\Movies
```

MovieQuotes uses this location to discover and access movie files.

> Make sure the application has permission to read the configured directory.

---

## Cache Folder

MovieQuotes uses a cache directory for generated or temporary files.

Configure the **Cache Folder Path** to a location where MovieQuotes can create and modify files.

Example:

```text
/home/user/.cache/moviequotes
```

or:

```text
D:\MovieQuotes\Cache
```

The cache will contain generated video clips and other temporary application data.


## Subtitle Settings

MovieQuotes provides settings that control how subtitles are displayed during playback.

### Subtitle Font Size

The **Subtitle Font Size** setting controls the size of subtitles displayed by the application.

Choose a value that is comfortable to read on your screen.


## Appearance

### Dark Mode

MovieQuotes supports a dark-mode setting that controls the application's visual theme.

The setting can be changed from the application's settings page.


## AI Assistance

MovieQuotes can optionally use a local AI model to help generate information for study materials.

AI assistance can be used to suggest information such as:

- Translations
- Definitions
- Examples
- Synonyms
- Pronunciation
- Language level
- Other vocabulary-related information

The AI functionality is optional. MovieQuotes can be used without it.

### Ollama

The current AI integration uses **Ollama** as a local AI provider.

The Ollama service is expected to be available at:

```text
http://localhost:11434
```

The model used by the current implementation can be configured according to the application's AI configuration.

For example:

```text
Model: qwen3.5:9b
Endpoint: http://localhost:11434
```

Make sure the selected model has been downloaded and that Ollama is running before using AI-assisted study material generation.

---

## Settings Page

MovieQuotes provides a Settings page where user-configurable application settings can be managed.

Current settings include:

```text
Movies Library Path
Cache Folder Path
Subtitle Font Size
Dark Mode
```

Service credentials such as API keys and database connection strings should be treated separately from ordinary user preferences and should not be exposed unnecessarily.

---

## Development Configuration

When developing MovieQuotes locally, keep machine-specific configuration outside the repository whenever possible.

A typical development environment requires:

```text
MySQL
    └── MovieQuotesDb

TMDB
    └── API Key

Movie Library
    └── Local movie files

Cache
    └── Local cache directory

Optional AI
    └── Ollama
        └── Local model
```

---

## Keeping Secrets Safe

The following values should **never be committed to Git** when they contain real credentials:

- Database passwords
- Database connection strings containing passwords
- TMDB API keys
- AI provider API keys
- Other service credentials
