# PTZQueue-Web

Av InSupport Nätverksvideo AB för Ramundberget (2020).

## Installation
1. Kompilera och lägg binära filer på valfri plats.
2. Kör WebcamBridge.exe /install i cmd/powershell som admin.
3. Kontrollera att tjänsten WebcamBridge ligger med som tjänst i Windows.

 
## Anteckningar

- Åtkomst till kameran från webbserver och testmiljö
- Kamera nås externt från whitelistade IPs (Knowit och Insupport)


- .NET runtime v4.5
- Justerbar port för webserver i brygga

- Bildöverföring: 720p/1080p/4k fetch jpeg's

- Bild cache'as i bryggan och hämtas därifrån

### Webserver(Brygga) lyssnar på
- http://[bind]:[port]/image
- http://[bind]:[port]/ptz
- http://[bind]:[port]/getptz
- Queries forwardas till kamera

#### GET /image
Returnerar senaste bilden från minnescache.

#### GET /ptz
Skickar vidare query till kamera.
Exempel på queries:
1. zoom=[1-9999]
2. center=[x, -100 till 100],[y, -100 till 100]
3. continuouspantiltmove=[x],[y] (Borde ej användas pga känns laggigt och oresponsivt

#### GET /getptz
Returnerar en json-struktur med alla PTZ värden hos kameran. Denna information är ej i cache utan hämtas direkt från kameran vid anrop.
Det som är användbart här är egentligen bara zoom.

Exempel: `{"pan":"2.65","tilt":"1.29","zoom":"1","iris":"4999","focus":"9855","brightness":"4999","autofocus":"on","autoiris":"on"}`

## Konfigurationsfil till Windows Tjänst
*Format: nyckel = beskrivning (standardvärde)*
- ServerBind = Binder webserver till address (*)
- ServerPort = Port som webserver anvädnder
- CameraAddress = Address till kamera, kan vara http eller https
- CameraUsername = Användarnamn för inloggning i kameran (root)
- CameraPassword = Lösenord till kameran (pass)
- CameraResolution = Bildernas upplösning som läggs i cache (1920x1080)
- CameraImageFetchIntervalMs = Interval mellan hämtningar av bilder (100)
- TrustAllCertificates = Lita på ogiltiga certifikat, borde vara true om https används då kamerans cert är självsignerade (false)
- LaunchDebugger = `Debugger.Launch();`
