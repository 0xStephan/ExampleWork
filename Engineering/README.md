# Example of engineering application

## Python Controls
Industrial IoT device that communicates between the controls systems and the user interface systems.
API takes API calls from resident app (XamarinApp) and translates + communicates the requests via Modbus TCP to the controls network. The IoT acts as a secure middle man. Both networks are physically seperates as they are connected on seperate ethernet ports and also operate in different IP address ranges. This helps clamp down on re-entracy threats.

## XamarinApp (for Residents) 
Xamarin.Forms is a cross platform framework which allows you to build mobile applications for iOS, Android, etc. The core of the code is programmed in C#. It allows for shared librarires between multiple platforms. This was choosen as we had to implement custom functionality of forcing the user to use our own WiFi network + using our own self signed SSL certificates (the network is not connected to the internet for security reasons).
XamarinApp connects to a private network and allows the user to interact with the engineering controls system.
