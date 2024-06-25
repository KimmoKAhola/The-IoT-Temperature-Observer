# The IoT Temperature Observer 
Author: Kimmo Ahola (ka223pd)

1. [Introduction](#introduction)
2. [Objective](#objective)
3. [Materials](#materials-needed)
4. [Setup](#setup)
   1. [VS Code](#vs-code-instructions)
   2. [Thonny](#thonny-instructions)
   3. [Telegram bot](#how-to-setup-your-telegram-bot)
   4. [.NET API](#tools-needed-to-implement-the-net-api)
6. [Circuit Diagram](#circuit-diagram)
7. [Code Snippets](#code-snippets)
8. [Connectivity](#connectivity)
9. [Data Visualization](#data-visualizationpresentation)
    1. [Ubidots](#ubidots-dashboard)
    2. [Swagger API](#swagger-api)
    3. [Azure](#azure-sql-server)
10. [Try it!](#try-it)
11. [Final thoughts](#final-thoughts)

- [x] Title
- [x] Your name and student credentials (ka223pd)
- [ ] Short project overview
- [x] How much time it might take to do (approximation)

# Introduction

This repository serves as the code base for an IoT project for the summer course 24ST - 1DT305 at [LNU](https://lnu-ftk.instructure.com/courses/402).
The purpose of this project has been to create a simple endpoint that I can place somewhere and have it collect data. Users can interact with this controller by using a telegram bot and writing certain commands to it.

The future goals of this project is to build my own database and api services by combining the microcontrollers with .net APIs (I study to become a .NET Developer) and as such this codebase contains free to use code for .NET services as well. This will later be used to implement my own smart home (I hope).

Since this course is a beginner course aimed to teach microcontrollers/IoT and python the focus of this report will be on those parts. Links to tools used for the .NET code will be provided and simple python snippets on how to send data to these services will also be shown. The C# code and database design will however not be covered at all but feel free to contact me in swedish or english if you want more information.

The telegram bot code will also be kept beginner friendly and utilize HTTP requests to fetch/send data. This is not a scalable solution but will work fine for personal use.

Time required to implement this project yourself: 6-10 hours depending on experience (for the python part).

# Objective

Describe why you have chosen to build this specific device. What purpose does it serve? What do you want to do with the data, and what new insights do you think it will give?


- [ ] Why you chose the project
- [ ] What purpose does it serve
- [ ] What insights you think it will give

# Materials needed

- [x] List of material
- [x] What the different things (sensors, wires, controllers) do - short specifications
- [x] Where you bought them and how much they cost
- [ ] Add sum of all material
      
| Product | Quantity | Link  | Price (SEK) | Description |
| :---         |     ---:       |          :--- | ---: | :--- |
| Raspberry Pi Pico WH   | 1     | [electrokit](https://www.electrokit.com/raspberry-pi-pico-wh)    | 109 | The brain |
| USB-kabel A-hane - micro B hane 1.8m | 1 | [electrokit](https://www.electrokit.com/usb-kabel-a-hane-micro-b-5p-hane-1.8m) | 39 | Connect the Pico to a computer and power it |
| Kopplingsdäck 840 anslutningar      | 1       | [electrokit](https://www.electrokit.com/kopplingsdack-840-anslutningar)      | 69 | A board to connect sensors and the microcontroller |
| Labbsladd 20-pin 15cm hane/hane  | 1 | [electrokit](https://www.electrokit.com/labbsladd-20-pin-15cm-hane/hane) | 29 | Connect the sensors to the controller |
| Digital temperatur- och fuktsensor DHT11 | 1 | [electrokit](https://www.electrokit.com/digital-temperatur-och-fuktsensor-dht11) | 49 | Measure temperature and humidity (Accuracy: -1) |
| MCP9700 TO-92 Temperaturgivare | 1 | [electrokit](https://www.electrokit.com/mcp9700-to-92-temperaturgivare) | 12 | Measure temperature (Accuracy: -1) |
| Total price | | | -2000 |

# Setup

To connect your raspberry pi pico wh to your computer and upload code to it the following editors are recommended.

| Editor | Pros/Cons |
| :--- | :---|
| [VS Code](https://code.visualstudio.com/) | Used in this project. It is very extensible but can be overwhelming for a beginner. |
| [Thonny](https://thonny.org/) | Very beginner friendly and lightweight. Lacks a lot of features that VS Code has. |

If VS Code is used the Pymakr plugin has to be installed as well.
[Pymakr](https://docs.pycom.io/gettingstarted/software/vscode/)

Firmware for micropython can be found here. Put this on your microcontroller by pressing the BOOTSEL button while connecting the controller to a computer.
[MicroPython Firmware](https://micropython.org/download/)

### How to upload code to the microcontroller

## VS Code instructions

1. Install the pymakr extension
<p align="center">
      <img src="https://kimmoprojectstorage.blob.core.windows.net/lnu-tutorial/pymakr-install.png">
</p>

2. Connect to your plugged in device
<p align="center">
      <img src="https://kimmoprojectstorage.blob.core.windows.net/lnu-tutorial/vscode-connect.png">
</p>

3. Select the .py file you wish to upload
<p align="center">
      <img src="https://kimmoprojectstorage.blob.core.windows.net/lnu-tutorial/vscode-connect-2.png">
</p>

## Thonny instructions

### Will be implemented soon.

## How to setup your telegram bot

To get your own telegram bot, please click on this link and follow the instructions: https://telegram.me/BotFather.

It will look something like this
<p align="center">
      <img src="https://kimmoprojectstorage.blob.core.windows.net/lnu-tutorial/telegram-bot-instructions.png" alt="telegram-bot-instructions">
</p>

1. Click on the link you received from the bot to open the bot chat and interact with it.
2. Store the api key you received from the Botfather in your .env file and keep it secret.
3. You now have your own telegram bot to send notifications to yourself and your friends!
4. See the code section to get a code snippet you can use to get started.

## Tools needed to implement the .NET API

If you want to implement the .net code in the repository I recommend using the Jetbrains Rider IDE or Visual Studio Community edition. 
     
| IDE | Free? |
| :---| :--- |
| [Rider](https://www.jetbrains.com/rider/) | Paid product or free educational license |
| [Visual Studio Community edition](https://visualstudio.microsoft.com/vs/community/) | Yes |
| [VS Code C# instructions](https://code.visualstudio.com/docs/languages/csharp) | Yes, but I do not recommend VS Code for this purpose. |

Entity Framework Core has been used as the ORM of choice and SQL Server is the selected database provider. You will need to install the relevant nuget packages.

# Circuit Diagram

- [ ] Circuit diagram (can be hand drawn)
- [ ] *Electrical calculations (higher grade)

https://fritzing.org/

# Code snippets
Below are shortened code snippets to give an example of how the microcontroller can be used to read messages from a telegram chat bot and send data to the ubidots api. To view the full code, please check the python files in the repository.
Click on the collapsed sections to view the code.

<details>
      
<summary>Connect to WIFI</summary> 
      
### boot.py
```python=
import network
from time import sleep
import machine

class Boot:
    @staticmethod
    def connect(WIFI_SSID, WIFI_PASS):
        wlan = network.WLAN(network.STA_IF)         # Put modem on Station mode
        if not wlan.isconnected():                  # Check if already connected
            print('connecting to network...')
            wlan.active(True)                       # Activate network interface
            wlan.config(pm = 0xa11140)
            wlan.connect(WIFI_SSID, WIFI_PASS)  # Your WiFi Credential
            print('Waiting for connection...', end='')
            while not wlan.isconnected() and wlan.status() >= 0:
                print('.', end='')
                sleep(1)
        ip = wlan.ifconfig()[0]
        print('\nConnected on {}'.format(ip))
        return ip
```

</details>

<details>
<summary>Read sensitive/hidden variables</summary>      

### configuration.py
```python=
# Use this to read variables from an .env file
class Configuration:
    @staticmethod
    def read_env_file(file_path):
        env_vars = {}
        with open(file_path) as f:
            for line in f:
                line = line.strip()
                if line and not line.startswith('#'):
                    key, value = line.split('=', 1)
                    value = value.strip('"')
                    env_vars[key] = value
        return env_vars
```
### .env file containing sensitive information. Remember to add .env to your gitignore!
``` .env
WIFI_SSID="this is your wifi id"
WIFI_PASS="this is your wifi password"
BOT_TOKEN="This is the telegram bot token"
API_TOKEN="This is the ubidots token"
YOUR_TELEGRAM_CHAT_ID="This is your saved telegram ID"
```

</details>

<details>
      
<summary>Save data to ubidots</summary>      

### save_data.py

```python=
def sendData(self, device, variable, value):
        try:
            url = "https://industrial.api.ubidots.com/"
            url = url + "api/v1.6/devices/" + device
            headers = {"X-Auth-Token": self.api_token, "Content-Type": "application/json"}
            data = self.build_json(variable, value)

            if data is not None:
                print("senddata.py ok",data)
                req = requests.post(url=url, headers=headers, json=data)
                return req.json()
            else:
                pass
        except:
            pass
```

</details>

<details>
      
<summary>Save data to Azure</summary>

```
def send_to_api(self, temperature):
        # this assumes that you have your own azure web service
        url = f'https://{YOUR_AZURE_PROGRAM}.azurewebsites.net/{YOUR_END_POINT}'

        # you might want to implement authorization
        headers = {
            'Content-Type': 'application/json'
        }
        data = {
            "temperature" : temperature
        }
        
        try:
            # use post to save it to a database
            req = requests.post(url, headers=headers, json=data)
            if req.status_code == 200:
                print("Save successful")
            else:
                print(req.status_code)
        except Exception as e:
            print(e)
        finally:
            if req:
                req.close()
```      
</details>

<details>
      
<summary>Telegram bot code</summary>

```python=
# function to receive messages sent to the bot
def get_telegram_updates(offset=None):
    # put your bot token here
    url = f'https://api.telegram.org/bot{BOT_TOKEN}/getUpdates'
    if offset is not None:
        url += f'?offset={offset}'
    try:
        response = requests.get(url)
        if response.status_code == 200:
            data = json.loads(response.text)
            return data.get('result', [])
        else:
            print(f"Failed to get updates: {response.status_code}")
            return []
    except Exception as e:
        print(f"Exception in get_telegram_updates: {e}")
        return []
    finally:
        if response:
            response.close()

# method to handle commands written to the bot
def process_telegram_messages(updates):
    # This will loop through all the unhandled messages the bot has received.
    for update in updates:
        # Get the user message from the json response
        message = update.get('message', {})

        # Extract the user command
        text = message.get('text', '')

        # Extract the chat_id from the json. Use this to send a message back
        chat_id = message.get('chat', {}).get('id', '')

        # Here you can handle all the commands you wish to use.
        # The commands /temperature and /dht_sensor are provided here and will send back sensor readings to the user
        if text.startswith('/temperature'):
            try:
                value = temp_sensor.read_temperature()
                send_message(chat_id, f"{value}")
            except Exception as e:
                pass

        elif text.startswith('/dht_sensor'):
            try:
                dht_val_1, dht_val_2 = dht_sensor.read_values() # temperature, humidity
                send_message(chat_id, f"{dht_val_1} {dht_val_2}")
            except Exception as e:
                pass

        else:
            try:
                send_message(chat_id, "Unknown command.")
            except Exception as e:
                pass

# function to send back a message to a user
def send_message(chat_id, text):
    url = f'https://api.telegram.org/bot{BOT_TOKEN}/sendMessage'
    data = {
        'chat_id': chat_id,
        'text': text
    }
    headers = {
        'Content-Type': 'application/json'
    }
    req = requests.post(url=url, headers=headers, json=data)
    req.close()
```

</details>

<details>
      
<summary>The main loop</summary>      

### main.py
```python=
import time
from machine import Pin, PWM
import machine
import urequests as requests
from time import sleep
import json
from boot import Boot
from configuration import Configuration as variables
from temperature_sensor import TemperatureSensor
from save_data import SaveData
from temp import DHT_Sensor as DHTSensor

led = Pin("LED", Pin.OUT)

env_vars = variables.read_env_file('.env')

DEVICE_LABEL = "XXXXXX"
VARIABLE_LABEL = "XXXXXX"
VARIABLE_LABEL_DHT_1 = "XXXXXX"
VARIABLE_LABEL_DHT_2 = "XXXXXX"

WIFI_SSID = env_vars.get('WIFI_SSID')
WIFI_PASS = env_vars.get('WIFI_PASSWORD')
DELAY = 10  # Delay in seconds

TOKEN = env_vars.get('API_TOKEN')
BOT_TOKEN = env_vars.get('BOT_TOKEN')

DHT_PIN = 26 # pin 26 chosen for this sensor
TEMP_PIN = 27 # pin 27 for this sensor

# use the constructors for the sensors
# see the python classes for these sensors
dht_sensor = DHTSensor(DHT_PIN)
temp_sensor = TemperatureSensor(TEMP_PIN)

# save to dashboard. Token is api token found on the ubidots api credential page.
save = SaveData(TOKEN)

# Initialize WIFI connection
Boot.connect(WIFI_SSID, WIFI_PASS)

last_update_id = None

while True:
    # read temperature from the temp sensor
    value = temp_sensor.read_temperature()

    # Here you could add a trigger to notify yourself
    # if value > 30:
    #       send_message(YOUR_CHAT_ID, "The temperature is over 30 degrees. Drink some water!")

    # read temperature and humidity from the DHT 11 sensor
    dht_val_1, dht_val_2 = dht_sensor.read_values()

    # send data to the dashboard
    save.sendData(DEVICE_LABEL, VARIABLE_LABEL, value)
    save.sendData(DEVICE_LABEL, VARIABLE_LABEL_DHT_1, dht_val_1)
    save.sendData(DEVICE_LABEL, VARIABLE_LABEL_DHT_2, dht_val_2)
    # check for new bot messages
    try:
        updates = get_telegram_updates(offset=last_update_id)
        if updates:
            process_telegram_messages(updates, token)
            last_update_id = updates[-1]['update_id'] + 1

    except Exception as e:
        print(f"Error in main loop: {e}")
    # loop delay
    sleep(DELAY)
```

</details>


# Connectivity

- [ ] How often is the data sent?
## Currently everything is written as a continuous loop without any asynchronous methods. A simple sleep delay of 10 seconds is used in the main loop. With all the delays of saving/fetching from api and database this turns out to be 2-3 calls per minute.
- [ ] Which wireless protocols did you use (WiFi, LoRa, etc …)?
## WIFI. Why? 100 % uptime, fast response time etc.
cons: needlessly high resolution. Currently 2-3 datapoints are saved per minutes which is unneccessarily high. This will be changed in the future.
- [ ] Which transport protocols were used (MQTT, webhook, etc …)
## webhook? HTTP requests, post, get, patch
- [ ] *Elaborate on the design choices regarding data transmission and wireless protocols. That is how your choices affect the device range and battery consumption.

# Data Visualization/presentation

- [ ] Provide visual examples on how the dashboard looks. Pictures needed.

# CHANGE ME CHANGE ME CHANGE ME!!!
Dashboard: ![myimage-alt-tag](https://raw.githubusercontent.com/KimmoKAhola/The-IoT-Temperature-Observer/master/pictures/lnu-dashboard.png)
- [ ] How often is data saved in the database.
- [ ] *Explain your choice of database.
- [ ] *Automation/triggers of the data.
- [ ] Describe platform in terms of functionality
- [ ] *Explain and elaborate what made you choose this platform
Ubidots as well as Azure (free for 12 months if you are a student)

## Ubidots Dashboard
[Public Dashboard](https://stem.ubidots.com/app/dashboards/public/dashboard/QiS5cV6BLo26QOs3kU8ZUUYNLR0JPOHqLFPNH-FtdNE)

## Swagger API
This is an endpoint to fetch my saved sensor data. Use the GUI to click on the topmost option, /PlantData/Temperature, and click on execute to view the data. Note that some of the options are protected.
[API](https://plantobserverapi.azurewebsites.net/swagger/index.html)

You can also view this to get the JSON data directly [JSON](https://plantobserverapi.azurewebsites.net/PlantData/Temperature)

## Azure SQL server
Why azure? I study .net and it is very integrated with that tech stack. Also possible to use the service for free for up to 12 months if you apply for it with school email.
[Azure](https://azure.microsoft.com/sv-se)

# Try it!
## Telegram chat bot
Interact with it if you want to and have a telegram account. If the microcontroller is live you will receive a response within 20 seconds.

https://t.me/PlantObserverBot

This is an example of responses your bot might have.
<p align="center">
      <img src="https://kimmoprojectstorage.blob.core.windows.net/lnu-tutorial/telegram-example-response.png">
</p>

# Final thoughts

- [ ] Show final results of the project
- [ ] Pictures
