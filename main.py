import machine
import time
from machine import Pin, PWM
import network
import urequests as requests
from time import sleep
import json
from boot import Boot
from configuration import Configuration as variables

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
led_status = False

DECIMAL_PRECISION = 1 # Round to 1 decimal for all values
UPPER_BOUND_16_BIT = 65535 # 2^16-1
UPPER_BOUND_8_BIT = 255 # 2^8-1
LOWER_BOUND_8_BIT = 0

RED_PIN = 18
BLUE_PIN = 17
GREEN_PIN = 16
TEMP_PIN = 27
USERNAME = env_vars.get('USERNAME')
PASSWORD = env_vars.get('PASSWORD')

red_pwm = PWM(Pin(RED_PIN))
blue_pwm = PWM(Pin(BLUE_PIN))
green_pwm = PWM(Pin(GREEN_PIN))

adc = machine.ADC(TEMP_PIN)
sf = 4095/65535
volt_per_adc = (3.3 / 4095)

# Read temperature in C
def read_temperature(): 
    millivolts = adc.read_u16()
    adc_12b = millivolts * sf
    volt = adc_12b * volt_per_adc
    dx = abs(50 - 0)
    dy = abs(0 - 0.5)
    shift = volt - 0.5
    temp = shift / (dy / dx)
    return round(temp, DECIMAL_PRECISION)

# Builds the json to send the request
def build_json(variable, value):
    try:
        data = {variable: {"value": value}}
        return data
    except:
        return None

# Sending data to Ubidots Restful Webserice
def sendData(device, variable, value):
    try:
        url = "https://industrial.api.ubidots.com/"
        url = url + "api/v1.6/devices/" + device
        headers = {"X-Auth-Token": TOKEN, "Content-Type": "application/json"}
        data = build_json(variable, value)

        if data is not None:
            print(data)
            req = requests.post(url=url, headers=headers, json=data)
            return req.json()
        else:
            pass
    except:
        pass

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
                value = read_temperature()
                send_message(chat_id, f"Current temperature in Kimmo's room: {value} degrees C")
            except Exception as e:
                print(f"Error reading temperature: {e}")
        
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
                send_message(chat_id, "Help")
            except Exception as e:
                print(f"{e}")
        else:
            try:
                send_message(chat_id, "Unknown command. Type /Commands to see a list of available commands.")
            except Exception as e:
                print(f"Error with the else: {e}")


def toggle_led():
    global led_status
    try:
        led_status = not led.value()
        led.value(led_status)
        sendData(DEVICE_LABEL, LED_LABEL, int(led_status))

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
    req = requests.post(url=url, headers=headers, json=payload)
    req.close()

def get_token():
    url = f'https://plantobserverapi.azurewebsites.net/PlantData/token?user={USERNAME}&password={PASSWORD}'
    headers = {
        "Accept": 'text/plain'
    }
    params = {
        'user' : USERNAME,
        'password' : PASSWORD
    }
    try:
        response = requests.post(url, headers=headers, json=params)
        if response.status_code == 200:
            return response.text
        else:
            print(response.status_code)
    except Exception as e:
        print(e)
    finally:
        if response:
            response.close()

def send_to_api(token, temperature):
    url = f'https://plantobserverapi.azurewebsites.net/PlantData/post'
    headers = {
        'Content-Type': 'application/json',
        'Authorization': f'Bearer {token}'
    }
    data = {
        "temperature" : temperature
    }
    try:
        req = requests.post(url, headers=headers, json=data)
        if req.status_code == 200:
            print("Post successful")
        else:
            print(req.status_code)
    except Exception as e:
        print(e)
    finally:
        if req:
            req.close()

Boot.connect(WIFI_SSID, WIFI_PASS)

last_update_id = None
current_time = time.time()

while True:
    token = get_token()
    value = read_temperature()
    """
    if value > 25:
        send_message(792140634, "VARNING!! VARMT!!")
    """
    send_to_api(token, value)
    returnValue = sendData(DEVICE_LABEL, VARIABLE_LABEL, value)
    try:
        updates = get_telegram_updates(offset=last_update_id)
        if updates:
            process_telegram_messages(updates, token)
            last_update_id = updates[-1]['update_id'] + 1

    except Exception as e:
        print(f"Error in main loop: {e}")
    sleep(DELAY)
