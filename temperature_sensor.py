import machine

class TemperatureSensor:
    def __init__(self, pin):
        self.adc = machine.ADC(pin)
        self.sf = 4095/65535
        self.volt_per_adc = (3.3 / 4095)
        self.DECIMAL_PRECISION = 1
    
    def read_temperature(self): 
        millivolts = self.adc.read_u16()
        adc_12b = millivolts * self.sf
        volt = adc_12b * self.volt_per_adc
        dx = abs(50 - 0)
        dy = abs(0 - 0.5)
        shift = volt - 0.5
        temp = shift / (dy / dx)
        return round(temp, self.DECIMAL_PRECISION)