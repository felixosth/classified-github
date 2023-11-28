Utvecklare: Felix Östh - felix.osth@insupport.se

Installation:
	Lägg mappen med alla filer (TryggLarm) i SDK submappen för huvudmappen för Milestone installationen, standard är
	C:\Program Files\Milestone och då ska pluginet läggas på C:\Program Files\Milestone\MIPPlugins\TryggLarm
	Starta om alla Milestone applikationer som är öppna under installationen (inkl. Event Server).

	Om installationen gick som det skulle så finns det ett nytt objekt under MIP Plug-Ins i Management klienten

Konfiguration
	SMTP
		Fälten förklarar sig själva. Mail klienten använder sig av denna information för alla epost-meddelanden som skickas via plugin'et.

	SMS
		"Send as" = Sändaren av SMSet som kommer upp i telefonen
		"CustomerID" = CustomerID för tjänsten TeleOffice har visat
		"Unique GUID" = ^

		Var noga med att manuellt klicka Spara

	Larmgrupper
		Högerklicka och sen "Add new" för att lägga till en ny grupp. Välj ett namn, telfonnr för sms och epost för epostmeddelanden.
		Rutorna till höger om telefonnr och epost är inställningar om det ska skickas sms eller epost till dessa.

		Alarm definitions
			Skriv manuellt namnet på ett larm och tryck "Add alarm" för att lägga till larmet till objektet.

		Alarm timestamp offset
			Tidpunkten programmet letar efter en bild till att skicka i tex mail. Om ett larm triggrar på motion detection så kan man tex 
			bara se en fot på bilden. Om man ändrar offset på en sekund senare så kanske man ser hela människan om denne går förbi kameran.

		Mail formatting & SMS
			Formatering på mail och sms. Variabler inlagda är följande:

			%name% = Namnet på objektet
			%tel% = Telefonnr kopplat till objekt
			%email% = Epostadress kopplat till objekt
			%alarm% = Namn på larm som triggrats

		Attatch related camera image
			Programmet letar genom relaterade kameror (ställs in i larm dialogen) till larmet och tar inspelad bild närmst till tidpunkt av larm.
