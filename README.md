# AlicatConfigurator

This tool allows you to communicate with Alicat devices via serial port.\
You can read and set values.

Supported devices are Alicat [Mass Flow Controllers](https://www.alicat.com/product/gas-mass-flow-controllers/) and [Pressure Controllers](https://www.alicat.com/product/pressure-controllers-for-flowing-processes/).

.NET Framework 4.5.2 is required.

## Alicat Configuration

1. Set SETPT to "Serial / Front Panel"
   - MENU -> CONTROL -> SETPT SOURCE
2. Set UNIT ID to A (or other letter)
   - MENU -> ADV SETUP -> COMM SETUP -> UNIT ID

## Usage

1. Connect the Alicat to your computer
2. Run the application

![alicat_disconnected](https://user-images.githubusercontent.com/24697440/51985306-da278800-249d-11e9-8b09-96e15c073f02.PNG)

3. Select Device, Port, Baud Rate, ID and Delay (seconds between refreshes)
4. Click the Connect button

![alicat_connected](https://user-images.githubusercontent.com/24697440/51985272-cc720280-249d-11e9-8af3-60c1890b7145.PNG)

5. Current values are in the left border
6. You can set values in the right border