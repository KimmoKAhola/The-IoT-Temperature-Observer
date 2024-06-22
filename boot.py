# boot.py -- run on boot-up
import network
from time import sleep
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