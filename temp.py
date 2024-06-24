import dht
import machine

class DHT_Sensor:
    def __init__(self, pin):
        self.sensor = dht.DHT11(machine.Pin(pin))
    
    def read_values(self):
        self.sensor.measure()
        temperature = self.sensor.temperature()
        humidity = self.sensor.humidity()
        return temperature, humidity