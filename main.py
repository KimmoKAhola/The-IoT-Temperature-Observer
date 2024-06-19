import machine
import time
from machine import Pin, PWM
import network
import urequests as requests
from time import sleep
import random
import json

led = Pin("LED", Pin.OUT)



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

env_vars = read_env_file('.env')

TOKEN = env_vars.get('API_TOKEN')
DEVICE_LABEL = "Test"
VARIABLE_LABEL = "sensor"
LED_LABEL = "led_sensor"
WIFI_SSID = env_vars.get('WIFI_SSID')
WIFI_PASS = env_vars.get('WIFI_PASSWORD')
DELAY = 10  # Delay in seconds
DATABASE_DELAY = 60 # Delay for saving to database in seconds.
BOT_TOKEN = env_vars.get('BOT_TOKEN')
#CHAT_ID = env_vars.get('CHAT_ID')
CHAT_ID = env_vars.get('KIMMO_CHAT_ID')
led_status = False
temperature_mode = "C"
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

print(f"TOKEN: {TOKEN}")
print(f"DEVICE_LABEL: {DEVICE_LABEL}")
print(f"VARIABLE_LABEL: {VARIABLE_LABEL}")
print(f"WIFI_SSID: {WIFI_SSID}")
print(f"WIFI_PASS: {WIFI_PASS}")
print(f"DELAY: {DELAY}")

adc = machine.ADC(TEMP_PIN)
sf = 4095/65535 # Scale factor
volt_per_adc = (3.3 / 4095)

def read_temperature(temperature_mode): 
    millivolts = adc.read_u16()

    adc_12b = millivolts * sf

    volt = adc_12b * volt_per_adc

    dx = abs(50 - 0)
    dy = abs(0 - 0.5)

    shift = volt - 0.5

    temp = shift / (dy / dx)
    if temperature_mode == "C":
        return round(temp, DECIMAL_PRECISION)
    elif temperature_mode == "F":
        return round((temp*9/5)+32, DECIMAL_PRECISION)

def connect():
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

def set_blue():
    red_pwm = PWM(Pin(18))
    blue_pwm = PWM(Pin(17))
    green_pwm = PWM(Pin(16))

    red_pwm.duty_u16(50000)
    green_pwm.duty_u16(50000)
    blue_pwm.duty_u16(50000)
    
def set_green():
    red_pwm = PWM(Pin(18))
    blue_pwm = PWM(Pin(17))
    green_pwm = PWM(Pin(16))
    
    red_pwm.duty_u16(LOWER_BOUND_8_BIT)
    green_pwm.duty_u16(UPPER_BOUND_16_BIT)
    blue_pwm.duty_u16(LOWER_BOUND_8_BIT)
    
def set_red():
    red_pwm = PWM(Pin(18))
    blue_pwm = PWM(Pin(17))
    green_pwm = PWM(Pin(16))
    
    red_pwm.duty_u16(UPPER_BOUND_16_BIT)
    green_pwm.duty_u16(LOWER_BOUND_8_BIT)
    blue_pwm.duty_u16(LOWER_BOUND_8_BIT)

def set_white():
    red_pwm = PWM(Pin(18))
    blue_pwm = PWM(Pin(17))
    green_pwm = PWM(Pin(16))
    
    red_pwm.duty_u16(UPPER_BOUND_16_BIT)
    green_pwm.duty_u16(UPPER_BOUND_16_BIT)
    blue_pwm.duty_u16(UPPER_BOUND_16_BIT)
    pass

def add_user_to_database(name, chat_id):
    url = f'https://plantobserverapi.azurewebsites.net/Test/User'
    headers = {
        'Content-Type' : "aplication/json"
    }
    data = {
        'firstName' : name,
        'userChatId' : chat_id
    }
    response = None
    try:
        response = requests.post(url=url, headers=headers, json=data)
        if response.status_code == 200:
            print("ok")
        else:
            print(f"Failed to get updates: {response.status_code}")
    except Exception as e:
        print(e)
    finally:
        if response:
            response.close()

