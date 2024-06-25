# The IoT Temperature Observer 
Author: Kimmo Ahola (ka223pd)
Give a short and brief overview of what your project is about. What needs to be included:

- [x] Title
- [x] Your name and student credentials (ka223pd)
- [ ] Short project overview
- [ ] How much time it might take to do (approximation)

# Introduction
This repository serves as the code base for an IoT project for the summer course 24ST - 1DT305 at [LNU](https://lnu-ftk.instructure.com/courses/402).
The purpose of this project has been to create a simple endpoint that I can place somewhere and have it collect data. Users can interact with this controller by using a telegram app (some functionality is not available for everyone at the moment, like subscriber).

Since this course had a focus on microcontrollers/IoT and python the instructions will go in depth for those parts. 
The .NET api part is part of the code base and link to tools will be provided but will not be covered otherwise.

Current implementation: one microcontroller that reads values from different sensors and saves to a database. A telegram bot is connected to this microcontroller and users can, if they want to, request selected live data from the microcontroller.

Future ideas: implement more sensors and create a smart home. Want to create a network of sensors and visualize the data on a personal website. Want to receive triggers on my phone through a telegram bot/phone app whenever a trigger event happens.

How much time it may take: 6-10 depending on experience (for the python part only).

# Introduction - tutorial on how to....
# Objective

I want to create a sensor network. This is a tutorial to build the first sensor and save information to a database

Objective

Describe why you have chosen to build this specific device. What purpose does it serve? What do you want to do with the data, and what new insights do you think it will give?

- [ ] Why you chose the project
- [ ] What purpose does it serve
- [ ] What insights you think it will give

# Materials

Sensors used in this example: DHT_11 (temp + humidity). Analogue temperature sensor.

Material: Breadboard, Raspberry pi pico, jumper cables, USB cable

- [ ] List of material
- [ ] What the different things (sensors, wires, controllers) do - short specifications
- [ ] Where you bought them and how much they cost
      
| Product | Quantity | Link  | Price (SEK) | Description |
| :---         |     ---:       |          :--- | ---: | :--- |
| Raspberry Pi Pico WH   | 1     | [electrokit](https://www.electrokit.com/raspberry-pi-pico-wh)    | 109 | The brain |
| USB cable | 1 | [electrokit](https://www.electrokit.com/usb-kabel-a-hane-micro-b-5p-hane-1.8m) | 39 | Connect the Pico to a computer and power it |
| Kopplingsdäck 840 anslutningar      | 1       | [electrokit](https://www.electrokit.com/kopplingsdack-840-anslutningar)      | 69 | A board to connect sensors and the microcontroller |
| Connecting Cables  | 1 | [electrokit](https://www.electrokit.com/labbsladd-20-pin-15cm-hane/hane) | 29 | Connect the sensors to the controller |
| DHT 11 Temperature and Humidity Sensor | 1 | [electrokit](https://www.electrokit.com/digital-temperatur-och-fuktsensor-dht11) | 49 | Measure temperature and humidity (Accuracy: -1) |
| MCP9700 Temperature Sensor | 1 | [electrokit](https://www.electrokit.com/mcp9700-to-92-temperaturgivare) | 12 | Measure temperature (Accuracy: -1) |
| Total price | | | -2000 |



# Setup

A table of options this project can be implemented in

| IDE/Editor | Pros/Cons |
| :--- | :---|
| [VSCODE](https://code.visualstudio.com/) | Used in this project. Is very extensible but can be overwhelming for a beginner. |
| [Thonny](https://thonny.org/) | Very beginner friendly and lightweight |

If VSCode is used the Pymakr plugin has to be installed as well.
[Pymakr](https://docs.pycom.io/gettingstarted/software/vscode/)

# place images here on how to find it in the vscode plugin store etc

- [x] Chosen IDE
- [ ] How the code is uploaded
- [ ] Steps that you needed to do for your computer. Installation of Node.js, extra drivers, etc.



### if you wish to implement the .net code

If you want to implement the .net code in the repository I recommend using the Jetbrains Rider IDE or Visual Studio Community edition. VSCode can also be used but I would recommend a dedicated IDE.

| IDE | Free? |
| :---| :---|
| [Rider](https://www.jetbrains.com/rider/) | Paid product or free educational license |
| [Visual Studio Community edition](https://visualstudio.microsoft.com/vs/community/) | Yes |


# Circuit Diagram

- [ ] Circuit diagram (can be hand drawn)
- [ ] *Electrical calculations (higher grade)

https://fritzing.org/

# Code snippets
Below are shortened code snippets to give an example of how the microcontroller can be used to read messages from a telegram chat bot and send data to the ubidots api. To view the full code, please check the python files in the repository.
Click on the collapsed sections to view the code.

<details>
      
<summary>Click to view wifi connection code</summary> 
      
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
<summary>Click to view configuration code</summary>      

### configuration.py
``` 
python=
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
### .env file containing sensitive information
``` .env
WIFI_PASS="XXXXXXXXXX"
WIFI_SSID="XXXXXXXXXX"
BOT_TOKEN="XXXXXXXXXX"
API_TOKEN="XXXXXXXXXX"
USERNAME="XXXXXXXXXX"
PASSWORD="XXXXXXXXXX"
```

</details>

<details>
      
<summary>Click to view main program</summary>      

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

DEVICE_LABEL = "dashboard id"
VARIABLE_LABEL = "XXXXX"

LED_LABEL = "led_sensor"

WIFI_SSID = env_vars.get('WIFI_SSID')
WIFI_PASS = env_vars.get('WIFI_PASSWORD')
DELAY = 10  # Delay in seconds

TOKEN = env_vars.get('API_TOKEN')
BOT_TOKEN = env_vars.get('BOT_TOKEN')

DHT_PIN = 26 # pin 26 chosen for this sensor
TEMP_PIN = 27 # pin 27 for this sensor

# use the constructors for the sensors
dht_sensor = DHTSensor(DHT_PIN)
temp_sensor = TemperatureSensor(TEMP_PIN)

# save to dashboard. Token is api token found on the ubidots api credential page.
save = SaveData(TOKEN)

# function to receive messages sent to the bot
def get_telegram_updates(offset=None):
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
def process_telegram_messages(updates, token):

    for update in updates:
        message = update.get('message', {})
        text = message.get('text', '')
        chat_id = message.get('chat', {}).get('id', '')
        name = message.get('chat', {}).get('first_name')

        if text.startswith('/temperature'):
            try:
                value = temp_sensor.read_temperature()
                send_message(chat_id, f"Current temperature in Kimmo's room: {value} °C")
            except Exception as e:
                pass

        elif text.startswith('/dht_sensor'):
            try:
                send_message(chat_id, f"{dht_val_1} {dht_val_2}")
            except Exception as e:
                pass

        elif text.startswith('/toggle_led'):
            try:
                toggle_led()
                if led_status:
                    status = "ON"
                else:
                    status = "OFF"
                send_message(chat_id, f"LED toggled {status}")
            except Exception as e:
                pass

        else:
            try:
                send_message(chat_id, "Unknown command. Type /commands to see a list of available commands.")
            except Exception as e:
                pass


def toggle_led():
    global led_status
    try:
        led_status = not led.value()
        led.value(led_status)

        # send data to the ubidots dashboard
        save.sendData(DEVICE_LABEL, LED_LABEL, int(led_status))

    except Exception as e:
        print(f"Error toggling LED: {e}")

# function to send back a message to a user
# read your bot token from the telegram app and keep it safe
def send_message(chat_id, text):
    url = f'https://api.telegram.org/bot{BOT_TOKEN}/sendMessage'
    payload = {
        'chat_id': chat_id,
        'text': text
    }
    headers = {
        'Content-Type': 'application/json'
    }
    req = requests.post(url=url, headers=headers, json=payload)
    req.close()


# Initialize WIFI connection
Boot.connect(WIFI_SSID, WIFI_PASS)

last_update_id = None

while True:
    # token for saving to my own database
    token = save.get_token(USERNAME, PASSWORD)
    # read temperature from the temp sensor
    value = temp_sensor.read_temperature()
    # read temperature and humidity from the DHT 11 sensor
    dht_val_1, dht_val_2 = dht_sensor.read_values()
    # save to my own database
    save.send_to_api(token=token, temperature=value, dht_temperature=dht_val_1, dht_humidity=dht_val_2, sensor_id=SENSOR_ID)
    # send data to the dashboard
    save.sendData(DEVICE_LABEL, VARIABLE_LABEL, value)

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

<details>
      
<summary>Click to view send data code</summary>      

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
Dashboard: ![myimage-alt-tag](https://raw.githubusercontent.com/KimmoKAhola/The-IoT-Temperature-Observer/master/pictures/lnu-dashboard.png)
- [ ] How often is data saved in the database.
- [ ] *Explain your choice of database.
- [ ] *Automation/triggers of the data.
- [ ] Describe platform in terms of functionality
- [ ] *Explain and elaborate what made you choose this platform
Ubidots as well as Azure (free for 12 months if you are a student)

## Ubidots Dashboard
[Public Dashboard](https://stem.ubidots.com/app/dashboards/public/dashboard/QiS5cV6BLo26QOs3kU8ZUUYNLR0JPOHqLFPNH-FtdNE)
[API](https://plantobserverapi.azurewebsites.net/swagger/index.html)
## Azure SQL server
Why azure? I study .net and it is very integrated with that tech stack. Also possible to use the service for free for up to 12 months (a certain amount of credit is provided) if you apply for it with school email.
[Azure](https://azure.microsoft.com/sv-se)

# Try it!
Click here to write to the bot. Click on the menu to see available commands.
# picture of telegram chat here
# Final thoughts


- [ ] Show final results of the project
- [ ] Pictures


## API
https://plantobserverapi.azurewebsites.net/swagger/index.html

## Telegram chat bot
Interact with it if you want to. Note that some options may only work for me personally at the moment.
https://t.me/PlantObserverBot


