# Getting Started

This guide explains how to set up MovieQuotes for local development.

## Prerequisites

Before running MovieQuotes, make sure you have the following installed:

- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
- [MySQL](https://www.mysql.com)
- [VLC](https://github.com/videolan/libvlcsharp)
- [FFmpeg](https://ffmpeg.org/)

(optionally)You will also need a [**TMDB API key**](https://www.themoviedb.org/settings/api) if you want to use MovieQuotes' movie metadata features.

(optionally) If you want to use the AI-assisted study material generation, you will also need a local AI runtime such as **Ollama**.


## 1. Clone the Repository

Clone the repository and move into the project directory:

```bash
git clone https://github.com/ibram-reda/MovieQuotes.git
cd MovieQuotes
```


## 2. Prepare the Database

MovieQuotes currently uses **MySQL** as its database.

Create a database for the application:

```sql
CREATE DATABASE MovieQuotesDb;
```

You can then configure MovieQuotes with the connection string for your database.

Example:

```text
Server=localhost;Port=3306;Database=MovieQuotesDb;User=root;Password=your_password;
```


## 3. Configure MovieQuotes

MovieQuotes requires some application-specific configuration before it can run.

The main configuration includes:

- Database connection string
- TMDB API key
- Movie library path
- Cache folder path
- Subtitle settings
- AI configuration

For detailed information about each setting, see the [Configuration Guide](configuration.md).

For local development, keep secrets such as API keys and database passwords outside the source code.



## 4. Prepare Your Movie Library

MovieQuotes works with your local movie files and their subtitle files.

Create a directory for your movie library and organize your movies according to the structure expected by the application.

For example:

```text
Movies/
├── Movie A/
│   ├── Movie A.mp4
│   ├── cover.jpg
│   └── subtitles
│       └── movie A.en.srt
│
├── Movie B/
│   ├── Movie B.mp4
│   ├── cover.jpg
│   └── subtitles
│       └── movie B.en.srt
│
└── Movie C/
│   ├── Movie C.mp4
│   ├── cover.jpg
│   └── subtitles
│       └── movie C.en.srt
```

The movie files provide the video context, while the subtitle files provide the dialogue that MovieQuotes uses to find and study phrases.


## 5. Install VLC and FFmpeg

MovieQuotes uses **VLC** for video playback and **FFmpeg** for video processing.

### Linux

On Ubuntu, you can install them with:

```bash
sudo apt update
sudo apt install vlc ffmpeg
```

Depending on your Linux distribution and the libraries required by your .NET environment, additional VLC development packages may be required.

### Windows

on windows you don't need to install LibVLC just use the package `VideoLAN.LibVLC.Windows` in Project MovieQoutes.UI for ffmpeg on windows you can download the binaries in some location and add a path to it in the environment variables ... just make the command `ffmpeg` avalible in the cmd.


## 6. Optional: Set Up AI Assistance

MovieQuotes can use AI to help enrich study materials with information such as:

- Translation
- Definition
- Examples
- Synonyms
- Pronunciation
- Language level
- Additional learning information

For local development, MovieQuotes can be configured to use **Ollama**.

Install Ollama and download the model configured for your application.

For example:

```bash
ollama pull qwen3.5:9b
```

Make sure Ollama is running before using AI-assisted features.

See the [Configuration Guide](configuration.md) for AI configuration.


## 7. Build the Project

Restore the .NET dependencies:

```bash
dotnet restore
```

Then build the solution:

```bash
dotnet build
```


## 8. Run MovieQuotes

Run the UI project:

```bash
dotnet run --project UI
```

If the project path or project name differs in your checkout, use the corresponding `.csproj` file.

---

## 9. Run the Tests

To run the test suite:

```bash
dotnet test
```

It is recommended to run the tests after making changes to the application.


## Troubleshooting

### MovieQuotes cannot connect to MySQL

Check that:

- MySQL is running.
- The database exists.
- The configured server and port are correct.
- The username and password are correct.
- The connection string points to the correct database.

### Movies do not play

Check that:

- VLC is installed correctly.
- The movie file can be played by VLC.
- The configured movie library path is correct.

### Video processing does not work

Check that FFmpeg is installed and available to MovieQuotes.

### AI suggestions do not work

Check that:

- Ollama is installed.
- Ollama is running.
- The configured model is available.
- MovieQuotes is configured to use the correct Ollama endpoint and model.


## Next Steps

After completing the setup, take a look at:

- [Configuration](configuration.md) — detailed application configuration
- [Architecture](architecture.md) — understand the project structure