def process_telegram_messages(updates):
    print("Crash?")
    processed_messages = []
    for update in updates:
        message = update.get('message', {})
        text = message.get('text', '')
        chat_id = message.get('chat', {}).get('id', '')

        if chat_id in processed_messages:
            send_message(chat_id, "You have spammed too much, please calm down. Your messages have been ignored.")
            continue

        processed_messages.append(chat_id)
        #add_user_to_database("test", chat_id)

        if text.startswith('/temperature'):
            try:
                value = read_temperature(temperature_mode)
                send_message(chat_id, f"Current temperature in Kimmo's room: {value} degrees {temperature_mode}")
            except Exception as e:
                print(f"Error reading temperature: {e}")
        elif text.startswith('/blue'):
            try:
                set_blue()
                send_message(chat_id, "blue")
            except Exception as e:
                print(e)
        elif text.startswith('/red'):
            try:
                set_red()
            except Exception as e:
                print(e)
        elif text.startswith('/green'):
            try:
                set_green()
            except Exception as e:
                print(e)
        elif text.startswith('/white'):
            try:
                set_white()
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

        elif text.startswith('/toggle_unit'):
            try:
                toggle_unit()
                send_message(chat_id, f"The temperature mode has been set to: {temperature_mode}")
            except Exception as e:
                print(f"{e}")
        elif text.startswith('/rgb'):
            try:
                rgb_str = text.split('(')[1].split(')')[0]
                r, g, b = map(int, rgb_str.split(','))
                
                if not (LOWER_BOUND_8_BIT <= r <= UPPER_BOUND_8_BIT and LOWER_BOUND_8_BIT <= g <= UPPER_BOUND_8_BIT and LOWER_BOUND_8_BIT <= b <= UPPER_BOUND_8_BIT):
                    raise ValueError("RGB values must be between 0 and 255")
                
                red_pwm = PWM(Pin(RED_PIN))
                blue_pwm = PWM(Pin(BLUE_PIN))
                green_pwm = PWM(Pin(GREEN_PIN))

                print(red_pwm, blue_pwm, green_pwm)
                red_u16 = int(r / UPPER_BOUND_8_BIT * UPPER_BOUND_16_BIT)
                green_u16 = int(g / UPPER_BOUND_8_BIT * UPPER_BOUND_16_BIT)
                blue_u16 = int(b / UPPER_BOUND_8_BIT * UPPER_BOUND_16_BIT)
                
                red_pwm.duty_u16(50000)
                green_pwm.duty_u16(50000)
                blue_pwm.duty_u16(50000)
                
                send_message(chat_id, f"Set RGB color to R:{r}, G:{g}, B:{b}")

            except ValueError as ve:
                print(f"Value error: {ve}")
                send_message(chat_id, "Horunge. Each value must be between 0 and 255.")

            except Exception as e:
                print(f"Error handling /rgb command: {e}")
                send_message(chat_id, "Error handling /rgb command")

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

def toggle_unit():
    global temperature_mode
    if temperature_mode == "C":
        temperature_mode = "F"
    else:
        temperature_mode = "C"
    print(temperature_mode)

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
    url = f'https://plantobserverapi.azurewebsites.net/Test/token?user={USERNAME}&password={PASSWORD}'
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
    url = f'https://plantobserverapi.azurewebsites.net/Test/post'
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

connect()

last_update_id = None

while True:
    token = get_token()
    value = read_temperature(temperature_mode)
    send_to_api(token, value)
    returnValue = sendData(DEVICE_LABEL, VARIABLE_LABEL, value)
    try:
        updates = get_telegram_updates(offset=last_update_id)
        if updates:
            process_telegram_messages(updates)
            last_update_id = updates[-1]['update_id'] + 1
    except Exception as e:
        print(f"Error in main loop: {e}")
    sleep(DELAY)

