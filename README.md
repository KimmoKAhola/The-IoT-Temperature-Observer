# The IoT Temperature Observer 
## Kimmo Ahola (ka223pd)
Give a short and brief overview of what your project is about. What needs to be included:

- [x] Title
- [x] Your name and student credentials (ka223pd)
- [ ] Short project overview
- [ ] How much time it might take to do (approximation)

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

Chosen IDE/Editor - VSCode, Rider for the .net api.
Pymakr

[VSCODE](https://code.visualstudio.com/)
[Pymark](https://docs.pycom.io/gettingstarted/software/vscode/)

# place images here on how to find it in the vscode plugin store etc

- [ ] Chosen IDE
- [ ] How the code is uploaded
- [ ] Steps that you needed to do for your computer. Installation of Node.js, extra drivers, etc.

[For the api](https://www.jetbrains.com/rider/)


### chosen IDE/Editor
### How the code is uploaded
### Steps needed, installation etc


# Circuit Diagram

- [ ] Circuit diagram (can be hand drawn)
- [ ] *Electrical calculations (higher grade)

https://fritzing.org/

# Code snippets

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

``` configuration.py
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

``` .env
WIFI_PASS="XXXXXXXXXX"
WIFI_SSID="XXXXXXXXXX"
BOT_TOKEN="XXXXXXXXXX"
API_TOKEN="XXXXXXXXXX"
```

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

TOKEN = env_vars.get('API_TOKEN')
DEVICE_LABEL = "Test"
VARIABLE_LABEL = "sensor"
LED_LABEL = "led_sensor"
WIFI_SSID = env_vars.get('WIFI_SSID')
WIFI_PASS = env_vars.get('WIFI_PASSWORD')
DELAY = 10  # Delay in seconds
DATABASE_DELAY = 60 # Delay for saving to database in seconds.
BOT_TOKEN = env_vars.get('BOT_TOKEN')
SENSOR_ID = Boot.get_sensor_id()

DECIMAL_PRECISION = 1 # Round to 1 decimal for all values
UPPER_BOUND_16_BIT = 65535 # 2^16-1
UPPER_BOUND_8_BIT = 255 # 2^8-1
LOWER_BOUND_8_BIT = 0

RED_PIN = 18
BLUE_PIN = 17
GREEN_PIN = 16
DHT_PIN = 26
TEMP_PIN = 27
USERNAME = env_vars.get('USERNAME')
PASSWORD = env_vars.get('PASSWORD')

red_pwm = PWM(Pin(RED_PIN))
blue_pwm = PWM(Pin(BLUE_PIN))
green_pwm = PWM(Pin(GREEN_PIN))

dht_sensor = DHTSensor(DHT_PIN)
temp_sensor = TemperatureSensor(TEMP_PIN)
save = SaveData(TOKEN)

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

def add_user_to_database(name, chat_id, content, token):
    print("start", name)
    url = f'https://plantobserverapi.azurewebsites.net/PlantData/PostMessage'
    headers = {
        'Content-Type' : "application/json",
        'Authorization': f'Bearer {token}'
    }
    data = {
        'firstName' : f"{name}",
        'userChatId' : f"{chat_id}",
        'content' : f"{content}"
    }
    print(data)
    try:
        response = requests.post(url=url, headers=headers, json=data)
        if response.status_code == 200:
            print("ok")
        else:
            print(f"{response.status_code}")
    except Exception as e:
        print(e)
    finally:
        if response:
            response.close()

def process_telegram_messages(updates, token):
    processed_messages = []
    for update in updates:
        message = update.get('message', {})
        text = message.get('text', '')
        chat_id = message.get('chat', {}).get('id', '')
        name = message.get('chat', {}).get('first_name')

        # remove bad characters from name. Very hacky way to do it
        temp_name = ""
        for char in name:
            if char.isalpha():
                temp_name += char
        name = temp_name

        if chat_id in processed_messages:
            send_message(chat_id, "You have spammed too much, please calm down. Your messages have been ignored.")
            continue

        processed_messages.append(chat_id)
        add_user_to_database(name, chat_id, text, token)

        if text.startswith('/temperature'):
            try:
                value = temp_sensor.read_temperature()
                send_message(chat_id, f"Current temperature in Kimmo's room: {value} °C")
            except Exception as e:
                print(f"Error reading temperature: {e}")

        elif text.startswith('/commands'):
            try:
                send_message(chat_id, "/temperature\n/all_data\n/dht_sensor\n/toggle_led\n")
            except Exception as e:
                print(e)
        elif text.startswith('/all_data'):
            try:
                send_message(chat_id, f"Temp: {value} C, Temp 2: {dht_val_1} C, Humidity: {dht_val_2} %")
            except Exception as e:
                print(f"{e}")

        elif text.startswith('/dht_sensor'):
            try:
                send_message(chat_id, f"{dht_val_1} {dht_val_2}")
            except Exception as e:
                print(f"{e}")
        elif text.startswith('/subscribe'):
            try:
                result = save.update_subscriber_status(chat_id, True, token=token)
                if result:
                    send_message(chat_id, "You are now subscribed.")
            except Exception as e:
                print(e)
        elif text.startswith('/unsubscribe'):
            try:
                result = save.update_subscriber_status(chat_id, False, token=token)
                if result:
                    send_message(chat_id, "You are now unsubscribed.")
            except Exception as e:
                print(e)
        elif text.startswith('/toggle_led'):
            try:
                toggle_led()
                if led_status:
                    status = "ON"
                else:
                    status = "OFF"
                send_message(chat_id, f"LED toggled {status}")
            except Exception as e:
                print(f"Error toggling LED: {e}")
        
        elif text.startswith('/masoud'):
            try:
                send_message(chat_id, "MASOUD")
            except Exception as e:
                print(f"Error with masoud: {e}")
        elif text.startswith('/help'):
            try:
                send_message(chat_id, "help")
            except Exception as e:
                print(f"{e}")
        else:
            try:
                send_message(chat_id, "Unknown command. Type /commands to see a list of available commands.")
            except Exception as e:
                print(f"Error with the else: {e}")


def toggle_led():
    global led_status
    try:
        led_status = not led.value()
        led.value(led_status)
        save.sendData(DEVICE_LABEL, LED_LABEL, int(led_status))

    except Exception as e:
        print(f"Error toggling LED: {e}")
    print("Toggled LED to:", led_status)

def send_message(chat_id, text):
    url = f'https://api.telegram.org/bot{BOT_TOKEN}/sendMessage'
    payload = {
        'chat_id': chat_id,
        'text': text
    }
    headers = {
        'Content-Type': 'application/json'
    }
    print("send message:", text)
    req = requests.post(url=url, headers=headers, json=payload)
    req.close()


Boot.connect(WIFI_SSID, WIFI_PASS)

last_update_id = None
current_time = time.time()

while True:
    token = save.get_token(USERNAME, PASSWORD)
    value = temp_sensor.read_temperature()
    dht_val_1, dht_val_2 = dht_sensor.read_values()
    save.send_to_api(token=token, temperature=value, dht_temperature=dht_val_1, dht_humidity=dht_val_2, sensor_id=SENSOR_ID)
    save.sendData(DEVICE_LABEL, VARIABLE_LABEL, value)
    try:
        updates = get_telegram_updates(offset=last_update_id)
        if updates:
            process_telegram_messages(updates, token)
            last_update_id = updates[-1]['update_id'] + 1

    except Exception as e:
        print(f"Error in main loop: {e}")
    """
    """
    sleep(DELAY)
```

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

# Connectivity

- [ ] How often is the data sent?
- [ ] Which wireless protocols did you use (WiFi, LoRa, etc …)?
- [ ] Which transport protocols were used (MQTT, webhook, etc …)
- [ ] *Elaborate on the design choices regarding data transmission and wireless protocols. That is how your choices affect the device range and battery consumption.

# Data Visualization/presentation

- [ ] Provide visual examples on how the dashboard looks. Pictures needed.
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


