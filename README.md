# The IoT Temperature and Humidity Observer 
Author: Kimmo Ahola (ka223pd)

# Table of Contents

1. [Introduction](#introduction)
2. [Objective](#objective)
3. [Materials](#materials-needed)
4. [Setup](#setup)
   1. [VS Code](#vs-code-instructions)
   2. [Telegram bot](#how-to-setup-your-telegram-bot)
   3. [.NET API](#tools-needed-to-implement-the-net-api)
5. [Circuit Diagram](#circuit-diagram)
6. [Code Snippets](#code-snippets)
7. [Connectivity](#connectivity)
8. [Data Visualization](#data-visualizationpresentation)
    1. [Ubidots](#ubidots-dashboard)
    2. [Swagger API](#swagger-api)
    3. [Azure](#azure-sql-server)
9. [Try it!](#try-it)
10. [Final thoughts](#final-thoughts)

# Introduction

This repository serves as the code base for an IoT project for the summer course 24ST - 1DT305 at [LNU](https://lnu-ftk.instructure.com/courses/402). 

[Link to Github repository](https://github.com/KimmoKAhola/The-IoT-Temperature-Observer)

The purpose of this project has been to create a simple sensor endpoint that I can place somewhere and collect data which is then saved to a database. Users can interact with this controller by using a telegram bot and receive live data from it.

The future goals of this project is to expand on this idea and to implement more sensors in different parts of the country and build a larger sensor network for personal use.

Since this course is a beginner course aimed to teach microcontrollers/IoT and python the focus of this report will be on python and raspberry pi pico wh. Links to the tools used for the .NET code will be provided and simple python code snippets on how to send data to these services will also be shown. The .NET code is available in the github repository but will not be covered in detail.

For the sake of simplicity the telegram bot will use HTTP requests to fetch/send data and the bot will be run on the microcontroller.

Time required to implement this project yourself: 6-10 hours depending on experience.

# Objective
- [ ] What insights you think it will give

I chose this project to lay the foundation to a larger personal project, sort of a proof of concept. The idea behind it is to give me a deepened understanding for microcontrollers, python and coding in general.

# Materials needed
      
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

### How to upload code to the microcontroller

1. Press and hold the BOOTSEL button on the microcontroller
2. Connect the controller to a computer with a USB cable.
3. Load the controller with the latest micropython firmware. [Link to MicroPython firmware](https://micropython.org/download/)

## Upload code to the microcontroller with VS Code

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

## How to setup your telegram bot

To get your own telegram bot you will need a telegram account. Click on the link and follow the instructions to create your own bot: https://telegram.me/BotFather.

The procedure will look something like this:
<p align="center">
      <img src="https://kimmoprojectstorage.blob.core.windows.net/lnu-tutorial/telegram-bot-instructions.png" alt="telegram-bot-instructions">
</p>

1. Click on the link you received from the bot to open the bot chat and interact with it.
2. Store the api key you received from the Botfather in your .env file and keep it secret.
3. You now have your own telegram bot to send notifications to yourself and your friends!
4. See the code section to get a code snippet you can use to get started.

## Tools needed to implement the .NET API

If you want to implement the .net code from the github repository I recommend using the Jetbrains Rider IDE or Visual Studio Community edition. 
     
| IDE | Free? |
| :---| :--- |
| [Rider](https://www.jetbrains.com/rider/) | Paid product or free educational license |
| [Visual Studio Community edition](https://visualstudio.microsoft.com/vs/community/) | Yes |
| [VS Code C# instructions](https://code.visualstudio.com/docs/languages/csharp) | Yes, but I do not recommend VS Code for this purpose. |

Entity Framework Core has been used as the ORM of choice and SQL Server is the selected database provider. You will need to install the relevant nuget packages.

# Circuit Diagram
      
<p align="center">
   <img src="https://kimmoprojectstorage.blob.core.windows.net/lnu-tutorial/circuit-diagram.png">
</p>

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

The main loop in the code has a 10 second delay. Since the code is written synchronously it means that due to all the delays of calling the different API:s this delay turns out to be closer to 20-25 seconds. This is gives a good balance between datapoints from the sensors and response speed in the chat. It is possible to write the code asynchronously and solve this issue of code blocking, but since I plan on moving the bot away from the microcontroller to a dedicated server in the future it is unnecessary to rewrite the code right now.

WIFI is used as the wireless protocol for simplicity. As the code for the chat bot is run on the board itself constant uptime is preferred to have good response speed. HTTP requests are used to transfer data to the different services for data visualization.

# Data Visualization/presentation

## Ubidots Dashboard
[Link to the public Ubidots dashboard](https://stem.ubidots.com/app/dashboards/public/dashboard/QiS5cV6BLo26QOs3kU8ZUUYNLR0JPOHqLFPNH-FtdNE)

Dashboard: ![myimage-alt-tag](https://kimmoprojectstorage.blob.core.windows.net/lnu-tutorial/ubidots-temp-dashboard-picture.png)
For visualization of the project the ubidots service has been used. The reason for choosing this service was mainly because of its ease of use. Within a few minutes and a few lines of code the project had a working dashboard that can be viewed by the public. It is also possible to implement triggers on the dashboard that are disconnected from the code on the microcontroller which is an added bonus. The data is sent to Ubidots roughly 2-3 times per minute and it is also possible to implement calculations on the data live on the dashboard.

This platform has a free license for students and saves the data for up to 1 month with a limitation on the number of datapoints per day/month. As this project was meant to be a proof of concept for a future project this solution worked perfectly fine for visualization.

## Azure SQL server
[Azure](https://azure.microsoft.com/sv-se)
To save my data permanently I have chosen to use a database on Azure services. This service has been chosen for several reasons: it is heavily integrated with my favorite tech stach at the moment (.NET), it has a 12 month free plan for students and is reasonably cheap to use when it is not free. The purpose of permanently saving the data is to be able to use any kind of visualization library in the future to view historical data without any time limit. It is also possible to save readings from different sensors in a database with ease when using relational databases.

# Try it!
You have read through all of this text. Wouldn't it be fun to see the result?

## Telegram chat bot
Interact with it if you want to and have a telegram account. If the microcontroller is live you will receive a response within 20 seconds, if the controller is disconnected the message will be sent when the controller goes live the next time.

https://t.me/PlantObserverBot

This is an example of responses your bot might have.
<p align="center">
      <img src="https://kimmoprojectstorage.blob.core.windows.net/lnu-tutorial/telegram-example-response.png">
</p>

## Swagger API
This is an endpoint to fetch my saved sensor data. Use the GUI to click on the topmost option, /PlantData/Temperature, and click on execute to view the data. Note that some of the options are protected.
[API](https://plantobserverapi.azurewebsites.net/swagger/index.html)

You can also view this to get the JSON data directly [JSON](https://plantobserverapi.azurewebsites.net/PlantData/Temperature)

# Final thoughts

![temp pic](https://kimmoprojectstorage.blob.core.windows.net/lnu-tutorial/temp-project-picture.jpg)
A better picture will be added for the final draft.

## Write something about results and how I focused more on the software part of this project.

The focus of this project has mainly been on the software side instead of the hardware side. I focused more on laying the foundation for code expansion rather than on any advanced sensor readings. This project uses two different temperature sensors. Both sensors are of the cheaper kind and have an accuracy spread of ±2°C which is quite large but by using 2 different sensors perhaps a better reading could perhaps be obtained by using the mean value. For future project it might be a better idea to use better sensors and receive accurate readings.

The code for the telegram chat bot uses simple HTTP request to fetch/request data. This is not a scalable solution for larger projects but will work fine for personal use. 
Some improvements that can be made: use as little code as possible on the microcontrollers. The controllers should preferrably use as little computational power as possible and the logic for calculation should be done on another service. This gives us the option to improve the functionality of the bot service as well as decrease the power consumption of the microcontroller. The current implementation does not have the possibility to run without a USB cable which is a big limitation.

When the code for the chat bot is moved to another service another, more energy efficient, protocol will be used.

