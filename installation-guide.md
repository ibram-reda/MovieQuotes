## Linux
this steps works fine for ubuntu linux 24.04 first we need to prepare a database, and optain connection string  to it 

### 1. Database preperation
the follwoing line will install mysql in your system
```bash
sudo apt update
sudo apt upgrade
sudo apt install mysql-server
```
if every thing goes fine then the follwoing line will execute with no error .. if you get command not found then there is some problem in 
```
mysql -V
```

(optional) secure your sql installation by answering few questions
```bash
sudo mysql_secure_installation
```

now we need to set password for our root user, we will need this in our connection string get into your mysql as the root user:
```bash
mysql -u root
```
and Change the root password
```sql
ALTER USER 'root'@'localhost' IDENTIFIED BY 'NewPassword';
FLUSH PRIVILEGES;
```
Replace `NewPassword` with your desired password. after finsh type `Exit` to return

your connection string now is ready is like the follwoing 
```
Server=localhost;Database=MovieQuotesDb;uid=root;pwd=<your root password here>;
```
update your connection string in AppSetting in [api project][5] and in the [UI Project][4]

### 2. install .NET sdk
to [install .Net] in your system
```bash
sudo add-apt-repository ppa:dotnet/backports
```
```bash
sudo apt-get update && \
  sudo apt-get install -y dotnet-sdk-9.0
```
check if every thing is fine
```bash
dotnet --list-sdks
```

### 3. Manage User Secrets and Third-Party API Keys (Optional)

*MovieQuotes* integrates with [TMDB](https://www.themoviedb.org) to fetch movie metadata when creating a movie instance, including posters, backdrops, genres, and the TMDB ID. 

Because the TMDB API key has usage quotas and rate limits, it is not included in the repository. You can create your own API key from TMDB and store it locally using .NET User Secrets.

Navigate to the UI project:

```bash
cd ./src/MovieQuotes.UI
```

Add your TMDB API key:

```bash
dotnet user-secrets set "TmdbApiKey" "<your-api-key>"
```

You should also configure your database connection string the same way:

```bash
dotnet user-secrets set "ConnectionStrings:Default" "Server=myServer;Database=MovieQuotesDb;"
```

**Note** *MovieQuotes* will still work without a TMDB API key, but in that case you will need to provide all movie data manually instead of having it fetched automatically from TMDB.

### 4. install VLC and ffmpeg
this app 'MovieQuotes' use [Vlc] as vedio player
```bash
sudo apt install libvlc-dev
sudo apt install vlc
```
and use ffmpeg to cut and generate vedio clips
```bash 
sudo apt install ffmpeg
```

### 5. Create the database
this app use entity framework to work with database so we need to [install entity framework tools] 
```bash
dotnet tool install --global dotnet-ef
```
Navigate your Terminal to the location `./Src/MovieQuotes.Infrastructure` and update the database it will create the database and it's tables
```bash
cd ./src/MovieQuotes.Infrastructure/
dotnet ef database update -s ../MovieQuotes.Api/MovieQuotes.Api.csproj
```

### 6. Set the Cash Folder location
in [Domain Constants](./Src/MovieQuotes.Domain/Models/Constants.cs#L5) and [Application Constants](./Src/MovieQuotes.Application/Features/Constants.cs#L5) File change the `CashPath` Constant to a location on your system to generate short video clips on it. 

### 7. run the application
navigate your terminal to the uI project
```bash
cd ./src/MovieQuotes.UI
```
and Build and run the appliation
```
dotnet run
```

### 8. populate with data and movies
download your vedios from internet or from anywhere but we need 3 basic file ber each movie we need Photo called `cover.jpg` we need vedio `<yourmovieName>.mp4` and subtitle file `<yourMovieName>.en.srt` locate your movies in folder structure like the  following - you can have optional more file like `info.json` file that contains IMDBID and description of the movie

```
<root folder>
├─── Pretty woman (1995)
│       ├── pretty.woman.180p.mp4
│       ├── cover.jpg
│       └── subtitles
│           └── pretty.woman.en.srt
├─── The Physician (2013)
│       ├── The.Physician.2013.1080p.mkv
│       ├── info.json
│       ├── cover.jpg
│       └── subtitles
│           └── The.Physician.2013.en.srt
```

info.json content example
```json
﻿{
  "IMDBId": "tt2101473",
  "Title": "The Physician",
  "Year": 2013,
  "Description": "In 11th-century Persia, a surgeons apprentice disguises himself as a Jew to study at a school that does not admit Christians."
}
```
you can automaticlly get this data in the application in the home view click the button called `change folder` and navigate to your root folder and select it, the data will apperear in out of sync Quee, click the auto add it will automaticaly added to your database.



## Addtional information
if you like to work with other RDBMS rather than mysql just sutep it and get the connection strings add it update your connection string in AppSetting in [api project][5] and in the [UI Project][4]

get the correct [enity framework database providers] nuget package i use mysql 
```
MySql.EntityFrameworkCore
```
to work with sqlserver you need to install this package instead
```
Microsoft.EntityFrameworkCore.SqlServer
```

and then remove the [migration folder in Infrastructure](./Src/MovieQuotes.Infrastructure/Migrations/) projet and reginerate it with the new provider
```bash
cd ./Src/MovieQuotes.Infrastructure/
dotnet ef Migrations add IntialCreate -s ../MovieQuotes.Api/MovieQuotes.Api.csproj
```


### windows notes
on windows you don't need to install LibVLC just use the package  `VideoLAN.LibVLC.Windows` in Project MovieQoutes.UI
for ffmpeg on windows you can download the binaries in some location and add a path to it in the environment variables ... just make the command `ffmpeg` avalible in the cmd.


[install .Net]: https://learn.microsoft.com/en-us/dotnet/core/install/linux-ubuntu-install?tabs=dotnet9&pivots=os-linux-ubuntu-2404

[vlc]: https://code.videolan.org/videolan/LibVLCSharp/-/blob/3.x/docs/linux-setup.md

[install entity framework tools]: https://learn.microsoft.com/en-gb/ef/core/cli/dotnet#installing-the-tools

[enity framework database providers]: https://learn.microsoft.com/en-us/ef/core/what-is-new/nuget-packages#database-providers

[4]: https://github.com/ibram-reda/MovieQuotes/blob/c998380d594a228ee3aeaaeaae2c1e8d6ecedd36/Src/MovieQuotes.UI/Extensions/ServiceCollectionExtensions.cs#L34

[5]: https://github.com/ibram-reda/MovieQuotes/blob/c998380d594a228ee3aeaaeaae2c1e8d6ecedd36/Src/MovieQuotes.Api/appsettings.json#L11
