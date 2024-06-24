import urequests as requests

def print_method_name(func):
    def wrapper(*args, **kwargs):
        print(f"Method: {func.__name__}")
        return func(*args, **kwargs)
    return wrapper

class SaveData:
    def __init__(self, api_token):
        self.api_token = api_token
        pass
    
    def get_token(self, username, password):
        url = f'https://plantobserverapi.azurewebsites.net/PlantData/token?user={username}&password={password}'
        headers = {
            "Accept": 'text/plain'
        }
        params = {
            'user' : username,
            'password' : password
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

    def build_json(self, variable, value):
        try:
            data = {variable: {"value": value}}
            return data
        except:
            return None
        
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
        
    def send_to_api(self, token, temperature, dht_temperature, dht_humidity, sensor_id):
        url = f'https://plantobserverapi.azurewebsites.net/PlantData/post'
        headers = {
            'Content-Type': 'application/json',
            'Authorization': f'Bearer {token}'
        }
        data = {
            "temperature" : float(temperature),
            "dht_temperature": int(dht_temperature),
            "dht_humidity" : int(dht_humidity),
            "sensor_id" : sensor_id
        }
        print(data)
        try:
            req = requests.post(url, headers=headers, json=data)
            if req.status_code == 200:
                print("save_data.py Post successful")
            else:
                print(req.status_code)
        except Exception as e:
            print(e)
        finally:
            if req:
                req.close()

    def update_subscriber_status(self, id, value, token):
        url = f'https://plantobserverapi.azurewebsites.net/PlantData/{id}'
        headers = {
            'Content-Type': 'application/json',
            'Authorization': f'Bearer {token}'
        }
        data = [
            { 
            "op": "replace", 
            "path": "/isSubscriber", 
            "value": value }
        ]
        print(url)
        print(data)
        try:
            req = requests.patch(url, headers=headers, json=data)
            if req.status_code == 200:
                print("update.py successful")
                return True
            else:
                print(req.status_code)
        except Exception as e:
            print(e)
        finally:
            if req:
                req.close()
        pass