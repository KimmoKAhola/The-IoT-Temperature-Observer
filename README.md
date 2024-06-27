# TODO
- [ ] Implement circuit diagram
- [ ] Good pictures of the project
- [ ] Good looking dashboard
- [ ] Thonny instructions
- [ ] Double-check the code snippets, clean them if needed.

# The IoT Temperature Observer 
Author: Kimmo Ahola (ka223pd)

# Table of Contents

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

[Github Repository](https://github.com/KimmoKAhola/The-IoT-Temperature-Observer)

This repository serves as the code base for an IoT project for the summer course 24ST - 1DT305 at [LNU](https://lnu-ftk.instructure.com/courses/402).
The purpose of this project has been to create a simple endpoint that I can place somewhere and have it collect data. Users can interact with this controller by using a telegram bot and writing certain commands to it.

The future goals of this project is to build my own database and api services by combining the microcontrollers with .net APIs (I study to become a .NET Developer) and as such this codebase contains free to use code for .NET services as well. This will later be used to implement my own smart home (I hope).

Since this course is a beginner course aimed to teach microcontrollers/IoT and python the focus of this report will be on those parts. Links to tools used for the .NET code will be provided and simple python snippets on how to send data to these services will also be shown. The C# code is available in the repo but will not be covered at all.

The telegram bot code will also be kept beginner friendly and utilize HTTP requests to fetch/send data. This is not a scalable solution but will work fine for personal use.

Time required to implement this project yourself: 6-10 hours depending on experience (for the python part).

# Objective

- [x] Why you chose the project
- [x] What purpose does it serve
- [ ] What insights you think it will give

I chose this project to lay the foundation to a larger project, sort of a proof of concept. The idea behind it is to give me a deepened understanding for microcontrollers, python and coding in general.
I want to give friends & family small sensors to use at home to measure something and collect the data in a personal database and use that info for some visualization/calculation.

Insights: ? Think about this.

# Materials needed

- [x] List of material
- [x] What the different things (sensors, wires, controllers) do - short specifications
- [x] Where you bought them and how much they cost
- [x] Add sum of all material
- [ ] change price and sum to the rightmost column
      
| Product | Quantity | Link  | Description | Price (SEK) |
| :---         |     ---:       | :--- | :--- | ---: |
| Raspberry Pi Pico WH   | 1     | [Electrokit](https://www.electrokit.com/raspberry-pi-pico-wh) | The main part of the project. The "brain". | 109 |
| USB cable A-male - microB-male 1.8m  | 1 | [Electrokit](https://www.electrokit.com/usb-kabel-a-hane-micro-b-5p-hane-1.8m) | Connect the Pico to a computer and power it | 39 |
| Solderless Breadboard 840 tie-points       | 1       | [Electrokit](https://www.electrokit.com/kopplingsdack-840-anslutningar) | A board to connect sensors and the microcontroller |69 |
| Jumper wires 20-pin 30cm male/male   | 1 | [Electrokit](https://www.electrokit.com/labbsladd-20-pin-15cm-hane/hane) | Connect the sensors to the controller | 29 |
| Digital temperature and humidity sensor DHT11 | 1 | [Electrokit](https://www.electrokit.com/digital-temperatur-och-fuktsensor-dht11) |  Measure temperature and humidity (Accuracy: ±2℃, ±5％RH) |49 |
| MCP9700 TO-92 Temperaturgivare | 1 | [Electrokit](https://www.electrokit.com/mcp9700-to-92-temperaturgivare) | Measure temperature (Accuracy: ±2°C) | 12 |
| Total price | | | | 307|

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

To get your own telegram bot you will need a telegram account. Click on this link and follow the instructions to create your own bot: https://telegram.me/BotFather.

The procedure will look something like this:
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
      
<summary>Save data to your own database through azure api</summary>

```
def send_to_api(self, temperature):
        # this assumes that you have your own azure web service
        url = f'https://{YOUR_AZURE_PROGRAM}.azurewebsites.net/{YOUR_END_POINT}'

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
      
<summary>Telegram bot code, using HTTP requests</summary>

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
        # The command /temperature is used to send reading back to a the user.
        if text.startswith('/temperature'):

        value = temp_sensor.read_temperature()
        send_message(chat_id, f"{value}")

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
# add necessary imports

last_update_id = None

while True:
    # read temperature from a sensor
    value = sensor.read_temperature()

    # Here you could add a trigger to notify yourself
    # if value > 30:
    #       send_message(YOUR_CHAT_ID, "The temperature is over 30 degrees. Drink some water!")

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

- [x] Which wireless protocols did you use (WiFi, LoRa, etc …)?
- [x] How often is the data sent?

The main loop in the code has a 10 second delay. Since the code is written synchronously it means that due to all the delays of calling the different API:s this delay turns out to be closer to 20-25 seconds. This is gives a good balance between datapoints from the sensors and response speed in the chat. It is possible to write the code asynchronously and solve this issue of code blocking, but since I plan on moving the bot away from the microcontroller to a dedicated server in the future it is unnecessary to rewrite the code right now.

Currently WIFI is used as the wireless protocol. As the code for the chat bot is run on the board itself constant uptime is needed to have good response speed. When the code for the chat bot is moved to another service another, more energy efficient, protocol will be used.

HTTP requests are used to transfer data. JSON, post, get, patch etc

# Data Visualization/presentation

- [x] Provide visual examples on how the dashboard looks. Pictures needed.
- [x] How often is data saved in the database.
- [x] *Explain your choice of database.
- [x] *Automation/triggers of the data.
- [x] *Explain and elaborate what made you choose this platform
- [ ] Describe platform in terms of functionality
- [ ] Future plans for visualization

# CHANGE ME CHANGE ME CHANGE ME!!!
## Ubidots Dashboard
[Public Dashboard](https://stem.ubidots.com/app/dashboards/public/dashboard/QiS5cV6BLo26QOs3kU8ZUUYNLR0JPOHqLFPNH-FtdNE)

Dashboard: ![myimage-alt-tag](https://kimmoprojectstorage.blob.core.windows.net/lnu-tutorial/ubidots-temp-dashboard-picture.png)
About 2-3 times per minute for the sensor data table. Users and user messages are saved to their tables whenever the bot reads the messages.
Ubidots is a convenient low code solution to quickly give a good looking visual representation. Since I plan to move away from this in the future and utilize something else I save the data permanently to a SQL Server hosted on Azure. This choice is based on Azure's heavy integration with the .NET framework and the fact that it is possible to use it for free for up to 12 months with a student account (that's how they get you hooked...).
The bot gets triggered whenever there are new unhandled messages.
Ubidots as well as Azure (free for 12 months if you are a student)

## Swagger API
This is an endpoint to fetch my saved sensor data. Use the GUI to click on the topmost option, /PlantData/Temperature, and click on execute to view the data. Note that some of the options are protected.
[API](https://plantobserverapi.azurewebsites.net/swagger/index.html)

You can also view this to get the JSON data directly [JSON](https://plantobserverapi.azurewebsites.net/PlantData/Temperature)

## Azure SQL server
Why azure? I study .net and it is very integrated with that tech stack. Also possible to use the service for free for up to 12 months if you apply for it with school email.
[Azure](https://azure.microsoft.com/sv-se)

# Try it!
You have read through all this text. Wouldn't it be fun to try it?

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

## Add pictures here
